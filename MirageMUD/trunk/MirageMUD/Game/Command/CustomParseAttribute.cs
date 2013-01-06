using System;
using Mirage.Game.Command.Infrastructure;

namespace Mirage.Game.Command
{
    [AttributeUsageAttribute(System.AttributeTargets.Parameter)]
    public class CustomParseAttribute : CommandArgumentAttribute
    {
    }
}
