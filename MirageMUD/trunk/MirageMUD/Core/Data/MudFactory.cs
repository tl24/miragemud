using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;

namespace Mirage.Core.Data
{
    /// <summary>
    /// Abstraction class for creating and referencing types
    /// </summary>
    public class MudFactory
    {
        private static WindsorContainer _instance;

        static MudFactory()
        {
            _instance = new WindsorContainer(new XmlInterpreter());
        }

        public static TObjectInterface GetObject<TObjectInterface>()
        {
            return _instance.Resolve<TObjectInterface>();
        }

        public static object GetObject(Type interfaceType)
        {
            return _instance.Resolve(interfaceType);
            
        }

        /// <summary>
        /// Retrieves the given object using a string key
        /// </summary>
        /// <param name="key">the key</param>
        /// <returns>constructed object</returns>
        public static object GetObject(string key)
        {
            return _instance.Resolve(key);
        }
    }
}
