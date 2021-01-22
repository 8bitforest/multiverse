using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Multiverse.Utils
{
    internal static class ReflectionUtils
    {
        public delegate object Getter(object instance);

        public delegate void Setter(object instance, object value);

        private static readonly ParameterExpression FieldParam = Expression.Parameter(typeof(object));
        private static readonly ParameterExpression OwnerParam = Expression.Parameter(typeof(object));

        public static Type GetGenericTypeDefinition(Type type)
        {
            return type.IsGenericType ? type.GetGenericTypeDefinition() : type.BaseType;
        }

        public static Type GetGenericType(Type type)
        {
            if (type.IsGenericType)
                return type.GenericTypeArguments[0];
            return type.IsArray ? type.GetElementType() : type;
        }

        public static IEnumerable<MemberInfo> GetAllFieldsProperties(Type type)
        {
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                yield return field;
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                yield return prop;
        }

        public static Setter GetFieldPropertySetter(MemberInfo info)
        {
            var ownerExp = info.DeclaringType!.IsValueType
                ? Expression.Unbox(OwnerParam, info.DeclaringType!)
                : Expression.Convert(OwnerParam, info.DeclaringType!);
            var expression = Expression.PropertyOrField(ownerExp, info.Name);
            return Expression.Lambda<Setter>(Expression.Assign(expression,
                Expression.Convert(FieldParam, GetFieldPropertyType(info))), OwnerParam, FieldParam).Compile();
        }

        public static Getter GetFieldPropertyGetter(MemberInfo info)
        {
            var expression = Expression.PropertyOrField(Expression.Convert(OwnerParam, info.DeclaringType!), info.Name);
            return Expression.Lambda<Getter>(Expression.Convert(expression, typeof(object)), OwnerParam).Compile();
        }

        public static Type GetFieldPropertyType(MemberInfo info)
        {
            return info switch
            {
                FieldInfo field => field.FieldType,
                PropertyInfo prop => prop.PropertyType,
                _ => null
            };
        }
    }
}