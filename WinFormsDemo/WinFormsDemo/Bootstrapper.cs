using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FirstPrismModule;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using SecondPrismModule;
using WinFormsUnityBootStrapper;

namespace WinFormsDemo
{
    internal class Bootstrapper: SimpleUnityBootstrapper
    {
        protected override Form CreateShell()
        {
            return ServiceLocator.Current.GetInstance<Shell>();
        }

        /// <summary>
        /// Initializes the shell.
        /// </summary>
        protected override void InitializeShell()
        {
        }

        /// <summary>
        /// Configures the <see cref="IModuleCatalog"/> used by Prism.
        /// </summary>
        protected override void ConfigureModuleCatalog()
        {
            List<Type> types = new List<Type>();
            types.Add(typeof(CompositeWFModule));
            types.Add(typeof(SecondModule));

            foreach (var type in types)
            {
                ModuleCatalog.AddModule(new ModuleInfo()
                                            {
                                                ModuleName = type.Name,
                                                ModuleType = type.AssemblyQualifiedName,
                                                InitializationMode = InitializationMode.WhenAvailable
                                            });
            }
        }
    }
}
