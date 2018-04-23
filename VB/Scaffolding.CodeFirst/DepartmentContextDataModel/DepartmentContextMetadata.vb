Imports DevExpress.Mvvm.DataAnnotations
Imports Scaffolding.CodeFirst.Localization
Imports Scaffolding.CodeFirst.Model

Namespace Scaffolding.CodeFirst.DepartmentContextDataModel

	Public Class DepartmentContextMetadataProvider
		Public Shared Sub BuildMetadata(ByVal builder As MetadataBuilder(Of Course))
			builder.DisplayName(DepartmentContextResources.Course)
			builder.Property(Function(x) x.CourseID).DisplayName(DepartmentContextResources.Course_CourseID)
			builder.Property(Function(x) x.Title).DisplayName(DepartmentContextResources.Course_Title)
			builder.Property(Function(x) x.Department).DisplayName(DepartmentContextResources.Course_Department)
		End Sub
		Public Shared Sub BuildMetadata(ByVal builder As MetadataBuilder(Of Department))
			builder.DisplayName(DepartmentContextResources.Department)
			builder.Property(Function(x) x.DepartmentID).DisplayName(DepartmentContextResources.Department_DepartmentID)
			builder.Property(Function(x) x.Name).DisplayName(DepartmentContextResources.Department_Name)
		End Sub
		Public Shared Sub BuildMetadata(ByVal builder As MetadataBuilder(Of Employee))
			builder.DisplayName(DepartmentContextResources.Employee)
			builder.Property(Function(x) x.EmployeeID).DisplayName(DepartmentContextResources.Employee_EmployeeID)
			builder.Property(Function(x) x.Name).DisplayName(DepartmentContextResources.Employee_Name)
			builder.Property(Function(x) x.Department).DisplayName(DepartmentContextResources.Employee_Department)
		End Sub
	End Class
End Namespace