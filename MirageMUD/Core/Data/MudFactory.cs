using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Mirage.Core.Data
{
    /// <summary>
    /// Abstraction class for creating and referencing types
    /// </summary>
    public class MudFactory
    {
        // holds references to created singletons
        private static HybridDictionary singletons = new HybridDictionary(true);

        public static TObjectInterface GetObject<TObjectInterface>()
        {
            return (TObjectInterface) GetObject(typeof(TObjectInterface));
        }

        public static VResultType GetObject<TObjectInterface, VResultType>()
            where VResultType : TObjectInterface
        {
            return (VResultType)GetObject<TObjectInterface>();
        }

        public static object GetObject(Type interfaceType)
        {
            if (singletons.Contains(interfaceType))
                return singletons[interfaceType];
            else
            {
                MudFactoryMapping mapping = Config.Mappings[interfaceType];
                if (mapping == null)
                    throw new ApplicationException("No mud factory mapping found for " + interfaceType.Name);
                return GetObject(mapping);
            }
        }

        /// <summary>
        /// Retrieves the given object using a string key
        /// </summary>
        /// <param name="key">the key</param>
        /// <returns>constructed object</returns>
        public static object GetObject(string key)
        {
            if (singletons.Contains(key))
                return singletons[key];
            else
            {
                MudFactoryMapping mapping = Config.Mappings[key];
                if (mapping == null)
                    throw new ApplicationException("No mud factory mapping found for " + key);
                return GetObject(mapping);
            }
        }

        private static object GetObject(MudFactoryMapping mapping)
        {

            object result = Activator.CreateInstance(Type.GetType(mapping.MappedType));
            if (mapping.LifeCycle == "singleton")
                singletons[mapping.Key] = result;

            return result;
        }

        private static MudFactoryConfigSection Config
        {
            get { return ConfigurationManager.GetSection("MirageMUD/MudFactory") as MudFactoryConfigSection; }
        }
    }

    class MudFactoryConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("Mappings", IsDefaultCollection = true)]
        public MudFactoryMappingsCollection Mappings
        {
            get { return this["Mappings"] as MudFactoryMappingsCollection; }
        }
    }

    public class MudFactoryMappingsCollection : ConfigurationElementCollection
    {
        public MudFactoryMapping this[int index]
        {
            get
            {
                return base.BaseGet(index) as MudFactoryMapping;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public MudFactoryMapping this[object key]
        {
            get
            {
                return base.BaseGet(key) as MudFactoryMapping;
            }
            set
            {
                if (base.BaseGet(key) != null)
                {
                    base.BaseRemove(key);
                }
                this.BaseAdd(value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new MudFactoryMapping();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MudFactoryMapping)element).Key;
        }
    }

    public class MudFactoryMapping : ConfigurationElement
    {
        /// <summary>
        /// An optional name to invoke the mapping by
        /// </summary>
        [ConfigurationProperty("name")]
        public string Name
        {
            get { return base["name"] as string; }
        }

        /// <summary>
        /// This is the type Key for this mapping such as an interface
        /// </summary>
        [ConfigurationProperty("factory-type")]
        public string FactoryType
        {
            get { return base["factory-type"] as string; }
        }

        /// <summary>
        /// The type that will be returned when an object of this mapping is requested
        /// </summary>
        [ConfigurationProperty("mapped-type", IsRequired = true)]
        public string MappedType
        {
            get { return base["mapped-type"] as string; }
        }

        [ConfigurationProperty("key", IsKey = true)]
        public object Key
        {
            get
            {
                if (string.IsNullOrEmpty(FactoryType))
                    return Name;
                else
                    return Type.GetType(FactoryType);
            }
        }

        [ConfigurationProperty("lifecycle", DefaultValue="singleton")]
        [RegexStringValidator("singleton|dynamic")]
        public string LifeCycle
        {
            get
            {
                return base["lifecycle"] as string;
            }
        }       
    }
}
