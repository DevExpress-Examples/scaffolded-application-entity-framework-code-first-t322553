using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using Scaffolding.CodeFirst.Model;
using System.Data.Entity;
using DevExpress.Internal;

namespace Scaffolding.CodeFirst {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private void Application_Startup(object sender, StartupEventArgs e) {
            ApplicationThemeHelper.UpdateApplicationThemeName();
            DbEngineDetector.PatchConnectionStringsAndConfigureEntityFrameworkDefaultConnectionFactory();
            Database.SetInitializer<DepartmentContext>(new DepartmentContextInitializer());
        }

        protected override void OnExit(ExitEventArgs e) {
            ApplicationThemeHelper.SaveApplicationThemeName();
        }
    }
}
