using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Resources;
using System.Reflection;

namespace Mirage.Communication
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
            resourceManager = new ResourceManager("Mirage.Strings", Assembly.GetExecutingAssembly());
            _cache = new Dictionary<string, TemplateDefinition>();            
            initted = true;
        }

        private ITemplate LookupTemplate(string name)
        {
            return new TemplateRenderer(GetDefinition(name), null);
        }

        public TemplateDefinition GetDefinition(string name)
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
            return def;
        }

        public static TemplateDefinition GetTemplateDefinition(string name)
        {
            return _instance.GetDefinition(name);
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


}
