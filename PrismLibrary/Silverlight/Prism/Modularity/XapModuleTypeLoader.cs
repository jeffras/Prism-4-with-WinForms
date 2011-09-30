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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Resources;
using System.Xml;
using System.Net;

namespace Microsoft.Practices.Prism.Modularity
{
    /// <summary>
    /// Component responsible for downloading remote modules 
    /// and load their <see cref="Type"/> into the current application domain.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Xap")]
    public class XapModuleTypeLoader : IModuleTypeLoader
    {
        private Dictionary<Uri, List<ModuleInfo>> downloadingModules = new Dictionary<Uri, List<ModuleInfo>>();
        private HashSet<Uri> downloadedUris = new HashSet<Uri>();

        /// <summary>
        /// Raised repeatedly to provide progress as modules are loaded in the background.
        /// </summary>
        public event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;

        private void RaiseModuleDownloadProgressChanged(ModuleDownloadProgressChangedEventArgs e)
        {
            if (this.ModuleDownloadProgressChanged != null)
            {
                this.ModuleDownloadProgressChanged(this, e);
            }
        }

        /// <summary>
        /// Raised when a module is loaded or fails to load.
        /// </summary>
        public event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;

        private void RaiseLoadModuleCompleted(ModuleInfo moduleInfo, Exception error)
        {
            this.RaiseLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, error));
        }

        private void RaiseLoadModuleCompleted(LoadModuleCompletedEventArgs e)
        {
            if (this.LoadModuleCompleted != null)
            {
                this.LoadModuleCompleted(this, e);
            }
        }

        /// <summary>
        /// Evaluates the <see cref="ModuleInfo.Ref"/> property to see if the current typeloader will be able to retrieve the <paramref name="moduleInfo"/>.
        /// </summary>
        /// <param name="moduleInfo">Module that should have it's type loaded.</param>
        /// <returns><see langword="true"/> if the current typeloader is able to retrieve the module, otherwise <see langword="false"/>.</returns>
        public bool CanLoadModuleType(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null) throw new ArgumentNullException("moduleInfo");
            if (!string.IsNullOrEmpty(moduleInfo.Ref))
            {
                Uri uri;
                return Uri.TryCreate(moduleInfo.Ref, UriKind.RelativeOrAbsolute, out uri);
            }

            return false;
        }

        /// <summary>
        /// Retrieves the <paramref name="moduleInfo"/>.
        /// </summary>
        /// <param name="moduleInfo">Module that should have it's type loaded.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Error is sent to completion event")]
        public void LoadModuleType(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
            {
                throw new System.ArgumentNullException("moduleInfo");
            }

            try
            {
                Uri uri = new Uri(moduleInfo.Ref, UriKind.RelativeOrAbsolute);

                // If this module has already been downloaded, I fire the completed event.
                if (this.IsSuccessfullyDownloaded(uri))
                {
                    this.RaiseLoadModuleCompleted(moduleInfo, null);
                }
                else
                {
                    bool needToStartDownload = !this.IsDownloading(uri);

                    // I record downloading for the moduleInfo even if I don't need to start a new download
                    this.RecordDownloading(uri, moduleInfo);

                    if (needToStartDownload)
                    {
                        IFileDownloader downloader = this.CreateDownloader();
                        downloader.DownloadProgressChanged += this.IFileDownloader_DownloadProgressChanged;
                        downloader.DownloadCompleted += this.IFileDownloader_DownloadCompleted;
                        downloader.DownloadAsync(uri, uri);
                    }
                }
            }
            catch (Exception ex)
            {
                this.RaiseLoadModuleCompleted(moduleInfo, ex);
            }
        }

        /// <summary>
        /// Creates the <see cref="IFileDownloader"/> used to retrieve the remote modules.
        /// </summary>
        /// <returns>The <see cref="IFileDownloader"/> used to retrieve the remote modules.</returns>
        protected virtual IFileDownloader CreateDownloader()
        {
            return new FileDownloader();
        }

        void IFileDownloader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // I ensure the download completed is on the UI thread so that types can be loaded into the application domain.
            if (!Deployment.Current.Dispatcher.CheckAccess())
            {
                Deployment.Current.Dispatcher.BeginInvoke(new Action<DownloadProgressChangedEventArgs>(this.HandleModuleDownloadProgressChanged), e);
            }
            else
            {
                this.HandleModuleDownloadProgressChanged(e);
            }
        }

        private void HandleModuleDownloadProgressChanged(DownloadProgressChangedEventArgs e)
        {
            Uri uri = (Uri)e.UserState;
            List<ModuleInfo> moduleInfos = this.GetDownloadingModules(uri);

            foreach (ModuleInfo moduleInfo in moduleInfos)
            {
                this.RaiseModuleDownloadProgressChanged(new ModuleDownloadProgressChangedEventArgs(moduleInfo, e.BytesReceived, e.TotalBytesToReceive));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void IFileDownloader_DownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            // A new IFileDownloader instance is created for each download.
            // I unregister the event to allow for garbage collection.
            IFileDownloader fileDownloader = (IFileDownloader)sender;
            fileDownloader.DownloadProgressChanged -= this.IFileDownloader_DownloadProgressChanged;
            fileDownloader.DownloadCompleted -= this.IFileDownloader_DownloadCompleted;

            // I ensure the download completed is on the UI thread so that types can be loaded into the application domain.
            if (!Deployment.Current.Dispatcher.CheckAccess())
            {
                Deployment.Current.Dispatcher.BeginInvoke(new Action<DownloadCompletedEventArgs>(this.HandleModuleDownloaded), e);
            }
            else
            {
                this.HandleModuleDownloaded(e);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception sent to completion event")]
        private void HandleModuleDownloaded(DownloadCompletedEventArgs e)
        {
            Uri uri = (Uri)e.UserState;
            List<ModuleInfo> moduleInfos = this.GetDownloadingModules(uri);

            Exception error = e.Error;
            if (error == null)
            {
                try
                {
                    this.RecordDownloadComplete(uri);

                    Debug.Assert(!e.Cancelled, "Download should not be cancelled");
                    Stream stream = e.Result;

                    foreach (AssemblyPart part in GetParts(stream))
                    {
                        LoadAssemblyFromStream(stream, part);
                    }

                    this.RecordDownloadSuccess(uri);
                }
                catch (Exception ex)
                {
                    error = ex;
                }
                finally
                {
                    e.Result.Close();
                }
            }

            foreach (ModuleInfo moduleInfo in moduleInfos)
            {
                this.RaiseLoadModuleCompleted(moduleInfo, error);
            }
        }

        private bool IsDownloading(Uri uri)
        {
            lock (this.downloadingModules)
            {
                return this.downloadingModules.ContainsKey(uri);
            }
        }

        private void RecordDownloading(Uri uri, ModuleInfo moduleInfo)
        {
            lock (this.downloadingModules)
            {
                List<ModuleInfo> moduleInfos;
                if (!this.downloadingModules.TryGetValue(uri, out moduleInfos))
                {
                    moduleInfos = new List<ModuleInfo>();
                    this.downloadingModules.Add(uri, moduleInfos);
                }

                if (!moduleInfos.Contains(moduleInfo))
                {
                    moduleInfos.Add(moduleInfo);
                }
            }
        }

        private List<ModuleInfo> GetDownloadingModules(Uri uri)
        {
            lock (this.downloadingModules)
            {
                return new List<ModuleInfo>(this.downloadingModules[uri]);
            }
        }

        private void RecordDownloadComplete(Uri uri)
        {
            lock (this.downloadingModules)
            {                
                if (!this.downloadingModules.ContainsKey(uri))
                {
                    this.downloadingModules.Remove(uri);
                }
            }
        }

        private bool IsSuccessfullyDownloaded(Uri uri)
        {
            lock (this.downloadedUris)
            {
                return this.downloadedUris.Contains(uri);
            }
        }

        private void RecordDownloadSuccess(Uri uri)
        {
            lock (this.downloadedUris)
            {
                this.downloadedUris.Add(uri);
            }
        }

        private static IEnumerable<AssemblyPart> GetParts(Stream stream)
        {
            List<AssemblyPart> assemblyParts = new List<AssemblyPart>();

            var streamReader = new StreamReader(Application.GetResourceStream(new StreamResourceInfo(stream, null), new Uri("AppManifest.xaml", UriKind.Relative)).Stream);
            using (XmlReader xmlReader = XmlReader.Create(streamReader))
            {
                xmlReader.MoveToContent();
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "Deployment.Parts")
                    {
                        using (XmlReader xmlReaderAssemblyParts = xmlReader.ReadSubtree())
                        {
                            while (xmlReaderAssemblyParts.Read())
                            {
                                if (xmlReaderAssemblyParts.NodeType == XmlNodeType.Element && xmlReaderAssemblyParts.Name == "AssemblyPart")
                                {
                                    AssemblyPart assemblyPart = new AssemblyPart();
                                    assemblyPart.Source = xmlReaderAssemblyParts.GetAttribute("Source");
                                    assemblyParts.Add(assemblyPart);
                                }
                            }
                        }

                        break;
                    }
                }
            }

            return assemblyParts;
        }

        private static void LoadAssemblyFromStream(Stream sourceStream, AssemblyPart assemblyPart)
        {
            Stream assemblyStream = Application.GetResourceStream(
                new StreamResourceInfo(sourceStream, null),
                new Uri(assemblyPart.Source, UriKind.Relative)).Stream;

            assemblyPart.Load(assemblyStream);
        }
    }
}
