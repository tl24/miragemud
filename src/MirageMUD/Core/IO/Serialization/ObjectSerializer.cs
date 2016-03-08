using System;
using System.Configuration;
using Mirage.Core.Transactions;

namespace Mirage.Core.IO.Serialization
{
    public class ObjectStorageFactory
    {
        private static ObjectStorageConfiguration config;

        static ObjectStorageFactory()
        {
            config = (ObjectStorageConfiguration)ConfigurationManager.GetSection("MirageMUD/ObjectStorageFactory");

        }

        public static IPersistenceManager GetPersistenceManager(Type t)
        {
            foreach (PersistenceManagerConfig manager in config.PersistenceManagers)
            {
                if (manager.GetPersistedType().IsAssignableFrom(t))
                {
                    return (IPersistenceManager) Activator.CreateInstance(manager.GetFactoryType(), manager.BasePath, manager.GetPersistedType(), manager.FileExtension);
                }
            }
            throw new ApplicationException("No persistence manager found for type: " + t);
        }
    }

    /// <summary>
    /// An object that will Deserialize a given object
    /// </summary>
    public interface IPersistenceManager
    {
        /// <summary>
        ///     Load the object refered to by id
        /// </summary>
        /// <param name="name">the id of the object</param>
        /// <returns>the loaded object</returns>
        object Load(string id);

       /// <summary>
        ///     Save the given object using the passed transaction
        /// </summary>
        /// <param name="o">the object to save</param>
        /// <param name="id">the name of the object</param>
        /// <param name="txn">the transaction that the Serialization process will be part of</param>
        void Save(object o, string id, ITransaction txn);

        /// <summary>
        ///     Save the given object.  The save process will be
        /// a single transaction.
        /// </summary>
        /// <param name="o">the object to save</param>
        /// <param name="id">the id of the object</param>
        void Save(object o, string id);
    }

    public class ObjectStorageConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("PersistenceManagers", IsDefaultCollection = true)]
        public PersistenceManagerConfigCollection PersistenceManagers {
            get { return this["PersistenceManagers"] as PersistenceManagerConfigCollection; }
        }
    }

    public class PersistenceManagerConfig : ConfigurationElement
    {
        private Type _factoryType;
        private Type _persistedType;

        [ConfigurationProperty("name", IsKey=true, IsRequired=true)]
        public string Name {
            get { return this["name"] as string; }
        }

        [ConfigurationProperty("base-path")]
        public string BasePath
        {
            get { return this["base-path"] as string; }
        }

        [ConfigurationProperty("file-extension")]
        public string FileExtension {
            get { return this["file-extension"] as string; }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string FactoryClass {
            get { return this["type"] as string; }
        }

        public Type GetFactoryType()
        {
            if (_factoryType == null)
            {
                _factoryType = Type.GetType(FactoryClass, true);
            }
            return _factoryType;

        }

        [ConfigurationProperty("persisted-type", IsRequired = true)]
        public string PersistedClass
        {
            get { return this["persisted-type"] as string; }
        }

        public Type GetPersistedType()
        {
            if (_persistedType == null)
            {
                _persistedType = Type.GetType(PersistedClass, true);
            }
            return _persistedType;
        }

    }

    public class PersistenceManagerConfigCollection : ConfigurationElementCollection
    {
        public PersistenceManagerConfig this[int index]
        {
            get
            {
                return base.BaseGet(index) as PersistenceManagerConfig;
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

        public new PersistenceManagerConfig this[string key] {
            get {
                return base.BaseGet(key) as PersistenceManagerConfig;
            }
            set {
                if (base.BaseGet(key) != null)
                {
                    base.BaseRemove(key);
                }
                this.BaseAdd(value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new PersistenceManagerConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PersistenceManagerConfig)element).Name;
        }

    }
}
