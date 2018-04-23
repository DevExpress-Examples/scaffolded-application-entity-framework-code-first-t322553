Imports System
Imports System.Linq
Imports System.Data
Imports System.Linq.Expressions
Imports System.Collections.Generic
Imports DevExpressWalkthrough.VB.Common.Utils
Imports DevExpressWalkthrough.VB.Common.DataModel
Imports DevExpress.Mvvm
Imports System.Collections
Imports System.ComponentModel
Imports DevExpress.Data.Linq
Imports DevExpress.Data.Linq.Helpers
Imports DevExpress.Data.Async.Helpers
Namespace Global.DevExpressWalkthrough.VB.Common.DataModel.DesignTime
    Friend Class DesignTimeUnitOfWorkFactory(Of TUnitOfWork As IUnitOfWork)
        Implements IUnitOfWorkFactory(Of TUnitOfWork)
        Private _getUnitOfWork As Func(Of TUnitOfWork)
        Public Sub New(ByVal getUnitOfWork As Func(Of TUnitOfWork))
            Me._getUnitOfWork = getUnitOfWork
        End Sub
        Private Function CreateUnitOfWork() As TUnitOfWork Implements IUnitOfWorkFactory(Of TUnitOfWork).CreateUnitOfWork
            Return _getUnitOfWork()
        End Function
        Private Function CreateInstantFeedbackSource(Of TEntity As {Class, New}, TProjection As Class, TPrimaryKey)(ByVal getRepositoryFunc As Func(Of TUnitOfWork, IRepository(Of TEntity, TPrimaryKey)), ByVal projection As Func(Of IRepositoryQuery(Of TEntity), IQueryable(Of TProjection))) As IInstantFeedbackSource(Of TProjection) Implements IUnitOfWorkFactory(Of TUnitOfWork).CreateInstantFeedbackSource
            Return New DesignTimeInstantFeedbackSource(Of TProjection, TPrimaryKey)()
        End Function
    End Class
End Namespace
