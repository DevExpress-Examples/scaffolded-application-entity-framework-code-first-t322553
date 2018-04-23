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
Imports DevExpress.Mvvm
Imports System.Collections
Imports System.ComponentModel
Imports DevExpress.Data.Linq
Imports DevExpress.Data.Linq.Helpers
Imports DevExpress.Data.Async.Helpers
Namespace Global.DevExpressWalkthrough.VB.DepartmentContextDataModel
    ''' <summary>
    ''' Provides methods to obtain the relevant IUnitOfWorkFactory.
    ''' </summary>
    Public Module UnitOfWorkSource
        ''' <summary>
        ''' Returns the IUnitOfWorkFactory implementation based on the current mode (run-time or design-time).
        ''' </summary>
        Public Function GetUnitOfWorkFactory() As IUnitOfWorkFactory(Of IDepartmentContextUnitOfWork)
            Return GetUnitOfWorkFactory(ViewModelBase.IsInDesignMode)
        End Function
        ''' <summary>
        ''' Returns the IUnitOfWorkFactory implementation based on the given mode (run-time or design-time).
        ''' </summary>
        ''' <param name="isInDesignTime">Used to determine which implementation of IUnitOfWorkFactory should be returned.</param>
        Public Function GetUnitOfWorkFactory(ByVal isInDesignTime As Boolean) As IUnitOfWorkFactory(Of IDepartmentContextUnitOfWork)
            If isInDesignTime Then
                Return New DesignTimeUnitOfWorkFactory(Of IDepartmentContextUnitOfWork)(Function() New DepartmentContextDesignTimeUnitOfWork())
            End If
            Return New DbUnitOfWorkFactory(Of IDepartmentContextUnitOfWork)(Function() New DepartmentContextUnitOfWork(Function() New DepartmentContext()))
        End Function
    End Module
End Namespace
