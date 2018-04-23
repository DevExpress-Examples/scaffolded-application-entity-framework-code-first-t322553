Imports System
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.POCO
Imports DevExpress.Mvvm.DataModel
Imports DevExpress.Mvvm.ViewModel
Imports Scaffolding.CodeFirst.DepartmentContextDataModel
Imports Scaffolding.CodeFirst.Common
Imports Scaffolding.CodeFirst.Model

Namespace Scaffolding.CodeFirst.ViewModels

	''' <summary>
	''' Represents the single Employee object view model.
	''' </summary>
	Partial Public Class EmployeeViewModel
		Inherits SingleObjectViewModel(Of Employee, Integer, IDepartmentContextUnitOfWork)

		''' <summary>
		''' Creates a new instance of EmployeeViewModel as a POCO view model.
		''' </summary>
		''' <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
		Public Shared Function Create(Optional ByVal unitOfWorkFactory As IUnitOfWorkFactory(Of IDepartmentContextUnitOfWork) = Nothing) As EmployeeViewModel
			Return ViewModelSource.Create(Function() New EmployeeViewModel(unitOfWorkFactory))
		End Function

		''' <summary>
		''' Initializes a new instance of the EmployeeViewModel class.
		''' This constructor is declared protected to avoid undesired instantiation of the EmployeeViewModel type without the POCO proxy factory.
		''' </summary>
		''' <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
		Protected Sub New(Optional ByVal unitOfWorkFactory As IUnitOfWorkFactory(Of IDepartmentContextUnitOfWork) = Nothing)
			MyBase.New(If(unitOfWorkFactory, UnitOfWorkSource.GetUnitOfWorkFactory()), Function(x) x.Employees, Function(x) x.Name)
		End Sub


		''' <summary>
		''' The view model that contains a look-up collection of Departments for the corresponding navigation property in the view.
		''' </summary>
		Public ReadOnly Property LookUpDepartments() As IEntitiesViewModel(Of Department)
			Get
				Return GetLookUpEntitiesViewModel(Function(x As EmployeeViewModel) x.LookUpDepartments, Function(x) x.Departments)
			End Get
		End Property

	End Class
End Namespace
