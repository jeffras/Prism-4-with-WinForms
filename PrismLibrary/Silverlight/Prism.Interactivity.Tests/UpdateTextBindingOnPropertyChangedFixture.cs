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
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.Interactivity;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Prism.Interactivity.Tests
{
    [TestClass]
    public class UpdateTextBindingOnPropertyChangedFixture : SilverlightTest
    {
        [TestMethod]
        [Asynchronous]
        [Timeout(5000)]
        public void TestMethod1()
        {
            var behavior = new UpdateTextBindingOnPropertyChanged();

            var bindingSource = new BindingSource() { Value = "InitialValue" };
            var binding = new Binding("Value") { Source = bindingSource, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.Explicit };

            var textBox = new TextBox();
            base.TestPanel.Children.Add(textBox);

            EnqueueCallback(() =>
                                {
                                    textBox.SetBinding(TextBox.TextProperty, binding);
                                    System.Windows.Interactivity.Interaction.GetBehaviors(textBox).Add(behavior);

                                    textBox.Text = "NewValue";
                                });

            EnqueueCallback(() => Assert.AreEqual("NewValue", bindingSource.Value));
            EnqueueTestComplete();
        }
    }

    public class BindingSource
    {
        public string Value { get; set; }
    }
}