using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace Shoop.IO.Serialization
{
    class JSONSerializerAdapter : IObjectDeserializer, IObjectSerializer
    {
        private JsonSerializer _serializer;
        private Type _objectType;
        private string _basePath;
        private string _name;
        private string ext = ".js";

        public JSONSerializerAdapter(string basePath, Type t, string ext)
        {
            this._serializer = new JsonSerializer();
            _serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            this._objectType = t;
            this._basePath = basePath;
            this.ext = ext;
        }

        #region IObjectDeserializer Members

        public object Deserialize(string name)
        {
            JsonReader jrdr = new JsonReader(new StreamReader(Path.Combine(_basePath, name + ext)));
            object value = _serializer.Deserialize(jrdr,_objectType);
            jrdr.Close();
            return value;
        }

        #endregion

        #region IObjectSerializer Members

        public void Serialize(object o, string name, ITransaction txn)
        {
            SerializeHelper(o, name, txn);
        }

        public void Serialize(object o, string name)
        {
            ITransaction txn = TransactionFactory.startTransaction();
            try
            {
                SerializeHelper(o, name, txn);
                txn.commit();
            }
            catch (Exception e)
            {
                txn.rollback();
                Console.WriteLine(e);
            }
        }

        private void SerializeHelper(object o, string name, ITransaction txn)
        {

            JsonWriter writer = new JsonWriter(new StreamWriter(txn.aquireOutputFileStream(Path.Combine(_basePath, name + ext), false)));
            try
            {
                _serializer.Serialize(writer, o);
            }
            finally
            {
                writer.Close();
            }
        }
        #endregion
    }
}
