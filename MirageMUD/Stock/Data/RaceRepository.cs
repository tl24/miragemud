using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data;

namespace Mirage.Stock.Data
{
    public interface IRaceRepository : IEnumerable<Race>
    {
        ICollection<Race> Races { get; }
    }

    public class RaceRepository  : JsonSimpleRepository<Race>, IRaceRepository
    {
        public RaceRepository()
            : base("races.jsx")
        {
        }        

        public ICollection<Race> Races
        {
            get { return Items; }
        }       
    }
}
