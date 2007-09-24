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
    public class XmlPersistenceAdapter : IPersistenceManager
    {

        private XmlSerializer _serializer;
        private string _basePath;
        private string _name;
        private string ext = ".xml";

        public XmlPersistenceAdapter(string basePath, Type t, string ext)
        {
            _serializer = new XmlSerializer(t);
            this._basePath = basePath;
            this.ext = ext;
        }

        #region IPersistenceManager Members

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
            Stream stm = txn.aquireOutputFileStream(Path.Combine(_basePath, id + ext), false);
            try
            {
                _serializer.Serialize(stm, o);
            }
            finally
            {
                stm.Close();
            }
        }

        public object Load(string id)
        {
            StreamReader stm = new StreamReader(Path.Combine(_basePath, id + ext));
            object value = _serializer.Deserialize(stm);
            stm.Close();
            return value;
        }

        #endregion
    }
}
