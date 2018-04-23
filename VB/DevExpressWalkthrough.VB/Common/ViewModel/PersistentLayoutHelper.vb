Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text
Imports DevExpress.Mvvm
Namespace Global.DevExpressWalkthrough.VB.Common.ViewModel
    Public Class PersistentLayoutHelper
        Public Shared Property PersistentLogicalLayout As String
            Get
                Return LayoutSettings.[Default].LogicalLayout
            End Get
            Set
                LayoutSettings.[Default].LogicalLayout = value
            End Set
        End Property
        Private Shared _persistentViewsLayout As Dictionary(Of String, String)
        Public Shared ReadOnly Property PersistentViewsLayout As Dictionary(Of String, String)
            Get
                If _persistentViewsLayout Is Nothing Then
                    _persistentViewsLayout = LogicalLayoutSerializationHelper.Deserialize(LayoutSettings.[Default].ViewsLayout)
                End If
                Return _persistentViewsLayout
            End Get
        End Property
        Public Shared Sub TryDeserializeLayout(ByVal service As ILayoutSerializationService, ByVal viewName As String)
            Dim state As String = Nothing
            If service IsNot Nothing AndAlso PersistentLayoutHelper.PersistentViewsLayout.TryGetValue(viewName, state) Then
                service.Deserialize(state)
            End If
        End Sub
        Public Shared Sub TrySerializeLayout(ByVal service As ILayoutSerializationService, ByVal viewName As String)
            If service IsNot Nothing Then
                PersistentLayoutHelper.PersistentViewsLayout(viewName) = service.Serialize()
            End If
        End Sub
        Public Shared Sub SaveLayout()
            LayoutSettings.[Default].ViewsLayout = LogicalLayoutSerializationHelper.Serialize(PersistentViewsLayout)
            LayoutSettings.[Default].Save()
        End Sub
        Public Shared Sub ResetLayout()
            PersistentViewsLayout.Clear()
            PersistentLogicalLayout = Nothing
            SaveLayout()
        End Sub
    End Class
End Namespace
