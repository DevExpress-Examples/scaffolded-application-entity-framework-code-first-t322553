Imports DevExpress.Mvvm.DataModel
Imports DevExpress.Mvvm.DataModel.DesignTime
Imports Scaffolding.CodeFirst.Model
Imports System
Imports System.Collections.Generic
Imports System.Linq

Namespace Scaffolding.CodeFirst.DepartmentContextDataModel

	''' <summary>
	''' A DepartmentContextDesignTimeUnitOfWork instance that represents the design-time implementation of the IDepartmentContextUnitOfWork interface.
	''' </summary>
	Public Class DepartmentContextDesignTimeUnitOfWork
		Inherits DesignTimeUnitOfWork
		Implements IDepartmentContextUnitOfWork

		''' <summary>
		''' Initializes a new instance of the DepartmentContextDesignTimeUnitOfWork class.
		''' </summary>
		Public Sub New()
		End Sub

		Private ReadOnly Property IDepartmentContextUnitOfWork_Courses() As IRepository(Of Course, Integer) Implements IDepartmentContextUnitOfWork.Courses
			Get
				Return GetRepository(Function(x As Course) x.CourseID)
			End Get
		End Property

		Private ReadOnly Property IDepartmentContextUnitOfWork_Departments() As IRepository(Of Department, Integer) Implements IDepartmentContextUnitOfWork.Departments
			Get
				Return GetRepository(Function(x As Department) x.DepartmentID)
			End Get
		End Property

		Private ReadOnly Property IDepartmentContextUnitOfWork_Employees() As IRepository(Of Employee, Integer) Implements IDepartmentContextUnitOfWork.Employees
			Get
				Return GetRepository(Function(x As Employee) x.EmployeeID)
			End Get
		End Property
	End Class
End Namespace
