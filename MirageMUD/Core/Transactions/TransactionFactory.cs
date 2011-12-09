using System;
using System.Collections.Generic;
using System.IO;

namespace Mirage.Core.Transactions
{
    public static class TransactionFactory {

        private static ITransactionService defaultService;

        public static ITransaction StartTransaction()
        {
            return GetDefaultService().StartTransaction();
        }

        private static ITransactionService GetDefaultService()
        {
            if (defaultService == null)
            {
                lock (typeof(TransactionFactory))
                {
                    if (defaultService == null)
                    {
                        defaultService = new SimpleTransactionService();
                    }
                }
            }
            return defaultService;
        }
    }

    public interface ITransactionService
    {
        ITransaction StartTransaction();
    }

    public interface ITransaction
    {
        Stream AquireOutputFileStream(string uri, bool append);
        Stream AquireInputFileStream(string uri, FileMode mode);
        void Commit();
        void Rollback();
    }

    public class SimpleTransactionService : ITransactionService
    {
        #region ITransactionService Members

        public ITransaction StartTransaction()
        {
            return new Transaction();
        }

        #endregion

        public class Transaction : ITransaction, IDisposable
        {
            private bool committed = false;
            private bool inprocess = true;
            private Dictionary<string, string> txnItems = new Dictionary<string, string>();
            #region ITransaction Members

            public Stream AquireOutputFileStream(string uri, bool append)
            {
                string dir = Path.GetDirectoryName(uri);
                string tmpDir = Path.Combine(dir, ".txn");
                if (!Directory.Exists(tmpDir))
                {
                    Directory.CreateDirectory(tmpDir);
                }
                string newUri = Path.Combine(tmpDir, Path.GetFileName(uri));
                if (File.Exists(newUri)) {
                    File.Delete(newUri);
                }
                if (append && File.Exists(uri))
                {
                    File.Copy(uri, newUri);
                }
                txnItems[newUri] = uri;
                return new FileStream(newUri, append ? FileMode.Append : FileMode.Create, FileAccess.Write);
            }

            public Stream  AquireInputFileStream(string uri, FileMode mode)
            {
                return new FileStream(uri, mode, FileAccess.Read);              	            
            }

            public void  Commit()
            {
                //TODO: In a real transaction system we'd probably keep a log of this stuff
                // so we could roll back
                // copy the temp files we created onto the original files
                foreach (KeyValuePair<string, string> keyValue in txnItems)
                {
                    string newFile = keyValue.Key;
                    string oldFile = keyValue.Value;
                    File.Delete(oldFile);
                    File.Copy(newFile, oldFile);
                }
                //Delete all the temp files, only after we're sure we copied over to the
                //original files successfully
                foreach (string deleteFile in txnItems.Keys)
                {
                    
                    File.Delete(deleteFile);
                }
                txnItems.Clear();
                committed = true;
                inprocess = false;
            }

            public void  Rollback()
            {
                //Delete all the temp files, only after we're sure we copied over to the
                //original files successfully
                foreach (string deleteFile in txnItems.Keys)
                {
                    File.Delete(deleteFile);
                }
                txnItems.Clear();
                inprocess = false;
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                if (inprocess && !committed)
                {
                    Rollback();
                }
            }

            #endregion
        }
    }

}
