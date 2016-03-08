using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Reflection;
using System.Linq.Expressions;

namespace Mirage.Game.World.Query
{
    public class UriContainerProvider
    {
        Type _objectType;
        ConcurrentDictionary<string, UriMetaData> _metaData = new ConcurrentDictionary<string, UriMetaData>(StringComparer.CurrentCultureIgnoreCase);
        public UriContainerProvider(Type objectType)
        {
            _objectType = objectType;
        }

        public object GetChild(object target, string uri)
        {
            object child = null;
            IUriContainer container = target as IUriContainer;
            if (container != null)
            {
                child = container.GetChild(uri);
            }
            if (child != null)
                return child;
            UriMetaData propMetaData = _metaData.GetOrAdd(uri, CreateMetaData);
            if (propMetaData == null)
                return null;
            else
                return propMetaData.Getter(target);
        }

        public QueryHints GetChildHints(object target, string uri)
        {
            QueryHints hints = 0;
            IUriContainer container = target as IUriContainer;
            if (container != null)
            {
                hints = container.GetChildHints(uri);
            }
            if (hints != 0)
                return hints;
            UriMetaData propMetaData = _metaData.GetOrAdd(uri, CreateMetaData);
            if (propMetaData == null)
                return 0;
            else
                return propMetaData.Hints;
        }

        private UriMetaData CreateMetaData(string uri)
        {
            var prop = _objectType.GetProperty(uri, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop != null)
            {
                UriMetaData metaData = new UriMetaData();
                var hintAttr = (UriChildAttribute)System.Attribute.GetCustomAttribute(prop, typeof(UriChildAttribute));
                if (hintAttr != null)
                    metaData.Hints = hintAttr.Hints;
                var parmExpr = Expression.Parameter(typeof(object));
                var body = Expression.Property(Expression.Convert(parmExpr, _objectType), prop);
                metaData.Getter = Expression.Lambda<Func<object, object>>(body, parmExpr).Compile();
                return metaData;
            }
            return null;
        }


        class UriMetaData {
            public Func<object, object> Getter;
            public QueryHints Hints;
        }


    }
}
