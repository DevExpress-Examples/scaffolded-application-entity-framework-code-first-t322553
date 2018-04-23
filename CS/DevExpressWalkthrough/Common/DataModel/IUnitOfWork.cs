using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace DevExpressWalkthrough.Common.DataModel {
    /// <summary>
    /// The IUnitOfWork interface represents the Unit Of Work pattern 
    /// such that it can be used to query from a database and group together changes that will then be written back to the store as a unit. 
    /// </summary>
    public interface IUnitOfWork {
        /// <summary>
        /// Saves all changes made in this unit of work to the underlying store.
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Checks if the unit of work is tracking any new, deleted, or changed entities or relationships that will be sent to the store if SaveChanges() is called.
        /// </summary>
        bool HasChanges();
    }

    /// <summary>
    /// Provides the method to create a unit of work of a given type.
    /// </summary>
    /// <typeparam name="TUnitOfWork">A unit of work type.</typeparam>
    public interface IUnitOfWorkFactory<TUnitOfWork> where TUnitOfWork : IUnitOfWork {

        /// <summary>
        /// Creates a new unit of work.
        /// </summary>
        TUnitOfWork CreateUnitOfWork();

        /// <summary>
        /// Creates a new IInstantFeedbackSource instance.
        /// </summary>
        IInstantFeedbackSource<TProjection> CreateInstantFeedbackSource<TEntity, TProjection, TPrimaryKey>(Func<TUnitOfWork, IRepository<TEntity, TPrimaryKey>> getRepositoryFunc, Func<IRepositoryQuery<TEntity>, IQueryable<TProjection>> projection)
            where TEntity : class, new()
            where TProjection : class;
    }

    /// <summary>
    /// A data source suitable as an Instant Feedback source.
    /// The GetList() method of the base IListSource interface is expected to return an instance of an internal type that happens to
    /// implement the IList interface. As such the IInstantFeedbackSource interface can only be implemented as a wrapper
    /// for an existing Instant Feedback source, e.g. EntityInstantFeedbackSource or WcfInstantFeedbackDataSource.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IInstantFeedbackSource<TEntity> : IListSource
        where TEntity : class {
        /// <summary>
        /// Invalidate all loaded entities. This method is used to make changes made to the data source visible to
        /// consumers if this Instant Feedback source.
        /// Currently, in scaffolded implementations this method only works for WCF when the MergeOption set to NoTracking 
        /// and for EntityFramework when a projection is used.
        /// </summary>
        void Refresh();

        /// <summary>
        /// Get the value of a property.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="threadSafeProxy">A proxy object.</param>
        /// <param name="propertyExpression">An expression specifying the property which value is to be fetched.</param>
        /// <returns></returns>
        TProperty GetPropertyValue<TProperty>(object threadSafeProxy, Expression<Func<TEntity, TProperty>> propertyExpression);

        /// <summary>
        /// Check if a proxy object is in the Loaded state.
        /// If a proxy object is not in the Loaded state, its properties have not yet been initialized.
        /// </summary>
        /// <param name="threadSafeProxy"></param>
        /// <returns></returns>
        bool IsLoadedProxy(object threadSafeProxy);
    }
}
