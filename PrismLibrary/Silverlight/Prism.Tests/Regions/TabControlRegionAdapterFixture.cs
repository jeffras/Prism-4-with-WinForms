//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Regions.Behaviors;
using Microsoft.Practices.Prism.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Prism.Tests.Regions
{
    [TestClass]
    public class TabControlRegionAdapterFixture
    {
        [TestMethod]
        public void AdapterAssociatesSelectorWithRegion()
        {
            var control = new TabControl();
            IRegionAdapter adapter = new TestableTabControlRegionAdapter();

            IRegion region = adapter.Initialize(control, "region");
            Assert.IsNotNull(region);
        }

        [TestMethod]
        public void ShouldMoveAlreadyExistingContentInControlToRegion()
        {
            var control = new TabControl();
            var view = new TabItem();
            control.Items.Add(view);
            IRegionAdapter adapter = new TabControlRegionAdapter(null);

            var region = adapter.Initialize(control, "region");

            Assert.AreEqual(1, region.Views.Count());
            Assert.AreSame(view, region.Views.ElementAt(0));
            Assert.AreSame(view, control.Items[0]);
        }

        [TestMethod]
        public void ControlWithExistingItemSourceThrows()
        {
            var tabControl = new TabControl() { ItemsSource = new List<string>() };

            IRegionAdapter adapter = new TestableTabControlRegionAdapter();

            try
            {
                var region = (MockRegion)adapter.Initialize(tabControl, "region");
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(InvalidOperationException));
                StringAssert.Contains(ex.Message, "ItemsControl's ItemsSource property is not empty.");
            }
        }

        [TestMethod]
        public void AdapterSynchronizesViewsWithItemCollection()
        {
            var tabControl = new TabControl();
            object model1 = new { Id = "Model 1" };
            object model2 = new { Id = "Model 2" };
            IRegionAdapter adapter = new TabControlRegionAdapter(null);

            var region = adapter.Initialize(tabControl, "region");
            Assert.AreEqual(0, region.Views.Count());

            region.Add(model1);
            Assert.AreEqual(1, tabControl.Items.Count);
            Assert.AreSame(model1, ((TabItem)tabControl.Items[0]).Content);

            region.Add(model2);
            Assert.AreEqual(2, tabControl.Items.Count);
            Assert.AreSame(model2, ((TabItem)tabControl.Items[1]).Content);

            region.Remove(model1);
            Assert.AreEqual(1, tabControl.Items.Count);
            Assert.AreSame(model2, ((TabItem)tabControl.Items[0]).Content);
        }

        [TestMethod]
        public void AdapterSynchronizesSelectorSelectionWithRegion()
        {
            var tabControl = new TabControl();
            object model1 = new object();
            object model2 = new object();
            IRegionAdapter adapter = new TabControlRegionAdapter(null);

            var region = adapter.Initialize(tabControl, "region");
            region.Add(model1);
            region.Add(model2);

            Assert.IsFalse(region.ActiveViews.Contains(model2));

            tabControl.SelectedItem = tabControl.Items.ElementAt(1);

            Assert.IsTrue(region.ActiveViews.Contains(model2));
            Assert.IsFalse(region.ActiveViews.Contains(model1));

            tabControl.SelectedItem = tabControl.Items.ElementAt(0);

            Assert.IsTrue(region.ActiveViews.Contains(model1));
            Assert.IsFalse(region.ActiveViews.Contains(model2));
        }

        [TestMethod]
        public void AdapterDoesNotPreventRegionFromBeingGarbageCollected()
        {
            var tabControl = new TabControl();
            object model = new object();
            IRegionAdapter adapter = new TabControlRegionAdapter(null);

            var region = adapter.Initialize(tabControl, "region");
            region.Add(model);

            WeakReference regionWeakReference = new WeakReference(region);
            WeakReference controlWeakReference = new WeakReference(tabControl);
            Assert.IsTrue(regionWeakReference.IsAlive);
            Assert.IsTrue(controlWeakReference.IsAlive);

            region = null;
            tabControl = null;
            GC.Collect();
            GC.Collect();

            Assert.IsFalse(regionWeakReference.IsAlive);
            Assert.IsFalse(controlWeakReference.IsAlive);
        }

        [TestMethod]
        public void ActivatingTheViewShouldUpdateTheSelectedItem()
        {
            var tabControl = new TabControl();
            var view1 = new object();
            var view2 = new object();

            IRegionAdapter adapter = new TabControlRegionAdapter(null);

            var region = adapter.Initialize(tabControl, "region");
            region.Add(view1);
            region.Add(view2);

            region.Activate(view2);

            Assert.IsNotNull(tabControl.SelectedItem);
            Assert.AreEqual(view2, ((ContentControl)tabControl.SelectedItem).Content);

            region.Activate(view1);

            Assert.IsNotNull(tabControl.SelectedItem);
            Assert.AreEqual(view1, ((ContentControl)tabControl.SelectedItem).Content);
        }

        [TestMethod]
        public void DeactivatingTheSelectedViewShouldUpdateTheSelectedItem()
        {
            var tabControl = new TabControl();
            var view1 = new object();
            IRegionAdapter adapter = new TabControlRegionAdapter(null);
            var region = adapter.Initialize(tabControl, "region");
            region.Add(view1);

            region.Activate(view1);

            Assert.AreEqual(view1, ((ContentControl)tabControl.SelectedItem).Content);

            region.Deactivate(view1);

            Assert.IsNull(tabControl.SelectedItem);
        }

        [TestMethod]
        public void ShouldSetStyleInContainerTabItem()
        {
            var tabControl = new TabControl();
            Style style = new Style(typeof(TabItem));
            TabControlRegionAdapter.SetItemContainerStyle(tabControl, style);
            var adapter = new TabControlRegionAdapter(null);
            var region = adapter.Initialize(tabControl, "region");

            region.Add(new object());

            TabItem container = (TabItem)tabControl.Items[0];
            Assert.AreSame(style, container.Style);
        }

        [TestMethod]
        public void ShouldSetDataContextInContainerTabItemToContainedFrameworkElementsDataContext()
        {
            var tabControl = new TabControl();
            var adapter = new TabControlRegionAdapter(null);
            var region = adapter.Initialize(tabControl, "region");

            var dataContext = new object();
            var view = new MockFrameworkElement() { DataContext = dataContext };
            region.Add(view);

            TabItem container = (TabItem)tabControl.Items[0];
            Assert.AreSame(dataContext, container.DataContext);
        }

        [TestMethod]
        public void ShouldSetDataContextInContainerTabItemToContainedObject()
        {
            var tabControl = new TabControl();
            var adapter = new TabControlRegionAdapter(null);
            var region = adapter.Initialize(tabControl, "region");

            var view = new object();
            region.Add(view);

            TabItem container = (TabItem)tabControl.Items[0];
            Assert.AreSame(view, container.DataContext);
        }

        [TestMethod]
        public void ShouldRemoveViewFromTabItemOnViewRemovalFromRegion()
        {
            var tabControl = new TabControl();
            var view = new UserControl();
            IRegionAdapter adapter = new TabControlRegionAdapter(null);
            var region = adapter.Initialize(tabControl, "region");

            region.Add(view);
            Assert.IsNotNull(view.Parent);
            var tabItem = (TabItem)view.Parent;

            region.Remove(view);
            Assert.IsNull(tabItem.Content);
            Assert.IsNull(view.Parent);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void HostControlThatsNotTabControlThrows()
        {
            var control = new MockDependencyObject();

            IRegionAdapter adapter = new TabControlRegionAdapter(null);
            var region = adapter.Initialize(control, "region");

        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void HostControlCannotBeSetAfterAttach()
        {
            var tabControl2 = new TabControl();
            var tabControl1 = new TabControl();

            IRegionAdapter adapter = new TabControlRegionAdapter(null);
            var region = adapter.Initialize(tabControl1, "region");

            var behavior = region.Behaviors["TabControlRegionSyncBehavior"] as IHostAwareRegionBehavior;
            behavior.HostControl = tabControl2;

        }

        [TestMethod]
        public void AddedTabItemsAppearInTabControl()
        {
            var tabControl = new TabControl();
            var view = new TabItem();
            IRegionAdapter adapter = new TabControlRegionAdapter(null);
            var region = adapter.Initialize(tabControl, "region");

            region.Add(view);
            Assert.AreEqual(1, tabControl.Items.Count());
            Assert.IsNotNull(view.Parent);
            Assert.AreSame(tabControl, view.Parent);
            Assert.AreSame(view, region.Views.ElementAt(0));
        }

        [TestMethod]
        public void TabsWithViewSortHintAreSortedProperly()
        {
            var tabControl = new TabControl();
            var view1 = new MockSortableView1();
            var view2 = new MockSortableView2();
            var view3 = new MockSortableView3();

            IRegionAdapter adapter = new TabControlRegionAdapter(null);

            var region = adapter.Initialize(tabControl, "region");
            Assert.AreEqual(0, region.Views.Count());

            region.Add(view2);
            Assert.AreEqual(1, tabControl.Items.Count);
            Assert.AreSame(view2, ((TabItem)tabControl.Items[0]).Content);

            region.Add(view1);
            Assert.AreEqual(2, tabControl.Items.Count);
            Assert.AreSame(view1, ((TabItem)tabControl.Items[0]).Content);
            Assert.AreSame(view2, ((TabItem)tabControl.Items[1]).Content);
            
            region.Add(view3);
            Assert.AreEqual(3, tabControl.Items.Count);
            Assert.AreSame(view1, ((TabItem)tabControl.Items[0]).Content);
            Assert.AreSame(view2, ((TabItem)tabControl.Items[1]).Content);
            Assert.AreSame(view3, ((TabItem)tabControl.Items[2]).Content);
            
        }

        internal class SimpleModel
        {
            public IEnumerable Enumerable { get; set; }
        }

        internal class TestableTabControlRegionAdapter : TabControlRegionAdapter
        {
            private readonly MockPresentationRegion region = new MockPresentationRegion();

            public TestableTabControlRegionAdapter()
                : base(null)
            {

            }

            protected override IRegion CreateRegion()
            {
                return this.region;
            }
        }
    }
}