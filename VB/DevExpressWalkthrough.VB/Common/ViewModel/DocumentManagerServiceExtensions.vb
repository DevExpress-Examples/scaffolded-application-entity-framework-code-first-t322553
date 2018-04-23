Imports System
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Linq
Imports System.Linq.Expressions
Imports DevExpress.Mvvm
Namespace Global.DevExpressWalkthrough.VB.Common.ViewModel
    ''' <summary>
    ''' Provides the extension methods that are used to implement the IDocumentManagerService interface.
    ''' </summary>
    Public Module DocumentManagerServiceExtensions
        ''' <summary>
        ''' Creates and shows a document containing a single object view model for the existing entity.
        ''' </summary>
        ''' <param name="documentManagerService">An instance of the IDocumentManager interface used to create and show the document.</param>
        ''' <param name="parentViewModel">An object that is passed to the view model of the created view.</param>
        ''' <param name="primaryKey">An entity primary key.</param>
        <System.Runtime.CompilerServices.Extension> _
        Public Function ShowExistingEntityDocument(Of TEntity, TPrimaryKey)(ByVal documentManagerService As IDocumentManagerService, ByVal parentViewModel As Object, ByVal primaryKey As TPrimaryKey) As IDocument
            Dim document As IDocument = If(FindEntityDocument(Of TEntity, TPrimaryKey)(documentManagerService, primaryKey), CreateDocument(Of TEntity)(documentManagerService, primaryKey, parentViewModel))
            If document IsNot Nothing Then
                document.Show()
            End If
            Return document
        End Function
        ''' <summary>
        ''' Creates and shows a document containing a single object view model for new entity.
        ''' </summary>
        ''' <param name="documentManagerService">An instance of the IDocumentManager interface used to create and show the document.</param>
        ''' <param name="parentViewModel">An object that is passed to the view model of the created view.</param>
        ''' <param name="newEntityInitializer">An optional parameter that provides a function that initializes a new entity.</param>
        <System.Runtime.CompilerServices.Extension> _
        Public Sub ShowNewEntityDocument(Of TEntity)(ByVal documentManagerService As IDocumentManagerService, ByVal parentViewModel As Object, Optional ByVal newEntityInitializer As Action(Of TEntity) = Nothing)
            Dim document As IDocument = CreateDocument(Of TEntity)(documentManagerService, If(newEntityInitializer, (Sub(ByVal x) DefaultEntityInitializer(x))), parentViewModel)
            If document IsNot Nothing Then
                document.Show()
            End If
        End Sub
        ''' <summary>
        ''' Searches for a document that contains a single object view model editing entity with a specified primary key.
        ''' </summary>
        ''' <param name="documentManagerService">An instance of the IDocumentManager interface used to find a document.</param>
        ''' <param name="primaryKey">An entity primary key.</param>
        <System.Runtime.CompilerServices.Extension> _
        Public Function FindEntityDocument(Of TEntity, TPrimaryKey)(ByVal documentManagerService As IDocumentManagerService, ByVal primaryKey As TPrimaryKey) As IDocument
            If documentManagerService Is Nothing Then
                Return Nothing
            End If
            For Each document As IDocument In documentManagerService.Documents
                Dim entityViewModel As ISingleObjectViewModel(Of TEntity, TPrimaryKey) = TryCast(document.Content, ISingleObjectViewModel(Of TEntity, TPrimaryKey))
                If entityViewModel IsNot Nothing AndAlso Object.Equals(entityViewModel.PrimaryKey, primaryKey) Then
                    Return document
                End If
            Next
            Return Nothing
        End Function
        Private Sub DefaultEntityInitializer(Of TEntity)(ByVal entity As TEntity)
        End Sub
        Private Function CreateDocument(Of TEntity)(ByVal documentManagerService As IDocumentManagerService, ByVal parameter As Object, ByVal parentViewModel As Object) As IDocument
            If documentManagerService Is Nothing Then
                Return Nothing
            End If
            Dim document = documentManagerService.CreateDocument(GetDocumentTypeName(Of TEntity)(), parameter, parentViewModel)
            document.Id = "_" + Guid.NewGuid().ToString().Replace("-"C, "_"C)
            document.DestroyOnClose = False
            Return document
        End Function
        Public Function GetDocumentTypeName(Of TEntity)() As String
            Return GetType(TEntity).Name + "View"
        End Function
    End Module
End Namespace
