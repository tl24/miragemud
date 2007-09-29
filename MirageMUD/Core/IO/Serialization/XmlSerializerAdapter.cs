using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Mirage.Core.IO.Serialization
{
    /// <summary>
    /// Serializes/Deserializes object into xml format
    /// </summary>
    public class XmlPersistenceAdapter : FileSerializerAdapterBase
    {

        private XmlSerializer _serializer;
        private string _name;

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
