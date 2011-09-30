using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Common;
using Common.Events;
using Common.dto;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;

namespace FirstPrismModule
{
    public class ModuleView_ViewModel
    {
        IUnityContainer m_Container;
        private IEventAggregator _eventAggregator;
        private readonly CommandProxy _commandProxy;
        public ModuleView_ViewModel(IUnityContainer container, IEventAggregator evtAggregator, CommandProxy commandProxy)
        {
            m_Container = container;
            _eventAggregator = evtAggregator;
            _commandProxy = commandProxy;
            this.SaveCommand = new DelegateCommand<object>(this.Save, this.CanSave);
            commandProxy.SaveAllCommand.RegisterCommand(SaveCommand);
        }

        public event EventHandler<DataEventArgs<ModuleView_ViewModel>> Saved;

        public DelegateCommand<object> SaveCommand { get; private set; }

        public List<Product> Products
        {
            get
            {
                return new List<Product>
            {
                new Product{ProductId=1,Name="Cheese"},
                new Product{ProductId=2,Name="Wine"}
            };
            }
        }

        public void SaveSelection(Product product)
        {
            // do async to show the thread marshalling
            Action doIt = delegate()
            {
                _eventAggregator.GetEvent<ProductSavedEvent>().Publish(product);
            };
            doIt.BeginInvoke(null, null);
        }

        private bool CanSave(object arg)
        {
            return true;
        }

        private void Save(object obj)
        {
            MessageBox.Show("Save");

            // Notify that the order was saved.
            this.OnSaved(new DataEventArgs<ModuleView_ViewModel>(this));
        }

        private void OnSaved(DataEventArgs<ModuleView_ViewModel> e)
        {
            EventHandler<DataEventArgs<ModuleView_ViewModel>> savedHandler = this.Saved;
            if (savedHandler != null)
            {
                savedHandler(this, e);
            }
        }
    }
}
