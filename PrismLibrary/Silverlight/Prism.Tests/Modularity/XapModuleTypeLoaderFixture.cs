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
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Collections.Generic;

namespace Microsoft.Practices.Prism.Tests.Modularity
{
    [TestClass]
    public class XapModuleTypeLoaderFixture
    {
        [TestMethod]
        public void ShouldCallDownloadAsync()
        {
            var mockFileDownloader = new MockFileDownloader();
            var remoteModuleUri = "http://MyModule.xap";
            var moduleInfo = new ModuleInfo() { Ref = remoteModuleUri };
            XapModuleTypeLoader typeLoader = new TestableXapModuleTypeLoader(mockFileDownloader);

            typeLoader.LoadModuleType(moduleInfo);

            Assert.IsTrue(mockFileDownloader.DownloadAsyncCalled);
            Assert.AreEqual(remoteModuleUri, mockFileDownloader.downloadAsyncArgumentUri.OriginalString);
        }

        [TestMethod]
        public void ShouldReturnErrorIfDownloadFails()
        {
            var mockFileDownloader = new MockFileDownloader();
            var remoteModuleUri = "http://MyModule.xap";
            var moduleInfo = new ModuleInfo() { Ref = remoteModuleUri };
            XapModuleTypeLoader typeLoader = new TestableXapModuleTypeLoader(mockFileDownloader);
            ManualResetEvent callbackEvent = new ManualResetEvent(false);
            Exception error = null;

            typeLoader.LoadModuleCompleted += delegate(object sender, LoadModuleCompletedEventArgs e)
            {
                error = e.Error;
                callbackEvent.Set();
            };

            typeLoader.LoadModuleType(moduleInfo);
            mockFileDownloader.InvokeOpenReadCompleted(new DownloadCompletedEventArgs(null, new InvalidOperationException("Mock Ex"), false, mockFileDownloader.DownloadAsyncArgumentUserToken));
            Assert.IsTrue(callbackEvent.WaitOne(500));

            Assert.IsNotNull(error);
            Assert.IsInstanceOfType(error, typeof(InvalidOperationException));
        }

        [TestMethod]
        public void ShouldLoadDownloadedAssemblies()
        {
            // NOTE: This test method uses a resource that is built in a pre-build event when building the project. The resource is a
            // dynamically generated XAP file that is built with the Mocks/Modules/createXap.bat batch file.
            // If this test is failing unexpectedly, it may be that the batch file is not working correctly in the current environment.

            var mockFileDownloader = new MockFileDownloader();
            const string moduleTypeName = "Microsoft.Practices.Prism.Tests.Mocks.Modules.RemoteModule, RemoteModuleA, Version=0.0.0.0";
            var remoteModuleUri = "http://MyModule.xap";
            var moduleInfo = new ModuleInfo() { Ref = remoteModuleUri };
            XapModuleTypeLoader typeLoader = new TestableXapModuleTypeLoader(mockFileDownloader);
            ManualResetEvent callbackEvent = new ManualResetEvent(false);

            typeLoader.LoadModuleCompleted += delegate(object sender, LoadModuleCompletedEventArgs e)
            {
                callbackEvent.Set();
            };

            typeLoader.LoadModuleType(moduleInfo);

            Assert.IsNull(Type.GetType(moduleTypeName));

            Stream xapStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Practices.Prism.Tests.Mocks.Modules.RemoteModules.xap");
            mockFileDownloader.InvokeOpenReadCompleted(new DownloadCompletedEventArgs(xapStream, null, false, mockFileDownloader.DownloadAsyncArgumentUserToken));
            Assert.IsTrue(callbackEvent.WaitOne(500));

            Assert.IsNotNull(Type.GetType(moduleTypeName));
        }

        [TestMethod]
        public void ShouldDownloadOnlyOnceIfRetrievingTwoGroupsFromSameUri()
        {
            var mockFileDownloader = new MockFileDownloader();
            var remoteModuleUri = "http://MyPackage.xap";
            var moduleInfo1 = new ModuleInfo() { Ref = remoteModuleUri };
            var moduleInfo2 = new ModuleInfo() { Ref = remoteModuleUri };
            XapModuleTypeLoader typeLoader = new TestableXapModuleTypeLoader(mockFileDownloader);

            typeLoader.LoadModuleType(moduleInfo1);
            mockFileDownloader.DownloadAsyncCalled = false;
            typeLoader.LoadModuleType(moduleInfo2);

            Assert.IsFalse(mockFileDownloader.DownloadAsyncCalled);
        }    
    
        [TestMethod]
        public void ShouldRaiseDownloadCompletedForEachModuleInfoWhenDownloadedFromSameUri()
        {
            var mockFileDownloader = new MockFileDownloader();

            const string moduleATypeName = "Microsoft.Practices.Prism.Tests.Mocks.Modules.RemoteModule, RemoteModuleA, Version=0.0.0.0";
            var remoteModuleUri = "http://MyModule.xap";
            var moduleInfoA = new ModuleInfo() { Ref = remoteModuleUri, ModuleName = "ModuleA", ModuleType = moduleATypeName };

            const string moduleBTypeName = "Microsoft.Practices.Prism.Tests.Mocks.Modules.RemoteModule, RemoteModuleB, Version=0.0.0.0";
            var moduleInfoB = new ModuleInfo() { Ref = remoteModuleUri, ModuleName = "ModuleB", ModuleType = moduleBTypeName };

            XapModuleTypeLoader typeLoader = new TestableXapModuleTypeLoader(mockFileDownloader);

            List<ModuleInfo> downloadedModuleInfos = new List<ModuleInfo>();
            typeLoader.LoadModuleCompleted += (o, e) =>
                {
                    downloadedModuleInfos.Add(e.ModuleInfo);
                };

            typeLoader.LoadModuleType(moduleInfoA);
            typeLoader.LoadModuleType(moduleInfoB);

            Stream xapStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Practices.Prism.Tests.Mocks.Modules.RemoteModules.xap");
            mockFileDownloader.InvokeOpenReadCompleted(new DownloadCompletedEventArgs(xapStream, null, false, mockFileDownloader.DownloadAsyncArgumentUserToken));


            Assert.IsTrue(downloadedModuleInfos.Contains(moduleInfoA));
            Assert.IsTrue(downloadedModuleInfos.Contains(moduleInfoB));
        }

