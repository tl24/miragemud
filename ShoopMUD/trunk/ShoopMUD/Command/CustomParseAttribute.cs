using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Command
{
    [AttributeUsageAttribute(System.AttributeTargets.Parameter)]
    public class CustomParseAttribute : BaseArgumentAttribute
    {
    }
}
