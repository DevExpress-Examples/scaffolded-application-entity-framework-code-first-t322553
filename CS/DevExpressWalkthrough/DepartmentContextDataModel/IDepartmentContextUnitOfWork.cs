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
    /// IDepartmentContextUnitOfWork extends the IUnitOfWork interface with repositories representing specific entities.
    /// </summary>
    public interface IDepartmentContextUnitOfWork : IUnitOfWork {

        /// <summary>
        /// The Course entities repository.
        /// </summary>
        IRepository<Course, int> Courses { get; }

        /// <summary>
        /// The Department entities repository.
        /// </summary>
        IRepository<Department, int> Departments { get; }

        /// <summary>
        /// The Employee entities repository.
        /// </summary>
        IRepository<Employee, int> Employees { get; }
    }
}
