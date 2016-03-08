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




}
