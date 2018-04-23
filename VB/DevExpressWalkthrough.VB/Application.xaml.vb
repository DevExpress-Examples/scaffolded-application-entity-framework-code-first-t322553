Imports System.Data.Entity

Class Application
    Protected Overrides Sub OnStartup(e As StartupEventArgs)
        Database.SetInitializer(Of DepartmentContext)(New DepartmentContextInitializer())
        MyBase.OnStartup(e)
    End Sub
End Class
