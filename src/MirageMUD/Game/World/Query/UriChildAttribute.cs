using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Game.World.Query
{
    [global::System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class UriChildAttribute : System.Attribute
    {
        public UriChildAttribute()
        {
        }

        public UriChildAttribute(string name)
        {
            this.Name = name;
        }

        public UriChildAttribute(QueryHints hints)
        {
            this.Hints = hints;
        }

        public UriChildAttribute(string name, QueryHints hints)
        {
            this.Name = name;
            this.Hints = hints;
        }

        public string Name { get; set; }

        public QueryHints Hints { get; set; }
    }
}
