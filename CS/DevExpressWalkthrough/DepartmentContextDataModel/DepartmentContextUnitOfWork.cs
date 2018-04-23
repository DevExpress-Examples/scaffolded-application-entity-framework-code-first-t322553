using System;
using System.Linq;
using System.Data;
using System.Linq.Expressions;
using System.Collections.Generic;
using DevExpressWalkthrough.Common.Utils;
using DevExpressWalkthrough.Common.DataModel;
using DevExpressWalkthrough.Common.DataModel.DesignTime;
using DevExpressWalkthrough.Common.DataModel.EntityFramework;
using DevExpressWalkthrough;

namespace DevExpressWalkthrough.DepartmentContextDataModel {
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
