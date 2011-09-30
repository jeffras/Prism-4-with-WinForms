using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using System.Windows.Forms;

namespace FirstPrismModule
{
public class CompositeWFModule : IModule
{
    IUnityContainer _container;
    public CompositeWFModule(IUnityContainer container)
    {
        _container = container;
    }
    #region IModule Members

    public void Initialize()
    {
        Panel mainRegion = _container.Resolve<Panel>("MainRegion");
        ModuleView view = _container.Resolve<ModuleView>();
        view.Dock = DockStyle.Fill;
        mainRegion.Controls.Add(view);
    }

    #endregion
}
}
