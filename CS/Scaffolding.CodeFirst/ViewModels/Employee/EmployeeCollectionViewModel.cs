using System;
using System.Linq;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.DataModel;
using DevExpress.Mvvm.ViewModel;
using Scaffolding.CodeFirst.DepartmentContextDataModel;
using Scaffolding.CodeFirst.Common;
using Scaffolding.CodeFirst.Model;

namespace Scaffolding.CodeFirst.ViewModels {

    /// <summary>
    /// Represents the Employees collection view model.
    /// </summary>
    public partial class EmployeeCollectionViewModel : CollectionViewModel<Employee, int, IDepartmentContextUnitOfWork> {

        /// <summary>
        /// Creates a new instance of EmployeeCollectionViewModel as a POCO view model.
        /// </summary>
        /// <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
        public static EmployeeCollectionViewModel Create(IUnitOfWorkFactory<IDepartmentContextUnitOfWork> unitOfWorkFactory = null) {
            return ViewModelSource.Create(() => new EmployeeCollectionViewModel(unitOfWorkFactory));
        }

        /// <summary>
        /// Initializes a new instance of the EmployeeCollectionViewModel class.
        /// This constructor is declared protected to avoid undesired instantiation of the EmployeeCollectionViewModel type without the POCO proxy factory.
        /// </summary>
        /// <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
        protected EmployeeCollectionViewModel(IUnitOfWorkFactory<IDepartmentContextUnitOfWork> unitOfWorkFactory = null)
            : base(unitOfWorkFactory ?? UnitOfWorkSource.GetUnitOfWorkFactory(), x => x.Employees) {
        }
    }
}