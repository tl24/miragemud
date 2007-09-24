using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;
using System.IO;

namespace Mirage.Core.IO.Serialization
{
    class JsonExPersistenceAdapter : IPersistenceManager
    {
        private Serializer _serializer;
        private Type _objectType;
        private string _basePath;
        private string _name;
        private string ext = ".jsx";

        public JsonExPersistenceAdapter(string basePath, Type t, string ext)
        {
            this._serializer = Serializer.GetSerializer(t);
            this._serializer.Context.ReferenceWritingType = SerializationContext.ReferenceOption.WriteIdentifier;
            this._objectType = t;
            this._basePath = basePath;
            this.ext = ext;
        }

        #region IPersistenceManager Members

        public object Load(string id)
        {
            StreamReader rdr = new StreamReader(Path.Combine(_basePath, id + ext));
            object value = _serializer.Deserialize(rdr);
            rdr.Close();
            return value;
        }

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

            StreamWriter writer = new StreamWriter(txn.aquireOutputFileStream(Path.Combine(_basePath, id + ext), false));
            try
            {
                _serializer.Serialize(o, writer);
            }
            finally
            {
                writer.Close();
            }
        }
        #endregion
    }
}
