using KD.Data;
using KD.Model.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace KD.Model.ModelsForAdd
{
    public class ModelOfAddApplicability : ModelBase
    {
        private DataManager _dataManager;
        private Manager _manager;
        private int? _selectedSection;
        private int _idDetail;
        private bool _progressIsIndeterminate;

        public string Title { get; set; }
        public int? SelectedSection { get { return _selectedSection; } set { _selectedSection = value; GetComboProducts(_selectedSection); } }
        public int? SelectedProducts { get; set; }
        public bool ProgressIsIndeterminate { get { return _progressIsIndeterminate; } set { _progressIsIndeterminate = value; OnPropertyChanged("ProgressIsIndeterminate"); } }
        public ObservableCollection<ModelComboBox> ComboProducts;
        public ObservableCollection<ModelComboBox> ComboSectoins;

        public ModelOfAddApplicability(int idDetail, Manager manager)
        {
            this._dataManager = new DataManager();
            this._idDetail = idDetail;
            this._manager = manager;

            ComboSectoins = new ObservableCollection<ModelComboBox>(){
                new ModelComboBox((int)TypeDetail.Product, "Изделия"),
                new ModelComboBox((int)TypeDetail.Complex, "Комплексы"),
                new ModelComboBox((int)TypeDetail.AssemblyUnit, "Сборочные единицы"),
                new ModelComboBox((int)TypeDetail.Komplect, "Комплекты")
            };
            ComboProducts = new ObservableCollection<ModelComboBox>();

            Detail detail = _dataManager.GetDetailById(idDetail);
            Title = String.Format("Добавление применяемости для {0} {1}", detail.Number, detail.Name);
        }

        public Task AddApplicabilityAssync()
        {
            return Task.Run(() => AddApplicability());
        }

        private void AddApplicability()
        {
            ProgressIsIndeterminate = true;
            if (SelectedProducts == null) throw new Exception("Не выбрана применяемость для детали!");
            _dataManager.AddApplicability(_idDetail, SelectedProducts.Value, SelectedSection.Value);
            ProgressIsIndeterminate = false;
        }

        private async void GetComboProducts(int? selected)
        {
            if (selected == null) return;

            ComboProducts.Clear();
            if (selected == 0)
            {
                List<Product> result = await _dataManager.GetProductsAsync();
                List<Product> products = _dataManager.GetApplicabilityProducts(_idDetail);
                foreach (var product in products) { result.Remove(product); }
                foreach (var product in result) { ComboProducts.Add(new ModelComboBox(product.Id, String.Format("{0} {1}", product.Number, product.Name))); }
            }
            else
            {
                List<Detail> result = await _dataManager.GetDetailsAsync(selected.Value);
                List<Detail> products = _dataManager.GetApplicabilityDetailSB(_idDetail);
                foreach (var product in products) { result.Remove(product); }
                foreach (var detail in result) { ComboProducts.Add(new ModelComboBox(detail.Id, String.Format("{0} {1}", detail.Number, detail.Name))); }
            }
        }
    }
}
