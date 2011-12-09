using System.Collections.Generic;

namespace Mirage.Game.World
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
