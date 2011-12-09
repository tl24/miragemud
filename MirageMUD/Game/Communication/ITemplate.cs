using System.Collections.Generic;

namespace Mirage.Game.Communication
{
    public interface ITemplate
    {
        /// <summary>
        /// The raw text of the template
        /// </summary>
        string RawText { get; }

        /// <summary>
        /// The parent template if any
        /// </summary>
        ITemplate ParentTemplate { get; set;}

        /// <summary>
        /// The available parameters to be replaced
        /// </summary>
        ICollection<string> AvailableParameters { get; }

        /// <summary>
        /// Parameter values for this template
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object this[string key] { get; set; }

        /// <summary>
        /// A context object to derive parameters from, such as a player reference
        /// </summary>
        object Context { get; set; }

        /// <summary>
        /// Render the template into final form, replacing any substitutions and adding
        /// an automatic newline at the end
        /// </summary>
        /// <returns>rendered template</returns>
        string Render();

        /// <summary>
        /// Render the template into final form, replacing any substitutions.  A newline
        /// will be added if suppressNewline is false.
        /// </summary>
        /// <returns>rendered template</returns>
        string Render(bool suppressNewline);

        /// <summary>
        /// Resolves the parameter value in either this or the parent template's parameters
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string ResolveParameter(string key);

    }
}
