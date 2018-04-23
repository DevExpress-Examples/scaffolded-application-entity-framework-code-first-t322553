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
