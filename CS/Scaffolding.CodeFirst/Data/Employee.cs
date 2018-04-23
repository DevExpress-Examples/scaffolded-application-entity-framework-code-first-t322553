using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Scaffolding.CodeFirst.Model {
    public class Employee {
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public int DepartmentID { get; set; }
        public virtual Department Department { get; set; }
    }
}