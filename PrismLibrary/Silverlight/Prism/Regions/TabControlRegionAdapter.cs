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
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Properties;
using Microsoft.Practices.Prism.Regions.Behaviors;

namespace Microsoft.Practices.Prism.Regions
{
    /// <summary>
    /// Adapter that creates a new <see cref="Region"/> and binds all
    /// the views to the adapted <see cref="TabControl"/>.
    /// </summary>
    /// <remarks>
    /// This adapter is needed on Silverlight because the <see cref="TabControl"/> doesn't 
    /// automatically create <see cref="TabItem"/>s when new items are added to 
    /// the <see cref="ItemsControl.Items"/> collection.
    /// </remarks>
    public class TabControlRegionAdapter : RegionAdapterBase<TabControl>
    {
        /// <summary>
        /// <see cref="Style"/> to set to the created <see cref="TabItem"/>.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.RegisterAttached("ItemContainerStyle", typeof(Style), typeof(TabControlRegionAdapter), null);

        /// <summary>
        /// Initializes a new instance of the <see cref="TabControlRegionAdapter"/> class.
        /// </summary>
        /// <param name="regionBehaviorFactory">The factory used to create the region behaviors to attach to the created regions.</param>
        public TabControlRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        /// <summary>
        /// Gets the <see cref="ItemContainerStyleProperty"/> property value.
        /// </summary>
        /// <param name="target">Target object of the attached property.</param>
        /// <returns>Value of the <see cref="ItemContainerStyleProperty"/> property.</returns>
        public static Style GetItemContainerStyle(DependencyObject target)
        {
            if (target == null) throw new ArgumentNullException("target");
            return (Style)target.GetValue(ItemContainerStyleProperty);
        }

        /// <summary>
        /// Sets the <see cref="ItemContainerStyleProperty"/> property value.
        /// </summary>
        /// <param name="target">Target object of the attached property.</param>
        /// <param name="value">Value to be set on the <see cref="ItemContainerStyleProperty"/> property.</param>
        public static void SetItemContainerStyle(DependencyObject target, Style value)
        {
            if (target == null) throw new ArgumentNullException("target");
            target.SetValue(ItemContainerStyleProperty, value);
        }

        /// <summary>
        /// Adapts a <see cref="TabControl"/> to an <see cref="IRegion"/>.
        /// </summary>
        /// <param name="region">The new region being used.</param>
        /// <param name="regionTarget">The object to adapt.</param>
        protected override void Adapt(IRegion region, TabControl regionTarget)
        {
            if (regionTarget == null) throw new ArgumentNullException("regionTarget");
            bool itemsSourceIsSet = regionTarget.ItemsSource != null;

            if (itemsSourceIsSet)
            {
                throw new InvalidOperationException(Resources.ItemsControlHasItemsSourceException);
            }
        }

        /// <summary>
        /// Attach new behaviors.
        /// </summary>
        /// <param name="region">The region being used.</param>
        /// <param name="regionTarget">The object to adapt.</param>
        /// <remarks>
        /// This class attaches the base behaviors and also keeps the <see cref="TabControl.SelectedItem"/> 
        /// and the <see cref="IRegion.ActiveViews"/> in sync.
        /// </remarks>
        protected override void AttachBehaviors(IRegion region, TabControl regionTarget)
        {
            if (region == null) throw new ArgumentNullException("region");
            base.AttachBehaviors(region, regionTarget);
            if (!region.Behaviors.ContainsKey(TabControlRegionSyncBehavior.BehaviorKey))
            {
                region.Behaviors.Add(TabControlRegionSyncBehavior.BehaviorKey, new TabControlRegionSyncBehavior { HostControl = regionTarget });
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="Region"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="Region"/>.</returns>
        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }
    }
}
