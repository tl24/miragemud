using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Collections;

namespace Mirage.Game.World.Query
{
    /// <summary>
    /// Resolves and loads objects identified by ObjectUris.  Objects are identified by their Uri.
    /// </summary>
    /// <example>
    /// Example Uris:
    /// /Players/Bob
    /// /Areas/NewbieLand/Rooms/StartRoom
    /// Items/Sword
    /// </example>
    public class ObjectUriResolver
    {
        ConcurrentDictionary<Type, UriContainerProvider> _providers = new ConcurrentDictionary<Type, UriContainerProvider>();

        /// <summary>
        /// Constructs an ObjectUriResolver with no baseObject.  Absolute queries
        /// will not be able to be resolved with this resolver.
        /// </summary>
        public ObjectUriResolver()
        {
        }

        /// <summary>
        /// Constructs an ObjectUriResolver with the <paramref name="baseObject"/>
        /// used for absolute queries
        /// </summary>
        /// <param name="baseObject">the base object to use for resolving absolute queries</param>
        public ObjectUriResolver(object baseObject)
        {
            BaseObject = baseObject;
        }

        /// <summary>
        /// The base object that is used when resolving absolute uris
        /// </summary>
        public object BaseObject { get; private set; }

        /// <summary>
        /// Resolves the object for <paramref name="uri"/>
        /// </summary>
        /// <param name="uri">object uri</param>
        /// <returns>the object identified by the uri</returns>
        public object Resolve(string uri)
        {
            return Resolve(new ObjectUri(uri));
        }

        /// <summary>
        /// Resolves the object for <paramref name="uri"/>
        /// </summary>
        /// <param name="uri">object uri</param>
        /// <returns>the object identified by the uri</returns>
        public object Resolve(ObjectUri uri)
        {
            return Resolve(BaseObject, uri);
        }

        /// <summary>
        /// Resolves the object for <paramref name="uri"/>, using
        /// <paramref name="root"/> as the starting point for relative uris.
        /// </summary>
        /// <param name="root">the root object for relative uris</param>
        /// <param name="uri">the uri to resolve</param>
        /// <returns>the object identified by the uri</returns>
        public object Resolve(object root, string uri)
        {
            return Resolve(root, new ObjectUri(uri));
        }

        /// <summary>
        /// Resolves the object for <paramref name="uri"/>, using
        /// <paramref name="root"/> as the starting point for relative uris.
        /// </summary>
        /// <param name="root">the root object for relative uris</param>
        /// <param name="uri">the uri to resolve</param>
        /// <returns>the object identified by the uri</returns>
        public object Resolve(object root, ObjectUri uri)
        {
            var currentRoot = root;
            foreach (var part in uri)
            {
                if (part == "/")
                {
                    currentRoot = BaseObject ?? root;
                    continue;
                }
                else if (part == ".")
                {
                    // refers to the current object
                    continue;
                }

                if (currentRoot == null)
                    break;

                var provider = _providers.GetOrAdd(currentRoot.GetType(), t => new UriContainerProvider(t));
                var newRoot = provider.GetChild(currentRoot, part);

                if (newRoot == null && IsCollection(currentRoot))
                {
                    foreach (var item in GetCollectionEnumerable(currentRoot))
                    {
                        ISupportUri uriItem = item as ISupportUri;
                        if (uriItem != null && string.Compare(part, uriItem.Uri, StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            newRoot = item;
                            break;
                        }
                    }
                }
                currentRoot = newRoot;
            }
            return currentRoot;
        }

        /// <summary>
        /// Checks to see if the object is a collection.
        /// </summary>
        /// <param name="coll">object to check</param>
        /// <returns></returns>
        private bool IsCollection(object coll)
        {
            return (coll is IEnumerable);
        }

        /// <summary>
        /// Gets an enumerable object for a collection.  The enumerable is
        /// then used in a foreach statement to search the collection
        /// </summary>
        /// <param name="coll"></param>
        /// <returns></returns>
        private IEnumerable GetCollectionEnumerable(object coll)
        {
            IEnumerable e = null;
            if (coll is IDictionary)
            {
                e = ((IDictionary)coll).Values;
            }
            // if not dictionary just try regular enumerator
            if (e == null)
            {
                e = coll as IEnumerable;
            }
            return e;
        }
    }
}
