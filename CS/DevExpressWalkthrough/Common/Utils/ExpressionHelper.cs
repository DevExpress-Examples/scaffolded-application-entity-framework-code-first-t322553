using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DevExpressWalkthrough.Common.Utils {
    /// <summary>
    /// Provides methods to perform operations with lambda expression trees.
    /// </summary>
	public class ExpressionHelper {
        class ValueHolder {
            public readonly object value;
            public ValueHolder(object value) {
                this.value = value;
            }
        }

        static readonly Dictionary<Type, object> TraitsCache = new Dictionary<Type, object>();

        static Expression GetConstExpression(Type type, object value) {
            return Expression.Convert(Expression.Field(Expression.Constant(new ValueHolder(value)), "value"), type);
        }

        internal static bool IsTuple<T>() {
            var type = typeof(T);
            return type.IsGenericType && type.GetGenericTypeDefinition().Name.StartsWith("Tuple`");
        }

		/// <summary>
        /// If value is a Tuple, this method collects its items into an array of objects. For other types it creates an array of objects with the value being its only element.
        /// </summary>
        /// <typeparam name="T">The type of the property. Possibly a Tuple.</typeparam>
        /// <param name="value">An object from which an array of objects is created. It it supposed to be a primary key value.</param>
        public static object[] GetKeyPropertyValues<T>(T value) {
            if(ExpressionHelper.IsTuple<T>()) {
                return typeof(T).GetProperties().Where(p => p.Name.StartsWith("Item")).Select(p => p.GetValue(value, null)).ToArray();
            }
            return new object[] { value };
        }

        /// <summary>
        /// Builds a lambda expression that compares an entity property value with a given constant value.
        /// </summary>
        /// <typeparam name="TPropertyOwner">An owner type of the property.</typeparam>
        /// <typeparam name="TPrimaryKey">A primary key property type.</typeparam>
        /// <param name="getKeyExpression">A lambda expression that returns the property value for a given entity.</param> TODO: rewrite
        /// <param name="key">A constant value to compare with entity property value.</param>
        public static Expression<Func<TPropertyOwner, bool>> GetKeyEqualsExpression<TGetKeyExpressionOwner, TPropertyOwner, TPrimaryKey>(Expression<Func<TGetKeyExpressionOwner, TPrimaryKey>> getKeyExpression, TPrimaryKey key) {
            if(key == null)
                return k => false;
            var entityParam = Expression.Parameter(typeof(TPropertyOwner));
            var keyProperties = GetKeyProperties(getKeyExpression);
            var keyValues = GetKeyPropertyValues(key);
            if(keyProperties.Count() != keyValues.Count())
                throw new Exception();
            var propertyEqualExprs = keyProperties.Zip(keyValues, (p, v) => {
                var constExpr = GetConstExpression(p.PropertyType, v);
                var propertyExpr = Expression.Property(entityParam, typeof(TPropertyOwner).GetProperty(p.Name));
                return Expression.Equal(propertyExpr, constExpr);
            });
            var andExpr = propertyEqualExprs.Aggregate((Expression)Expression.Constant(true), (a, e) => Expression.And(a, e));
            return Expression.Lambda<Func<TPropertyOwner, bool>>(andExpr, entityParam);
        }

        /// <summary>
        /// Returns an instance of the EntityTraits class that encapsulates operations to obtain and set the primary key value of a given entity.
        /// </summary>
        /// <typeparam name="TOwner">A type used as a key to cache compiled lambda expressions.</typeparam>
        /// <typeparam name="TPropertyOwner">An owner type of the primary key property.</typeparam>
        /// <typeparam name="TProperty">A primary key property type.</typeparam>
        /// <param name="owner">An instance of the TOwner type which type is used as a key to cache compiled lambda expressions.</param>
        /// <param name="getPropertyExpression">A lambda expression that returns the primary key value for a given entity.</param>
        public static EntityTraits<TPropertyOwner, TProperty> GetEntityTraits<TOwner, TPropertyOwner, TProperty>(TOwner owner, Expression<Func<TPropertyOwner, TProperty>> getPropertyExpression) {
            object traits = null;
            if(!TraitsCache.TryGetValue(owner.GetType(), out traits)) {
                traits = new EntityTraits<TPropertyOwner, TProperty>(getPropertyExpression.Compile(), GetSetKeyAction(getPropertyExpression), GetHasKeyFunction(getPropertyExpression));
                TraitsCache[owner.GetType()] = traits;
            }
            return (EntityTraits<TPropertyOwner, TProperty>)traits;
        }

        /// <summary>
        /// Determines whether the given entity satisfies the condition represented by a lambda expression.
        /// </summary>
        /// <typeparam name="TEntity">A type of the given object.</typeparam>
        /// <param name="entity">An object to test.</param>
        /// <param name="predicate">A function that determines whether the given object satisfies the condition.</param>
        public static bool IsFitEntity<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> predicate) where TEntity : class {
            return predicate == null || (new TEntity[] { entity }.AsQueryable().Any(predicate));
        }

		/// <summary>
        /// Creates an instance of a generic Tuple type from items.
        /// </summary>
        /// <typeparam name="TupleType">A tuple type.</typeparam>
        /// <param name="items">Objects that will comprise the tuple.</param>
        public static TupleType MakeTuple<TupleType>(object[] items) {
            var args = typeof(TupleType).GetGenericArguments();
            if(args.Count() != items.Count())
                throw new Exception();
            var create = typeof(Tuple).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(m => m.Name == "Create" && m.GetGenericArguments().Count() == args.Count());
            return (TupleType)create.MakeGenericMethod(args).Invoke(null, items);
        }

		/// <summary>
        /// Get an expression with incapsulating a lamda that given an object of type TOwner returns the value of property propertyName.
        /// </summary>
        /// <typeparam name="TOwner">The name of type that contains the property.</typeparam>
		/// <typeparam name="TProperty">The type of a property.</typeparam>
        /// <param name="propertyName">The name of a property.</param>
        public static Expression<Func<TOwner, TProperty>> GetPropertyExpression<TOwner, TProperty>(string propertyName) {
            var parameter = Expression.Parameter(typeof(TOwner));
            return Expression.Lambda<Func<TOwner, TProperty>>(Expression.Property(parameter, propertyName), parameter);
        }

        /// <summary>
        /// Converts a property reference represented as a lambda expression to a property name.
        /// </summary>
        /// <param name="expression">A lambda expression that returns the property value.</param>
        public static string GetPropertyName(LambdaExpression expression) {
            Expression body = expression.Body;
            if(body is UnaryExpression) {
                body = ((UnaryExpression)body).Operand;
            }
            var memberExpression = UnpackNullableMemberExpression((MemberExpression)body);
            return memberExpression.Member.Name;
        }

        static MemberExpression UnpackNullableMemberExpression(MemberExpression memberExpression) {
            if(memberExpression != null && IsNullableValueExpression(memberExpression))
                memberExpression = (MemberExpression)memberExpression.Expression;
            return memberExpression;
        }

        static bool IsNullableValueExpression(MemberExpression memberExpression) {
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            return Nullable.GetUnderlyingType(propertyInfo.ReflectedType) != null;
        }

		/// <summary>
        /// Gets an array of PropertyInfo objects that describe the properties that comprise the primary key of the TPropertyOwner type.
        /// </summary>
        /// <typeparam name="TPropertyOwner">A type with a primary key.</typeparam>
        /// <typeparam name="TProperty">The type of the primary key. Possibly a Tuple type.</typeparam>
        /// <param name="getPropertyExpression">An expression that when compiled and evaluated returns the value of the primary key of an TPropertyOwner object.</param>
        public static PropertyInfo[] GetKeyProperties<TPropertyOwner, TProperty>(Expression<Func<TPropertyOwner, TProperty>> getPropertyExpression) {
            var memberExpr = UnpackNullableMemberExpression(getPropertyExpression.Body as MemberExpression);
            var methodCallExpr = getPropertyExpression.Body as MethodCallExpression;
            IEnumerable<string> propertyNames;
            if(memberExpr != null) {
                propertyNames = new string[] { memberExpr.Member.Name };
            } else if (methodCallExpr != null) {
                if(methodCallExpr.Method.DeclaringType != typeof(Tuple) || methodCallExpr.Method.Name != "Create") {
                    throw new Exception();
                }
                var args = methodCallExpr.Arguments.Cast<MemberExpression>();
                propertyNames = args.Select(a => a.Member.Name);
            } else {
                propertyNames = Enumerable.Empty<string>();
            }
            return propertyNames.Select(p => typeof(TPropertyOwner).GetProperty(p)).ToArray();
        }

        public static Action<TPropertyOwner, TProperty> GetSetKeyAction<TPropertyOwner, TProperty>(Expression<Func<TPropertyOwner, TProperty>> getKeyExpression) {
            var properties = GetKeyProperties(getKeyExpression);
            return (o, val) => {
                var values = GetKeyPropertyValues(val);
                values.Zip(properties, (v, p) => {
                    p.SetValue(o, v, null);
                    return "";
                }).ToArray();
            };
        }

        static bool IsNullable(Type type) {
            return Nullable.GetUnderlyingType(type) != null;
        }

        static Func<TPropertyOwner, bool> GetHasKeyFunction<TPropertyOwner, TProperty>(Expression<Func<TPropertyOwner, TProperty>> getKeyExpression) {
            var properties = GetKeyProperties(getKeyExpression);
            return o => properties.All(p => !IsNullable(p.PropertyType) || p.GetValue(o, null) != null);
        }
    }

    /// <summary>
    /// Incapsulates operations to obtain and set the primary key value of a given entity.
    /// </summary>
    /// <typeparam name="TEntity">An owner type of the primary key property.</typeparam>
    /// <typeparam name="TPrimaryKey">A primary key property type.</typeparam>
    public class EntityTraits<TEntity, TPrimaryKey> {

        /// <summary>
        /// Initializes a new instance of EntityTraits class.
        /// </summary>
        /// <param name="getPrimaryKeyFunction">A function that returns the primary key value of a given entity.</param>
        /// <param name="setPrimaryKeyAction">An action that assigns the primary key value to a given entity.</param>
        /// <param name="hasPrimaryKeyFunction">A function that determines whether given the entity has a primary key assigned.</param>
        public EntityTraits(Func<TEntity, TPrimaryKey> getPrimaryKeyFunction, Action<TEntity, TPrimaryKey> setPrimaryKeyAction, Func<TEntity, bool> hasPrimaryKeyFunction) {
            this.GetPrimaryKey = getPrimaryKeyFunction;
            this.SetPrimaryKey = setPrimaryKeyAction;
            this.HasPrimaryKey = hasPrimaryKeyFunction;
        }

        /// <summary>
        /// The function that returns the primary key value of a given entity.
        /// </summary>
        public Func<TEntity, TPrimaryKey> GetPrimaryKey { get; private set; }

        /// <summary>
        /// The action that assigns the primary key value to a given entity.
        /// </summary>
        public Action<TEntity, TPrimaryKey> SetPrimaryKey { get; private set; }

        /// <summary>
        /// A function that determines whether the given entity has a primary key assigned (the primary key is not null). Always returns true if the primary key is a non-nullable value type.
        /// </summary>
        /// <returns></returns>
        public Func<TEntity, bool> HasPrimaryKey { get; private set; }
    }
}
