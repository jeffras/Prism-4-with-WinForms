using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common;
using Common.Events;
using Common.dto;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Unity;

namespace SecondPrismModule
{
    public class AnotherView_ViewModel : NotificationObject
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly CommandProxy _commandProxy;

        public AnotherView_ViewModel(IEventAggregator evtAggregator, CommandProxy commandProxy)
        {
            _eventAggregator = evtAggregator;
            _commandProxy = commandProxy;
            SaveCommand = new DelegateCommand<object>(Save, CanSave);
            _commandProxy.SaveAllCommand.RegisterCommand(SaveCommand);

            _eventAggregator.GetEvent<ValidateProductEvent>().Subscribe(ValidateProduct, ThreadOption.PublisherThread);
            _eventAggregator.GetEvent<CreateProductFailedEvent>().Subscribe(NotifyCreateProductFailed, ThreadOption.SubscriberAffinityThread);
        }

        public DelegateCommand<object> SaveCommand { get; private set; }

        private void NotifyCreateProductFailed(String s)
        {
            CanCreate = false;
            MessageBox.Show(s, "AnotherView");
        }

        private void ValidateProduct(Product p)
        {
            CanCreate = (p != null && p.Name.Length > 0);
        }

        private bool _canCreate = false;
        public bool CanCreate
        {
            get { return _canCreate; }
            set 
            { 
                _canCreate = value;
                RaisePropertyChanged(() => CanCreate);
            }
        }

        private bool CanSave(object arg)
        {
            return true;
        }

        private void Save(object obj)
        {
            foreach (Product product in (IList<Product>)obj)
            {
                if (product.ProductId != 1)
                    MessageBox.Show("I Saved " + product.Name, "AnotherView");
                else
                    MessageBox.Show("I can't save " + product.Name + "!!", "AnotherView");
            }
        }
    }
}
