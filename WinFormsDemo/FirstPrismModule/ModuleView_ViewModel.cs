using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;
using Common;
using Common.Events;
using Common.dto;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;

namespace FirstPrismModule
{
    public class ModuleView_ViewModel : NotificationObject
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly CommandProxy _commandProxy;
        public ModuleView_ViewModel(IEventAggregator evtAggregator, CommandProxy commandProxy)
        {
            _eventAggregator = evtAggregator;
            _commandProxy = commandProxy;

            SaveCommand = new DelegateCommand<object>(Save, CanSave);
            _commandProxy.SaveAllCommand.RegisterCommand(SaveCommand);
            
            _eventAggregator.GetEvent<CreateProductEvent>().Subscribe(CreateProduct, ThreadOption.SubscriberAffinityThread);
        }

        public void CreateProduct(Product p)
        {
            if (_products.Any(a => a.Name == p.Name))
            {
                _eventAggregator.GetEvent<CreateProductFailedEvent>().Publish("Product " + p.Name + " already exists!");
                return;
            }

            var productWithMaxId = _products.Aggregate((agg, next) => next.ProductId > agg.ProductId ? next : agg);
            p.ProductId = productWithMaxId.ProductId + 1;
            _products.Add(p);
            RaisePropertyChanged(() => Products);
            SelectedProductIndex = Products.IndexOf(Products.First(a => a.Name == p.Name));
        }

        public event EventHandler<DataEventArgs<ModuleView_ViewModel>> Saved;

        public DelegateCommand<object> SaveCommand { get; private set; }

        private BindingList<Product> _products = new BindingList<Product>()
        {
            new Product{ProductId=1,Name="Cheese"},
            new Product{ProductId=2,Name="Wine"}
        };

        public BindingList<Product> Products
        {
            get
            {
                return _products;
            }
            set 
            {
                _products = value;
                RaisePropertyChanged(() => Products);
            }
        }

        private int _selectedProductIndex;
        public int SelectedProductIndex { 
            get { return _selectedProductIndex; }
            set
            {
                _selectedProductIndex = value;
                RaisePropertyChanged(() => SelectedProductIndex); 
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
            foreach (Product product in (IList<Product>)obj)
            {
                if (product.ProductId == 1)
                    MessageBox.Show("Saved " + product.Name, "ModuleView");
                else
                    MessageBox.Show("Don't know how to save " + product.Name + "!!", "ModuleView");
            }
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
