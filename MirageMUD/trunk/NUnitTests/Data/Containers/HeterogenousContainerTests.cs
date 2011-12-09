using System.Collections.Generic;
using Mirage.Game.World.Containers;
using NUnit.Framework;
using NUnitTests.Mock;

namespace NUnitTests.Data.Containers
{
    [TestFixture]
    public class HeterogenousContainerTests
    {
        public HeterogenousContainer container;
        public LinkedList<MockContainableA> AList;
        public LinkedList<MockContainableB> BList;

        [SetUp]
        public void Setup()
        {
            container = new HeterogenousContainer();
            AList = new LinkedList<MockContainableA>();
            container.AddContainer(typeof(MockContainableA), 
                new GenericCollectionContainer<MockContainableA>(AList));

            BList = new LinkedList<MockContainableB>();
            container.AddContainer(typeof(MockContainableB),
                new GenericCollectionContainer<MockContainableB>(BList));
        }


        [Test]
        public void TestCanAddB()
        {
            MockContainableB item = new MockContainableB();
            Assert.IsTrue(container.CanAdd(item));
        }

        [Test]
        public void TestCantAddD()
        {
            MockContainableD item = new MockContainableD();
            Assert.IsFalse(container.CanAdd(item));
        }

        [Test]
        public void TestCanAddSubA()
        {
            MockContainableSubA item = new MockContainableSubA();
            Assert.IsTrue(container.CanAdd(item));
        }

        [Test]
        public void TestAddRemoveA()
        {
            MockContainableA item = new MockContainableA();
            container.Add(item);
            Assert.IsTrue(AList.Contains(item));
            Assert.AreSame(container, item.Container);

            container.Remove(item);
            Assert.IsFalse(AList.Contains(item));
            Assert.IsNull(item.Container);
        }

        [Test]
        public void TestRemoveItemNotInCollection()
        {
            MockContainableD item = new MockContainableD();
            container.Remove(item);
        }

        [Test]
        [ExpectedException(typeof(ContainerAddException))]
        public void TestAddD()
        {
            MockContainableD item = new MockContainableD();
            container.Add(item);
        }

        [Test]
        public void TestEnumerateFilter()
        {
            int itemACount = 0,  itemSubACount = 0;

            MockContainableA itemA = new MockContainableA();
            MockContainableB itemB = new MockContainableB();
            MockContainableSubA itemSubA = new MockContainableSubA();

            container.Add(itemA);
            container.Add(itemB);
            container.Add(itemSubA);

            foreach (object item in container.Contents(typeof(MockContainableA)))
            {
                // should get itemA, anditemSubA
                if (item == itemA)
                    itemACount++;
                else if (item == itemSubA)
                    itemSubACount++;
                else
                    Assert.Fail("Type filter failed");
            }

            Assert.AreEqual(1, itemACount, "Invalid Count for ItemA");
            Assert.AreEqual(1, itemSubACount, "Invalid Count for ItemSubA");
        }

        [Test]
        public void TestEnumerateNoFilter()
        {
            int itemACount = 0, itemSubACount = 0, itemBCount = 0;

            MockContainableA itemA = new MockContainableA();
            MockContainableB itemB = new MockContainableB();
            MockContainableSubA itemSubA = new MockContainableSubA();

            container.Add(itemA);
            container.Add(itemB);
            container.Add(itemSubA);

            foreach (object item in container.Contents())
            {
                // should get itemA, anditemSubA
                if (item == itemA)
                    itemACount++;
                else if (item == itemSubA)
                    itemSubACount++;
                else if (item == itemB)
                    itemBCount++;
                else
                    Assert.Fail("Type filter failed: " + item.GetType().Name);
            }

            Assert.AreEqual(1, itemACount, "Invalid Count for ItemA");
            Assert.AreEqual(1, itemSubACount, "Invalid Count for ItemSubA");
            Assert.AreEqual(1, itemBCount, "Invalid Count for ItemB");
        }

    }
}
