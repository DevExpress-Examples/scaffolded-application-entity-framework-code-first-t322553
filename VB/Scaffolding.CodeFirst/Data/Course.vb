Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

Namespace Scaffolding.CodeFirst.Model
	Public Class Course
		Public Property CourseID() As Integer
		Public Property Title() As String
		Public Property DepartmentID() As Integer
		Public Overridable Property Department() As Department
	End Class
End Namespace