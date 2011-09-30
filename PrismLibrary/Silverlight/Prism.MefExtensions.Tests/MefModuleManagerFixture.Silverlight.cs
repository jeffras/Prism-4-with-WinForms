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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.Practices.Prism.MefExtensions.Tests
{
    public partial class MefModuleManagerFixture
    {
        [TestMethod]
        public void ConstructorThrowsWithNullModuleInitializer()
        {
            try
            {
                new MefModuleManager(null, new Mock<IModuleCatalog>().Object, new Mock<ILoggerFacade>().Object);
                Assert.Fail("No exception thrown when expected");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("moduleInitializer", ex.GetParameterName());
            }
        }

        [TestMethod]
        public void ConstructorThrowsWithNullModuleCatalog()
        {
            try
            {
                new MefModuleManager(new Mock<IModuleInitializer>().Object, null, new Mock<ILoggerFacade>().Object);
                Assert.Fail("No exception thrown when expected");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("moduleCatalog", ex.GetParameterName());
            }
        }

        [TestMethod]
        public void ConstructorThrowsWithNullLogger()
        {
            try
            {
                new MefModuleManager(new Mock<IModuleInitializer>().Object, new Mock<IModuleCatalog>().Object, null);
                Assert.Fail("No exception thrown when expected");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("loggerFacade", ex.GetParameterName());
            }
        }

        [TestMethod]
        public void UnknownExportedModuleIsAddedAndInitializedByModuleInitializer()
        {
            var aggregateCatalog = new AggregateCatalog();
            var compositionContainer = new CompositionContainer(aggregateCatalog);

            var moduleCatalog = new ModuleCatalog();

            var mockModuleTypeLoader = new Mock<MefXapModuleTypeLoader>(new DownloadedPartCatalogCollection());

            compositionContainer.ComposeExportedValue<IModuleCatalog>(moduleCatalog);
            compositionContainer.ComposeExportedValue<MefXapModuleTypeLoader>(mockModuleTypeLoader.Object);

            bool wasInit = false;
            var mockModuleInitializer = new Mock<IModuleInitializer>();
            mockModuleInitializer.Setup(x => x.Initialize(It.IsAny<ModuleInfo>())).Callback(() => wasInit = true);

            var mockLoggerFacade = new Mock<ILoggerFacade>();

            var moduleManager =
                new MefModuleManager(mockModuleInitializer.Object, moduleCatalog, mockLoggerFacade.Object);

            aggregateCatalog.Catalogs.Add(new TypeCatalog(typeof(TestMefModule)));

            compositionContainer.SatisfyImportsOnce(moduleManager);

            moduleManager.Run();

            Assert.IsTrue(wasInit);
            Assert.IsTrue(moduleCatalog.Modules.Any(mi => mi.ModuleName == "TestMefModule"));
        }

        [TestMethod]
        public void DeclaredModuleWithoutTypeInUnreferencedAssemblyIsUpdatedWithTypeNameFromExportAttribute()
        {
            var aggregateCatalog = new AggregateCatalog();
            var compositionContainer = new CompositionContainer(aggregateCatalog);

            var mockModuleTypeLoader = new Mock<MefXapModuleTypeLoader>(new DownloadedPartCatalogCollection());
            mockModuleTypeLoader.Setup(tl => tl.CanLoadModuleType(It.IsAny<ModuleInfo>())).Returns(true);

            var moduleCatalog = new ModuleCatalog();
            var moduleInfo = new ModuleInfo { ModuleName = "TestMefModule" };
            moduleCatalog.AddModule(moduleInfo);

            compositionContainer.ComposeExportedValue<IModuleCatalog>(moduleCatalog);
            compositionContainer.ComposeExportedValue<MefXapModuleTypeLoader>(mockModuleTypeLoader.Object);

            bool wasInit = false;
            var mockModuleInitializer = new Mock<IModuleInitializer>();
            mockModuleInitializer.Setup(x => x.Initialize(It.IsAny<ModuleInfo>())).Callback(() => wasInit = true);

            var mockLoggerFacade = new Mock<ILoggerFacade>();

            var moduleManager =
                new MefModuleManager(mockModuleInitializer.Object, moduleCatalog, mockLoggerFacade.Object);

            compositionContainer.SatisfyImportsOnce(moduleManager);
            moduleManager.Run();

            Assert.IsFalse(wasInit);

            aggregateCatalog.Catalogs.Add(new TypeCatalog(typeof(TestMefModule)));

            compositionContainer.SatisfyImportsOnce(moduleManager);

            mockModuleTypeLoader.Raise(tl => tl.LoadModuleCompleted += null, new LoadModuleCompletedEventArgs(moduleInfo, null));

            Assert.AreEqual(typeof(TestMefModule).AssemblyQualifiedName, moduleInfo.ModuleType);
            Assert.IsTrue(wasInit);
        }

        [TestMethod]
        public void DeclaredModuleWithTypeInUnreferencedAssemblyIsUpdatedWithTypeNameFromExportAttribute()
        {
            var aggregateCatalog = new AggregateCatalog();
            var compositionContainer = new CompositionContainer(aggregateCatalog);

            var mockModuleTypeLoader = new Mock<MefXapModuleTypeLoader>(new DownloadedPartCatalogCollection());
            mockModuleTypeLoader.Setup(tl => tl.CanLoadModuleType(It.IsAny<ModuleInfo>())).Returns(true);

            var moduleCatalog = new ModuleCatalog();
            var moduleInfo = new ModuleInfo { ModuleName = "TestMefModule", ModuleType = "some type" };
            moduleCatalog.AddModule(moduleInfo);

            compositionContainer.ComposeExportedValue<IModuleCatalog>(moduleCatalog);
            compositionContainer.ComposeExportedValue<MefXapModuleTypeLoader>(mockModuleTypeLoader.Object);

            bool wasInit = false;
            var mockModuleInitializer = new Mock<IModuleInitializer>();
            mockModuleInitializer.Setup(x => x.Initialize(It.IsAny<ModuleInfo>())).Callback(() => wasInit = true);

            var mockLoggerFacade = new Mock<ILoggerFacade>();

            var moduleManager =
                new MefModuleManager(mockModuleInitializer.Object, moduleCatalog, mockLoggerFacade.Object);

            compositionContainer.SatisfyImportsOnce(moduleManager);
            moduleManager.Run();

            Assert.IsFalse(wasInit);

            aggregateCatalog.Catalogs.Add(new TypeCatalog(typeof(TestMefModule)));

            compositionContainer.SatisfyImportsOnce(moduleManager);

            mockModuleTypeLoader.Raise(tl => tl.LoadModuleCompleted += null, new LoadModuleCompletedEventArgs(moduleInfo, null));

            Assert.AreEqual(typeof(TestMefModule).AssemblyQualifiedName, moduleInfo.ModuleType);
            Assert.IsTrue(wasInit);
        }
    }

    [ModuleExport(typeof(TestMefModule))]
    public class TestMefModule : IModule
    {
        public void Initialize()
        {
        }
    }

    public static class ArgumentNullExceptionExtensions
    {
        public static string GetParameterName(this ArgumentNullException exception)
        {
            const string markerText = "Parameter name: ";
            string message = exception.Message;
            int index = message.LastIndexOf(markerText);

            string parameterName = message.Substring(index + markerText.Length);

            return parameterName;
        }
    }
}