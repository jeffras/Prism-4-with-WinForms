using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Events;
using Common.dto;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace SecondPrismModule
{
    public class SecondModule : IModule
    {
        IUnityContainer _container;
        public SecondModule(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            Panel secondRegion = _container.Resolve<Panel>("SecondRegion");
            AnotherView view = _container.Resolve<AnotherView>();
            view.Dock = DockStyle.Fill;
            secondRegion.Controls.Add(view);
        }
    }
}
