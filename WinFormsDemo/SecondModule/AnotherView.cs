using System.Windows.Forms;
using Common.dto;
using Common.Events;
using Microsoft.Practices.Prism.Events;

namespace SecondPrismModule
{
    public partial class AnotherView : UserControl
    {
        private IEventAggregator _eventAggregator;
        public AnotherView(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            InitializeComponent();
            eventAggregator.GetEvent<ProductSavedEvent>().Subscribe(OnProductSaved, ThreadOption.SubscriberAffinityThread);
        }

        private void OnProductSaved(Product p)
        {
            this.label1.Text = p.Name + " " + p.ProductId.ToString();
        }
    }
}
