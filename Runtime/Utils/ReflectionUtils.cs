using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Multiverse.Utils
{
    internal static class ReflectionUtils
    {
        public static Type GetGenericTypeDefinition(Type type)
        {
            if (type.IsGenericParameter && type.GetGenericParameterConstraints().Length == 1)
                return type.GetGenericParameterConstraints()[0];
            if (type.IsGenericType)
                return type.GetGenericTypeDefinition();
            return type.BaseType;
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