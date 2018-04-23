using DevExpress.Mvvm.DataModel;
using DevExpress.Mvvm.DataModel.EF6;
using Scaffolding.CodeFirst.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scaffolding.CodeFirst.DepartmentContextDataModel {

    /// <summary>
    /// A DepartmentContextUnitOfWork instance that represents the run-time implementation of the IDepartmentContextUnitOfWork interface.
    /// </summary>
    public class DepartmentContextUnitOfWork : DbUnitOfWork<DepartmentContext>, IDepartmentContextUnitOfWork {

        public DepartmentContextUnitOfWork(Func<DepartmentContext> contextFactory)
            : base(contextFactory) {
        }

        IRepository<Course, int> IDepartmentContextUnitOfWork.Courses
        {
            get { return GetRepository(x => x.Set<Course>(), (Course x) => x.CourseID); }
        }

        IRepository<Department, int> IDepartmentContextUnitOfWork.Departments
        {
            get { return GetRepository(x => x.Set<Department>(), (Department x) => x.DepartmentID); }
        }

        IRepository<Employee, int> IDepartmentContextUnitOfWork.Employees
        {
            get { return GetRepository(x => x.Set<Employee>(), (Employee x) => x.EmployeeID); }
        }
    }
}
