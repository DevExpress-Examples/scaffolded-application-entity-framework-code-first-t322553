Imports DevExpress.Mvvm.DataModel
Imports Scaffolding.CodeFirst.Model
Imports System
Imports System.Collections.Generic
Imports System.Linq

Namespace Scaffolding.CodeFirst.DepartmentContextDataModel

	''' <summary>
	''' IDepartmentContextUnitOfWork extends the IUnitOfWork interface with repositories representing specific entities.
	''' </summary>
	Public Interface IDepartmentContextUnitOfWork
		Inherits IUnitOfWork

		''' <summary>
		''' The Course entities repository.
		''' </summary>
		ReadOnly Property Courses() As IRepository(Of Course, Integer)

		''' <summary>
		''' The Department entities repository.
		''' </summary>
		ReadOnly Property Departments() As IRepository(Of Department, Integer)

		''' <summary>
		''' The Employee entities repository.
		''' </summary>
		ReadOnly Property Employees() As IRepository(Of Employee, Integer)
	End Interface
End Namespace
