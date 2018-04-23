Imports System
Imports System.Linq
Imports System.Data
Imports System.Linq.Expressions
Imports System.Collections.Generic
Imports DevExpressWalkthrough.VB.Common.Utils
Imports DevExpressWalkthrough.VB.Common.DataModel
Imports DevExpressWalkthrough.VB.Common.DataModel.DesignTime
Imports DevExpressWalkthrough.VB.Common.DataModel.EntityFramework
Imports DevExpressWalkthrough.VB
Namespace Global.DevExpressWalkthrough.VB.DepartmentContextDataModel
    ''' <summary>
    ''' A DepartmentContextUnitOfWork instance that represents the run-time implementation of the IDepartmentContextUnitOfWork interface.
    ''' </summary>
    Public Class DepartmentContextUnitOfWork
        Inherits DbUnitOfWork(Of DepartmentContext)
        Implements IDepartmentContextUnitOfWork
        Public Sub New(ByVal contextFactory As Func(Of DepartmentContext))
            MyBase.New(contextFactory)
        End Sub
        Private ReadOnly Property Courses As IRepository(Of Course, Integer) Implements IDepartmentContextUnitOfWork.Courses
            Get
                Return GetRepository(Function(ByVal x) x.[Set](Of Course)(), Function(ByVal x As Course) x.CourseID)
            End Get
        End Property
        Private ReadOnly Property Departments As IRepository(Of Department, Integer) Implements IDepartmentContextUnitOfWork.Departments
            Get
                Return GetRepository(Function(ByVal x) x.[Set](Of Department)(), Function(ByVal x As Department) x.DepartmentID)
            End Get
        End Property
        Private ReadOnly Property Employees As IRepository(Of Employee, Integer) Implements IDepartmentContextUnitOfWork.Employees
            Get
                Return GetRepository(Function(ByVal x) x.[Set](Of Employee)(), Function(ByVal x As Employee) x.EmployeeID)
            End Get
        End Property
    End Class
End Namespace
