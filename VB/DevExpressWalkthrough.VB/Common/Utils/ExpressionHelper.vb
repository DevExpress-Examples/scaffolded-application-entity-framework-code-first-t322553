Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Reflection
Namespace Global.DevExpressWalkthrough.VB.Common.Utils
  ''' <summary>
  ''' Provides methods to perform operations with lambda expression trees.
  ''' </summary>
  Public Class ExpressionHelper
    Friend Class ValueHolder
      Public ReadOnly value As Object
      Public Sub New(ByVal value As Object)
        Me.value = value
      End Sub
    End Class
    Private Shared ReadOnly _TraitsCache As Dictionary(Of Type, Object) = New Dictionary(Of Type, Object)()
    Private Shared Function GetConstExpression(ByVal type As Type, ByVal value As Object) As Expression
      Return Expression.Convert(Expression.Field(Expression.Constant(New ValueHolder(value)), "value"), type)
    End Function
    Friend Shared Function IsTuple(Of T)() As Boolean
      Dim type = GetType(T)
      Return type.IsGenericType AndAlso type.GetGenericTypeDefinition().Name.StartsWith("Tuple`")
    End Function    
    ''' <summary>
    ''' If value is a Tuple, this method collects its items into an array of objects. For other types it creates an array of objects with the value being its only element.
    ''' </summary>
    ''' <typeparam name="T">The type of the property. Possibly a Tuple.</typeparam>
    ''' <param name="value">An object from which an array of objects is created. It it supposed to be a primary key value.</param>
    Public Shared Function GetKeyPropertyValues(Of T)(ByVal value As T) As Object()
      If ExpressionHelper.IsTuple(Of T)() Then
        Return GetType(T).GetProperties().Where(Function(ByVal p) p.Name.StartsWith("Item")).[Select](Function(ByVal p) p.GetValue(value, Nothing)).ToArray()
      End If
      Return New Object() { value }
    End Function    
    ''' <summary>
    ''' Builds a lambda expression that compares an entity property value with a given constant value.
    ''' </summary>
    ''' <typeparam name="TPropertyOwner">An owner type of the property.</typeparam>
    ''' <typeparam name="TPrimaryKey">A primary key property type.</typeparam>
    ''' <param name="getKeyExpression">A lambda expression that returns the property value for a given entity.</param>
    ''' TODO: rewrite
    ''' <param name="key">A constant value to compare with entity property value.</param>
    Public Shared Function GetKeyEqualsExpression(Of TGetKeyExpressionOwner, TPropertyOwner, TPrimaryKey)(ByVal getKeyExpression As Expression(Of Func(Of TGetKeyExpressionOwner, TPrimaryKey)), ByVal key As TPrimaryKey) As Expression(Of Func(Of TPropertyOwner, Boolean))
      If key Is Nothing Then
        Return Function(ByVal k) False
      End If
      Dim entityParam = Expression.Parameter(GetType(TPropertyOwner))
      Dim keyProperties = GetKeyProperties(getKeyExpression)
      Dim keyValues = GetKeyPropertyValues(key)
      If keyProperties.Count() <> keyValues.Count() Then
        Throw New Exception()
      End If
      Dim propertyEqualExprs = keyProperties.Zip(keyValues, Function(ByVal p, ByVal v)
        Dim constExpr = GetConstExpression(p.PropertyType, v)
        Dim propertyExpr = Expression.[Property](entityParam, GetType(TPropertyOwner).GetProperty(p.Name))
        Return Expression.Equal(propertyExpr, constExpr)
      End Function)
      Dim andExpr = propertyEqualExprs.Aggregate(CType(Expression.Constant(True), Expression), Function(ByVal a, ByVal e) Expression.[And](a, e))
      Return Expression.Lambda(Of Func(Of TPropertyOwner, Boolean))(andExpr, entityParam)
    End Function    
    ''' <summary>
    ''' Returns an instance of the EntityTraits class that encapsulates operations to obtain and set the primary key value of a given entity.
    ''' </summary>
    ''' <typeparam name="TOwner">A type used as a key to cache compiled lambda expressions.</typeparam>
    ''' <typeparam name="TPropertyOwner">An owner type of the primary key property.</typeparam>
    ''' <typeparam name="TProperty">A primary key property type.</typeparam>
    ''' <param name="owner">An instance of the TOwner type which type is used as a key to cache compiled lambda expressions.</param>
    ''' <param name="getPropertyExpression">A lambda expression that returns the primary key value for a given entity.</param>
    Public Shared Function GetEntityTraits(Of TOwner, TPropertyOwner, TProperty)(ByVal owner As TOwner, ByVal getPropertyExpression As Expression(Of Func(Of TPropertyOwner, TProperty))) As EntityTraits(Of TPropertyOwner, TProperty)
      Dim traits As Object = Nothing
      If Not _TraitsCache.TryGetValue(owner.[GetType](), traits) Then
        traits = New EntityTraits(Of TPropertyOwner, TProperty)(getPropertyExpression.Compile(), GetSetKeyAction(getPropertyExpression), GetHasKeyFunction(getPropertyExpression))
        _TraitsCache(owner.[GetType]()) = traits
      End If
      Return CType(traits, EntityTraits(Of TPropertyOwner, TProperty))
    End Function    
    ''' <summary>
    ''' Determines whether the given entity satisfies the condition represented by a lambda expression.
    ''' </summary>
    ''' <typeparam name="TEntity">A type of the given object.</typeparam>
    ''' <param name="entity">An object to test.</param>
    ''' <param name="predicate">A function that determines whether the given object satisfies the condition.</param>
    Public Shared Function IsFitEntity(Of TEntity As Class)(ByVal entity As TEntity, ByVal predicate As Expression(Of Func(Of TEntity, Boolean))) As Boolean
      Return predicate Is Nothing OrElse (New TEntity() { entity }.AsQueryable().Any(predicate))
    End Function    
    ''' <summary>
    ''' Creates an instance of a generic Tuple type from items.
    ''' </summary>
    ''' <typeparam name="TupleType">A tuple type.</typeparam>
    ''' <param name="items">Objects that will comprise the tuple.</param>
    Public Shared Function MakeTuple(Of TupleType)(ByVal items As Object()) As TupleType
      Dim args = GetType(TupleType).GetGenericArguments()
      If args.Count() <> items.Count() Then
        Throw New Exception()
      End If
      Dim create = GetType(Tuple).GetMethods(BindingFlags.[Static] Or BindingFlags.[Public]).First(Function(ByVal m) m.Name = "Create" AndAlso m.GetGenericArguments().Count() = args.Count())
      Return CType(create.MakeGenericMethod(args).Invoke(Nothing, items), TupleType)
    End Function    
    ''' <summary>
    ''' Get an expression with incapsulating a lamda that given an object of type TOwner returns the value of property propertyName.
    ''' </summary>
    ''' <typeparam name="TOwner">The name of type that contains the property.</typeparam>
    ''' <typeparam name="TProperty">The type of a property.</typeparam>
    ''' <param name="propertyName">The name of a property.</param>
    Public Shared Function GetPropertyExpression(Of TOwner, TProperty)(ByVal propertyName As String) As Expression(Of Func(Of TOwner, TProperty))
      Dim parameter = Expression.Parameter(GetType(TOwner))
      Return Expression.Lambda(Of Func(Of TOwner, TProperty))(Expression.[Property](parameter, propertyName), parameter)
    End Function    
    ''' <summary>
    ''' Converts a property reference represented as a lambda expression to a property name.
    ''' </summary>
    ''' <param name="expression">A lambda expression that returns the property value.</param>
    Public Shared Function GetPropertyName(ByVal expression As LambdaExpression) As String
      Dim body As Expression = expression.Body
      If TypeOf body Is UnaryExpression Then
        body = CType(body, UnaryExpression).Operand
      End If
      Dim memberExpression = UnpackNullableMemberExpression(CType(body, MemberExpression))
      Return memberExpression.Member.Name
    End Function
    Private Shared Function UnpackNullableMemberExpression(ByVal memberExpression As MemberExpression) As MemberExpression
      If memberExpression IsNot Nothing AndAlso IsNullableValueExpression(memberExpression) Then
        memberExpression = CType(memberExpression.Expression, MemberExpression)
      End If
      Return memberExpression
    End Function
    Private Shared Function IsNullableValueExpression(ByVal memberExpression As MemberExpression) As Boolean
      Dim propertyInfo = CType(memberExpression.Member, PropertyInfo)
      Return Nullable.GetUnderlyingType(propertyInfo.ReflectedType) IsNot Nothing
    End Function    
    ''' <summary>
    ''' Gets an array of PropertyInfo objects that describe the properties that comprise the primary key of the TPropertyOwner type.
    ''' </summary>
    ''' <typeparam name="TPropertyOwner">A type with a primary key.</typeparam>
    ''' <typeparam name="TProperty">The type of the primary key. Possibly a Tuple type.</typeparam>
    ''' <param name="getPropertyExpression">An expression that when compiled and evaluated returns the value of the primary key of an TPropertyOwner object.</param>
    Public Shared Function GetKeyProperties(Of TPropertyOwner, TProperty)(ByVal getPropertyExpression As Expression(Of Func(Of TPropertyOwner, TProperty))) As PropertyInfo()
      Dim memberExpr = UnpackNullableMemberExpression(TryCast(getPropertyExpression.Body, MemberExpression))
      Dim methodCallExpr = TryCast(getPropertyExpression.Body, MethodCallExpression)
      Dim propertyNames As IEnumerable(Of String)
      If memberExpr IsNot Nothing Then
        propertyNames = New String() { memberExpr.Member.Name }
      Else
        If methodCallExpr IsNot Nothing Then
          If methodCallExpr.Method.DeclaringType <> GetType(Tuple) OrElse methodCallExpr.Method.Name <> "Create" Then
            Throw New Exception()
          End If
          Dim args = methodCallExpr.Arguments.Cast(Of MemberExpression)()
          propertyNames = args.[Select](Function(ByVal a) a.Member.Name)
        Else
          propertyNames = Enumerable.Empty(Of String)()
        End If
      End If
      Return propertyNames.[Select](Function(ByVal p) GetType(TPropertyOwner).GetProperty(p)).ToArray()
    End Function
    Public Shared Function GetSetKeyAction(Of TPropertyOwner, TProperty)(ByVal getKeyExpression As Expression(Of Func(Of TPropertyOwner, TProperty))) As Action(Of TPropertyOwner, TProperty)
      Dim properties = GetKeyProperties(getKeyExpression)
      Return Sub(ByVal o, ByVal val)
        Dim values = GetKeyPropertyValues(val)
        values.Zip(properties, Function(ByVal v, ByVal p)
          p.SetValue(o, v, Nothing)
          Return ""
        End Function).ToArray()
      End Sub
    End Function
    Private Shared Function IsNullable(ByVal type As Type) As Boolean
      Return Nullable.GetUnderlyingType(type) IsNot Nothing
    End Function
    Private Shared Function GetHasKeyFunction(Of TPropertyOwner, TProperty)(ByVal getKeyExpression As Expression(Of Func(Of TPropertyOwner, TProperty))) As Func(Of TPropertyOwner, Boolean)
      Dim properties = GetKeyProperties(getKeyExpression)
      Return Function(ByVal o) properties.All(Function(ByVal p) Not IsNullable(p.PropertyType) OrElse p.GetValue(o, Nothing) IsNot Nothing)
    End Function
  End Class  
  ''' <summary>
  ''' Incapsulates operations to obtain and set the primary key value of a given entity.
  ''' </summary>
  ''' <typeparam name="TEntity">An owner type of the primary key property.</typeparam>
  ''' <typeparam name="TPrimaryKey">A primary key property type.</typeparam>
  Public Class EntityTraits(Of TEntity, TPrimaryKey)
    Private _HasPrimaryKey As Func(Of TEntity, Boolean)
    Private _SetPrimaryKey As Action(Of TEntity, TPrimaryKey)
    Private _GetPrimaryKey As Func(Of TEntity, TPrimaryKey)    
    ''' <summary>
    ''' Initializes a new instance of EntityTraits class.
    ''' </summary>
    ''' <param name="getPrimaryKeyFunction">A function that returns the primary key value of a given entity.</param>
    ''' <param name="setPrimaryKeyAction">An action that assigns the primary key value to a given entity.</param>
    ''' <param name="hasPrimaryKeyFunction">A function that determines whether given the entity has a primary key assigned.</param>
    Public Sub New(ByVal getPrimaryKeyFunction As Func(Of TEntity, TPrimaryKey), ByVal setPrimaryKeyAction As Action(Of TEntity, TPrimaryKey), ByVal hasPrimaryKeyFunction As Func(Of TEntity, Boolean))
      Me._GetPrimaryKey = getPrimaryKeyFunction
      Me._SetPrimaryKey = setPrimaryKeyAction
      Me._HasPrimaryKey = hasPrimaryKeyFunction
    End Sub    
    ''' <summary>
    ''' The function that returns the primary key value of a given entity.
    ''' </summary>
    Public ReadOnly Property GetPrimaryKey As Func(Of TEntity, TPrimaryKey)
      Get
        Return _GetPrimaryKey
      End Get
    End Property    
    ''' <summary>
    ''' The action that assigns the primary key value to a given entity.
    ''' </summary>
    Public ReadOnly Property SetPrimaryKey As Action(Of TEntity, TPrimaryKey)
      Get
        Return _SetPrimaryKey
      End Get
    End Property    
    ''' <summary>
    ''' A function that determines whether the given entity has a primary key assigned (the primary key is not null). Always returns true if the primary key is a non-nullable value type.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property HasPrimaryKey As Func(Of TEntity, Boolean)
      Get
        Return _HasPrimaryKey
      End Get
    End Property
  End Class
End Namespace
