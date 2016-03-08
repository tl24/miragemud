using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Mirage.Core.Command
{
    public interface IReflectedCommandFactory
    {
        IReflectedCommandGroup GetCommandGroup(Type groupType);
        ICommand CreateCommand(MethodInfo method, IReflectedCommandGroup commandGroup);
    }
}
