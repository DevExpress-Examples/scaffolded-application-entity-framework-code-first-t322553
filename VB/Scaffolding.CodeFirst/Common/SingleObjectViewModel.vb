Imports System
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Collections.Generic
Imports System.Linq
Imports System.Linq.Expressions
Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.POCO
Imports DevExpress.Mvvm.DataAnnotations
Imports DevExpress.Mvvm.ViewModel
Imports DevExpress.Mvvm.DataModel

Namespace Scaffolding.CodeFirst.Common
	''' <summary>
	''' The base class for POCO view models exposing a single entity of a given type and CRUD operations against this entity.
	''' This is a partial class that provides the extension point to add custom properties, commands and override methods without modifying the auto-generated code.
	''' </summary>
	''' <typeparam name="TEntity">An entity type.</typeparam>
	''' <typeparam name="TPrimaryKey">A primary key value type.</typeparam>
	''' <typeparam name="TUnitOfWork">A unit of work type.</typeparam>
	Public MustInherit Partial Class SingleObjectViewModel(Of TEntity As Class, TPrimaryKey, TUnitOfWork As IUnitOfWork)
		Inherits SingleObjectViewModelBase(Of TEntity, TPrimaryKey, TUnitOfWork)

		''' <summary>
		''' Initializes a new instance of the SingleObjectViewModel class.
		''' </summary>
		''' <param name="unitOfWorkFactory">A factory used to create the unit of work instance.</param>
		''' <param name="getRepositoryFunc">A function that returns the repository representing entities of a given type.</param>
		''' <param name="getEntityDisplayNameFunc">An optional parameter that provides a function to obtain the display text for a given entity. If ommited, the primary key value is used as a display text.</param>
		Protected Sub New(ByVal unitOfWorkFactory As IUnitOfWorkFactory(Of TUnitOfWork), ByVal getRepositoryFunc As Func(Of TUnitOfWork, IRepository(Of TEntity, TPrimaryKey)), Optional ByVal getEntityDisplayNameFunc As Func(Of TEntity, Object) = Nothing)
			MyBase.New(unitOfWorkFactory, getRepositoryFunc, getEntityDisplayNameFunc)
		End Sub
	End Class
End Namespace