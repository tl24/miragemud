using System;
namespace Mirage.Core.Command
{
    public interface IReflectedCommandGroup
    {
        object GetInstance();
    }
}
