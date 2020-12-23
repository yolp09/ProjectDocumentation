using KD.Command;
using KD.Model;
using KD.Model.PageModel;
using KD.View.Pages;
using KD.ViewModel.Common;
using System.Collections.ObjectModel;

namespace KD.ViewModel
{
    public class ProductsViewModel : PageViewModelBase
    {
        private ApplicationMain _applicationMain;
        private Manager _manager;
        private ModelOfPageProduct _model;
        private string _serachText;

        public override string Title { get { return "Изделия"; } }
        public string SearchText { get { return _serachText; } set { _serachText = value; _model.SearchProduct(value, SelectedCombobox); OnPropertyChanged("SearchText"); } }
        public ProductModel SelectedProduct { get { return _model.SelectedProduct; } set { _model.SelectedProduct = value; OnPropertyChanged("SelectedProduct"); } }
        public ObservableCollection<ProductModel> Products { get { return _model.Products; } set { _model.Products = value; } }

        public RelayCommand DoubleClickCommand { get { return new RelayCommand((obj) => { DoubleClick(obj); }); } }

        public ProductsViewModel(ApplicationMain applicationMain, Manager manager)
        {
            this._applicationMain = applicationMain;
            this._manager = manager;

            _model = this._manager.ModelOfPageProduct;
            _model.PropertyChanged += (s, e) => { OnPropertyChanged(e.PropertyName); };

            ComboBoxItems.Add("По обозначению");
            ComboBoxItems.Add("По наименованию");
            SelectedCombobox = ComboBoxItems[0];
        }

        private void DoubleClick(object obj)
        {
            ProductModel product = obj as ProductModel;
            if (product != null)
            {
                _applicationMain.NextPage(new PageDetailsInProduct(_applicationMain, _manager, product.IdProduct, TypeDetail.Product));
            }
        }
    }
}
