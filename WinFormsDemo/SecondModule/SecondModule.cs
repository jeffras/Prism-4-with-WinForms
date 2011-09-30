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
        IUnityContainer m_Container;
        private IEventAggregator _eventAggregator;
        public SecondModule(IUnityContainer container, IEventAggregator evtAggregator)
        {
            m_Container = container;
            _eventAggregator = evtAggregator;
        }

        public void Initialize()
        {
            Panel secondRegion = m_Container.Resolve<Panel>("SecondRegion");
            AnotherView view = m_Container.Resolve<AnotherView>();
            view.Dock = DockStyle.Fill;
            secondRegion.Controls.Add(view);
        }
    }
}
