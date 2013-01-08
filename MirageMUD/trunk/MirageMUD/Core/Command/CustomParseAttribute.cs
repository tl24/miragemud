using System;

namespace Mirage.Core.Command
{
    [AttributeUsageAttribute(System.AttributeTargets.Parameter)]
    public class CustomParseAttribute : CommandArgumentAttribute
    {
    }
}
