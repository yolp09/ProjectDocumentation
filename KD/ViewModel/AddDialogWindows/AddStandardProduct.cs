using KD.Command;
using KD.Model;
using KD.Model.ModelsForAdd;
using KD.View.NotificationMessages;
using KD.ViewModel.Common;
using Microsoft.Win32;
using System;
using System.Windows;

namespace KD.ViewModel.AddDialogWindows
{
    public class AddStandardProduct : NotificationMessage
    {
        private ApplicationMain _applicationMain;
        private Manager _manager;
        private TypeDetail _typeDetail;
        private ModelOfAddStandardProduct _model;

        public string NameProduct { get { return _model.NameProduct; } set { _model.NameProduct = value; } }
        public string UploadedFileInformation { get { return _model.UploadedFileInformation; } set { _model.UploadedFileInformation = value; } }
        public bool ProgressIsIndeterminate { get { return _model.ProgressIsIndeterminate; } set { _model.ProgressIsIndeterminate = value; } }
        public bool ProgressIsIndeterminateFile { get { return _model.ProgressIsIndeterminateFile; } set { _model.ProgressIsIndeterminateFile = value; } }
        public bool CheckedScan { get { return _model.CheckedScan; } set { _model.CheckedScan = value; } }
        public Visibility VisibilityScan { get; set; }

        public RelayCommand AddCommand { get { return new RelayCommand((obj) => { Add(); }); } }
        public RelayCommand CloseCommand { get { return new RelayCommand((obj) => { MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null); _model.CleaningFields(); }); } }
        public RelayCommand AddScanCommand { get { return new RelayCommand((obj) => { AddScan(); }); } }

        public AddStandardProduct(ApplicationMain applicationMain, string title, TypeDetail typeDetail, Manager manager)
        {
            VisibilityScan = typeDetail == TypeDetail.OthresProduct ? Visibility.Visible : Visibility.Collapsed;
            Title = title;
            this._applicationMain = applicationMain;
            this._manager = manager;
            this._typeDetail = typeDetail;

            this._model = new ModelOfAddStandardProduct();
            _model.PropertyChanged += (s, e) => { OnPropertyChanged(e.PropertyName); };
        }

        private async void AddScan()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Файлы изображений (*.bmp, *.jpg, *.jpeg, *.png, *.pdf)|*.bmp;*.jpg;*.jpeg;*.png;*.pdf";
                if (ofd.ShowDialog().Value) { if (ofd.FileName != null) { await _model.AddFileAsync(ofd.FileName); } }
            }
            catch (Exception ex)
            {
                _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "AddStandardProduct");
                ProgressIsIndeterminateFile = false;
                UploadedFileInformation = "";
            }
        }

        private async void Add()
        {
            try
            {
                await _model.AddStandardProductAsync(_typeDetail);
                _manager.CollectionUpdate(_typeDetail);
                if (_manager.ModelOfPageDetailsInProduct != null) _manager.ModelOfPageDetailsInProduct.GetDetail();
                MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null);
                _applicationMain.DialogWindowShow(new InfoNotificationMessage(String.Format("{0} добавлено!!!", NameProduct)), "MainWindow");
                _model.CleaningFields();
            }
            catch (Exception ex) { _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "AddStandardProduct"); }
            finally { ProgressIsIndeterminate = false; }
        }
    }
}
