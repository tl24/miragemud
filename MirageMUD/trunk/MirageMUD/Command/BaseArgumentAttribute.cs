using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Command
{
    [AttributeUsageAttribute(System.AttributeTargets.Parameter,AllowMultiple=false)]
    public abstract class BaseArgumentAttribute : System.Attribute
    {
    }
}
