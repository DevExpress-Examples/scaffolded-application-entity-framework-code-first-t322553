Imports System.Data.Entity

Namespace Scaffolding.CodeFirst.Model
	Public Class DepartmentContext
		Inherits DbContext

		Public Property Departments() As DbSet(Of Department)
		Public Property Courses() As DbSet(Of Course)
		Public Property Employees() As DbSet(Of Employee)
	End Class
End Namespace