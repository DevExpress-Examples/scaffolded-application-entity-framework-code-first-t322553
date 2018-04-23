Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

Namespace Scaffolding.CodeFirst.Model
	Public Class Employee
		Public Property EmployeeID() As Integer
		Public Property Name() As String
		Public Property DepartmentID() As Integer
		Public Overridable Property Department() As Department
	End Class
End Namespace