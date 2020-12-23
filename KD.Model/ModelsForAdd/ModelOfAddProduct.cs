using KD.Data;
using KD.Model.Common;
using System;
using System.Threading.Tasks;

namespace KD.Model.ModelsForAdd
{
    public class ModelOfAddProduct : ModelBase
    {
        private DataManager _dataManager;
        private bool _progressIsIndeterminate;

        public string NumberProduct { get; set; }
        public string NameProduct { get; set; }
        public bool ProgressIsIndeterminate { get { return _progressIsIndeterminate; } set { _progressIsIndeterminate = value; OnPropertyChanged("ProgressIsIndeterminate"); } }

        public ModelOfAddProduct()
        {
            this._dataManager = new DataManager();
        }

        public Task AddProductAsync()
        {
            return Task.Run(() => AddProduct());
        }

        public void CleaningFields()
        {
            NumberProduct = String.Empty; OnPropertyChanged("NumberProduct");
            NameProduct = String.Empty; OnPropertyChanged("NameProduct");
        }

        private void AddProduct()
        {
            ProgressIsIndeterminate = true;
            if (String.IsNullOrWhiteSpace(NumberProduct)) throw new Exception("Обозначение изделия не может быть пустой строкой или строкой, состоящей только из пробельных символов");
            if (String.IsNullOrWhiteSpace(NameProduct)) throw new Exception("Наменование изделия не может быть пустой строкой или строкой, состоящей только из пробельных символов");
            _dataManager.AddProduct(NumberProduct, NameProduct);
            ProgressIsIndeterminate = false;
        }
    }
}
