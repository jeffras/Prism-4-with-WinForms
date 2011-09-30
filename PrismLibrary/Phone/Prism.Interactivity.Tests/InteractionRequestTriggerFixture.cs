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
using System.Windows.Data;
using System.Windows.Interactivity;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Prism.Interactivity.Tests
{
    [TestClass]
    public class InteractionRequestTriggerFixture
    {
        [TestMethod]
        public void TriggerReceivesNotificationsFromBindedInteractionRequest()
        {
            var page = new MockFrameworkElement();
            
            var myNotificationAwareObject = new MyNotificationAwareClass();
            var binding = new Binding("InteractionRequest") { Source = myNotificationAwareObject, Mode = BindingMode.OneWay };
            var trigger = new InteractionRequestTrigger()
            {
                SourceObject = binding,
            };

            InteractionRequestedEventArgs eventArgs = null;
            var actionListener = new ActionListener((e) => { eventArgs = (InteractionRequestedEventArgs)e; });
            trigger.Actions.Add(actionListener);
            trigger.Attach(page);

            myNotificationAwareObject.InteractionRequest.Raise(new Notification { Title = "Bar" });
            Assert.IsNotNull(eventArgs);
            Assert.AreEqual("Bar", eventArgs.Context.Title);
        }

        public class MockFrameworkElement : FrameworkElement
        {
        }

        public class MyNotificationAwareClass
        {
            public MyNotificationAwareClass()
            {
                this.InteractionRequest = new InteractionRequest<Notification>();
            }

            public InteractionRequest<Notification> InteractionRequest { get; private set; }
        }

        public class ActionListener : TriggerAction<DependencyObject>
        {
            private Action<object> handler;

            public ActionListener(Action<object> handler)
            {
                this.handler = handler;
            }

            protected override void Invoke(object parameter)
            {
                this.handler(parameter);
            }
        }

    }
}
