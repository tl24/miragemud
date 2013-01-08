using System;
namespace Mirage.Game.Command.Infrastructure
{
    public interface IReflectedCommandGroup
    {
        object GetInstance();
    }
}
