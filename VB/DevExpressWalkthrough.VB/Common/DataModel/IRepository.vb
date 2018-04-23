Imports System
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Collections.Generic
Imports System.Reflection
Imports System.ComponentModel
Imports DevExpressWalkthrough.VB.Common.Utils
Namespace Global.DevExpressWalkthrough.VB.Common.DataModel
    ''' <summary>
    ''' The IRepository interface represents the read and write implementation of the Repository pattern 
    ''' such that it can be used to query entities of a given type. 
    ''' </summary>
    ''' <typeparam name="TEntity">A repository entity type.</typeparam>
    ''' <typeparam name="TPrimaryKey">An entity primary key type.</typeparam>
    Public Interface IRepository(Of TEntity As Class, TPrimaryKey)
        Inherits IReadOnlyRepository(Of TEntity)
        ''' <summary>
        ''' Finds an entity with the given primary key value. 
        ''' If an entity with the given primary key value exists in the unit of work, then it is returned immediately without making a request to the store. 
        ''' Otherwise, a request is made to the store for an entity with the given primary key value and this entity, if found, is attached to the unit of work and returned. 
        ''' If no entity is found in the unit of work or the store, then null is returned.
        ''' </summary>
        ''' <param name="primaryKey">The value of the primary key for the entity to be found.</param>
        Function Find(ByVal primaryKey As TPrimaryKey) As TEntity
        ''' <summary>
        ''' Marks the given entity as Added such that it will be commited to the store when IUnitOfWork.SaveChanges is called.
        ''' </summary>
        ''' <param name="entity">The entity to add.</param>
        Sub Add(ByVal entity As TEntity)
        ''' <summary>
        ''' Marks the given entity as Deleted such that it will be deleted from the store when IUnitOfWork.SaveChanges is called. 
        ''' Note that the entity must exist in the unit of work in some other state before this method is called.
        ''' </summary>
        ''' <param name="entity">The entity to remove.</param>
        Sub Remove(ByVal entity As TEntity)
        ''' <summary>
        ''' Creates a new instance of the entity type.
        ''' </summary>
        ''' <param name="add">A flag determining if the newly created entity is added to the repository.</param>
        Function Create(Optional ByVal add As Boolean = True) As TEntity
        ''' <summary>
        ''' Returns the state of the given entity.
        ''' </summary>
        ''' <param name="entity">An entity to get state from</param>
        Function GetState(ByVal entity As TEntity) As EntityState
        ''' <summary>
        ''' Changes the state of the specified entity to Modified if changes are not automatically tracked by the implementation.
        ''' </summary>
        ''' <param name="entity">An entity which state should be updated/</param>
        Sub Update(ByVal entity As TEntity)
        ''' <summary>
        ''' Reloads the entity from the store overwriting any property values with values from the store and returns a reloaded entity. 
        ''' This method returns the same entity instance with updated properties or new one depending on the implementation.
        ''' The entity will be in the Unchanged state after calling this method.
        ''' </summary>
        ''' <param name="entity">An entity to reload.</param>
        Function Reload(ByVal entity As TEntity) As TEntity
        ''' <summary>
        ''' The lambda-expression that returns the entity primary key.
        ''' </summary>
        ReadOnly Property GetPrimaryKeyExpression As Expression(Of Func(Of TEntity, TPrimaryKey))
        ''' <summary>
        ''' Returns the primary key value for the entity.
        ''' </summary>
        ''' <param name="entity">An entity for which to obtain a primary key value.</param>
        Function GetPrimaryKey(ByVal entity As TEntity) As TPrimaryKey
        ''' <summary>
        ''' Determines whether the given entity has the primary key assigned (the primary key is not null). Always returns true if the primary key is a non-nullable value type.
        ''' </summary>
        ''' <param name="entity">An entity to test.</param>
        Function HasPrimaryKey(ByVal entity As TEntity) As Boolean
        ''' <summary>
        ''' Assigns the given primary key value to a given entity.
        ''' </summary>
        ''' <param name="entity">An entity to which to assign the primary key value.</param>
        ''' <param name="primaryKey">A primary key value</param>
        Sub SetPrimaryKey(ByVal entity As TEntity, ByVal primaryKey As TPrimaryKey)
    End Interface
    ''' <summary>
    ''' Provides a set of extension methods to perform commonly used operations with IRepository.
    ''' </summary>
    Public Module RepositoryExtensions
        ''' <summary>
        ''' Builds a lambda expression that compares an entity primary key with the given constant value.
        ''' </summary>
        ''' <typeparam name="TEntity">A repository entity type.</typeparam>
        ''' <typeparam name="TProjection">A projection entity type.</typeparam>
        ''' <typeparam name="TPrimaryKey">An entity primary key type.</typeparam>
        ''' <param name="repository">A repository.</param>
        ''' <param name="primaryKey">A value to compare with the entity primary key.</param>
        <System.Runtime.CompilerServices.Extension> _
        Public Function GetProjectionPrimaryKeyEqualsExpression(Of TEntity As Class, TProjection, TPrimaryKey)(ByVal repository As IRepository(Of TEntity, TPrimaryKey), ByVal primaryKey As TPrimaryKey) As Expression(Of Func(Of TProjection, Boolean))
            Return ExpressionHelper.GetKeyEqualsExpression(Of TEntity, TProjection, TPrimaryKey)(repository.GetPrimaryKeyExpression, primaryKey)
        End Function
        ''' <summary>
        ''' Returns a primary key of the given entity.
        ''' </summary>
        ''' <typeparam name="TEntity">A repository entity type.</typeparam>
        ''' <typeparam name="TProjection">A projection entity type.</typeparam>
        ''' <typeparam name="TPrimaryKey">An entity primary key type.</typeparam>
        ''' <param name="repository">A repository.</param>
        ''' <param name="projectionEntity">An entity.</param>
        <System.Runtime.CompilerServices.Extension> _
        Public Function GetProjectionPrimaryKey(Of TEntity As Class, TProjection, TPrimaryKey)(ByVal repository As IRepository(Of TEntity, TPrimaryKey), ByVal projectionEntity As TProjection) As TPrimaryKey
            Return GetProjectionValue(projectionEntity, Function(ByVal x As TEntity)
                                                            If repository.HasPrimaryKey(x) Then
                                                                Return repository.GetPrimaryKey(x)
                                                            End If
                                                            Return CType(Nothing, TPrimaryKey)
                                                        End Function, Function(ByVal x As TProjection) GetProjectionKey(repository, x))
        End Function
        Private Function GetProjectionKey(Of TEntity As Class, TProjection, TPrimaryKey)(ByVal repository As IRepository(Of TEntity, TPrimaryKey), ByVal projection As TProjection) As TPrimaryKey
            Dim properties = ExpressionHelper.GetKeyProperties(repository.GetPrimaryKeyExpression)
            If ExpressionHelper.IsTuple(Of TPrimaryKey)() Then
                Dim objects = properties.[Select](Function(ByVal p) p.GetValue(projection, Nothing))
                Return ExpressionHelper.MakeTuple(Of TPrimaryKey)(objects.ToArray())
            End If
            Dim [property] = properties.[Single]()
            Return CType(projection.[GetType]().GetProperty([property].Name).GetValue(projection, Nothing), TPrimaryKey)
        End Function
        Private Sub SetProjectionKey(Of TEntity As Class, TProjection, TPrimaryKey)(ByVal repository As IRepository(Of TEntity, TPrimaryKey), ByVal projectionEntity As TProjection, ByVal primaryKey As TPrimaryKey)
            Dim properties = ExpressionHelper.GetKeyProperties(repository.GetPrimaryKeyExpression)
            Dim values = ExpressionHelper.GetKeyPropertyValues(primaryKey)
            If properties.Count() <> values.Count() Then
                Throw New Exception()
            End If
            Dim i As Integer = 0
            While i < values.Count()
                Dim projectionProperty = GetType(TProjection).GetProperty(properties(i).Name)
                projectionProperty.SetValue(projectionEntity, values(i), Nothing)
                i += 1
            End While
        End Sub
        <System.Runtime.CompilerServices.Extension> _
        Public Function GetSinglePropertyPrimaryKeyProjectionProperty(Of TEntity As Class, TProjection, TPrimaryKey)(ByVal repository As IRepository(Of TEntity, TPrimaryKey)) As Expression(Of Func(Of TProjection, TPrimaryKey))
            Dim properties = ExpressionHelper.GetKeyProperties(repository.GetPrimaryKeyExpression)
            Dim propertyName = properties.[Single]().Name
            Dim parameter = Expression.Parameter(GetType(TProjection))
            Return Expression.Lambda(Of Func(Of TProjection, TPrimaryKey))(Expression.[Property](parameter, propertyName), parameter)
        End Function
        Public Sub VerifyProjection(Of TEntity As Class, TProjection, TPrimaryKey)(ByVal repository As IRepository(Of TEntity, TPrimaryKey), ByVal projection As Func(Of IRepositoryQuery(Of TEntity), IQueryable(Of TProjection)))
            If GetType(TProjection) <> GetType(TEntity) AndAlso projection Is Nothing Then
                Throw New ArgumentException("Projection should not be null when its type is different from TEntity.")
            End If
            Dim entityKeyProperties = ExpressionHelper.GetKeyProperties(repository.GetPrimaryKeyExpression)
            Dim projectionKeyPropertyCount = entityKeyProperties.Count(Function(ByVal p)
                                                                           Dim properties = TypeDescriptor.GetProperties(GetType(TProjection))
                                                                           Dim [property] = properties(p.Name)
                                                                           Return [property] IsNot Nothing
                                                                       End Function)
            If projectionKeyPropertyCount <> entityKeyProperties.Count() Then
                Dim tprojectionName As String = GetType(TProjection).Name
                Dim message As String = String.Format("Projection type {0} should have the same primary key as its corresponding entity", tprojectionName)
                Throw New ArgumentException(message, tprojectionName)
            End If
        End Sub
        ''' <summary>
        ''' Sets the primary key of a given projection.
        ''' </summary>
        ''' <typeparam name="TEntity">A repository entity type.</typeparam>
        ''' <typeparam name="TProjection">A projection entity type.</typeparam>
        ''' <typeparam name="TPrimaryKey">An entity primary key type.</typeparam>
        ''' <param name="repository">A repository.</param>
        ''' <param name="projectionEntity">A projection.</param>
        ''' <param name="primaryKey">A new primary key value.</param>
        <System.Runtime.CompilerServices.Extension> _
        Public Sub SetProjectionPrimaryKey(Of TEntity As Class, TProjection, TPrimaryKey)(ByVal repository As IRepository(Of TEntity, TPrimaryKey), ByVal projectionEntity As TProjection, ByVal primaryKey As TPrimaryKey)
            If IsProjection(Of TEntity, TProjection)(projectionEntity) Then
                SetProjectionKey(Of TEntity, TProjection, TPrimaryKey)(repository, projectionEntity, primaryKey)
            Else
                repository.SetPrimaryKey(TryCast(projectionEntity, TEntity), primaryKey)
            End If
        End Sub
        ''' <summary>
        ''' Given a projection, this function returns the corresponding entity. 
        ''' If the projection has no corresponding entity, a new entity is created and added to the repository.
        ''' Before the new entity is returned, the applyProjectionPropertiesToEntity action is used to transfer property values from the projection to the entity.
        ''' </summary>
        ''' <typeparam name="TEntity">A repository entity type.</typeparam>
        ''' <typeparam name="TProjection">A projection entity type.</typeparam>
        ''' <typeparam name="TPrimaryKey">An entity primary key type.</typeparam>
        ''' <param name="repository">A repository.</param>
        ''' <param name="projectionEntity">A projection.</param>
        ''' <param name="applyProjectionPropertiesToEntity">An action which applies the projection properties to the newly created entity.</param>
        ''' 
        ''' 
        <System.Runtime.CompilerServices.Extension> _
        Public Function FindExistingOrAddNewEntity(Of TEntity As Class, TProjection, TPrimaryKey)(ByVal repository As IRepository(Of TEntity, TPrimaryKey), ByVal projectionEntity As TProjection, ByVal applyProjectionPropertiesToEntity As Action(Of TProjection, TEntity)) As TEntity
            Dim projection As Boolean = IsProjection(Of TEntity, TProjection)(projectionEntity)
            Dim entity = repository.Find(repository.GetProjectionPrimaryKey(projectionEntity))
            If entity Is Nothing Then
                If projection Then
                    entity = repository.Create()
                Else
                    entity = TryCast(projectionEntity, TEntity)
                    repository.Add(entity)
                End If
            End If
            If projection Then
                applyProjectionPropertiesToEntity(projectionEntity, entity)
            End If
            Return entity
        End Function
        ''' <summary>
        ''' Gets whether the given entity is detached from the unit of work.
        ''' </summary>
        ''' <typeparam name="TEntity">A repository entity type.</typeparam>
        ''' <typeparam name="TProjection">A projection entity type.</typeparam>
        ''' <typeparam name="TPrimaryKey">An entity primary key type.</typeparam>
        ''' <param name="repository">A repository.</param>
        ''' <param name="projectionEntity">An entity.</param>
        <System.Runtime.CompilerServices.Extension> _
        Public Function IsDetached(Of TEntity As Class, TProjection, TPrimaryKey)(ByVal repository As IRepository(Of TEntity, TPrimaryKey), ByVal projectionEntity As TProjection) As Boolean
            Return GetProjectionValue(projectionEntity, Function(ByVal x As TEntity) repository.GetState(x) = EntityState.Detached, Function(ByVal x As TProjection) False)
        End Function
        ''' <summary>
        ''' Determines whether the given entity has the primary key assigned (the primary key is not null). Always returns true if the primary key is a non-nullable value type.
        ''' </summary>
        ''' <typeparam name="TEntity">A repository entity type.</typeparam>
        ''' <typeparam name="TProjection">A projection entity type.</typeparam>
        ''' <typeparam name="TPrimaryKey">An entity primary key type.</typeparam>
        ''' <param name="repository">A repository.</param>
        ''' <param name="projectionEntity">An entity.</param>
        <System.Runtime.CompilerServices.Extension> _
        Public Function ProjectionHasPrimaryKey(Of TEntity As Class, TProjection, TPrimaryKey)(ByVal repository As IRepository(Of TEntity, TPrimaryKey), ByVal projectionEntity As TProjection) As Boolean
            Return GetProjectionValue(projectionEntity, Function(ByVal x As TEntity) repository.HasPrimaryKey(x), Function(ByVal x As TProjection) True)
        End Function
        ''' <summary>
        ''' Loads from the store or updates an entity with the given primary key value. If no entity with the given primary key is found in the store, returns null.
        ''' </summary>
        ''' <typeparam name="TEntity">A repository entity type.</typeparam>
        ''' <typeparam name="TProjection">A projection entity type.</typeparam>
        ''' <typeparam name="TPrimaryKey">An entity primary key type.</typeparam>
        ''' <param name="repository">A repository.</param>
        ''' <param name="projection">A LINQ function used to transform entities from the repository entity type to the projection entity type.</param>
        ''' <param name="primaryKey">A value to compare with the entity primary key.</param>
        <System.Runtime.CompilerServices.Extension> _
        Public Function FindActualProjectionByKey(Of TEntity As Class, TProjection, TPrimaryKey)(ByVal repository As IRepository(Of TEntity, TPrimaryKey), ByVal projection As Func(Of IRepositoryQuery(Of TEntity), IQueryable(Of TProjection)), ByVal primaryKey As TPrimaryKey) As TProjection
            Dim primaryKeyEqualsExpression = GetProjectionPrimaryKeyEqualsExpression(Of TEntity, TProjection, TPrimaryKey)(repository, primaryKey)
            Dim result = repository.GetFilteredEntities(Nothing, projection).Where(primaryKeyEqualsExpression).Take(1).ToArray().FirstOrDefault() 'WCF incorrect FirstOrDefault implementation workaround
            Return GetProjectionValue(result, Function(ByVal x As TEntity) If(x IsNot Nothing, repository.Reload(x), Nothing), Function(ByVal x As TProjection) x)
        End Function
        Private Function GetProjectionValue(Of TEntity, TProjection, TEntityResult, TProjectionResult)(ByVal value As TProjection, ByVal entityFunc As Func(Of TEntity, TEntityResult), ByVal projectionFunc As Func(Of TProjection, TProjectionResult)) As TProjectionResult
            If GetType(TEntity) <> GetType(TProjection) OrElse GetType(TEntityResult) <> GetType(TProjectionResult) Then
                Return projectionFunc(value)
            End If
            Return CType(CType(entityFunc(CType(CType(value, Object), TEntity)), Object), TProjectionResult)
        End Function
        Private Function IsProjection(Of TEntity, TProjection)(ByVal projection As TProjection) As Boolean
            Return Not (TypeOf projection Is TEntity)
        End Function
    End Module
End Namespace
