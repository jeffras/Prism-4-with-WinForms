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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Properties;

namespace Microsoft.Practices.Prism.Regions.Behaviors
{
    /// <summary>
    /// Behavior that generates <see cref="TabItem"/> containers for the added items
    /// and also keeps the <see cref="TabControl.SelectedItem"/> and the <see cref="IRegion.ActiveViews"/> in sync.
    /// </summary>
    public class TabControlRegionSyncBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        ///<summary>
        /// The behavior key for this region sync behavior.
        ///</summary>
        public const string BehaviorKey = "TabControlRegionSyncBehavior";

        private static readonly DependencyProperty IsGeneratedProperty =
            DependencyProperty.RegisterAttached("IsGenerated", typeof(bool), typeof(TabControlRegionSyncBehavior), null);

        private TabControl hostControl;

        /// <summary>
        /// Gets or sets the <see cref="DependencyObject"/> that the <see cref="IRegion"/> is attached to.
        /// </summary>
        /// <value>A <see cref="DependencyObject"/> that the <see cref="IRegion"/> is attached to.
        /// This is usually a <see cref="FrameworkElement"/> that is part of the tree.</value>
        public DependencyObject HostControl
        {
            get
            {
                return this.hostControl;
            }

            set
            {
                TabControl newValue = value as TabControl;
                if (newValue == null)
                {
                    throw new InvalidOperationException(Resources.HostControlMustBeATabControl);
                }

                if (IsAttached)
                {
                    throw new InvalidOperationException(Resources.HostControlCannotBeSetAfterAttach);
                }

                this.hostControl = newValue;
            }
        }

        /// <summary>
        /// Override this method to perform the logic after the behavior has been attached.
        /// </summary>
        protected override void OnAttach()
        {
            if (this.hostControl == null)
            {
                throw new InvalidOperationException(Resources.HostControlCannotBeNull);
            }

            this.SynchronizeItems();

            this.hostControl.SelectionChanged += this.OnSelectionChanged;
            this.Region.ActiveViews.CollectionChanged += this.OnActiveViewsChanged;
            this.Region.Views.CollectionChanged += this.OnViewsChanged;
        }

        /// <summary>
        /// Gets the item contained in the <see cref="TabItem"/>.
        /// </summary>
        /// <param name="tabItem">The container item.</param>
        /// <returns>The item contained in the <paramref name="tabItem"/> if it was generated automatically by the behavior; otherwise <paramref name="tabItem"/>.</returns>
        protected virtual object GetContainedItem(TabItem tabItem)
        {
            if (tabItem == null) throw new ArgumentNullException("tabItem");
            if ((bool)tabItem.GetValue(IsGeneratedProperty))
            {
                return tabItem.Content;
            }

            return tabItem;
        }

        /// <summary>
        /// Override to change how TabItem's are prepared for items.
        /// </summary>
        /// <param name="item">The item to wrap in a TabItem</param>
        /// <param name="parent">The parent <see cref="DependencyObject"/></param>
        /// <returns>A tab item that wraps the supplied <paramref name="item"/></returns>
        protected virtual TabItem PrepareContainerForItem(object item, DependencyObject parent)
        {
            TabItem container = item as TabItem;
            if (container == null)
            {
                object dataContext = GetDataContext(item);
                container = new TabItem();
                container.Content = item;
                container.Style = TabControlRegionAdapter.GetItemContainerStyle(parent);
                container.DataContext = dataContext; // To run with SL 2
                container.Header = dataContext; // To run with SL 3                  
                container.SetValue(IsGeneratedProperty, true);
            }

            return container;
        }

        /// <summary>
        /// Undoes the effects of the <see cref="PrepareContainerForItem"/> method.
        /// </summary>
        /// <param name="tabItem">The container element for the item.</param>
        protected virtual void ClearContainerForItem(TabItem tabItem)
        {
            if (tabItem == null) throw new ArgumentNullException("tabItem");
            if ((bool)tabItem.GetValue(IsGeneratedProperty))
            {
                tabItem.Content = null;
            }
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <param name="item">The item to get the container for.</param>
        /// <param name="itemCollection">The parent's <see cref="ItemCollection"/>.</param>
        /// <returns>The element that is used to display the given item.</returns>
        protected virtual TabItem GetContainerForItem(object item, ItemCollection itemCollection)
        {
            if (itemCollection == null) throw new ArgumentNullException("itemCollection");
            TabItem container = item as TabItem;
            if (container != null && ((bool)container.GetValue(IsGeneratedProperty)) == false)
            {
                return container;
            }

            foreach (TabItem tabItem in itemCollection)
            {
                if ((bool)tabItem.GetValue(IsGeneratedProperty))
                {
                    if (tabItem.Content == item)
                    {
                        return tabItem;
                    }
                }
            }


            return null;
        }

        /// <summary>
        /// Return the appropriate data context.  If the item is a FrameworkElement it cannot be a data context in Silverlight, so we use its data context.
        /// Otherwise, we just us the item as the data context.
        /// </summary>
        private static object GetDataContext(object item)
        {
            FrameworkElement frameworkElement = item as FrameworkElement;
            return frameworkElement == null ? item : frameworkElement.DataContext;
        }

        private void SynchronizeItems()
        {
            List<object> existingItems = new List<object>();
            if (this.hostControl.Items.Count > 0)
            {
                // Control must be empty before "Binding" to a region
                foreach (object childItem in this.hostControl.Items)
                {
                    existingItems.Add(childItem);
                }
            }

            foreach (object view in this.Region.Views)
            {
                TabItem tabItem = this.PrepareContainerForItem(view, this.hostControl);
                this.hostControl.Items.Add(tabItem);
            }

            foreach (object existingItem in existingItems)
            {
                this.Region.Add(existingItem);
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // e.OriginalSource == null, that's why we use sender.
            if (this.hostControl == sender)
            {
                foreach (TabItem tabItem in e.RemovedItems)
                {
                    object item = this.GetContainedItem(tabItem);

                    // check if the view is in both Views and ActiveViews collections (there may be out of sync)
                    if (this.Region.Views.Contains(item) && this.Region.ActiveViews.Contains(item))
                    {
                        this.Region.Deactivate(item);
                    }
                }

                foreach (TabItem tabItem in e.AddedItems)
                {
                    object item = this.GetContainedItem(tabItem);
                    if (!this.Region.ActiveViews.Contains(item))
                    {
                        this.Region.Activate(item);
                    }
                }
            }
        }

        private void OnActiveViewsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                this.hostControl.SelectedItem = this.GetContainerForItem(e.NewItems[0], this.hostControl.Items);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove
                && this.hostControl.SelectedItem != null
                && e.OldItems.Contains(this.GetContainedItem((TabItem)this.hostControl.SelectedItem)))
            {
                this.hostControl.SelectedItem = null;
            }
        }

        private void OnViewsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                int startingIndex = e.NewStartingIndex;
                foreach (object newItem in e.NewItems)
                {
                    TabItem tabItem = this.PrepareContainerForItem(newItem, this.hostControl);
                    this.hostControl.Items.Insert(startingIndex, tabItem);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object oldItem in e.OldItems)
                {
                    TabItem tabItem = this.GetContainerForItem(oldItem, this.hostControl.Items);
                    this.hostControl.Items.Remove(tabItem);
                    this.ClearContainerForItem(tabItem);
                }
            }
        }
    }
}
