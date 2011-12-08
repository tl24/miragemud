using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;
using Mirage.Game.World;

namespace Mirage.Game.World
{
    /// <summary>
    /// Container class to represent the top-level object sent to
    /// the gui builder upon initialization.
    /// </summary>
    public class MudWorld
    {
        [JsonExIgnore]
        [EditorCollection(typeof(Area))]
        public IDictionary<string, Area> Areas
        {
            get { return new Dictionary<string, Area>(); }
        }
    }
}
