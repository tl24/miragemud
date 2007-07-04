using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Data.Attribute
{
    public class BaseAttribute : Thing, IAttribute
    {
        private IAttributable _target;

        public IAttributable Target
        {
            get { return this._target; }
            set { this._target = value; }
        }

        public BaseAttribute() : base()
        {
        }

        public BaseAttribute(IAttributable target) : base()
        {
            this._target = target;
        }
    }
}
