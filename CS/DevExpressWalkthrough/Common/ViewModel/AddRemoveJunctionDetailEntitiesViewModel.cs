using System;
using System.Linq.Expressions;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.Mvvm.POCO;
using System.Linq;
using DevExpressWalkthrough.Common.Utils;
using DevExpressWalkthrough.Common.DataModel;

namespace DevExpressWalkthrough.Common.ViewModel {
    public class AddRemoveJunctionDetailEntitiesViewModel<TEntity, TPrimaryKey, TDetailEntity, TDetailPrimaryKey, TJunction, TJunctionPrimaryKey, TUnitOfWork> : AddRemoveDetailEntitiesViewModel<TEntity, TPrimaryKey, TDetailEntity, TDetailPrimaryKey, TUnitOfWork>
        where TEntity : class
        where TDetailEntity : class
        where TJunction : class, new()
        where TJunctionPrimaryKey : class
        where TUnitOfWork : IUnitOfWork {

        public static AddRemoveJunctionDetailEntitiesViewModel<TEntity, TPrimaryKey, TDetailEntity, TDetailPrimaryKey, TJunction, TJunctionPrimaryKey, TUnitOfWork> CreateViewModel(
            IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory,
            Func<TUnitOfWork, IRepository<TEntity, TPrimaryKey>> getRepositoryFunc,
            Func<TUnitOfWork, IRepository<TDetailEntity, TDetailPrimaryKey>> getDetailsRepositoryFunc,
            Func<TUnitOfWork, IRepository<TJunction, TJunctionPrimaryKey>> getJunctionRepositoryFunc,
            Expression<Func<TJunction, TPrimaryKey>> getEntityForeignKey,
            Expression<Func<TJunction, TDetailPrimaryKey>> getDetailForeignKey,
            TPrimaryKey id) {
            return ViewModelSource.Create(() => new AddRemoveJunctionDetailEntitiesViewModel<TEntity, TPrimaryKey, TDetailEntity, TDetailPrimaryKey, TJunction, TJunctionPrimaryKey, TUnitOfWork>(unitOfWorkFactory, getRepositoryFunc, getDetailsRepositoryFunc, getJunctionRepositoryFunc, getEntityForeignKey, getDetailForeignKey, id));
        }

        readonly Func<TUnitOfWork, IRepository<TJunction, TJunctionPrimaryKey>> getJunctionRepositoryFunc;
        readonly Expression<Func<TJunction, TPrimaryKey>> getEntityForeignKey;
        readonly Expression<Func<TJunction, TDetailPrimaryKey>> getDetailForeignKey;

        IRepository<TDetailEntity, TDetailPrimaryKey> DetailsRepository { get { return getDetailsRepositoryFunc(UnitOfWork); } }
        IRepository<TJunction, TJunctionPrimaryKey> JunctionRepository { get { return getJunctionRepositoryFunc(UnitOfWork); } }
        public override bool IsCreateDetailButtonVisible { get { return false; } }

        public override ICollection<TDetailEntity> DetailEntities
        {
            get
            {
                if(Entity == null)
                    return Enumerable.Empty<TDetailEntity>().ToArray();
                var entityPrimaryKey = Repository.GetPrimaryKey(Entity);
                var junctions = JunctionRepository.Where(GetJunctionPredicate(entityPrimaryKey));
                return junctions.Join(DetailsRepository, getDetailForeignKey, DetailsRepository.GetPrimaryKeyExpression, (j, d) => d).ToArray();
            }
        }

        protected AddRemoveJunctionDetailEntitiesViewModel(
                IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory,
                Func<TUnitOfWork, IRepository<TEntity, TPrimaryKey>> getRepositoryFunc,
                Func<TUnitOfWork, IRepository<TDetailEntity, TDetailPrimaryKey>> getDetailsRepositoryFunc,
                Func<TUnitOfWork, IRepository<TJunction, TJunctionPrimaryKey>> getJunctionRepositoryFunc,
                Expression<Func<TJunction, TPrimaryKey>> getEntityForeignKey,
                Expression<Func<TJunction, TDetailPrimaryKey>> getDetailForeignKey,
                TPrimaryKey id)
            : base(unitOfWorkFactory, getRepositoryFunc, getDetailsRepositoryFunc, null, id) {
            this.getJunctionRepositoryFunc = getJunctionRepositoryFunc;
            this.getEntityForeignKey = getEntityForeignKey;
            this.getDetailForeignKey = getDetailForeignKey;
        }

        Expression<Func<TJunction, bool>> GetJunctionPredicate(TPrimaryKey primaryKey) {
            var param = Expression.Parameter(typeof(TJunction));
            var entityForeignKeyExpr = Expression.Property(param, ExpressionHelper.GetPropertyName(getEntityForeignKey));
            var expr = Expression.Equal(entityForeignKeyExpr, Expression.Constant(primaryKey));
            return Expression.Lambda<Func<TJunction, bool>>(expr, param);
        }

        Expression<Func<TJunction, bool>> GetJunctionPredicate(TPrimaryKey primaryKey, TDetailPrimaryKey detailPrimaryKey) {
            var param = Expression.Parameter(typeof(TJunction));
            var entityForeignKeyExpr = Expression.Property(param, ExpressionHelper.GetPropertyName(getEntityForeignKey));
            var expr = Expression.Equal(entityForeignKeyExpr, Expression.Constant(primaryKey));
            var detailForeignKeyExpr = Expression.Property(param, ExpressionHelper.GetPropertyName(getDetailForeignKey));
            var detailEqual = Expression.Equal(detailForeignKeyExpr, Expression.Constant(detailPrimaryKey));
            expr = Expression.And(expr, detailEqual);
            return Expression.Lambda<Func<TJunction, bool>>(expr, param);
        }

        public override void AddDetailEntities() {
            var availableEntities = DetailsRepository.ToList().Except(DetailEntities).ToArray();
            var selectEntitiesViewModel = new SelectDetailEntitiesViewModel<TDetailEntity>(availableEntities);
            if(DialogService.ShowDialog(MessageButton.OKCancel, CommonResources.AddRemoveDetailEntities_SelectObjects, selectEntitiesViewModel) == MessageResult.OK && selectEntitiesViewModel.SelectedEntities.Any()) {
                foreach(var selectedEntity in selectEntitiesViewModel.SelectedEntities) {
                    var junction = new TJunction();
                    var entityKey = Repository.GetPrimaryKey(Entity);
                    var detailKey = DetailsRepository.GetPrimaryKey(selectedEntity);
                    var junctionType = typeof(TJunction);
                    junctionType.GetProperty(ExpressionHelper.GetPropertyName(getEntityForeignKey)).SetValue(junction, entityKey, null);
                    junctionType.GetProperty(ExpressionHelper.GetPropertyName(getDetailForeignKey)).SetValue(junction, detailKey, null);
                    JunctionRepository.Add(junction);
                }
                SaveChangesAndNotify(selectEntitiesViewModel.SelectedEntities);
            }
        }

        public override void RemoveDetailEntities() {
            if(!SelectedEntities.Any())
                return;
            var entityKey = Repository.GetPrimaryKey(Entity);
            foreach(var selectedEntity in SelectedEntities) {
                var detailKey = DetailsRepository.GetPrimaryKey(selectedEntity);
                var junction = JunctionRepository.First(GetJunctionPredicate(entityKey, detailKey));
                JunctionRepository.Remove(junction);
            }
            SaveChangesAndNotify(SelectedEntities);
            SelectedEntities.Clear();
        }
    }
}