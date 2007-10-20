using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Mirage.Core.Util;

namespace NUnitTests.Util
{
    [TestFixture]
    public class HashSetTests
    {
        [Test]
        public void TestAddDuplicates()
        {
            HashSet<int> set = new HashSet<int>();
            set.Add(1);
            set.Add(2);
            set.Add(2);
            

            CompareTo<int>(set, new int[] { 1, 2 });
        }

        [Test]
        public void TestCaseInsenstiveStrings()
        {
            HashSet<string> set = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
            set.Add("aaa");
            set.Add("AAA");
            set.Add("BBB");
            set.Remove("bbb");
            CompareTo<string>(set, new string[] { "aaa" }, StringComparer.CurrentCultureIgnoreCase);
        }

        [Test]
        public void TestAllInSet()
        {
            HashSet<int> set = new HashSet<int>();
            set.Add(8);
            set.Add(6);
            set.Add(4);
            set.Add(2);
            int[] check = { 4, 6, 8 };
            Assert.IsTrue(set.ContainsAll(check));
            set.Remove(6);
            Assert.IsFalse(set.ContainsAll(check));
            Assert.IsTrue(set.ContainsAll(new int[0]));
        }

        [Test]
        public void TestAnyInSet()
        {
            HashSet<int> set = new HashSet<int>();
            set.Add(8);
            set.Add(6);
            set.Add(4);
            set.Add(2);
            int[] check = { 3, 6, 9 };
            Assert.IsTrue(set.ContainsAny(check));
            check = new int[] { 1, 3 };
            Assert.IsFalse(set.ContainsAll(check));
            Assert.IsFalse(set.ContainsAny(new int[0]));
        }

        /// <summary>
        /// Compares the set to the array with the default equality comparer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set"></param>
        /// <param name="expectedItems"></param>
        public void CompareTo<T>(ISet<T> set, T[] expectedItems)
        {
            CompareTo<T>(set, expectedItems, EqualityComparer<T>.Default);
        }
        /// <summary>
        /// Compares a set to an array of items.  All items in the array must appear in the
        /// set exactly once and must not contain any other elements besides those in the array.
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="set">the set to test</param>
        /// <param name="expectedItems">the expected items in the set</param>
        /// <param name="equalityComparer">equality comparer to compare with</param>
        public void CompareTo<T>(ISet<T> set, T[] expectedItems, IEqualityComparer<T> equalityComparer)
        {
            Assert.AreEqual(expectedItems.Length, set.Count);
            int[] counts = new int[expectedItems.Length];
            foreach (T item in set)
            {
                bool found = false;
                for (int i = 0; i < expectedItems.Length; i++)
                {
                    if (equalityComparer.Equals(expectedItems[i], item))
                    {
                        counts[i]++;
                        if (counts[i] > 1)
                            Assert.Fail("Set contains duplicate items of " + item);
                        found = true;
                        break;
                    }
                }
                if (!found)
                    Assert.Fail("Set contains an extra item " + item);
            }
            // make sure all items were in the set
            for (int i = 0; i < counts.Length; i++)
            {
                Assert.AreEqual(1, counts[i], "Invalid set item count for item " + expectedItems[i]);
            }
        }
    }
}
