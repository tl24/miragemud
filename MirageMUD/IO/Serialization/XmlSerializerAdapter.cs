using System;
using System.IO;
using System.Xml.Serialization;

namespace Mirage.IO.Serialization
{
    /// <summary>
    /// Serializes/Deserializes object into xml format
    /// </summary>
    public class XmlPersistenceAdapter : FileSerializerAdapterBase
    {

        private XmlSerializer _serializer;

        public XmlPersistenceAdapter(string basePath, Type t, string ext) : base(basePath, ext)
        {
            _serializer = new XmlSerializer(t);
        }

        protected override object LoadFromReader(TextReader reader)
        {
            return _serializer.Deserialize(reader);
        }

        protected override void SerializeToWriter(object o, TextWriter writer)
        {
            _serializer.Serialize(writer, o);
        }

    }
}
