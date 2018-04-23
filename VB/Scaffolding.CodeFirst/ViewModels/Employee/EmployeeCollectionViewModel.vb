Imports System
Imports System.Linq
Imports DevExpress.Mvvm.POCO
Imports DevExpress.Mvvm.DataModel
Imports DevExpress.Mvvm.ViewModel
Imports Scaffolding.CodeFirst.DepartmentContextDataModel
Imports Scaffolding.CodeFirst.Common
Imports Scaffolding.CodeFirst.Model

Namespace Scaffolding.CodeFirst.ViewModels

	''' <summary>
	''' Represents the Employees collection view model.
	''' </summary>
	Partial Public Class EmployeeCollectionViewModel
		Inherits CollectionViewModel(Of Employee, Integer, IDepartmentContextUnitOfWork)

		''' <summary>
		''' Creates a new instance of EmployeeCollectionViewModel as a POCO view model.
		''' </summary>
		''' <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
		Public Shared Function Create(Optional ByVal unitOfWorkFactory As IUnitOfWorkFactory(Of IDepartmentContextUnitOfWork) = Nothing) As EmployeeCollectionViewModel
			Return ViewModelSource.Create(Function() New EmployeeCollectionViewModel(unitOfWorkFactory))
		End Function

		''' <summary>
		''' Initializes a new instance of the EmployeeCollectionViewModel class.
		''' This constructor is declared protected to avoid undesired instantiation of the EmployeeCollectionViewModel type without the POCO proxy factory.
		''' </summary>
		''' <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
		Protected Sub New(Optional ByVal unitOfWorkFactory As IUnitOfWorkFactory(Of IDepartmentContextUnitOfWork) = Nothing)
			MyBase.New(If(unitOfWorkFactory, UnitOfWorkSource.GetUnitOfWorkFactory()), Function(x) x.Employees)
		End Sub
	End Class
End Namespace