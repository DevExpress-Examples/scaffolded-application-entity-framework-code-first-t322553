using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.ViewModel;
using Scaffolding.CodeFirst.DepartmentContextDataModel;

namespace Scaffolding.CodeFirst.ViewModels {
    /// <summary>
    /// Represents the root POCO view model for the DepartmentContext data model.
    /// </summary>
    public partial class DepartmentContextViewModel : DocumentsViewModel<DepartmentContextModuleDescription, IDepartmentContextUnitOfWork> {

        const string TablesGroup = "Tables";

        const string ViewsGroup = "Views";

        /// <summary>
        /// Creates a new instance of DepartmentContextViewModel as a POCO view model.
        /// </summary>
        public static DepartmentContextViewModel Create() {
            return ViewModelSource.Create(() => new DepartmentContextViewModel());
        }

        /// <summary>
        /// Initializes a new instance of the DepartmentContextViewModel class.
        /// This constructor is declared protected to avoid undesired instantiation of the DepartmentContextViewModel type without the POCO proxy factory.
        /// </summary>
        protected DepartmentContextViewModel()
            : base(UnitOfWorkSource.GetUnitOfWorkFactory()) {
        }

        protected override DepartmentContextModuleDescription[] CreateModules() {
            return new DepartmentContextModuleDescription[] {
                new DepartmentContextModuleDescription("Courses", "CourseCollectionView", TablesGroup, GetPeekCollectionViewModelFactory(x => x.Courses)),
                new DepartmentContextModuleDescription("Departments", "DepartmentCollectionView", TablesGroup, GetPeekCollectionViewModelFactory(x => x.Departments)),
                new DepartmentContextModuleDescription("Employees", "EmployeeCollectionView", TablesGroup, GetPeekCollectionViewModelFactory(x => x.Employees)),
            };
        }



    }

    public partial class DepartmentContextModuleDescription : ModuleDescription<DepartmentContextModuleDescription> {
        public DepartmentContextModuleDescription(string title, string documentType, string group, Func<DepartmentContextModuleDescription, object> peekCollectionViewModelFactory = null)
            : base(title, documentType, group, peekCollectionViewModelFactory) {
        }
    }
}