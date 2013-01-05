using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Core.Transactions
{
    public interface ITransactionService
    {
        /// <summary>
        /// Starts a transaction
        /// </summary>
        /// <returns>the transaction</returns>
        ITransaction StartTransaction();
    }
}
