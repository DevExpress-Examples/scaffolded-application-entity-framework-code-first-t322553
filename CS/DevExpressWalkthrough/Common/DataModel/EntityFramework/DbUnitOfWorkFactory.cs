using System;
using System.Linq;
using System.Data;
using System.Linq.Expressions;
using System.Collections.Generic;
using DevExpressWalkthrough.Common.Utils;
using DevExpressWalkthrough.Common.DataModel;
using DevExpressWalkthrough.Common.DataModel.DesignTime;
using DevExpress.Mvvm;
using System.Collections;
using System.ComponentModel;
using DevExpress.Data.Linq;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Data.Async.Helpers;
using DevExpress.Xpf.Core.ServerMode;

namespace DevExpressWalkthrough.Common.DataModel.EntityFramework {
    class DbUnitOfWorkFactory<TUnitOfWork> : IUnitOfWorkFactory<TUnitOfWork> where TUnitOfWork : IUnitOfWork {
        Func<TUnitOfWork> createUnitOfWork;

        public DbUnitOfWorkFactory(Func<TUnitOfWork> createUnitOfWork) {
            this.createUnitOfWork = createUnitOfWork;
        }

        TUnitOfWork IUnitOfWorkFactory<TUnitOfWork>.CreateUnitOfWork() {
            return createUnitOfWork();
        }

        IInstantFeedbackSource<TProjection> IUnitOfWorkFactory<TUnitOfWork>.CreateInstantFeedbackSource<TEntity, TProjection, TPrimaryKey>(
            Func<TUnitOfWork, IRepository<TEntity, TPrimaryKey>> getRepositoryFunc,
            Func<IRepositoryQuery<TEntity>, IQueryable<TProjection>> projection) {
            var threadSafeProperties = new TypeInfoProxied(TypeDescriptor.GetProperties(typeof(TProjection)), null).UIDescriptors;
            if(projection == null) {
                projection = x => x as IQueryable<TProjection>;
            }
            var keyProperties = ExpressionHelper.GetKeyProperties(getRepositoryFunc(createUnitOfWork()).GetPrimaryKeyExpression);
            var keyExpression = keyProperties.Select(p => p.Name).Aggregate((l, r) => l + ";" + r);
            var source = new EntityInstantFeedbackSource((DevExpress.Data.Linq.GetQueryableEventArgs e) => e.QueryableSource = projection(getRepositoryFunc(createUnitOfWork()))) {
                KeyExpression = keyExpression
            };
            return new InstantFeedbackSource<TProjection>(source, threadSafeProperties);
        }
    }
}