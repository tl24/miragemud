using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Core
{
    public static class ReflectionUtils
    {
        /// <summary>
        /// Converts an anonymous type or any type with properties into a dictionary
        /// </summary>
        /// <param name="anonymousType">the anonymous type instance</param>
        /// <returns>a dictionary</returns>
        public static IDictionary<string, object> ObjectToDictionary(object anonymousType)
        {
            if (anonymousType == null)
                return null;
            if (anonymousType is IDictionary<string, object>)
                return (IDictionary<string, object>)anonymousType;

            return anonymousType.GetType()
                .GetProperties()
                .ToDictionary(p => p.Name, p => p.GetValue(anonymousType, null), StringComparer.CurrentCultureIgnoreCase);
        }

        public static bool IsGenericDictionary(object instance)
        {
            if (instance == null)
                return false;
            Type t = (instance as Type) ?? instance.GetType();
            return t.GetInterfaces().Any(i => IsGenericDictionaryInterface(i));
        }

        private static bool IsGenericDictionaryInterface(Type t)
        {
            if (!t.IsGenericType && !t.IsGenericTypeDefinition)
                return false;
            t = !t.IsGenericTypeDefinition ? t.GetGenericTypeDefinition() : t;
            return t == typeof(IDictionary<,>);
        }
    }
}
