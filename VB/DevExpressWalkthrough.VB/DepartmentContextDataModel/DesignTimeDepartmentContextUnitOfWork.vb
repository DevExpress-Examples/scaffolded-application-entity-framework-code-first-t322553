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
    ''' A DepartmentContextDesignTimeUnitOfWork instance that represents the design-time implementation of the IDepartmentContextUnitOfWork interface.
    ''' </summary>
    Public Class DepartmentContextDesignTimeUnitOfWork
        Inherits DesignTimeUnitOfWork
        Implements IDepartmentContextUnitOfWork
        ''' <summary>
        ''' Initializes a new instance of the DepartmentContextDesignTimeUnitOfWork class.
        ''' </summary>
        Public Sub New()
        End Sub
        Private ReadOnly Property Courses As IRepository(Of Course, Integer) Implements IDepartmentContextUnitOfWork.Courses
            Get
                Return GetRepository(Function(ByVal x As Course) x.CourseID)
            End Get
        End Property
        Private ReadOnly Property Departments As IRepository(Of Department, Integer) Implements IDepartmentContextUnitOfWork.Departments
            Get
                Return GetRepository(Function(ByVal x As Department) x.DepartmentID)
            End Get
        End Property
        Private ReadOnly Property Employees As IRepository(Of Employee, Integer) Implements IDepartmentContextUnitOfWork.Employees
            Get
                Return GetRepository(Function(ByVal x As Employee) x.EmployeeID)
            End Get
        End Property
    End Class
End Namespace
