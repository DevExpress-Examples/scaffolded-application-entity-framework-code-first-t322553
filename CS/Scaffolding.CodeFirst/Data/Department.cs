using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Scaffolding.CodeFirst.Model {
    public class Department {
        public Department() {
            Courses = new HashSet<Course>();
        }
        public int DepartmentID { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
    }
}