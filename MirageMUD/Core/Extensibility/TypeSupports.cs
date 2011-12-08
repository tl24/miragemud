using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Extensibility
{

    /// <summary>
    /// Helper class for casting that supports the IAttributable interface which allows objects to dynamically
    /// alter their supported interfaces at runtime
    /// </summary>
    public static class TypeSupports
    {
        /// <summary>
        /// Tries to cast the object as the given type, if the object doesn't implement or
        /// extend from the type then an exception is thrown
        /// </summary>
        /// <typeparam name="T">the type to cast as</typeparam>
        /// <param name="o">the target object</param>
        /// <returns>object</returns>
        /// <exception cref="InvalidCastException">if the object doesn't support the type</exception>
        public static T CastAs<T>(object o)
        {
            return (T)CastAs(o, typeof(T));
        }

        /// <summary>
        /// Tries to cast the object as the given type, if the object doesn't implement or
        /// extend from the type then an exception is thrown
        /// </summary>
        /// <param name="o">the target object</param>
        /// <param name="t">the type to cast as</typeparam>
        /// <returns>object</returns>
        /// <exception cref="InvalidCastException">if the object doesn't support the type</exception>
        public static object CastAs(object o, Type t)
        {
            if (o == null)
                return null;

            o = TryCastAs(o, t);
            if (o == null)
                throw new InvalidCastException("Can't cast from " + o.GetType().FullName + " to " + t.FullName);
            else
                return o;
        }

        /// <summary>
        /// Tries to cast the object as the given type, if the object doesn't implement or
        /// extend from the type then null is returned
        /// </summary>
        /// <typeparam name="T">the type to cast as</typeparam>
        /// <param name="o">the target object</param>
        /// <returns>object</returns>
        public static T TryCastAs<T>(object o)
        {
            return (T)TryCastAs(o, typeof(T));
        }

        /// <summary>
        /// Tries to cast the object as the given type, if the object doesn't implement or
        /// extend from the type then null is returned
        /// </summary>
        /// <param name="o">the target object</param>
        /// <param name="t">the type to cast as</typeparam>
        /// <returns>object</returns>
        public static object TryCastAs(object o, Type t)
        {
            if (t.IsInstanceOfType(o))
                return o;
            else if (o is IAttributable)
            {
                IAttributable attr = (IAttributable)o;
                return attr.TryGetAttribute(t);
            }
            return null;
        }

        /// <summary>
        /// Checks to see if the object implements or is the same or subclass of the given type.
        /// Same as the "is" operator with support for IAttributable.
        /// </summary>
        /// <typeparam name="T">the type to check</typeparam>
        /// <param name="o">the object to check</param>
        /// <returns>true if object implements type</returns>
        public static bool IsType<T>(object o)
        {
            if (o is T)
                return true;
            else
                return TryCastAs<T>(o) != null;
        }

        /// <summary>
        /// Checks to see if the object implements or is the same or subclass of the given type.
        /// Same as the "is" operator with support for IAttributable.
        /// </summary>
        /// <param name="o">the object to check</param>
        /// <param name="typeToCheck">the type to check</typeparam>
        /// <returns>true if object implements type</returns>
        public static bool IsType(object o, Type typeToCheck)
        {
            if (typeToCheck.IsInstanceOfType(o))
                return true;
            else
                return TryCastAs(o, typeToCheck) != null;
        }
    }
}
