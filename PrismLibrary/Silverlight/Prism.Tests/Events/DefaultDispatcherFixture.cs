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
using Microsoft.Practices.Prism.Events;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Prism.Presentation.Tests.Events
{
    [TestClass]
    public class DefaultDispatcherFixture : WorkItemTest
    {
        [TestMethod]
        [Asynchronous]
        public void CanInvokeUIThreadDispatcherFromBackgroundThread()
        {
            DefaultDispatcher dispatcher = new DefaultDispatcher();
            bool calledDelegate = false;

            BackgroundWorker backgroundThread = new BackgroundWorker();
            backgroundThread.DoWork += ((sender, e) => dispatcher.BeginInvoke(new Action<object>(o => calledDelegate = true), null));
            backgroundThread.RunWorkerAsync();

            EnqueueDelay(500);
            EnqueueCallback(() => Assert.IsTrue(calledDelegate));
            EnqueueTestComplete();
        }
    }
}
