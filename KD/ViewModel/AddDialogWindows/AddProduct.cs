using KD.Command;
using KD.Model;
using KD.Model.ModelsForAdd;
using KD.View.NotificationMessages;
using KD.ViewModel.Common;
using System;

namespace KD.ViewModel.AddDialogWindows
{
    public class AddProduct : NotificationMessage
    {
        private ApplicationMain _applicationMain;
        private Manager _manager;
        private ModelOfAddProduct _model;

        public string NumberProduct { get { return _model.NumberProduct; } set { _model.NumberProduct = value; } }
        public string NameProduct { get { return _model.NameProduct; } set { _model.NameProduct = value; } }
        public bool ProgressIsIndeterminate { get { return _model.ProgressIsIndeterminate; } set { _model.ProgressIsIndeterminate = value; } }

        public RelayCommand AddCommand { get { return new RelayCommand((obj) => { Add(); }); } }
        public RelayCommand CloseCommand { get { return new RelayCommand((obj) => { MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null); _model.CleaningFields(); }); } }

        public AddProduct(ApplicationMain applicationMain, Manager manager)
        {
            Title = "Добавление изделия";
            this._applicationMain = applicationMain;
            this._manager = manager;

            this._model = new ModelOfAddProduct();
            _model.PropertyChanged += (s, e) => { OnPropertyChanged(e.PropertyName); };
        }

        private async void Add()
        {
            try
            {
                await _model.AddProductAsync();
                _manager.ModelOfPageProduct.GetProducts();
                MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null);
                _applicationMain.DialogWindowShow(new InfoNotificationMessage(String.Format("Изделие {0} {1} добавлено!!!", NumberProduct, NameProduct)), "MainWindow");
                _model.CleaningFields();

            }
            catch (Exception ex) { _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "AddProduct"); }
            finally { ProgressIsIndeterminate = false; }
        }
    }
}
