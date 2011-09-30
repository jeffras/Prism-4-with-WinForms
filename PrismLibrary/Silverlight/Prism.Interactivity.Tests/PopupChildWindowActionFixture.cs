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
using System.Windows.Controls;
using System.Windows.Interactivity;
using Microsoft.Practices.Prism.Interactivity;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Prism.Interactivity.Tests
{
    [TestClass]
    public class PopupChildWindowActionFixture : SilverlightTest
    {
        [TestMethod]
        [Asynchronous]
        [Timeout(5000)]
        public void WhenInvokedWithAParameter_ThenOpensTheChildWindow()
        {
            var childWindow = new TestChildWindow { };
            var action = new PopupChildWindowAction { ChildWindow = childWindow };
            var trigger = new TestTrigger { Actions = { action } };
            var args = new InteractionRequestedEventArgs(null, () => { });

            trigger.InvokeActions(args);

            EnqueueConditional(() => childWindow.IsOpen);

            EnqueueCallback(() => { childWindow.Close(); });

            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Timeout(5000)]
        public void WhenChildWindowIsClosed_ThenTheEventCallbackIsExecuted()
        {
            var childWindow = new TestChildWindow { };
            var action = new PopupChildWindowAction { ChildWindow = childWindow };
            var trigger = new TestTrigger { Actions = { action } };
            var context = new Notification();
            var callbackInvoked = false;
            var args = new InteractionRequestedEventArgs(context, () => { callbackInvoked = true; });

            trigger.InvokeActions(args);

            EnqueueConditional(() => childWindow.IsOpen);

            EnqueueCallback(() => { childWindow.Close(); });

            EnqueueConditional(() => callbackInvoked);

            EnqueueTestComplete();
        }

        private class TestTrigger : TriggerBase<DependencyObject>
        {
            public new void InvokeActions(object parameter)
            {
                base.InvokeActions(parameter);
            }
        }

        private class TestChildWindow : ChildWindow
        {
            public bool IsOpen { get; private set; }

            protected override void OnOpened()
            {
                base.OnOpened();
                this.IsOpen = true;
            }

            protected override void OnClosed(System.EventArgs e)
            {
                this.IsOpen = false;
                base.OnClosed(e);
            }
        }
    }
}
