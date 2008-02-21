using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;
using System.IO;

namespace Mirage.Core.IO.Serialization
{
    /// <summary>
    /// Persistence adapater that uses file storage with the object encoded
    /// as JSON text.
    /// </summary>
    class JsonExPersistenceAdapter : FileSerializerAdapterBase
    {
        private Serializer _serializer;
        private string _name;

        public JsonExPersistenceAdapter(string basePath, Type t, string ext) : base(basePath, ext)
        {
            this._serializer = new Serializer(t);
            this._serializer.Context.ReferenceWritingType = SerializationContext.ReferenceOption.WriteIdentifier;
        }

        protected override object LoadFromReader(TextReader reader)
        {
            return _serializer.Deserialize(reader);
        }

        protected override void SerializeToWriter(object o, TextWriter writer)
        {
            _serializer.Serialize(o, writer);
        }
    }
}
