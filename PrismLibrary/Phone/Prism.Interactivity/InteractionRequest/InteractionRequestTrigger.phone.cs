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
using System.Windows;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace Microsoft.Practices.Prism.Interactivity.InteractionRequest
{
    /// <summary>
    /// Custom event trigger for using with <see cref="IInteractionRequest"/> objects.
    /// </summary>
    public class InteractionRequestTrigger : TriggerBase<FrameworkElement>
    {
        /// <summary>
        /// The binding to the <see cref="IInteractionRequest"/>.
        /// </summary>
        public static readonly DependencyProperty SourceObjectProperty =
            DependencyProperty.Register("SourceObject", typeof(Binding), typeof(InteractionRequestTrigger), new PropertyMetadata(HandleBindingChanged));

        private readonly BindingListener requestBindinglistener;

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionRequestTrigger"/> class. 
        /// </summary>
        public InteractionRequestTrigger()
        {
            this.requestBindinglistener = new BindingListener(this.ChangeHandler);
        }

        /// <summary>
        /// Gets or sets the <see cref="Binding"/> that should resolve to a <see cref="IInteractionRequest"/>
        /// </summary>
        public Binding SourceObject
        {
            get { return (Binding)GetValue(SourceObjectProperty); }
            set { SetValue(SourceObjectProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="IInteractionRequest"/> that is the result of the <see cref="SourceObject"/>.
        /// </summary>
        protected IInteractionRequest Request { get; set; }

        /// <summary>
        /// Invoked when the binding value changes.
        /// </summary>
        /// <param name="e"></param>
        protected void OnBindingChanged(DependencyPropertyChangedEventArgs e)
        {
            this.requestBindinglistener.Binding = (Binding)e.NewValue;
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>
        /// Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected override void OnDetaching()
        {
            this.HandleBindingChange(null);
            base.OnDetaching();
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            this.requestBindinglistener.Element = this.AssociatedObject;
            base.OnAttached();
        }

        /// <summary>
        /// Invoked when the interaction request is raised. 
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">InteractionRequest events</param>
        protected void Notify(object sender, InteractionRequestedEventArgs e)
        {
            this.InvokeActions(e);
        }

        private static void HandleBindingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((InteractionRequestTrigger)sender).OnBindingChanged(e);
        }

        private void ChangeHandler(object sender, BindingChangedEventArgs e)
        {
            this.HandleBindingChange(this.requestBindinglistener.Value as IInteractionRequest);
        }

        private void HandleBindingChange(IInteractionRequest newRequest)
        {
            if (this.Request != null)
            {
                this.Request.Raised -= this.Notify;
            }

            if (newRequest != null)
            {
                newRequest.Raised += this.Notify;
            }

            this.Request = newRequest;
        }
    }
}
