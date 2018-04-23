Imports System
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Collections.Generic
Imports System.Linq
Imports System.Linq.Expressions
Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.POCO
Imports DevExpress.Mvvm.DataAnnotations
Imports System.Collections.ObjectModel
Imports DevExpress.Data.Linq
Imports System.Collections
Imports DevExpress.Mvvm.ViewModel
Imports DevExpress.Mvvm.DataModel

Namespace Scaffolding.CodeFirst.Common
	Partial Public Class InstantFeedbackCollectionViewModel(Of TEntity As {Class, New}, TPrimaryKey, TUnitOfWork As IUnitOfWork)
		Inherits InstantFeedbackCollectionViewModelBase(Of TEntity, TEntity, TPrimaryKey, TUnitOfWork)

		Public Shared Function CreateInstantFeedbackCollectionViewModel(ByVal unitOfWorkFactory As IUnitOfWorkFactory(Of TUnitOfWork), ByVal getRepositoryFunc As Func(Of TUnitOfWork, IRepository(Of TEntity, TPrimaryKey)), Optional ByVal projection As Func(Of IRepositoryQuery(Of TEntity), IQueryable(Of TEntity)) = Nothing, Optional ByVal canCreateNewEntity As Func(Of Boolean) = Nothing) As InstantFeedbackCollectionViewModel(Of TEntity, TPrimaryKey, TUnitOfWork)
			Return ViewModelSource.Create(Function() New InstantFeedbackCollectionViewModel(Of TEntity, TPrimaryKey, TUnitOfWork)(unitOfWorkFactory, getRepositoryFunc, projection, canCreateNewEntity))
		End Function

		Protected Sub New(ByVal unitOfWorkFactory As IUnitOfWorkFactory(Of TUnitOfWork), ByVal getRepositoryFunc As Func(Of TUnitOfWork, IRepository(Of TEntity, TPrimaryKey)), Optional ByVal projection As Func(Of IRepositoryQuery(Of TEntity), IQueryable(Of TEntity)) = Nothing, Optional ByVal canCreateNewEntity As Func(Of Boolean) = Nothing)
			MyBase.New(unitOfWorkFactory, getRepositoryFunc, projection, canCreateNewEntity)
		End Sub
	End Class
End Namespace