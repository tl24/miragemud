using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Mirage.Core.Data.Items;

namespace NUnitTests
{
    [TestFixture]
    public class WornItemTests
    {
        [Test]
        public void WhenWearingItem_GetItemAtShouldReturnItem()
        {
            WornItems items = new WornItems();
            Armor a = new Armor();
            a.WearFlags = WearLocations.Arms;
            items.Add(a);
            Assert.AreEqual(1, items.Count, "Item not worn properly");
            Armor result = items.GetItemAt(WearLocations.Arms);
            Assert.AreSame(a, result, "Item not placed in correct slot");
        }

        [Test]
        public void WhenWearingItem_CantWearItemInSameSlot()
        {
            WornItems items = new WornItems();
            Armor arm1 = new Armor();
            arm1.WearFlags = WearLocations.Arms;
            Armor arm2 = new Armor();
            arm2.WearFlags = WearLocations.Arms;

            items.Add(arm1);

            bool excThrown = false;
            try
            {
                items.Add(arm2);
            }
            catch
            {
                excThrown = true;
            }
            Assert.IsTrue(excThrown, "Item can't be added in the same slot");
        }

        [Test]
        public void WhenRemovingItem_ItemIsRemovedCompletely()
        {
            WornItems items = new WornItems();
            Armor arm1 = new Armor();
            arm1.WearFlags = WearLocations.Arms;
            Armor arm2 = new Armor();
            arm2.WearFlags = WearLocations.Arms;

            items.Add(arm1);

            items.Remove(arm1);

            // add should succeed
            items.Add(arm2);

            Assert.AreEqual(1, items.Count, "Item not removed correctly");
        }

        [Test]
        public void WhenTestingOpenSlots_PartialFilledSlotsShouldNotBeOpen()
        {
            WornItems items = new WornItems();
            Armor arm1 = new Armor();
            arm1.WearFlags = WearLocations.Arms;
            items.Add(arm1);

            Assert.IsFalse(items.IsOpen(WearLocations.Arms | WearLocations.Feet));
        }

        [Test]
        public void WhenCallingGetItemsAtWithMultipleFlags_MultipleResultsShouldBeReturned()
        {
            WornItems items = new WornItems();
            Armor arm1 = new Armor();
            arm1.WearFlags = WearLocations.Arms;
            Armor head = new Armor();
            head.WearFlags = WearLocations.Head;
            Armor feet = new Armor();
            feet.WearFlags = WearLocations.Feet;

            items.Add(arm1);
            items.Add(head);
            items.Add(feet);

            List<Armor> expected = new List<Armor>();
            expected.Add(arm1);
            expected.Add(head);

            foreach (Armor item in items.GetItemsAt(WearLocations.Arms | WearLocations.Head))
            {
                // remove any expected items, list should be empty at the end
                if (expected.Contains(item))
                    expected.Remove(item);
                else
                    expected.Add(item);
            }

            Assert.AreEqual(0, expected.Count, "GetItemsAt returned the wrong items"); 
        }

        [Test]
        public void WhenRemove_WearLocation_IsCalled_TheItemIsRemovedAndReturned()
        {
            WornItems items = new WornItems();
            Armor arm1 = new Armor();
            arm1.WearFlags = WearLocations.Arms;
            items.Add(arm1);

            Armor removed = items.Remove(WearLocations.Arms);
            Assert.AreSame(arm1, removed, "Wrong item removed");
            Assert.IsFalse(items.Contains(arm1), "Item not removed");
        }
    }
}
