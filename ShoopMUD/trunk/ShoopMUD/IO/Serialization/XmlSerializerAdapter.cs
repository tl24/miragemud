using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Shoop.IO.Serialization
{
    /// <summary>
    /// Serializes/Deserializes object into xml format
    /// </summary>
    public class XmlSerializerAdapter : IObjectSerializer, IObjectDeserializer
    {

        private XmlSerializer _serializer;
        private string _basePath;
        private string _name;
        private string ext = ".xml";

        public XmlSerializerAdapter(string basePath, Type t, string ext)
        {
            _serializer = new XmlSerializer(t);
            this._basePath = basePath;
            this.ext = ext;
        }

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
            Stream stm = txn.aquireOutputFileStream(Path.Combine(_basePath, name + ext), false);
            try
            {
                _serializer.Serialize(stm, o);
            }
            finally
            {
                stm.Close();
            }
        }

        #endregion

        #region IObjectDeserializer Members

        public object Deserialize(string name)
        {
            StreamReader stm = new StreamReader(Path.Combine(_basePath, name + ext));
            object value = _serializer.Deserialize(stm);
            stm.Close();
            return value;
        }

        #endregion
    }
}
