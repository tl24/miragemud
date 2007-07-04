using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace Mirage.IO.Serialization
{
    class JSONPersistenceAdapter : IPersistenceManager
    {
        private JsonSerializer _serializer;
        private Type _objectType;
        private string _basePath;
        private string _name;
        private string ext = ".js";

        public JSONPersistenceAdapter(string basePath, Type t, string ext)
        {
            this._serializer = new JsonSerializer();
            _serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            this._objectType = t;
            this._basePath = basePath;
            this.ext = ext;
        }

        #region IObjectDeserializer Members

        public object Load(string id)
        {
            JsonReader jrdr = new JsonReader(new StreamReader(Path.Combine(_basePath, id + ext)));
            object value = _serializer.Deserialize(jrdr,_objectType);
            jrdr.Close();
            return value;
        }

        #endregion

        #region IObjectSerializer Members

        public void Save(object o, string id, ITransaction txn)
        {
            SerializeHelper(o, id, txn);
        }

        public void Save(object o, string id)
        {
            ITransaction txn = TransactionFactory.startTransaction();
            try
            {
                SerializeHelper(o, id, txn);
                txn.commit();
            }
            catch (Exception e)
            {
                txn.rollback();
                Console.WriteLine(e);
            }
        }

        private void SerializeHelper(object o, string id, ITransaction txn)
        {

            JsonWriter writer = new JsonWriter(new StreamWriter(txn.aquireOutputFileStream(Path.Combine(_basePath, id + ext), false)));
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
