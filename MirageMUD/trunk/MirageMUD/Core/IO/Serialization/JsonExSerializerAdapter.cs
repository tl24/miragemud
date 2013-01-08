using System;
using System.IO;
using JsonExSerializer;

namespace Mirage.Core.IO.Serialization
{
    /// <summary>
    /// Persistence adapter that uses file storage with the object encoded
    /// as JSON text.
    /// </summary>
    class JsonExPersistenceAdapter : FileSerializerAdapterBase
    {
        private Serializer _serializer;
 
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
