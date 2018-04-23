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
    Public Class AddRemoveDetailEntitiesViewModel(Of TEntity As Class, TPrimaryKey, TDetailEntity As Class, TDetailPrimaryKey, TUnitOfWork As IUnitOfWork)
        Inherits SingleObjectViewModelBase(Of TEntity, TPrimaryKey, TUnitOfWork)
        Private _SelectedEntities As ObservableCollection(Of TDetailEntity)
        Public Shared Function Create(ByVal unitOfWorkFactory As IUnitOfWorkFactory(Of TUnitOfWork), ByVal getRepositoryFunc As Func(Of TUnitOfWork, IRepository(Of TEntity, TPrimaryKey)), ByVal getDetailsRepositoryFunc As Func(Of TUnitOfWork, IRepository(Of TDetailEntity, TDetailPrimaryKey)), ByVal getDetailsFunc As Func(Of TEntity, ICollection(Of TDetailEntity)), ByVal id As TPrimaryKey) As AddRemoveDetailEntitiesViewModel(Of TEntity, TPrimaryKey, TDetailEntity, TDetailPrimaryKey, TUnitOfWork)
            Return ViewModelSource.Create(Function() New AddRemoveDetailEntitiesViewModel(Of TEntity, TPrimaryKey, TDetailEntity, TDetailPrimaryKey, TUnitOfWork)(unitOfWorkFactory, getRepositoryFunc, getDetailsRepositoryFunc, getDetailsFunc, id))
        End Function
        Protected ReadOnly getDetailsRepositoryFunc As Func(Of TUnitOfWork, IRepository(Of TDetailEntity, TDetailPrimaryKey))
        Private ReadOnly _getDetailsFunc As Func(Of TEntity, ICollection(Of TDetailEntity))
        Protected ReadOnly Property DialogService As IDialogService
            Get
                Return Me.GetRequiredService(Of IDialogService)()
            End Get
        End Property
        Protected ReadOnly Property DocumentManagerService As IDocumentManagerService
            Get
                Return Me.GetRequiredService(Of IDocumentManagerService)()
            End Get
        End Property
        Private ReadOnly Property DetailsRepository As IRepository(Of TDetailEntity, TDetailPrimaryKey)
            Get
                Return getDetailsRepositoryFunc(UnitOfWork)
            End Get
        End Property
        Public Overridable ReadOnly Property DetailEntities As ICollection(Of TDetailEntity)
            Get
                Return If(Entity IsNot Nothing, _getDetailsFunc(Entity), Enumerable.Empty(Of TDetailEntity)().ToArray())
            End Get
        End Property
        Public ReadOnly Property SelectedEntities As ObservableCollection(Of TDetailEntity)
            Get
                Return _SelectedEntities
            End Get
        End Property
        Public Overridable ReadOnly Property IsCreateDetailButtonVisible As Boolean
            Get
                Return True
            End Get
        End Property
        Protected Sub New(ByVal unitOfWorkFactory As IUnitOfWorkFactory(Of TUnitOfWork), ByVal getRepositoryFunc As Func(Of TUnitOfWork, IRepository(Of TEntity, TPrimaryKey)), ByVal getDetailsRepositoryFunc As Func(Of TUnitOfWork, IRepository(Of TDetailEntity, TDetailPrimaryKey)), ByVal getDetailsFunc As Func(Of TEntity, ICollection(Of TDetailEntity)), ByVal id As TPrimaryKey)
            MyBase.New(unitOfWorkFactory, getRepositoryFunc, Nothing)
            Me.getDetailsRepositoryFunc = getDetailsRepositoryFunc
            Me._getDetailsFunc = getDetailsFunc
            _SelectedEntities = New ObservableCollection(Of TDetailEntity)()
            If Me.IsInDesignMode() Then
                Return
            End If
            LoadEntityByKey(id)
            Messenger.[Default].Register(Me, Sub(ByVal m As EntityMessage(Of TDetailEntity, TDetailPrimaryKey))
                                                 If m.MessageType <> EntityMessageType.Added Then
                                                     Return
                                                 End If
                                                 Dim withParent = TryCast(m.Sender, ISupportParentViewModel)
                                                 If withParent Is Nothing OrElse withParent.ParentViewModel <> Me Then
                                                     Return
                                                 End If
                                                 Dim withEntity = TryCast(m.Sender, SingleObjectViewModel(Of TDetailEntity, TDetailPrimaryKey, TUnitOfWork))
                                                 Dim detailEntity = DetailsRepository.Find(DetailsRepository.GetPrimaryKey(withEntity.Entity))
                                                 If detailEntity Is Nothing Then
                                                     Return
                                                 End If
                                                 DetailEntities.Add(detailEntity)
                                                 SaveChangesAndNotify(New TDetailEntity() {detailEntity})
                                             End Sub)
        End Sub
        Public Overridable Sub CreateDetailEntity()
            DocumentManagerService.ShowNewEntityDocument(Of TDetailEntity)(Me)
        End Sub
        Public Overridable Sub EditDetailEntity()
            If SelectedEntities.Any() Then
                Dim detailKey = DetailsRepository.GetPrimaryKey(SelectedEntities.First())
                DocumentManagerService.ShowExistingEntityDocument(Of TDetailEntity, TDetailPrimaryKey)(Me, detailKey)
            End If
        End Sub
        Protected Overrides Sub OnInitializeInRuntime()
            MyBase.OnInitializeInRuntime()
            Messenger.[Default].Register(Of EntityMessage(Of TEntity, TPrimaryKey))(Me, Sub(ByVal m) OnMessage(m))
        End Sub
        Public Overridable Sub AddDetailEntities()
            Dim availalbleEntities = DetailsRepository.ToList().Except(DetailEntities).ToArray()
            Dim selectEntitiesViewModel = New SelectDetailEntitiesViewModel(Of TDetailEntity)(availalbleEntities)
            If DialogService.ShowDialog(MessageButton.OKCancel, CommonResources.AddRemoveDetailEntities_SelectObjects, selectEntitiesViewModel) = MessageResult.OK AndAlso selectEntitiesViewModel.SelectedEntities.Any() Then
                For Each selectedEntity In selectEntitiesViewModel.SelectedEntities
                    DetailEntities.Add(selectedEntity)
                Next
                SaveChangesAndNotify(selectEntitiesViewModel.SelectedEntities)
            End If
        End Sub
        Public Function CanAddDetailEntities() As Boolean
            Return Entity IsNot Nothing
        End Function
        Public Overridable Sub RemoveDetailEntities()
            If Not SelectedEntities.Any() Then
                Return
            End If
            For Each selectedEntity In SelectedEntities
                DetailEntities.Remove(selectedEntity)
            Next
            SaveChangesAndNotify(SelectedEntities)
            SelectedEntities.Clear()
        End Sub
        Public Function CanRemoveDetailEntities() As Boolean
            Return Entity IsNot Nothing
        End Function
        Protected Sub SaveChangesAndNotify(ByVal detailEntities As IEnumerable(Of TDetailEntity))
            Try
                UnitOfWork.SaveChanges()
                For Each detailEntity In detailEntities
                    Messenger.[Default].Send(New EntityMessage(Of TDetailEntity, TDetailPrimaryKey)(DetailsRepository.GetPrimaryKey(detailEntity), EntityMessageType.Changed))
                Next
                Reload()
            Catch e As DbException
                MessageBoxService.ShowMessage(e.ErrorMessage, e.ErrorCaption, MessageButton.OK, MessageIcon.[Error])
            End Try
        End Sub
        Private Sub OnMessage(ByVal message As EntityMessage(Of TEntity, TPrimaryKey))
            If message.MessageType = EntityMessageType.Changed AndAlso Entity IsNot Nothing AndAlso Object.Equals(PrimaryKey, message.PrimaryKey) Then
                Reload()
            End If
        End Sub
        Protected Overrides Sub OnEntityChanged()
            MyBase.OnEntityChanged()
            Me.RaisePropertyChanged(Function(ByVal x) x.DetailEntities)
        End Sub
    End Class
    Public Class SelectDetailEntitiesViewModel(Of TEntity As Class)
        Private _SelectedEntities As List(Of TEntity)
        Private _AvailableEntities As TEntity()
        Public Sub New(ByVal availableCourses As TEntity())
            _AvailableEntities = availableCourses
            _SelectedEntities = New List(Of TEntity)()
        End Sub
        Public ReadOnly Property AvailableEntities As TEntity()
            Get
                Return _AvailableEntities
            End Get
        End Property
        Public ReadOnly Property SelectedEntities As List(Of TEntity)
            Get
                Return _SelectedEntities
            End Get
        End Property
    End Class
End Namespace
