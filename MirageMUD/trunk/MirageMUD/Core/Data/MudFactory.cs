using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.Windsor.Configuration;
using Mirage.Core.IO;

namespace Mirage.Core.Data
{
    /// <summary>
    /// Abstraction class for creating and referencing types
    /// </summary>
    public class MudFactory
    {
        private static MyContainer _instance;

        static MudFactory()
        {
            _instance = new MyContainer(new XmlInterpreter());
            
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

        public static void RegisterService(Type classType)
        {
            _instance.AddComponent(classType.FullName, classType);
        }

        public static void RegisterService(string key, Type classType)
        {
            _instance.AddComponent(key, classType);
        }

        public static void RegisterService(string key, Type serviceType, Type classType)
        {
            _instance.AddComponent(key, serviceType, classType);
        }

        private class MyContainer : WindsorContainer
        {
            public MyContainer(IConfigurationInterpreter interpreter)
            {
                // Registers the type converter:

                IConversionManager manager = (IConversionManager)
                    Kernel.GetSubSystem(Castle.MicroKernel.SubSystemConstants.ConversionManagerKey);

                manager.Add(new ServiceEntryConverter());

                // Process the configuration

                interpreter.ProcessResource(interpreter.Source, Kernel.ConfigurationStore);

                // Install the components

                Installer.SetUp(this, Kernel.ConfigurationStore);
            }
        }
    }
}
