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
    public class AddRemoveDetailEntitiesViewModel<TEntity, TPrimaryKey, TDetailEntity, TDetailPrimaryKey, TUnitOfWork> : SingleObjectViewModelBase<TEntity, TPrimaryKey, TUnitOfWork>
        where TEntity : class
        where TDetailEntity : class
        where TUnitOfWork : IUnitOfWork {

        public static AddRemoveDetailEntitiesViewModel<TEntity, TPrimaryKey, TDetailEntity, TDetailPrimaryKey, TUnitOfWork> Create(IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory, Func<TUnitOfWork, IRepository<TEntity, TPrimaryKey>> getRepositoryFunc, Func<TUnitOfWork, IRepository<TDetailEntity, TDetailPrimaryKey>> getDetailsRepositoryFunc, Func<TEntity, ICollection<TDetailEntity>> getDetailsFunc, TPrimaryKey id) {
            return ViewModelSource.Create(() => new AddRemoveDetailEntitiesViewModel<TEntity, TPrimaryKey, TDetailEntity, TDetailPrimaryKey, TUnitOfWork>(unitOfWorkFactory, getRepositoryFunc, getDetailsRepositoryFunc, getDetailsFunc, id));
        }

        protected readonly Func<TUnitOfWork, IRepository<TDetailEntity, TDetailPrimaryKey>> getDetailsRepositoryFunc;
        readonly Func<TEntity, ICollection<TDetailEntity>> getDetailsFunc;

        protected IDialogService DialogService { get { return this.GetRequiredService<IDialogService>(); } }
        protected IDocumentManagerService DocumentManagerService { get { return this.GetRequiredService<IDocumentManagerService>(); } }

        IRepository<TDetailEntity, TDetailPrimaryKey> DetailsRepository { get { return getDetailsRepositoryFunc(UnitOfWork); } }

        public virtual ICollection<TDetailEntity> DetailEntities { get { return Entity != null ? getDetailsFunc(Entity) : Enumerable.Empty<TDetailEntity>().ToArray(); } }
        public ObservableCollection<TDetailEntity> SelectedEntities { get; private set; }
        public virtual bool IsCreateDetailButtonVisible { get { return true; } }

        protected AddRemoveDetailEntitiesViewModel(IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory, Func<TUnitOfWork, IRepository<TEntity, TPrimaryKey>> getRepositoryFunc, Func<TUnitOfWork, IRepository<TDetailEntity, TDetailPrimaryKey>> getDetailsRepositoryFunc, Func<TEntity, ICollection<TDetailEntity>> getDetailsFunc, TPrimaryKey id)
            : base(unitOfWorkFactory, getRepositoryFunc, null) {
            this.getDetailsRepositoryFunc = getDetailsRepositoryFunc;
            this.getDetailsFunc = getDetailsFunc;
            SelectedEntities = new ObservableCollection<TDetailEntity>();
            if(this.IsInDesignMode())
                return;
            LoadEntityByKey(id);
            Messenger.Default.Register(this, (EntityMessage<TDetailEntity, TDetailPrimaryKey> m) => {
                if(m.MessageType != EntityMessageType.Added)
                    return;
                var withParent = m.Sender as ISupportParentViewModel;
                if(withParent == null || withParent.ParentViewModel != this)
                    return;
                var withEntity = m.Sender as SingleObjectViewModel<TDetailEntity, TDetailPrimaryKey, TUnitOfWork>;
                var detailEntity = DetailsRepository.Find(DetailsRepository.GetPrimaryKey(withEntity.Entity));
                if(detailEntity == null)
                    return;
                DetailEntities.Add(detailEntity);
                SaveChangesAndNotify(new TDetailEntity[] { detailEntity });
            });
        }

        public virtual void CreateDetailEntity() {
            DocumentManagerService.ShowNewEntityDocument<TDetailEntity>(this);
        }

        public virtual void EditDetailEntity() {
            if(SelectedEntities.Any()) {
                var detailKey = DetailsRepository.GetPrimaryKey(SelectedEntities.First());
                DocumentManagerService.ShowExistingEntityDocument<TDetailEntity, TDetailPrimaryKey>(this, detailKey);
            }
        }

        protected override void OnInitializeInRuntime() {
            base.OnInitializeInRuntime();
            Messenger.Default.Register<EntityMessage<TEntity, TPrimaryKey>>(this, m => OnMessage(m));
        }

        public virtual void AddDetailEntities() {
            var availalbleEntities = DetailsRepository.ToList().Except(DetailEntities).ToArray();
            var selectEntitiesViewModel = new SelectDetailEntitiesViewModel<TDetailEntity>(availalbleEntities);
            if(DialogService.ShowDialog(MessageButton.OKCancel, CommonResources.AddRemoveDetailEntities_SelectObjects, selectEntitiesViewModel) == MessageResult.OK && selectEntitiesViewModel.SelectedEntities.Any()) {
                foreach(var selectedEntity in selectEntitiesViewModel.SelectedEntities) {
                    DetailEntities.Add(selectedEntity);
                }
                SaveChangesAndNotify(selectEntitiesViewModel.SelectedEntities);
            }
        }

        public bool CanAddDetailEntities() {
            return Entity != null;
        }

        public virtual void RemoveDetailEntities() {
            if(!SelectedEntities.Any())
                return;
            foreach(var selectedEntity in SelectedEntities) {
                DetailEntities.Remove(selectedEntity);
            }
            SaveChangesAndNotify(SelectedEntities);
            SelectedEntities.Clear();
        }

        public bool CanRemoveDetailEntities() {
            return Entity != null;
        }

        protected void SaveChangesAndNotify(IEnumerable<TDetailEntity> detailEntities) {
            try {
                UnitOfWork.SaveChanges();
                foreach(var detailEntity in detailEntities) {
                    Messenger.Default.Send(new EntityMessage<TDetailEntity, TDetailPrimaryKey>(DetailsRepository.GetPrimaryKey(detailEntity), EntityMessageType.Changed));
                }
                Reload();
            } catch(DbException e) {
                MessageBoxService.ShowMessage(e.ErrorMessage, e.ErrorCaption, MessageButton.OK, MessageIcon.Error);
            }
        }

        void OnMessage(EntityMessage<TEntity, TPrimaryKey> message) {
            if(message.MessageType == EntityMessageType.Changed && Entity != null && object.Equals(PrimaryKey, message.PrimaryKey))
                Reload();
        }

        protected override void OnEntityChanged() {
            base.OnEntityChanged();
            this.RaisePropertyChanged(x => x.DetailEntities);
        }
    }
    public class SelectDetailEntitiesViewModel<TEntity> where TEntity : class {
        public SelectDetailEntitiesViewModel(TEntity[] availableCourses) {
            AvailableEntities = availableCourses;
            SelectedEntities = new List<TEntity>();
        }
        public TEntity[] AvailableEntities { get; private set; }
        public List<TEntity> SelectedEntities { get; private set; }
    }
}