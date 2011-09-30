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
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Prism.Tests.ViewModel
{
    [TestClass]
    public class DataTemplateSelectorFixture
    {
        [TestMethod]
        public void WhenContentSet_SelectsTemplateFromResourceByName()
        {
            var selector = new DataTemplateSelector();
            var dataTemplate = new DataTemplate();
            Assert.AreNotSame(dataTemplate, selector.ContentTemplate);

            selector.Resources.Add("FirstContentType", dataTemplate);

            selector.Content = new FirstContentType();
            Assert.AreSame(dataTemplate, selector.ContentTemplate);
        }

        [TestMethod]
        public void WhenContentWithoutMatchingResourceSet_SetsTemplateToNull()
        {
            var selector = new DataTemplateSelector();
            var originalDataTemplate = new DataTemplate();
            selector.ContentTemplate = originalDataTemplate;

            selector.Content = new FirstContentType();

            Assert.IsNull(selector.ContentTemplate);
        }

        [TestMethod]
        public void WhenContentIsNull_ThenSetsTemplateToNull()
        {
            var selector = new DataTemplateSelector();
            var originalDataTemplate = new DataTemplate();

            selector.Content = new object();
            selector.ContentTemplate = originalDataTemplate;

            selector.Content = null;

            Assert.IsNull(selector.ContentTemplate);
        }

        [TestMethod]
        public void WhenChangingContentType_TemplateIsUpdatedBasedOnType()
        {
            var selector = new DataTemplateSelector();
            var firstDataTemplate = new DataTemplate();
            var secondDataTemplate = new DataTemplate();
            
            selector.Resources.Add("FirstContentType", firstDataTemplate);
            selector.Resources.Add("SecondContentType", secondDataTemplate);

            selector.Content = new FirstContentType();
            Assert.AreSame(firstDataTemplate, selector.ContentTemplate);

            selector.Content = new SecondContentType();
            Assert.AreSame(secondDataTemplate, selector.ContentTemplate);
        }

        
        public class FirstContentType {}

        public class SecondContentType {}
    }
}
