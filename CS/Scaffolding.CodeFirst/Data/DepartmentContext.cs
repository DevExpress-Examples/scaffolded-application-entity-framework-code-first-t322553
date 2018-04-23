using System.Data.Entity;

namespace Scaffolding.CodeFirst.Model {
    public class DepartmentContext : DbContext {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Employee> Employees { get; set; }
    }
}