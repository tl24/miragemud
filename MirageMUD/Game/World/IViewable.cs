using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Game.World
{
    public interface IViewable
    {
        string Title { get; }
        string ShortDescription { get; }
        string LongDescription { get; }
    }
}
