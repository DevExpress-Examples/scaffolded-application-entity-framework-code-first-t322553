using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace DevExpressWalkthrough {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            Database.SetInitializer<DepartmentContext>(new DepartmentContextInitializer());
            base.OnStartup(e);
        }
    }
}
