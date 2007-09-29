using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;
using Mirage.Core.Data;

namespace Mirage.Stock.Data
{
    /// <summary>
    /// Container class to represent the top-level object sent to
    /// the gui builder upon initialization.
    /// </summary>
    public class World
    {
        [JsonExIgnore]
        [EditorCollection(typeof(Area))]
        public IDictionary<string, Area> Areas
        {
            get { return new Dictionary<string, Area>(); }
        }
    }
}
