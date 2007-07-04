using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Communication
{
    public class TemplateRenderer : ITemplate
    {
        private TemplateDefinition _definition;
        private ITemplate _parent;
        private IDictionary<string, object> _params;
        private object _context;
        public TemplateRenderer(TemplateDefinition definition, object context)
        {
            this._definition = definition;
            this._context = context;
            _params = new Dictionary<string, object>();
        }



        #region ITemplate Members

        public string RawText
        {
            get { return _definition.Text; }
        }

        public ITemplate ParentTemplate
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public ICollection<string> AvailableParameters
        {
            get { return _definition.AvailableParameters; }
        }

        public object this[string key]
        {
            get
            {
                if (_params.ContainsKey(key))
                {
                    return _params[key];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                _params[key] = value;
            }
        }

        public object Context
        {
            get { return _context; }
            set { _context = value; }
        }

        public string Render()
        {
            return Render(false);
        }

        public string Render(bool suppressNewline)
        {
            return _definition.Render(this, suppressNewline);
        }

        #endregion

        public string ResolveParameter(string key)
        {
            object value = this[key];
            if (value == null && _parent != null)
            {
                value = _parent.ResolveParameter(key);
            }
            if (value != null && !(value is string))
                value = value.ToString();
            if (value == null)
                value = "";

            return (string)value;
        }

    }
}
