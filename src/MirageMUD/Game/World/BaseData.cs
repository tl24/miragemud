using System.Collections.Generic;
using Mirage.Game.World.Query;

namespace Mirage.Game.World
{
    public abstract class BaseData : Thing, ISupportUri
    {
        protected BaseData()
        {
            Uri = "";
        }

        #region IQueryable Members

        [Editor(IsKey = true, Priority = 1)]
        public string Uri { get; set; }

        [Editor(Priority=2, IsReadonly=true)]
        public virtual string FullUri
        {
            get { return this.Uri; }
        }

        public override string ToString()
        {
            return base.ToString() + " mud://" + FullUri;
        }
        #endregion

        /// <summary>
        /// Copy this items properties to another item
        /// </summary>
        /// <param name="other">other item to copy to</param>
        public virtual void CopyTo(BaseData other)
        {
            other.Uri = this.Uri;
        }




    }
}
