using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Mirage.Game.Command.Infrastructure
{
    public interface IReflectedCommandFactory
    {
        IReflectedCommandGroup GetCommandGroup(Type groupType);
        ICommand CreateCommand(MethodInfo method, IReflectedCommandGroup commandGroup);
    }
}
