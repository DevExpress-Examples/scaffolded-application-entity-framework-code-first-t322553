Imports System
Imports System.Linq.Expressions
Imports DevExpress.Mvvm
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports DevExpress.Mvvm.POCO
Imports System.Linq
Imports DevExpressWalkthrough.VB.Common.Utils
Imports DevExpressWalkthrough.VB.Common.DataModel
Namespace Global.DevExpressWalkthrough.VB.Common.ViewModel
    Public Class AddRemoveJunctionDetailEntitiesViewModel(Of TEntity As Class, TPrimaryKey, TDetailEntity As Class, TDetailPrimaryKey, TJunction As {Class, New}, TJunctionPrimaryKey As Class, TUnitOfWork As IUnitOfWork)
        Inherits AddRemoveDetailEntitiesViewModel(Of TEntity, TPrimaryKey, TDetailEntity, TDetailPrimaryKey, TUnitOfWork)
        Public Shared Function CreateViewModel(ByVal unitOfWorkFactory As IUnitOfWorkFactory(Of TUnitOfWork), ByVal getRepositoryFunc As Func(Of TUnitOfWork, IRepository(Of TEntity, TPrimaryKey)), ByVal getDetailsRepositoryFunc As Func(Of TUnitOfWork, IRepository(Of TDetailEntity, TDetailPrimaryKey)), ByVal getJunctionRepositoryFunc As Func(Of TUnitOfWork, IRepository(Of TJunction, TJunctionPrimaryKey)), ByVal getEntityForeignKey As Expression(Of Func(Of TJunction, TPrimaryKey)), ByVal getDetailForeignKey As Expression(Of Func(Of TJunction, TDetailPrimaryKey)), ByVal id As TPrimaryKey) As AddRemoveJunctionDetailEntitiesViewModel(Of TEntity, TPrimaryKey, TDetailEntity, TDetailPrimaryKey, TJunction, TJunctionPrimaryKey, TUnitOfWork)
            Return ViewModelSource.Create(Function() New AddRemoveJunctionDetailEntitiesViewModel(Of TEntity, TPrimaryKey, TDetailEntity, TDetailPrimaryKey, TJunction, TJunctionPrimaryKey, TUnitOfWork)(unitOfWorkFactory, getRepositoryFunc, getDetailsRepositoryFunc, getJunctionRepositoryFunc, getEntityForeignKey, getDetailForeignKey, id))
        End Function
        Private ReadOnly _getJunctionRepositoryFunc As Func(Of TUnitOfWork, IRepository(Of TJunction, TJunctionPrimaryKey))
        Private ReadOnly _getEntityForeignKey As Expression(Of Func(Of TJunction, TPrimaryKey))
        Private ReadOnly _getDetailForeignKey As Expression(Of Func(Of TJunction, TDetailPrimaryKey))
        Private ReadOnly Property DetailsRepository As IRepository(Of TDetailEntity, TDetailPrimaryKey)
            Get
                Return getDetailsRepositoryFunc(UnitOfWork)
            End Get
        End Property
        Private ReadOnly Property JunctionRepository As IRepository(Of TJunction, TJunctionPrimaryKey)
            Get
                Return _getJunctionRepositoryFunc(UnitOfWork)
            End Get
        End Property
        Public Overrides ReadOnly Property IsCreateDetailButtonVisible As Boolean
            Get
                Return False
            End Get
        End Property
        Public Overrides ReadOnly Property DetailEntities As ICollection(Of TDetailEntity)
            Get
                If Entity Is Nothing Then
                    Return Enumerable.Empty(Of TDetailEntity)().ToArray()
                End If
                Dim entityPrimaryKey = Repository.GetPrimaryKey(Entity)
                Dim junctions = JunctionRepository.Where(GetJunctionPredicate(entityPrimaryKey))
                Return junctions.Join(DetailsRepository, _getDetailForeignKey, DetailsRepository.GetPrimaryKeyExpression, Function(ByVal j, ByVal d) d).ToArray()
            End Get
        End Property
        Protected Sub New(ByVal unitOfWorkFactory As IUnitOfWorkFactory(Of TUnitOfWork), ByVal getRepositoryFunc As Func(Of TUnitOfWork, IRepository(Of TEntity, TPrimaryKey)), ByVal getDetailsRepositoryFunc As Func(Of TUnitOfWork, IRepository(Of TDetailEntity, TDetailPrimaryKey)), ByVal getJunctionRepositoryFunc As Func(Of TUnitOfWork, IRepository(Of TJunction, TJunctionPrimaryKey)), ByVal getEntityForeignKey As Expression(Of Func(Of TJunction, TPrimaryKey)), ByVal getDetailForeignKey As Expression(Of Func(Of TJunction, TDetailPrimaryKey)), ByVal id As TPrimaryKey)
            MyBase.New(unitOfWorkFactory, getRepositoryFunc, getDetailsRepositoryFunc, Nothing, id)
            Me._getJunctionRepositoryFunc = getJunctionRepositoryFunc
            Me._getEntityForeignKey = getEntityForeignKey
            Me._getDetailForeignKey = getDetailForeignKey
        End Sub
        Private Function GetJunctionPredicate(ByVal primaryKey As TPrimaryKey) As Expression(Of Func(Of TJunction, Boolean))
            Dim param = Expression.Parameter(GetType(TJunction))
            Dim entityForeignKeyExpr = Expression.[Property](param, ExpressionHelper.GetPropertyName(_getEntityForeignKey))
            Dim expr = Expression.Equal(entityForeignKeyExpr, Expression.Constant(primaryKey))
            Return Expression.Lambda(Of Func(Of TJunction, Boolean))(expr, param)
        End Function
        Private Function GetJunctionPredicate(ByVal primaryKey As TPrimaryKey, ByVal detailPrimaryKey As TDetailPrimaryKey) As Expression(Of Func(Of TJunction, Boolean))
            Dim param = Expression.Parameter(GetType(TJunction))
            Dim entityForeignKeyExpr = Expression.[Property](param, ExpressionHelper.GetPropertyName(_getEntityForeignKey))
            Dim expr = Expression.Equal(entityForeignKeyExpr, Expression.Constant(primaryKey))
            Dim detailForeignKeyExpr = Expression.[Property](param, ExpressionHelper.GetPropertyName(_getDetailForeignKey))
            Dim detailEqual = Expression.Equal(detailForeignKeyExpr, Expression.Constant(detailPrimaryKey))
            expr = Expression.[And](expr, detailEqual)
            Return Expression.Lambda(Of Func(Of TJunction, Boolean))(expr, param)
        End Function
        Public Overrides Sub AddDetailEntities()
            Dim availableEntities = DetailsRepository.ToList().Except(DetailEntities).ToArray()
            Dim selectEntitiesViewModel = New SelectDetailEntitiesViewModel(Of TDetailEntity)(availableEntities)
            If DialogService.ShowDialog(MessageButton.OKCancel, CommonResources.AddRemoveDetailEntities_SelectObjects, selectEntitiesViewModel) = MessageResult.OK AndAlso selectEntitiesViewModel.SelectedEntities.Any() Then
                For Each selectedEntity In selectEntitiesViewModel.SelectedEntities
                    Dim junction = New TJunction()
                    Dim entityKey = Repository.GetPrimaryKey(Entity)
                    Dim detailKey = DetailsRepository.GetPrimaryKey(selectedEntity)
                    Dim junctionType = GetType(TJunction)
                    junctionType.GetProperty(ExpressionHelper.GetPropertyName(_getEntityForeignKey)).SetValue(junction, entityKey, Nothing)
                    junctionType.GetProperty(ExpressionHelper.GetPropertyName(_getDetailForeignKey)).SetValue(junction, detailKey, Nothing)
                    JunctionRepository.Add(junction)
                Next
                SaveChangesAndNotify(selectEntitiesViewModel.SelectedEntities)
            End If
        End Sub
        Public Overrides Sub RemoveDetailEntities()
            If Not SelectedEntities.Any() Then
                Return
            End If
            Dim entityKey = Repository.GetPrimaryKey(Entity)
            For Each selectedEntity In SelectedEntities
                Dim detailKey = DetailsRepository.GetPrimaryKey(selectedEntity)
                Dim junction = JunctionRepository.First(GetJunctionPredicate(entityKey, detailKey))
                JunctionRepository.Remove(junction)
            Next
            SaveChangesAndNotify(SelectedEntities)
            SelectedEntities.Clear()
        End Sub
    End Class
End Namespace
