using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Mirage.Core.Communication
{

    /// <summary>
    /// An class that holds the definition for a template.  This class is immutable
    /// so that it may be cached by the application.   Use the TemplateRenderer class to
    /// assign replacement parameters and render the template.
    /// </summary>
    public class TemplateDefinition
    {
        /// <summary>
        /// The unparsed template text
        /// </summary>
        private string _text;

        /// <summary>
        /// The name of the template
        /// </summary>
        private string _name;

        /// <summary>
        /// Flag to indicate if the template has been parsed yet
        /// </summary>
        private bool _isParsed;

        /// <summary>
        /// Available parameters for the template.  This is only populated
        /// after the template is parsed.
        /// </summary>
        private IDictionary<string, int> _availableParams;

        /// <summary>
        /// Any references to other templates
        /// </summary>
        private IDictionary<string, TemplateDefinition> _templateReferences;

        /// <summary>
        /// Creates an instance of the template with the given name and template text.
        /// </summary>
        /// <param name="name">The name of the template for informational purposes</param>
        /// <param name="text">the template text</param>
        public TemplateDefinition(string name, string text, bool suppressNewLine)
        {
            this._name = name;
            this._text = text;
        }

        /// <summary>
        /// The template text
        /// </summary>
        public string Text
        {
            get { return this._text; }
        }

        /// <summary>
        /// name of the template
        /// </summary>
        public string Name
        {
            get { return this._name; }
        }

        /// <summary>
        /// The available replacement parameters defined in the template
        /// </summary>
        public ICollection<string> AvailableParameters
        {
            get
            {
                if (!_isParsed)
                {
                    parse();
                }
                return _availableParams.Keys;
            }
        }

        /// <summary>
        /// Parse the template to find all parameters referenced by the template.
        /// The _availableParams variable is not populated until this is called.  
        /// The template should be parsed as late as possible.
        /// </summary>
        private void parse()
        {
            _availableParams = new Dictionary<string, int>();
            _templateReferences = new Dictionary<string, TemplateDefinition>();
            Regex refParser = new Regex(@"\@\{([^}]+)\}");
            Match m = refParser.Match(_text);
            while (m.Success)
            {
                if (!_availableParams.ContainsKey(m.Groups[1].Value))
                {
                    TemplateDefinition reference = TemplateManager.GetTemplateDefinition(m.Groups[1].Value);
                    foreach (string name in reference.AvailableParameters)
                    {
                        AddAvailableParam(name);
                    }
                    
                    _templateReferences[m.Groups[1].Value] = reference;
                }
                else
                {
                    _availableParams[m.Groups[1].Value] = 1;
                }
                m = m.NextMatch();
            }

            Regex parser = new Regex(@"\$\{([^}]+)\}");
            m = parser.Match(_text);
            while (m.Success)
            {
                AddAvailableParam(m.Groups[1].Value);
                m = m.NextMatch();
            }
            _isParsed = true;
        }

        private void AddAvailableParam(string name)
        {
            if (_availableParams.ContainsKey(name))
            {
                // already there, increment the count
                _availableParams[name] += 1;
            }
            else
            {
                _availableParams[name] = 1;
            }
        }

        /// <summary>
        /// Renders the template to a string by replacing any parameters.  The
        /// renderer parameter is used to resolve parameters.  If suppressNewLine
        /// is false, a newline sequence will automatically be added to the template.
        /// </summary>
        /// <param name="renderer">template renderer containing any replacement values</param>
        /// <param name="suppressNewLine">true if automatic newline endings should be suppressed</param>
        /// <returns>replaced template text</returns>
        public string Render(TemplateRenderer renderer, bool suppressNewLine)
        {
            if (!_isParsed)
                parse();

            if (_availableParams.Count == 0)
                return _text;

            StringBuilder sb = new StringBuilder(_text);
            // resolve references first
            foreach (KeyValuePair<string, TemplateDefinition> pair in _templateReferences)
            {
                sb.Replace(@"@{" + pair.Key + "}", pair.Value.Render(renderer, true));
            }

            // parameters next
            foreach (string key in _availableParams.Keys)
            {
                sb.Replace(@"${" + key + "}", renderer.ResolveParameter(key));
            }
            //TODO make lineending a constant or configurable
            if (!suppressNewLine) {
                sb.Append('\r');
                sb.Append('\n');
            }
            return sb.ToString();
        }
    }
}
