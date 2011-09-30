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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Modularity;

namespace Microsoft.Practices.Prism.MefExtensions.Modularity
{
    /// <summary>
    /// Component responsible for coordinating the modules' type loading and module initialization process. 
    /// </summary>
    ///  <remarks>
    /// This extends the MefModuleManager for Silverlight to provide a MEF-based XAP type loader.
    /// </remarks>
    public partial class MefModuleManager : ModuleManager, IPartImportsSatisfiedNotification
    {
        #if SILVERLIGHT

        /// <summary>
        /// Get or sets the imported XAP module type loader
        /// </summary>
        /// <value>The mef xap module type loader.</value>
        /// <remarks>
        /// MEF requires this be public in Silverlight applications.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Xap"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mef")]
        [Import(AllowRecomposition = false)]
        public MefXapModuleTypeLoader MefXapModuleTypeLoader { get; set; }
#else        
        // I import that type loader using MEF as a singleton.
        [Import(AllowRecomposition = false)]
        private MefXapModuleTypeLoader MefXapModuleTypeLoader { get; set; }
#endif

        private IEnumerable<IModuleTypeLoader> mefTypeLoaders;

        /// <summary>
        /// Returns the list of registered <see cref="IModuleTypeLoader"/> instances that will be used to load the types of modules.
        /// </summary>
        /// <value>A collection of module type loaders.</value>
        public override IEnumerable<IModuleTypeLoader> ModuleTypeLoaders
        {
            get
            {
                if (this.mefTypeLoaders == null)
                {
                    this.mefTypeLoaders = new List<IModuleTypeLoader>()
                                          {
                                              this.MefXapModuleTypeLoader
                                          };
                }

                return this.mefTypeLoaders;
            }

            set
            {
                this.mefTypeLoaders = value;
            }
        }
    }
}
