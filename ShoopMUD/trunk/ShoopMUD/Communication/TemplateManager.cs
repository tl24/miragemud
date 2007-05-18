using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Resources;
using System.Reflection;

namespace Shoop.Communication
{
    public class TemplateManager
    {
        private static TemplateManager _instance;
        private IDictionary<string, TemplateDefinition> _cache;
        ResourceManager resourceManager;
        private bool initted;

        private TemplateManager()
        {
        }

        static TemplateManager()
        {
            _instance = new TemplateManager();
        }

        private void init()
        {
            resourceManager = new ResourceManager("Shoop.Strings", Assembly.GetExecutingAssembly());
            _cache = new Dictionary<string, TemplateDefinition>();            
            initted = true;
        }

        private ITemplate LookupTemplate(string name)
        {
            if (!initted)
                init();

            TemplateDefinition def = null;
            if (_cache.ContainsKey(name))
            {
                def = _cache[name];
            }
            else
            {
                string template = resourceManager.GetString(name);
                def = new TemplateDefinition(name, template, false);
                _cache[name] = def;
            }
            return new TemplateRenderer(def, null);
        }

        public static ITemplate GetTemplate(string name)
        {
            return _instance.LookupTemplate(name);
        }

        public static ITemplate CreateTemplate(string name, string templateText)
        {
            return new TemplateRenderer(new TemplateDefinition(name, templateText, false), null);
        }
    }

    public class TemplateDefinition
    {
        private enum TemplateParamType {
            Variable,
            TemplateRef
        }

        public TemplateDefinition(string name, string text, bool suppressNewLine)
        {
            this._name = name;
            this._text = text + (suppressNewLine ? "" : "\r\n");
        }

        private string _text;
        private string _name;
        private bool _isParsed;
        private IDictionary<string, TemplateParamType> _availableParams;

        public string Text
        {
            get { return this._text; }
        }

        public string Name
        {
            get { return this._name; }
        }

        public ICollection<string> AvailableParameters
        {
            get {
                if (!_isParsed)
                {
                    parse();
                }
                return _availableParams.Keys;
            }
        }

        private void parse()
        {
            _availableParams = new Dictionary<string, TemplateParamType>();
            Regex parser = new Regex(@"\$\{([^}]+)\}");
            Match m = parser.Match(_text);
            while (m.Success)
            {
                _availableParams[m.Groups[1].Value] = TemplateParamType.Variable;
                m = m.NextMatch();
            }
            _isParsed = true;
        }

        public string Render(TemplateRenderer renderer) {
            if (!_isParsed)
                parse();

            if (_availableParams.Count == 0)
                return _text;

            StringBuilder sb = new StringBuilder(_text);
            foreach (string key in _availableParams.Keys)
            {
                sb.Replace(@"${" + key + "}", renderer.ResolveParameter(key));
            }
            return sb.ToString();
        }
    }

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
            return _definition.Render(this);
        }


        #endregion
    
        public string  ResolveParameter(string key)
        {
 	        object value = this[key];
            if (value == null && _parent != null) {
                value = _parent.ResolveParameter(key);
            }
            if (value != null && !(value is string))
                value = value.ToString();
            if (value == null)
                value = "";

            return (string) value;
        }

    }
}
