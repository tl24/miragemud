using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Data
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
