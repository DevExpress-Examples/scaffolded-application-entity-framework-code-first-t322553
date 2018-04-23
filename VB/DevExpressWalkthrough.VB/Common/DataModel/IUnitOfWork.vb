Imports System
Imports System.ComponentModel
Imports System.Linq
Imports System.Linq.Expressions
Namespace Global.DevExpressWalkthrough.VB.Common.DataModel
    ''' <summary>
    ''' The IUnitOfWork interface represents the Unit Of Work pattern 
    ''' such that it can be used to query from a database and group together changes that will then be written back to the store as a unit. 
    ''' </summary>
    Public Interface IUnitOfWork
        ''' <summary>
        ''' Saves all changes made in this unit of work to the underlying store.
        ''' </summary>
        Sub SaveChanges()
        ''' <summary>
        ''' Checks if the unit of work is tracking any new, deleted, or changed entities or relationships that will be sent to the store if SaveChanges() is called.
        ''' </summary>
        Function HasChanges() As Boolean
    End Interface
    ''' <summary>
    ''' Provides the method to create a unit of work of a given type.
    ''' </summary>
    ''' <typeparam name="TUnitOfWork">A unit of work type.</typeparam>
    Public Interface IUnitOfWorkFactory(Of TUnitOfWork As IUnitOfWork)
        ''' <summary>
        ''' Creates a new unit of work.
        ''' </summary>
        Function CreateUnitOfWork() As TUnitOfWork
        ''' <summary>
        ''' Creates a new IInstantFeedbackSource instance.
        ''' </summary>
        Function CreateInstantFeedbackSource(Of TEntity As {Class, New}, TProjection As Class, TPrimaryKey)(ByVal getRepositoryFunc As Func(Of TUnitOfWork, IRepository(Of TEntity, TPrimaryKey)), ByVal projection As Func(Of IRepositoryQuery(Of TEntity), IQueryable(Of TProjection))) As IInstantFeedbackSource(Of TProjection)
    End Interface
    ''' <summary>
    ''' A data source suitable as an Instant Feedback source.
    ''' The GetList() method of the base IListSource interface is expected to return an instance of an internal type that happens to
    ''' implement the IList interface. As such the IInstantFeedbackSource interface can only be implemented as a wrapper
    ''' for an existing Instant Feedback source, e.g. EntityInstantFeedbackSource or WcfInstantFeedbackDataSource.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    Public Interface IInstantFeedbackSource(Of TEntity As Class)
        Inherits IListSource
        ''' <summary>
        ''' Invalidate all loaded entities. This method is used to make changes made to the data source visible to
        ''' consumers if this Instant Feedback source.
        ''' Currently, in scaffolded implementations this method only works for WCF when the MergeOption set to NoTracking 
        ''' and for EntityFramework when a projection is used.
        ''' </summary>
        Sub Refresh()
        ''' <summary>
        ''' Get the value of a property.
        ''' </summary>
        ''' <typeparam name="TProperty">The type of the property.</typeparam>
        ''' <param name="threadSafeProxy">A proxy object.</param>
        ''' <param name="propertyExpression">An expression specifying the property which value is to be fetched.</param>
        ''' <returns></returns>
        Function GetPropertyValue(Of TProperty)(ByVal threadSafeProxy As Object, ByVal propertyExpression As Expression(Of Func(Of TEntity, TProperty))) As TProperty
        ''' <summary>
        ''' Check if a proxy object is in the Loaded state.
        ''' If a proxy object is not in the Loaded state, its properties have not yet been initialized.
        ''' </summary>
        ''' <param name="threadSafeProxy"></param>
        ''' <returns></returns>
        Function IsLoadedProxy(ByVal threadSafeProxy As Object) As Boolean
    End Interface
End Namespace
