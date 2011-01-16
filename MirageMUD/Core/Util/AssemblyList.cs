using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Mirage.Core.Util
{
    /// <summary>
    /// Contains the searchable assemblies in the Mud
    /// </summary>
    public class AssemblyList : IEnumerable<Assembly>
    {
        private System.Collections.Generic.HashSet<Assembly> _assemblies;

        private static AssemblyList _instance = new AssemblyList();

        private AssemblyList()
        {
            _assemblies = new System.Collections.Generic.HashSet<Assembly>();            
            RegisterAssembly(this.GetType().Assembly);
            RegisterAssembly(Assembly.Load("Mirage.Stock"));
        }

        /// <summary>
        /// Returns an instance of the AssemblyList
        /// </summary>
        public static AssemblyList Instance
        {
            get { return _instance; }
        }

        public void ForEach(Action<Assembly> action)
        {
            foreach (Assembly a in this)
                action(a);
        }
        public void RegisterAssembly(Assembly theAssembly)
        {
            _assemblies.Add(theAssembly);
        }

        public void RegisterAssembly(string theAssembly)
        {
            _assemblies.Add(Assembly.Load(theAssembly));
        }

        public IEnumerator<Assembly> GetEnumerator()
        {
            return _assemblies.GetEnumerator();
        }


        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
