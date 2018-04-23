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
    Friend Class DesignTimeInstantFeedbackSource(Of TProjection As Class, TPrimaryKey)
        Implements IInstantFeedbackSource(Of TProjection)
        Private Sub Refresh() Implements IInstantFeedbackSource(Of TProjection).Refresh
        End Sub
        Private Function IsLoadedProxy(ByVal threadSafeProxy As Object) As Boolean Implements IInstantFeedbackSource(Of TProjection).IsLoadedProxy
            Return True
        End Function
        Private ReadOnly Property ContainsListCollection As Boolean Implements IListSource.ContainsListCollection
            Get
                Return True
            End Get
        End Property
        Private Function GetList() As IList Implements IListSource.GetList
            Return DesignTimeHelper.CreateDesignTimeObjects(Of TProjection)(2).ToList()
        End Function
        Private Function GetPropertyValue(Of TProperty)(ByVal threadSafeProxy As Object, ByVal propertyExpression As Expression(Of Func(Of TProjection, TProperty))) As TProperty Implements IInstantFeedbackSource(Of TProjection).GetPropertyValue
            Return CType(Nothing, TProperty)
        End Function
    End Class
End Namespace
