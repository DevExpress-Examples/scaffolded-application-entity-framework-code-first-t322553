Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.ComponentModel
Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.POCO
Imports DevExpressWalkthrough.VB.Common.DataModel
Imports DevExpressWalkthrough.VB.Common.ViewModel
Imports DevExpressWalkthrough.VB.DepartmentContextDataModel
Imports DevExpressWalkthrough.VB
Namespace Global.DevExpressWalkthrough.VB.ViewModels
    ''' <summary>
    ''' Represents the root POCO view model for the DepartmentContext data model.
    ''' </summary>
    Public Partial Class DepartmentContextViewModel
        Inherits DocumentsViewModel(Of DepartmentContextModuleDescription, IDepartmentContextUnitOfWork)
        Private Const _TablesGroup As String = "Tables"
        Private Const _ViewsGroup As String = "Views"
        ''' <summary>
        ''' Creates a new instance of DepartmentContextViewModel as a POCO view model.
        ''' </summary>
        Public Shared Function Create() As DepartmentContextViewModel
            Return ViewModelSource.Create(Function() New DepartmentContextViewModel())
        End Function
        ''' <summary>
        ''' Initializes a new instance of the DepartmentContextViewModel class.
        ''' This constructor is declared protected to avoid undesired instantiation of the DepartmentContextViewModel type without the POCO proxy factory.
        ''' </summary>
        Protected Sub New()
            MyBase.New(UnitOfWorkSource.GetUnitOfWorkFactory())
        End Sub
        Protected Overrides Function CreateModules() As DepartmentContextModuleDescription()
            Return New DepartmentContextModuleDescription() {New DepartmentContextModuleDescription("Courses", "CourseCollectionView", _TablesGroup, GetPeekCollectionViewModelFactory(Function(ByVal x) x.Courses)), New DepartmentContextModuleDescription("Departments", "DepartmentCollectionView", _TablesGroup, GetPeekCollectionViewModelFactory(Function(ByVal x) x.Departments)), New DepartmentContextModuleDescription("Employees", "EmployeeCollectionView", _TablesGroup, GetPeekCollectionViewModelFactory(Function(ByVal x) x.Employees))}
        End Function
    End Class
    Public Partial Class DepartmentContextModuleDescription
        Inherits ModuleDescription(Of DepartmentContextModuleDescription)
        Public Sub New(ByVal title As String, ByVal documentType As String, ByVal group As String, Optional ByVal peekCollectionViewModelFactory As Func(Of DepartmentContextModuleDescription, Object) = Nothing)
            MyBase.New(title, documentType, group, peekCollectionViewModelFactory)
        End Sub
    End Class
End Namespace
