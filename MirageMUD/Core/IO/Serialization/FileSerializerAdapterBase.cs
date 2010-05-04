using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Mirage.Core.IO.Serialization
{

    /// <summary>
    /// Base class for serializer adapters that use a file as the storage medium
    /// </summary>
    public abstract class FileSerializerAdapterBase : IPersistenceManager
    {
        protected string _basePath;
        protected string _ext = string.Empty;

        public FileSerializerAdapterBase(string basePath, string ext)
        {
            this._basePath = basePath;
            this._ext = ext;
            Directory.CreateDirectory(basePath);
        }

        /// <summary>
        /// Loads the object with the given id
        /// </summary>
        /// <param name="id">the id for the object</param>
        /// <returns>the depersisted object</returns>
        public virtual object Load(string id)
        {
            string path = Path.Combine(_basePath, id + _ext);
            try
            {
                using (StreamReader rdr = new StreamReader(path))
                {
                    return LoadFromReader(rdr);
                }
            }
            catch (FileNotFoundException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new Exception("Error loading file " + path, e);
            }
        }

        /// <summary>
        /// for sub-classes to override, Deserializes the object from the given
        /// stream reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected abstract object LoadFromReader(TextReader reader);

        public virtual void Save(object o, string id, ITransaction txn)
        {
            SerializeHelper(o, id, txn);
        }

        public virtual void Save(object o, string id)
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

        protected virtual void SerializeHelper(object o, string id, ITransaction txn)
        {
            string path = Path.Combine(_basePath, id + _ext);
            using (StreamWriter writer = new StreamWriter(txn.aquireOutputFileStream(path, false)))
            {
                try
                {
                    SerializeToWriter(o, writer);
                }
                catch (IOException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    throw new Exception("Error saving file " + path, e);
                }
            }
        }

        protected abstract void SerializeToWriter(object o, TextWriter writer);
    }
}
