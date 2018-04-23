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
    /// Represents the Courses collection view model.
    /// </summary>
    public partial class CourseCollectionViewModel : CollectionViewModel<Course, int, IDepartmentContextUnitOfWork> {

        /// <summary>
        /// Creates a new instance of CourseCollectionViewModel as a POCO view model.
        /// </summary>
        /// <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
        public static CourseCollectionViewModel Create(IUnitOfWorkFactory<IDepartmentContextUnitOfWork> unitOfWorkFactory = null) {
            return ViewModelSource.Create(() => new CourseCollectionViewModel(unitOfWorkFactory));
        }

        /// <summary>
        /// Initializes a new instance of the CourseCollectionViewModel class.
        /// This constructor is declared protected to avoid undesired instantiation of the CourseCollectionViewModel type without the POCO proxy factory.
        /// </summary>
        /// <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
        protected CourseCollectionViewModel(IUnitOfWorkFactory<IDepartmentContextUnitOfWork> unitOfWorkFactory = null)
            : base(unitOfWorkFactory ?? UnitOfWorkSource.GetUnitOfWorkFactory(), x => x.Courses) {
        }
    }
}