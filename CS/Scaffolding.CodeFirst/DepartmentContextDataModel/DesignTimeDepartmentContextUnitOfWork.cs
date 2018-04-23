using DevExpress.Mvvm.DataModel;
using DevExpress.Mvvm.DataModel.DesignTime;
using Scaffolding.CodeFirst.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scaffolding.CodeFirst.DepartmentContextDataModel {

    /// <summary>
    /// A DepartmentContextDesignTimeUnitOfWork instance that represents the design-time implementation of the IDepartmentContextUnitOfWork interface.
    /// </summary>
    public class DepartmentContextDesignTimeUnitOfWork : DesignTimeUnitOfWork, IDepartmentContextUnitOfWork {

        /// <summary>
        /// Initializes a new instance of the DepartmentContextDesignTimeUnitOfWork class.
        /// </summary>
        public DepartmentContextDesignTimeUnitOfWork() {
        }

        IRepository<Course, int> IDepartmentContextUnitOfWork.Courses
        {
            get { return GetRepository((Course x) => x.CourseID); }
        }

        IRepository<Department, int> IDepartmentContextUnitOfWork.Departments
        {
            get { return GetRepository((Department x) => x.DepartmentID); }
        }

        IRepository<Employee, int> IDepartmentContextUnitOfWork.Employees
        {
            get { return GetRepository((Employee x) => x.EmployeeID); }
        }
    }
}
