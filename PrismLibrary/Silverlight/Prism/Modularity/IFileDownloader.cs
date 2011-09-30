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
using System.Net;

namespace Microsoft.Practices.Prism.Modularity
{
    /// <summary>
    /// Defines a contract for the object used to download files asynchronously.
    /// </summary>
    public interface IFileDownloader
    {
        /// <summary>
        /// Raised whenever the download progress changes.
        /// </summary>
        event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

        /// <summary>
        /// Raised download is complete.
        /// </summary>
        event EventHandler<DownloadCompletedEventArgs> DownloadCompleted;

        /// <summary>
        /// Starts downloading asynchronously a file from <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The location of the file to be downloaded.</param>
        /// <param name="userToken">Provides a user-specified identifier for the asynchronous task.</param>
        void DownloadAsync(Uri uri, object userToken);
    }
}