using System.Collections.Generic;

namespace Mirage.Game.World
{
    public interface IAreaRepository : IEnumerable<IArea>
    {
        IArea Load(string areaUri);

        void LoadAll();

        void Save(IArea area);

        void Save(string areaUri);

        void Add(IArea area);

        void Update(IArea area);

        void Remove(IArea area);

        IDictionary<string, IArea> Areas { get; }
    }
}
