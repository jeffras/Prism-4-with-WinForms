using System;
using System.Windows.Forms;
using Common;
using Common.dto;

namespace FirstPrismModule
{
    public partial class ModuleView : UserControl
    {
        ModuleView_ViewModel _viewModel;
        private CommandProxy _commandProxy;
        public ModuleView(ModuleView_ViewModel viewModel, CommandProxy commandProxy)
        {
            _viewModel = viewModel;
            _commandProxy = commandProxy;

            InitializeComponent();
            ProductsBindingSource.DataSource = _viewModel;
            ProductsCB.DataBindings.Add("SelectedIndex", _viewModel, "SelectedProductIndex");
        }

        private void OnSaveSelection(object sender, EventArgs e)
        {
            _viewModel.SaveSelection(ProductsBindingSource.Current as Product);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _commandProxy.SaveAllCommand.Execute(ProductsBindingSource.List);
        }
    }
}
