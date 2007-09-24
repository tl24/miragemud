using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Data.Attribute
{
    public interface IAttribute 
    {
        IAttributable Target { get; set; }
    }
}
