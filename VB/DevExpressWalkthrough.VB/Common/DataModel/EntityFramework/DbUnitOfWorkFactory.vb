Imports System
Imports System.Linq
Imports System.Data
Imports System.Linq.Expressions
Imports System.Collections.Generic
Imports DevExpressWalkthrough.VB.Common.Utils
Imports DevExpressWalkthrough.VB.Common.DataModel
Imports DevExpressWalkthrough.VB.Common.DataModel.DesignTime
Imports DevExpress.Mvvm
Imports System.Collections
Imports System.ComponentModel
Imports DevExpress.Data.Linq
Imports DevExpress.Data.Linq.Helpers
Imports DevExpress.Data.Async.Helpers
Imports DevExpress.Xpf.Core.ServerMode
Namespace Global.DevExpressWalkthrough.VB.Common.DataModel.EntityFramework
    Friend Class DbUnitOfWorkFactory(Of TUnitOfWork As IUnitOfWork)
        Implements IUnitOfWorkFactory(Of TUnitOfWork)
        Private _createUnitOfWork As Func(Of TUnitOfWork)
        Public Sub New(ByVal createUnitOfWork As Func(Of TUnitOfWork))
            Me._createUnitOfWork = createUnitOfWork
        End Sub
        Private Function CreateUnitOfWork() As TUnitOfWork Implements IUnitOfWorkFactory(Of TUnitOfWork).CreateUnitOfWork
            Return _createUnitOfWork()
        End Function
        Private Function CreateInstantFeedbackSource(Of TEntity As {Class, New}, TProjection As Class, TPrimaryKey)(ByVal getRepositoryFunc As Func(Of TUnitOfWork, IRepository(Of TEntity, TPrimaryKey)), ByVal projection As Func(Of IRepositoryQuery(Of TEntity), IQueryable(Of TProjection))) As IInstantFeedbackSource(Of TProjection) Implements IUnitOfWorkFactory(Of TUnitOfWork).CreateInstantFeedbackSource
            Dim threadSafeProperties = New TypeInfoProxied(TypeDescriptor.GetProperties(GetType(TProjection)), Nothing).UIDescriptors
            If projection Is Nothing Then
                projection = Function(ByVal x) TryCast(x, IQueryable(Of TProjection))
            End If
            Dim keyProperties = ExpressionHelper.GetKeyProperties(getRepositoryFunc(_createUnitOfWork()).GetPrimaryKeyExpression)
            Dim keyExpression = keyProperties.[Select](Function(ByVal p) p.Name).Aggregate(Function(ByVal l, ByVal r) l + ";" + r)
            Dim source = New EntityInstantFeedbackSource(Sub(ByVal e As DevExpress.Data.Linq.GetQueryableEventArgs) e.QueryableSource = projection(getRepositoryFunc(_createUnitOfWork()))) With {.KeyExpression = keyExpression}
            Return New InstantFeedbackSource(Of TProjection)(source, threadSafeProperties)
        End Function
    End Class
End Namespace
