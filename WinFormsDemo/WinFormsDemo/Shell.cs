using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Events;
using Common.dto;
using Microsoft.Practices.Prism.Events;
using System.Threading;
using System.Diagnostics;
using Microsoft.Practices.Unity;

namespace WinFormsDemo
{
    public partial class Shell : Form
    {
        public Shell(IUnityContainer container, IEventAggregator evtAggregator)
        {
            InitializeComponent();
            container.RegisterInstance<Panel>("MainRegion", m_MainRegionPanel);
            container.RegisterInstance<Panel>("SecondRegion", m_SecondRegionPanel);
            evtAggregator.GetEvent<ProductSavedEvent>().Subscribe(OnProductSaved,ThreadOption.SubscriberAffinityThread);
        }

        void OnProductSaved(Product p)
        {
            m_StatusLabel.Text = p.Name + " saved"; 
        }
    }
}
