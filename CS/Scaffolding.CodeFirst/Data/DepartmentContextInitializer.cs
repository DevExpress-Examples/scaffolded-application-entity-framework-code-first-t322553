using System.Data.Entity;

namespace Scaffolding.CodeFirst.Model {
    public class DepartmentContextInitializer : DropCreateDatabaseIfModelChanges<DepartmentContext> {
        protected override void Seed(DepartmentContext context) {
            base.Seed(context);

            Department department1 = new Department { Name = "Music" };
            Department department2 = new Department { Name = "Journalism" };
            Department department3 = new Department { Name = "Management" };
            context.Departments.Add(department1);
            context.Departments.Add(department2);
            context.Departments.Add(department3);

            context.Courses.AddRange(new[] {
                new Course { Title = "First", Department = department1 },
                new Course { Title = "Second", Department = department1 },
                new Course { Title = "Third", Department = department1 },

                new Course { Title = "First", Department = department2 },
                new Course { Title = "Second", Department = department2 },
                new Course { Title = "Third", Department = department2 },

                new Course { Title = "First", Department = department3 },
                new Course { Title = "Second", Department = department3 },
                new Course { Title = "Third", Department = department3 },
            });

            context.Employees.AddRange(new[] {
                new Employee { Name = "Frankie West PhD", Department = department1 },
                new Employee { Name = "Jett Mitchell", Department = department1 },
                new Employee { Name = "Garrick Stiedemann DVM", Department = department1 },
                new Employee { Name = "Hettie Runte", Department = department1 },
                new Employee { Name = "Gabe Flatley", Department = department2 },
                new Employee { Name = "Zetta Beatty", Department = department2 },
                new Employee { Name = "Ms. Luis Jewess", Department = department2 },
                new Employee { Name = "Jefferey Legros III", Department = department3 },
                new Employee { Name = "Margaretta Roberts", Department = department3 },
            });

            context.SaveChanges();
        }
    }
}