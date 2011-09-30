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
using System.Linq;
using Microsoft.Phone.Shell;

namespace Microsoft.Practices.Prism.Interactivity
{
    /// <summary>
    /// Extensions to the <see cref="IApplicationBar"/>.
    /// </summary>
    public static class ApplicationBarExtensions
    {
        ///<summary>
        /// Finds an <see cref="ApplicationBarIconButton"/> by its name.
        ///</summary>
        ///<param name="appBar"></param>
        ///<param name="text"></param>
        ///<returns></returns>
        [CLSCompliant(false)]
        public static ApplicationBarIconButton FindButton(this IApplicationBar appBar, string text)
        {
            if (appBar == null) throw new ArgumentNullException("appBar");
            return (from object button in appBar.Buttons select button as ApplicationBarIconButton).FirstOrDefault(btn => btn != null && btn.Text == text);
        }
    }
}
