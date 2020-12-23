using KD.Data;
using KD.Model.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace KD.Model.PageModel
{
    public class ModelOfPageProduct : ModelBase
    {
        private DataManager _dataManager;

        public ProductModel SelectedProduct { get; set; }
        public ObservableCollection<ProductModel> Products;

        public ModelOfPageProduct()
        {
            this._dataManager = new DataManager();

            Products = new ObservableCollection<ProductModel>();
            GetProducts();
        }

        public void GetProducts()
        {
            Products.Clear();
            List<Product> result = _dataManager.GetProducts();

            foreach (var product in result) { Products.Add(new ProductModel(product)); }
        }

        public async void SearchProduct(string txt, string selectedComboBox)
        {
            Products.Clear();
            List<Product> result = await _dataManager.GetProductsAsync();
            result = selectedComboBox == "По наименованию" ? result.Where(p => p.Name.ToLower().Contains(txt.ToLower())).ToList() : result.Where(p => p.Number.ToLower().Contains(txt.ToLower())).ToList();

            foreach (var product in result) { Products.Add(new ProductModel(product)); }
        }
    }
}
