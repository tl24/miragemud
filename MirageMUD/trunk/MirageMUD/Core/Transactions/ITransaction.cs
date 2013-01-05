using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Mirage.Core.Transactions
{
    public interface ITransaction : IDisposable
    {
        Stream AquireOutputFileStream(string uri, bool append);
        Stream AquireInputFileStream(string uri, FileMode mode);
        void Commit();
        void Rollback();
    }
}
