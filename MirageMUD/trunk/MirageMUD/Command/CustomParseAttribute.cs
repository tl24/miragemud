using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Command
{
    [AttributeUsageAttribute(System.AttributeTargets.Parameter)]
    public class CustomParseAttribute : BaseArgumentAttribute
    {
    }
}
