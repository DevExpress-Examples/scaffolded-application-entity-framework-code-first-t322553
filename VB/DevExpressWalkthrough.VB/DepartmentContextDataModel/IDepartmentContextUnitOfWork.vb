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
    ''' IDepartmentContextUnitOfWork extends the IUnitOfWork interface with repositories representing specific entities.
    ''' </summary>
    Public Interface IDepartmentContextUnitOfWork
        Inherits IUnitOfWork
        ''' <summary>
        ''' The Course entities repository.
        ''' </summary>
        ReadOnly Property Courses As IRepository(Of Course, Integer)
        ''' <summary>
        ''' The Department entities repository.
        ''' </summary>
        ReadOnly Property Departments As IRepository(Of Department, Integer)
        ''' <summary>
        ''' The Employee entities repository.
        ''' </summary>
        ReadOnly Property Employees As IRepository(Of Employee, Integer)
    End Interface
End Namespace
