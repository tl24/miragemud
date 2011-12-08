using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Game.Command
{
    /// <summary>
    /// Handler interface for handling initial input for a connection until
    /// authentication occurs
    /// </summary>
    public interface ILoginInputHandler
    {
        void HandleInput(object input);
    }
}
