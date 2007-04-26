using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Data
{
    public class Room : BaseData
    {
        private string _name;
        private string _shortDescription;
        private string _longDescription;
        private Area _area;

        public Shoop.Data.Area Area
        {
            get { return this._area; }
            set { this._area = value; }
        }

        public string LongDescription
        {
            get { return this._longDescription; }
            set { this._longDescription = value; }
        }

        public string ShortDescription
        {
            get { return this._shortDescription; }
            set { this._shortDescription = value; }
        }

        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

    }
}
