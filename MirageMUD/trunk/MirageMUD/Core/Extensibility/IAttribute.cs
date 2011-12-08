using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Extensibility
{
    public interface IAttribute 
    {
        IAttributable Target { get; set; }
    }
}
