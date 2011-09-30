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
using Microsoft.Phone.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Prism.Interactivity.Tests
{
    [TestClass]
    public class ApplicationBarExtensionsFixture
    {
        [TestMethod]
        public void CanFindApplicationButtonInAppBar()
        {
            var bar = new ApplicationBar();
            var button1 = new ApplicationBarIconButton(new Uri("/button1.png", UriKind.Relative));
            button1.Text = "button1";
            var button2 = new ApplicationBarIconButton(new Uri("/button2.png", UriKind.Relative));
            button2.Text = "button2";
            bar.Buttons.Add(button1);
            bar.Buttons.Add(button2);

            ApplicationBarIconButton findButton = bar.FindButton("button1");

            Assert.AreEqual(button1, findButton);
        }

        [TestMethod]
        public void ReturnsNullWhenButtonIsNotFoundInAppBar()
        {
            var bar = new ApplicationBar();
            var button1 = new ApplicationBarIconButton(new Uri("/button1.png", UriKind.Relative));
            button1.Text = "button1";
            var button2 = new ApplicationBarIconButton(new Uri("/button2.png", UriKind.Relative));
            button2.Text = "button2";
            bar.Buttons.Add(button1);
            bar.Buttons.Add(button2);

            ApplicationBarIconButton findButton = bar.FindButton("button3");

            Assert.IsNull(findButton);
        }
    }
}