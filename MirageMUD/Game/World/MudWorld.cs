using System.Collections.Generic;
using JsonExSerializer;

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
