using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Data.Attribute
{
    public interface IAttribute 
    {
        IAttributable Target { get; set; }
    }
}
