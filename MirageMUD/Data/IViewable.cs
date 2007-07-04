using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Data
{
    public interface IViewable
    {
        string Title { get; }
        string ShortDescription { get; }
        string LongDescription { get; }
    }
}
