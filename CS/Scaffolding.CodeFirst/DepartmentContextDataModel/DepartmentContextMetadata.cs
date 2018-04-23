using DevExpress.Mvvm.DataAnnotations;
using Scaffolding.CodeFirst.Localization;
using Scaffolding.CodeFirst.Model;

namespace Scaffolding.CodeFirst.DepartmentContextDataModel {

    public class DepartmentContextMetadataProvider {
        public static void BuildMetadata(MetadataBuilder<Course> builder) {
            builder.DisplayName(DepartmentContextResources.Course);
            builder.Property(x => x.CourseID).DisplayName(DepartmentContextResources.Course_CourseID);
            builder.Property(x => x.Title).DisplayName(DepartmentContextResources.Course_Title);
            builder.Property(x => x.Department).DisplayName(DepartmentContextResources.Course_Department);
        }
        public static void BuildMetadata(MetadataBuilder<Department> builder) {
            builder.DisplayName(DepartmentContextResources.Department);
            builder.Property(x => x.DepartmentID).DisplayName(DepartmentContextResources.Department_DepartmentID);
            builder.Property(x => x.Name).DisplayName(DepartmentContextResources.Department_Name);
        }
        public static void BuildMetadata(MetadataBuilder<Employee> builder) {
            builder.DisplayName(DepartmentContextResources.Employee);
            builder.Property(x => x.EmployeeID).DisplayName(DepartmentContextResources.Employee_EmployeeID);
            builder.Property(x => x.Name).DisplayName(DepartmentContextResources.Employee_Name);
            builder.Property(x => x.Department).DisplayName(DepartmentContextResources.Employee_Department);
        }
    }
}