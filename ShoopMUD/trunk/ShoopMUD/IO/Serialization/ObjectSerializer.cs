using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;
using System.Reflection;

namespace Shoop.IO.Serialization
{
    public class ObjectSerializerFactory
    {
        private static ObjectSerializerConfiguration config;

        static ObjectSerializerFactory()
        {
            config = (ObjectSerializerConfiguration) ConfigurationManager.GetSection("serializerFactory");

        }

        public static IObjectSerializer getSerializer(string basePath, Type t)
        {
            if (config == null)
            {
                return new XmlSerializerAdapter(basePath, t, ".xml");
            }
            else
            {
                return (IObjectSerializer)createInstance(config.defaultFactory, basePath, t);
            }
        }

        private static object createInstance(string serializerKey, string basePath, Type serializedType)
        {
            return createInstance(config.Serializers[serializerKey], basePath, serializedType);
        }


        private static object createInstance(SerializerFactoryConfig conf, string basePath, Type serializedType)
        {
            Type t = Type.GetType(conf.ClassName);
            return t.GetConstructor(new Type[] { typeof(string), typeof(Type), typeof(string) }).Invoke(new object[] { basePath, serializedType, conf.Extension });
        }

        public static IObjectDeserializer getDeserializer(string basePath, Type t, string name)
        {
            if (config == null)
            {
                return new XmlSerializerAdapter(basePath, t, ".xml");
            }
            else
            {
                SerializerFactoryConfig conf = config.Serializers[config.defaultFactory];
                if (File.Exists(Path.Combine(basePath, name + conf.Extension)))
                {
                    return (IObjectDeserializer)createInstance(conf, basePath, t);
                }
                else
                {
                    // look for suitable type in order
                    foreach (SerializerFactoryConfig fconf in config.Serializers)
                    {
                        if (File.Exists(Path.Combine(basePath, name + fconf.Extension)))
                        {
                            return (IObjectDeserializer)createInstance(fconf, basePath, t);
                        }
                    }

                    // nothing matched, return the default
                    return (IObjectDeserializer)createInstance(conf, basePath, t);
                }
            }
        }

    }

    /// <summary>
    /// An object that will Deserialize a given object
    /// </summary>
    public interface IObjectDeserializer
    {
        /// <summary>
        ///     Deserialize the object refered to by name
        /// </summary>
        /// <param name="name">the name of the object</param>
        /// <returns>the created object</returns>
        object Deserialize(string name);
    }

    /// <summary>
    /// An object that will Serialize a given object
    /// </summary>
    public interface IObjectSerializer
    {
        /// <summary>
        ///     Serialize the given object using the passed transaction
        /// </summary>
        /// <param name="o">the object to serialize</param>
        /// <param name="name">the name of the object</param>
        /// <param name="txn">the transaction that the Serialization process will be part of</param>
        void Serialize(object o, string name, ITransaction txn);

        /// <summary>
        ///     Serialize the given object.  The Serialization process will be
        /// a single transaction.
        /// </summary>
        /// <param name="o">the object to serialize</param>
        /// <param name="name">the name of the object</param>
        void Serialize(object o, string name);
    }

    public class ObjectSerializerConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("default", DefaultValue="xml")]
        public string defaultFactory
        {
            get { return this["default"] as string; }
        }

        [ConfigurationProperty("serializers", IsDefaultCollection=true)]
        public SerializerFactoryConfigCollection Serializers {
            get { return this["serializers"] as SerializerFactoryConfigCollection; }
        }
    }

    public class SerializerFactoryConfig : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey=true, IsRequired=true)]
        public string Name {
            get { return this["name"] as string; }
        }

        [ConfigurationProperty("extension")]
        public string Extension {
            get { return this["extension"] as string; }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string ClassName {
            get { return this["type"] as String; }
        }
    }

    public class SerializerFactoryConfigCollection : ConfigurationElementCollection
    {
        public SerializerFactoryConfig this[int index]
        {
            get
            {
                return base.BaseGet(index) as SerializerFactoryConfig;
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

        public SerializerFactoryConfig this[string key] {
            get {
                return base.BaseGet(key) as SerializerFactoryConfig;
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
            return new SerializerFactoryConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SerializerFactoryConfig)element).Name;
        }

    }
}
