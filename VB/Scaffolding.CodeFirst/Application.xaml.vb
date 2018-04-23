Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.Linq
Imports System.Windows
Imports DevExpress.Xpf.Core
Imports Scaffolding.CodeFirst.Model
Imports System.Data.Entity
Imports DevExpress.Internal

Namespace Scaffolding.CodeFirst
	''' <summary>
	''' Interaction logic for App.xaml
	''' </summary>
	Partial Public Class App
		Inherits Application

		Private Sub Application_Startup(ByVal sender As Object, ByVal e As StartupEventArgs)
			ApplicationThemeHelper.UpdateApplicationThemeName()
			DbEngineDetector.PatchConnectionStringsAndConfigureEntityFrameworkDefaultConnectionFactory()
			Database.SetInitializer(Of DepartmentContext)(New DepartmentContextInitializer())
		End Sub

		Protected Overrides Sub OnExit(ByVal e As ExitEventArgs)
			ApplicationThemeHelper.SaveApplicationThemeName()
		End Sub
	End Class
End Namespace
