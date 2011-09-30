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
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Data;
using System.Windows.Controls;

namespace Microsoft.Practices.Prism.Interactivity.Tests
{
    [TestClass]
    public class BindingListenerFixture
    {
        [TestMethod]
        public void ListenerValueReturnsInitialValueIfElementDataContextHasntChanged()
        {
            var myObject = new MyClass { Value = "Initial" };
            var binding = new Binding("Value");
            var element = new TextBlock { DataContext = myObject };

            var listener = new BindingListener((s, e) => { })
            {
                Element = element,
                Binding = binding
            };

            var initialState = listener.Value;

            Assert.AreEqual("Initial", initialState);
        }

        [TestMethod]
        public void ListenerValueChangesWhenElementDataContextChanges()
        {
            var myObject = new MyClass { Value = "Initial" };
            var binding = new Binding("Value");
            var element = new TextBlock { DataContext = myObject };

            var finalState = string.Empty;
            BindingListener listener = null;
            listener = new BindingListener((s, e) => finalState = (string)listener.Value);
            listener.Element = element;
            listener.Binding = binding;

            var initialState = listener.Value;

            myObject.Value = "NewValue";

            Assert.AreEqual("Initial", initialState);
            Assert.AreEqual("NewValue", finalState);
        }

        [TestMethod]
        public void ListenerValueChangesWhenElementDataContextIsReplaced()
        {
            var myObject = new MyClass { Value = "Initial" };
            var mySecondObject = new MyClass { Value = "Second" };
            var binding = new Binding("Value");
            var element = new TextBlock { DataContext = myObject };

            var finalState = string.Empty;
            BindingListener listener = null;
            listener = new BindingListener((s, e) => finalState = (string)listener.Value);
            listener.Element = element;
            listener.Binding = binding;

            var initialState = listener.Value;

            element.DataContext = mySecondObject;

            Assert.AreEqual("Initial", initialState);
            Assert.AreEqual("Second", finalState);
        }

        public class MyClass : INotifyPropertyChanged
        {
            private string backingValue;

            public string Value
            {
                get
                {
                    return this.backingValue;
                }

                set
                {
                    this.backingValue = value;
                    this.InvokePropertyChanged(new PropertyChangedEventArgs("Value"));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected void InvokePropertyChanged(PropertyChangedEventArgs e)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) handler(this, e);
            }
        }

    }
}
