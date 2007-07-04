using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Data.Attribute
{
    public interface IAttribute 
    {
        IAttributable Target { get; set; }
    }
}
