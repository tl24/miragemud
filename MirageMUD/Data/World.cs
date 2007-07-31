using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;

namespace Mirage.Data
{
    /// <summary>
    /// Container class to represent the top-level object sent to
    /// the gui builder upon initialization.
    /// </summary>
    public class World
    {
        [JsonExIgnore]
        [EditorTreeProperty("GetAreas", "msg:/builder/area/AreaList", 
            "GetArea", "msg:builder/area/Area", typeof(Area))]
        public IDictionary<string, Area> Areas
        {
            get { return new Dictionary<string, Area>(); }
        }
    }
}
