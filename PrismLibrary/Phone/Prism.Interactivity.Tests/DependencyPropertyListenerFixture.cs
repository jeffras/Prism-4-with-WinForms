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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.Prism.Interactivity;

namespace TailSpin.PhoneClient.Tests.Infrastructure
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Windows.Data;
    using System.Windows.Controls;

    [TestClass]
    public class DependencyPropertyListenerFixture
    {
        [TestMethod]
        public void ListenerChangedIsRaisedWhenDataContextPropertyChanges()
        {
            var myObject = new MyClass {Value = "Initial"};
            var binding = new Binding("Value");
            var element = new TextBlock {DataContext = myObject};

            var currentState = string.Empty;
            var listener = new DependencyPropertyListener();
            listener.Changed += ((s, e) => currentState = (string) e.EventArgs.NewValue);
            listener.Attach(element, binding);

            var initialState = currentState;
            myObject.Value = "NewValue";
            var finalState = currentState;

            Assert.AreEqual("Initial", initialState);
            Assert.AreEqual("NewValue", finalState);
        }

        [TestMethod]
        public void ListenerChangedIsRaisedWhenDataContextIsReplaced()
        {
            var myObject = new MyClass {Value = "Initial"};
            var mySecondObject = new MyClass {Value = "Final"};
            var binding = new Binding("Value");
            var element = new TextBlock {DataContext = myObject};

            var currentState = string.Empty;
            var listener = new DependencyPropertyListener();
            listener.Changed += ((s, e) => currentState = (string) e.EventArgs.NewValue);
            listener.Attach(element, binding);

            var initialState = currentState;
            element.DataContext = mySecondObject;
            var finalState = currentState;

            Assert.AreEqual("Initial", initialState);
            Assert.AreEqual("Final", finalState);
        }

        public class MyClass : INotifyPropertyChanged
        {
            private string backingValue;
            
            public event PropertyChangedEventHandler PropertyChanged;

            public string Value
            {
                get { return this.backingValue; }

                set
                {
                    this.backingValue = value;
                    this.InvokePropertyChanged(new PropertyChangedEventArgs("Value"));
                }
            }

            public void InvokePropertyChanged(PropertyChangedEventArgs e)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) handler(this, e);
            }
        }
    }
}