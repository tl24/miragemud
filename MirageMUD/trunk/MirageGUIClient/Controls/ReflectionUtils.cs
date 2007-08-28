using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace MirageGUI.Controls
{
    public static class ReflectionUtils
    {
        /// <summary>
        /// Gets a single attribute of a given type, when you only expect one
        /// of an attribute to be there.  Returns null if not found
        /// </summary>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <param name="member">the member </param>
        /// <returns>attribute object</returns>
        public static T GetSingleAttribute<T>(MemberInfo member) 
            where T : Attribute
        {
            return GetSingleAttribute<T>(member, false);
        }

        /// <summary>
        /// Gets a single attribute of a given type, when you only expect one
        /// of an attribute to be there.  Returns null if not found
        /// </summary>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <param name="member">the member </param>
        /// <param name="inherit">true to inherit attributes from base</param>
        /// <returns>attribute object</returns>
        public static T GetSingleAttribute<T>(MemberInfo member, bool inherit)
            where T : Attribute
        {
            if (member.IsDefined(typeof(T), inherit))
                return (T)member.GetCustomAttributes(typeof(T), inherit)[0];
            else
                return null;
        }
    }
}
