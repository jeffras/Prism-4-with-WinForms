using System.Windows.Forms;
using Common.dto;
using Common.Events;
using Microsoft.Practices.Prism.Events;

namespace SecondPrismModule
{
    public partial class AnotherView : UserControl
    {
        AnotherView_ViewModel _viewModel;
        private IEventAggregator _eventAggregator;
        public AnotherView(AnotherView_ViewModel viewModel, IEventAggregator eventAggregator)
        {
            _viewModel = viewModel;
            _eventAggregator = eventAggregator;
            InitializeComponent();
            _eventAggregator.GetEvent<ProductSavedEvent>().Subscribe(OnProductSaved, ThreadOption.SubscriberAffinityThread);
            btnAddItem.DataBindings.Add("Enabled", _viewModel, "CanCreate");
        }

        private void OnProductSaved(Product p)
        {
            this.label1.Text = p.Name + " " + p.ProductId.ToString();
        }

        private void btnAddItem_Click(object sender, System.EventArgs e)
        {
             _eventAggregator.GetEvent<CreateProductEvent>().Publish(new Product(){Name = itemNameTB.Text});
        }

        private void itemNameTB_TextChanged(object sender, System.EventArgs e)
        {
            _eventAggregator.GetEvent<ValidateProductEvent>().Publish(new Product() { Name = itemNameTB.Text });
        }
    }
}
