using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpressWalkthrough.Common.Utils;
using DevExpressWalkthrough.DepartmentContextDataModel;
using DevExpressWalkthrough.Common.DataModel;
using DevExpressWalkthrough;
using DevExpressWalkthrough.Common.ViewModel;

namespace DevExpressWalkthrough.ViewModels {
    /// <summary>
    /// Represents the single Department object view model.
    /// </summary>
    public partial class DepartmentViewModel : SingleObjectViewModel<Department, int, IDepartmentContextUnitOfWork> {

        /// <summary>
        /// Creates a new instance of DepartmentViewModel as a POCO view model.
        /// </summary>
        /// <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
        public static DepartmentViewModel Create(IUnitOfWorkFactory<IDepartmentContextUnitOfWork> unitOfWorkFactory = null) {
            return ViewModelSource.Create(() => new DepartmentViewModel(unitOfWorkFactory));
        }

        /// <summary>
        /// Initializes a new instance of the DepartmentViewModel class.
        /// This constructor is declared protected to avoid undesired instantiation of the DepartmentViewModel type without the POCO proxy factory.
        /// </summary>
        /// <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
        protected DepartmentViewModel(IUnitOfWorkFactory<IDepartmentContextUnitOfWork> unitOfWorkFactory = null)
            : base(unitOfWorkFactory ?? UnitOfWorkSource.GetUnitOfWorkFactory(), x => x.Departments, x => x.Name) {
        }

        /// <summary>
        /// The view model for the DepartmentCourses detail collection.
        /// </summary>
        public CollectionViewModel<Course, int, IDepartmentContextUnitOfWork> DepartmentCoursesDetails
        {
            get { return GetDetailsCollectionViewModel((DepartmentViewModel x) => x.DepartmentCoursesDetails, x => x.Courses, x => x.DepartmentID, (x, key) => { x.DepartmentID = key; }); }
        }

        /// <summary>
        /// The view model for the DepartmentEmployees detail collection.
        /// </summary>
        public CollectionViewModel<Employee, int, IDepartmentContextUnitOfWork> DepartmentEmployeesDetails
        {
            get { return GetDetailsCollectionViewModel((DepartmentViewModel x) => x.DepartmentEmployeesDetails, x => x.Employees, x => x.DepartmentID, (x, key) => { x.DepartmentID = key; }); }
        }
    }
}