        [TestMethod]
        public void ShouldRaiseDownloadCompletedOnceIfSameModuleInfoInstanceRequestedTwice()
        {
            var mockFileDownloader = new MockFileDownloader();

            const string moduleATypeName = "Microsoft.Practices.Prism.Tests.Mocks.Modules.RemoteModule, RemoteModuleA, Version=0.0.0.0";
            var remoteModuleUri = "http://MyModule.xap";
            var moduleInfoA = new ModuleInfo() { Ref = remoteModuleUri, ModuleName = "ModuleA", ModuleType = moduleATypeName };

            const string moduleBTypeName = "Microsoft.Practices.Prism.Tests.Mocks.Modules.RemoteModule, RemoteModuleB, Version=0.0.0.0";
            var moduleInfoB = new ModuleInfo() { Ref = remoteModuleUri, ModuleName = "ModuleB", ModuleType = moduleBTypeName };

            XapModuleTypeLoader typeLoader = new TestableXapModuleTypeLoader(mockFileDownloader);

            List<ModuleInfo> downloadedModuleInfos = new List<ModuleInfo>();
            typeLoader.LoadModuleCompleted += (o, e) =>
            {
                downloadedModuleInfos.Add(e.ModuleInfo);
            };

            typeLoader.LoadModuleType(moduleInfoA);
            typeLoader.LoadModuleType(moduleInfoA);
            typeLoader.LoadModuleType(moduleInfoB);
            typeLoader.LoadModuleType(moduleInfoB);

            Stream xapStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Practices.Prism.Tests.Mocks.Modules.RemoteModules.xap");
            mockFileDownloader.InvokeOpenReadCompleted(new DownloadCompletedEventArgs(xapStream, null, false, mockFileDownloader.DownloadAsyncArgumentUserToken));


            Assert.AreEqual(2, downloadedModuleInfos.Count);
            Assert.IsTrue(downloadedModuleInfos.Contains(moduleInfoA));
            Assert.IsTrue(downloadedModuleInfos.Contains(moduleInfoB));
        }

        [TestMethod]
        public void ShouldNotDownloadAgainIfAlreadyDownloaded()
        {
            var mockFileDownloader = new MockFileDownloader();

            const string moduleATypeName = "Microsoft.Practices.Prism.Tests.Mocks.Modules.RemoteModule, RemoteModuleA, Version=0.0.0.0";
            var remoteModuleUri = "http://MyModule.xap";
            var moduleInfoA = new ModuleInfo() { Ref = remoteModuleUri, ModuleName = "ModuleA", ModuleType = moduleATypeName };

            XapModuleTypeLoader typeLoader = new TestableXapModuleTypeLoader(mockFileDownloader);

            List<ModuleInfo> downloadedModuleInfos = new List<ModuleInfo>();
            typeLoader.LoadModuleCompleted += (o, e) =>
            {
                downloadedModuleInfos.Add(e.ModuleInfo);
            };

            typeLoader.LoadModuleType(moduleInfoA);

            Stream xapStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Practices.Prism.Tests.Mocks.Modules.RemoteModules.xap");
            mockFileDownloader.InvokeOpenReadCompleted(new DownloadCompletedEventArgs(xapStream, null, false, mockFileDownloader.DownloadAsyncArgumentUserToken));

            mockFileDownloader.DownloadAsyncCalled = false;

            typeLoader.LoadModuleType(moduleInfoA);

            Assert.AreEqual(2, downloadedModuleInfos.Count);
            Assert.AreEqual(moduleInfoA, downloadedModuleInfos[0]);
            Assert.AreEqual(moduleInfoA, downloadedModuleInfos[1]);
            Assert.IsFalse(mockFileDownloader.DownloadAsyncCalled);            
        }
    }

    internal class TestableXapModuleTypeLoader : XapModuleTypeLoader
    {
        public IFileDownloader FileDownloader;

        public TestableXapModuleTypeLoader(IFileDownloader downloader)
        {
            this.FileDownloader = downloader;
        }

        protected override IFileDownloader CreateDownloader()
        {
            return this.FileDownloader;
        }
    }

    internal class MockFileDownloader : IFileDownloader
    {
        public bool DownloadAsyncCalled;
        public Uri downloadAsyncArgumentUri;
        public object DownloadAsyncArgumentUserToken;

        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

        public event EventHandler<DownloadCompletedEventArgs> DownloadCompleted;

        public void DownloadAsync(Uri uri, object userToken)
        {
            DownloadAsyncCalled = true;
            this.downloadAsyncArgumentUri = uri;
            DownloadAsyncArgumentUserToken = userToken;
        }

        public void InvokeOpenReadCompleted(DownloadCompletedEventArgs args)
        {
            DownloadCompleted.Invoke(this, args);
        }
    }
}
