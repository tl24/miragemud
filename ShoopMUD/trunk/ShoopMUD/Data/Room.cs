using System;
using System.Collections.Generic;
using System.Text;
using Shoop.Data.Query;

namespace Shoop.Data
{
    public class Room : BaseData, IViewable
    {
        private string _title;
        private string _shortDescription;
        private string _longDescription;
        private Area _area;
        private LinkedList<Animate> _animates;

        public Room()
            : base()
        {
            _animates = new LinkedList<Animate>();
            _uriProperties.Add("Animates", new QueryableCollectionAdapter<Animate>(_animates, "Animates"));
        }

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

        public string Title
        {
            get { return this._title; }
            set { this._title = value; }
        }

        public ICollection<Animate> Animates
        {
            get { return this._animates; }
        }

        public override string FullURI
        {
            get
            {
                if (this._area != null)
                    return this._area.FullURI + "/Rooms/" + this.URI;
                else
                    return this.URI;
            }
        }
    }
}
