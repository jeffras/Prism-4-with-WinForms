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
    /// Defines the component used to download files.
    /// </summary>
    /// <remarks>This is mainly a wrapper for the <see cref="WebClient"/> class that implements <see cref="IFileDownloader"/>.</remarks>
    public class FileDownloader : IFileDownloader
    {
        private readonly WebClient webClient = new WebClient();

        private event EventHandler<DownloadProgressChangedEventArgs> _downloadProgressChanged;
        private event EventHandler<DownloadCompletedEventArgs> _downloadCompleted;

        /// <summary>
        /// Raised whenever the download progress changes.
        /// </summary>
        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged
        {
            add
            {
                if (this._downloadProgressChanged == null)
                {
                    this.webClient.DownloadProgressChanged += this.WebClient_DownloadProgressChanged;
                }

                this._downloadProgressChanged += value;
            }

            remove
            {
                this._downloadProgressChanged -= value;
                if (this._downloadProgressChanged == null)
                {
                    this.webClient.DownloadProgressChanged -= this.WebClient_DownloadProgressChanged;
                }
            }
        }


        /// <summary>
        /// Raised download is complete.
        /// </summary>
        public event EventHandler<DownloadCompletedEventArgs> DownloadCompleted
        {
            add
            {
                if (this._downloadCompleted == null)
                {                    
                    this.webClient.OpenReadCompleted += this.WebClient_OpenReadCompleted;
                }
                
                this._downloadCompleted += value;
            }

            remove
            {
                this._downloadCompleted -= value;
                if (this._downloadCompleted == null)
                {
                    this.webClient.OpenReadCompleted -= this.WebClient_OpenReadCompleted;
                }
            }
        }                

        /// <summary>
        /// Starts downloading asynchronously a file from <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The location of the file to be downloaded.</param>
        /// <param name="userToken">Provides a user-specified identifier for the asynchronous task.</param>
        public void DownloadAsync(Uri uri, object userToken)
        {
            this.webClient.OpenReadAsync(uri, userToken);
        }

        private static DownloadCompletedEventArgs ConvertArgs(OpenReadCompletedEventArgs args)
        {            
            return new DownloadCompletedEventArgs(args.Error == null ? args.Result : null, args.Error, args.Cancelled, args.UserState);
        }

        void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this._downloadProgressChanged(this, e);
        }

        private void WebClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            this._downloadCompleted(this, ConvertArgs(e));
        }
    }
}
