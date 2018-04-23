Imports System
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.POCO
Imports DevExpressWalkthrough.VB.Common.Utils
Imports DevExpressWalkthrough.VB.DepartmentContextDataModel
Imports DevExpressWalkthrough.VB.Common.DataModel
Imports DevExpressWalkthrough.VB
Imports DevExpressWalkthrough.VB.Common.ViewModel
Namespace Global.DevExpressWalkthrough.VB.ViewModels
    ''' <summary>
    ''' Represents the single Course object view model.
    ''' </summary>
    Public Partial Class CourseViewModel
        Inherits SingleObjectViewModel(Of Course, Integer, IDepartmentContextUnitOfWork)
        ''' <summary>
        ''' Creates a new instance of CourseViewModel as a POCO view model.
        ''' </summary>
        ''' <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
        Public Shared Function Create(Optional ByVal unitOfWorkFactory As IUnitOfWorkFactory(Of IDepartmentContextUnitOfWork) = Nothing) As CourseViewModel
            Return ViewModelSource.Create(Function() New CourseViewModel(unitOfWorkFactory))
        End Function
        ''' <summary>
        ''' Initializes a new instance of the CourseViewModel class.
        ''' This constructor is declared protected to avoid undesired instantiation of the CourseViewModel type without the POCO proxy factory.
        ''' </summary>
        ''' <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
        Protected Sub New(Optional ByVal unitOfWorkFactory As IUnitOfWorkFactory(Of IDepartmentContextUnitOfWork) = Nothing)
            MyBase.New(If(unitOfWorkFactory, UnitOfWorkSource.GetUnitOfWorkFactory()), Function(ByVal x) x.Courses, Function(ByVal x) x.Title)
        End Sub
        ''' <summary>
        ''' The view model that contains a look-up collection of Departments for the corresponding navigation property in the view.
        ''' </summary>
        Public ReadOnly Property LookUpDepartments As IEntitiesViewModel(Of Department)
            Get
                Return GetLookUpEntitiesViewModel(Function(ByVal x As CourseViewModel) x.LookUpDepartments, Function(ByVal x) x.Departments)
            End Get
        End Property
    End Class
End Namespace
