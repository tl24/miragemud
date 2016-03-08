using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mirage.Game.World.Query;

namespace NUnitTests.Game.World.Query
{
    [TestFixture]
    public class ObjectUriResolverTests
    {

        [Test]
        public void TestResolveRelativePathWithNullRoot()
        {
            var resolver = new ObjectUriResolver();
            var result = resolver.Resolve(null, new ObjectUri("Foo"));
            Assert.IsNull(result);
        }

        [Test]
        public void TestResolveRelativeSingleObjectProperty()
        {
            var root = new RootObject()
            {
                SingleChild = new ChildObject()
            };

            var resolver = new ObjectUriResolver();
            var result = resolver.Resolve(root, new ObjectUri("SingleChild"));
            Assert.AreSame(root.SingleChild, result);
        }

        [Test]
        public void TestResolveRelativeNestedProperty()
        {
            var root = new RootObject();
            var child1 = new ChildObject();
            var child2 = new ChildObject();
            root.Items.Add(child1);
            root.Items.Add(child2);

            var resolver = new ObjectUriResolver();
            var result = resolver.Resolve(root, new ObjectUri("Items").Append(child2.Uri));
            Assert.AreSame(child2, result);
        }

        [Test]
        public void TestResolveRelativeDeeplyNestedProperty()
        {
            var root = new RootObject();
            var child1 = new ChildObject();
            var child2 = new RootObject();
            root.Items.Add(child1);
            root.Items.Add(child2);
            var subChild1 = new ChildObject();
            child2.Items.Add(subChild1);
            var resolver = new ObjectUriResolver();
            var result = resolver.Resolve(root, new ObjectUri("Items").Append(child2.Uri).Append("Items").Append(subChild1.Uri));
            Assert.AreSame(subChild1, result);
        }

        [Test]
        public void TestRelativePathUsingUriContainer()
        {
            var container = new Container();
            var child = new ChildObject();
            container.SetChild("MyChild", child);
            var resolver = new ObjectUriResolver();
            var result = resolver.Resolve(container, new ObjectUri("MyChild"));
            Assert.AreSame(child, result);
        }

        [Test]
        public void TestRelativePathUsingHybridUriContainer()
        {
            var container = new Container();
            var child = new ChildObject();
            container.DeclaredChild = child;
            var resolver = new ObjectUriResolver();
            var result = resolver.Resolve(container, new ObjectUri("DeclaredChild"));
            Assert.AreSame(child, result);
        }

        [Test]
        public void TestAbsolutePathUsesBaseObject()
        {
            var baseObject = new RootObject();
            var baseChild = new ChildObject();
            baseObject.SingleChild = baseChild;
            var resolver = new ObjectUriResolver(baseObject);
            var result = resolver.Resolve(new ObjectUri("/SingleChild"));
            Assert.AreSame(baseChild, result);
        }

        [Test]
        public void TestAbsolutePathUsesBaseObjectEvenWhenRelativeRootPassed()
        {
            var baseObject = new RootObject();
            var baseChild = new ChildObject();
            baseObject.SingleChild = baseChild;
            var relativeRoot = new RootObject();
            var resolver = new ObjectUriResolver(baseObject);
            var result = resolver.Resolve(relativeRoot, new ObjectUri("/SingleChild"));
            Assert.AreSame(baseChild, result);
        }

        [Test]
        public void TestAbsolutePathUsesRelativeRootWhenNoBaseObject()
        {
            var baseObject = new RootObject();
            var baseChild = new ChildObject();
            baseObject.SingleChild = baseChild;
            var resolver = new ObjectUriResolver();
            var result = resolver.Resolve(baseObject, new ObjectUri("/SingleChild"));
            Assert.AreSame(baseChild, result);
        }

        [Test]
        public void TestAbsoluteRootUriReturnsBaseObject()
        {
            var baseObject = new RootObject();
            var resolver = new ObjectUriResolver(baseObject);
            var result = resolver.Resolve(new ObjectUri("/"));
            Assert.AreSame(baseObject, result);
        }

        [Test]
        public void TestResolveRelativeStringUri()
        {
            var root = new RootObject()
            {
                SingleChild = new ChildObject()
            };

            var resolver = new ObjectUriResolver();
            var result = resolver.Resolve(root, "SingleChild");
            Assert.AreSame(root.SingleChild, result);
        }

        [Test]
        public void TestResolveAbsoluteStringUri()
        {
            var root = new RootObject()
            {
                SingleChild = new ChildObject()
            };

            var resolver = new ObjectUriResolver(root);
            var result = resolver.Resolve("/SingleChild");
            Assert.AreSame(root.SingleChild, result);
        }

        [Test]
        public void TestDotResolvesToRootObject()
        {
            var root = new RootObject()
            {
                SingleChild = new ChildObject()
            };

            var resolver = new ObjectUriResolver();
            var result = resolver.Resolve(root, ".");
            Assert.AreSame(root, result);
        }

        [Test]
        public void TestResolvePropertyOnCollection()
        {
            var root = new HybridCollection()
            {
                SingleChild = new ChildObject()
            };

            var resolver = new ObjectUriResolver();
            var result = resolver.Resolve(root, "SingleChild");
            Assert.AreSame(root.SingleChild, result);
        }

        [Test]
        public void TestResolveItemNotInCollection()
        {
            var root = new HybridCollection();

            var resolver = new ObjectUriResolver();
            var result = resolver.Resolve(root, "SingleChild");
            Assert.IsNull(result);
        }
    }

    public class BaseObject : ISupportUri
    {
        public BaseObject()
        {
            Uri = Guid.NewGuid().ToString();
        }
        public string Uri { get; set; }
        public string FullUri { get; set; }
    }

    public class RootObject : BaseObject
    {
        public RootObject()
        {
            Items = new List<BaseObject>();
        }

        public List<BaseObject> Items { get; set; }
        public BaseObject SingleChild { get; set; }
    }

    public class ChildObject : BaseObject
    {
        public ChildObject()
        {
        }
    }

    public class Container : BaseObject, IUriContainer
    {
        private Dictionary<string, Tuple<object, QueryHints>> _children = new Dictionary<string, Tuple<object, QueryHints>>(StringComparer.CurrentCultureIgnoreCase);

        public void SetChild(string uri, object child, QueryHints hints = 0)
        {
            _children[uri] = Tuple.Create(child, hints);
        }
        public object GetChild(string uri)
        {
            Tuple<object, QueryHints> child;
            if (_children.TryGetValue(uri, out child))
            {
                return child.Item1;
            }
            return null;
        }

        public QueryHints GetChildHints(string uri)
        {
            Tuple<object, QueryHints> child;
            if (_children.TryGetValue(uri, out child))
            {
                return child.Item2;
            }
            return 0;
        }

        public BaseObject DeclaredChild { get; set; }
    }

    public class HybridCollection : List<BaseObject>
    {
        public HybridCollection()
            : base()
        {
        }

        public BaseObject SingleChild { get; set; }
    }
}
