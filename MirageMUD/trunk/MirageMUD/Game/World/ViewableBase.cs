using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Game.World
{
    public class ViewableBase : BaseData, IViewable
    {
        private string _title;
        private string _shortDescription;
        private string _longDescription;

        [Editor(Priority = 3)]
        public virtual string Title
        {
            get { return this._title; }
            set { this._title = value; }
        }

        [Editor(Priority = 4)]
        public virtual string ShortDescription
        {
            get { return this._shortDescription; }
            set { this._shortDescription = value; }
        }

        [Editor(Priority = 5, EditorType = "Multiline")]
        public virtual string LongDescription
        {
            get { return this._longDescription; }
            set { this._longDescription = value; }
        }

        public override void CopyTo(BaseData other)
        {
            base.CopyTo(other);
            ViewableBase otherViewable = other as ViewableBase;
            if (otherViewable != null)
            {
                otherViewable.Title = this.Title;
                otherViewable.ShortDescription = this.ShortDescription;
                otherViewable.LongDescription = this.LongDescription;
            }
        }
    }
}
