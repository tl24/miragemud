using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using JsonExSerializer;

namespace Mirage.Game.World
{

    /// <summary>
    /// Helper class that overlays the properties of one object with the properties
    /// of another.
    /// </summary>
    public class ObjectUpdater
    {
        private object _source;
        private object _destination;

        /// <summary>
        /// Create an instance with source and destination objects.
        /// </summary>
        /// <param name="source">the source object</param>
        /// <param name="destination">object to be updated</param>
        public ObjectUpdater(object source, object destination)
        {
            if (source == null || destination == null)
                throw new ArgumentNullException("source and destination cannot be null");

            if (source.GetType() != destination.GetType())
                throw new ArgumentException("Type of Source does not match Type of destination");

            this._source = source;
            this._destination = destination;
        }

        /// <summary>
        /// Perform the updates of the objects
        /// </summary>
        public void Update()
        {
            Type instanceType = _source.GetType();
            foreach (PropertyInfo prop in instanceType.GetProperties())
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    UpdateProperty(prop);
                }
            }
        }

        /// <summary>
        /// Updates a single property on the object
        /// </summary>
        /// <param name="property">The property to update</param>
        protected virtual void UpdateProperty(PropertyInfo property)
        {
            foreach (System.Attribute attr in property.GetCustomAttributes(false))
            {
                if (attr is JsonExIgnoreAttribute || attr is EditorCollectionAttribute)
                    return;
            }
            // we got here, we must be able to update
            property.SetValue(_destination, property.GetValue(_source, null), null);
        }

        /// <summary>
        /// Copies properties from one object to another
        /// </summary>
        /// <param name="source">the source object</param>
        /// <param name="destination">object to be updated</param>
        public static void CopyObject(object source, object destination)
        {
            ObjectUpdater updater = new ObjectUpdater(source, destination);
            updater.Update();
        }
    }
}
