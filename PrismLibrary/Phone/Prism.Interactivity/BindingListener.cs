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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Practices.Prism.Interactivity
{
    /// <summary>
    /// The delegate to invoke when the value monitored by the <see cref="BindingListener"/> changes.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The binding changed information.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage ("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void ChangedHandler(object sender, BindingChangedEventArgs e);

    /// <summary>
    /// Attaches a binding lister to a <see cref="FrameworkElement"/> to enable the use
    /// of binding expressions from items that don't support <see cref="Binding"/> expressions (e.g. Expression Blend
    /// Silverlight 3 Behaviors).
    /// </summary>
    /// <remarks>
    /// This was taken, with permission, from Peter Blois (http://blois.us/blog/2009/04/datatrigger-bindings-on-non.html)
    /// </remarks>
    public class BindingListener
    {
        private static readonly List<DependencyPropertyListener> freeListeners = new List<DependencyPropertyListener>();

        private readonly ChangedHandler changedHandler;
        private Binding binding;
        private DependencyPropertyListener listener;
        private FrameworkElement target;
        private object value;

        ///<summary>
        /// Intantiates a new instance of the BindingListener.
        ///</summary>
        ///<param name="changedHandler"></param>
        public BindingListener(ChangedHandler changedHandler)
        {
            this.changedHandler = changedHandler;
        }

        /// <summary>
        /// Gets or sets the <see cref="Binding"/> to when attaching the dependency property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value")]
        public Binding Binding
        {
            get { return this.binding; }
            set
            {
                this.binding = value;
                this.Attach();
            }
        }

        /// <summary>
        /// Gets or sets the framework element to attach the listener to.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value")]
        public FrameworkElement Element
        {
            get { return this.target; }
            set
            {
                this.target = value;
                this.Attach();
            }
        }

        /// <summary>
        /// The value of the underlying monitored object.
        /// </summary>
        public object Value
        {
            get { return this.value; }
        }

        private void Attach()
        {
            this.Detach();

            if (this.target != null && this.binding != null)
            {
                this.listener = this.GetListener();
                this.listener.Attach(this.target, this.binding);
            }
        }

        private void Detach()
        {
            if (this.listener != null)
            {
                this.ReturnListener();
            }
        }

        private DependencyPropertyListener GetListener()
        {
            DependencyPropertyListener dependencyPropertyListener;

            if (freeListeners.Count != 0)
            {
                dependencyPropertyListener = freeListeners[freeListeners.Count - 1];
                freeListeners.RemoveAt(freeListeners.Count - 1);

                return dependencyPropertyListener;
            }
            else
            {
                dependencyPropertyListener = new DependencyPropertyListener();
            }

            dependencyPropertyListener.Changed += this.HandleValueChanged;

            return dependencyPropertyListener;
        }

        private void HandleValueChanged(object sender, BindingChangedEventArgs e)
        {
            this.value = e.EventArgs.NewValue;

            this.changedHandler(this, e);
        }

        private void ReturnListener()
        {
            this.listener.Changed -= this.HandleValueChanged;

            freeListeners.Add(this.listener);

            this.listener = null;
        }
    }
}