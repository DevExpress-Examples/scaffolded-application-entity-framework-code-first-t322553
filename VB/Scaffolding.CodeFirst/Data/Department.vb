Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

Namespace Scaffolding.CodeFirst.Model
	Public Class Department
		Public Sub New()
			Courses = New HashSet(Of Course)()
		End Sub
		Public Property DepartmentID() As Integer
		Public Property Name() As String
		Public Overridable Property Courses() As ICollection(Of Course)
	End Class
End Namespace