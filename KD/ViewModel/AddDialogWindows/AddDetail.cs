using KD.Command;
using KD.Model;
using KD.Model.Common;
using KD.Model.ModelsForAdd;
using KD.View.NotificationMessages;
using KD.ViewModel.Common;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace KD.ViewModel.AddDialogWindows
{
    public class AddDetail : NotificationMessage
    {
        private ModelOfAddDetail _model;
        private Manager _manager;
        private ApplicationMain _applicationMain;
        private TypeDetail _typeDetail;

        public string NumberDetail { get { return _model.NumberDetail; } set { _model.NumberDetail = value; } }
        public string NameDetail { get { return _model.NameDetail; } set { _model.NameDetail = value; } }
        public string Developer { get { return _model.Developer; } set { _model.Developer = value; } }
        public string UploadedFileInformation { get { return _model.UploadedFileInformation; } set { _model.UploadedFileInformation = value; } }
        public int? SelectedSection { set { _model.SelectedSection = value; } }
        public int? SelectedProducts { get { return _model.SelectedProducts; } set { _model.SelectedProducts = value; } }
        public bool ProgressIsIndeterminate { get { return _model.ProgressIsIndeterminate; } set { _model.ProgressIsIndeterminate = value; } }
        public bool ProgressIsIndeterminateFile { get { return _model.ProgressIsIndeterminateFile; } set { _model.ProgressIsIndeterminateFile = value; } }
        public bool CheckedScan { get { return _model.CheckedScan; } set { _model.CheckedScan = value; } }
        public Visibility VisibilityCheckBoxScan { get; set; }
        public Visibility VisibilityTextBlockScan { get; set; }
        public ObservableCollection<ModelComboBox> ComboProducts { get { return _model.ComboProducts; } set { _model.ComboProducts = value; } }
        public ReadOnlyObservableCollection<ModelComboBox> ComboSections { get; set; }

        public RelayCommand AddCommand { get { return new RelayCommand((obj) => { Add(); }); } }
        public RelayCommand AddScanCommand { get { return new RelayCommand((obj) => { AddScan(); }); } }
        public RelayCommand CloseCommand { get { return new RelayCommand((obj) => { MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null); _model.CleaningFields(); }); } }

        public AddDetail(ApplicationMain applicationMain, string title, TypeDetail typeDetail, Manager manager)
        {
            VisibilityCheckBoxScan = typeDetail == TypeDetail.Detail ? Visibility.Visible : Visibility.Collapsed;
            VisibilityTextBlockScan = typeDetail == TypeDetail.Detail ? Visibility.Collapsed : Visibility.Visible;
            Title = title;
            this._applicationMain = applicationMain;
            this._typeDetail = typeDetail;
            this._manager = manager;

            this._model = new ModelOfAddDetail();
            _model.PropertyChanged += (s, e) => { OnPropertyChanged(e.PropertyName); };

            ComboSections = new ReadOnlyObservableCollection<ModelComboBox>(_model.ComboSectoins);
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
                _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "AddDetail");
                ProgressIsIndeterminateFile = false;
                UploadedFileInformation = "";
            }
        }

        private async void Add()
        {
            try
            {
                await _model.AddDetailAssync(_typeDetail);
                if (_manager.ModelOfPageDetailsInProduct != null) _manager.ModelOfPageDetailsInProduct.GetDetail();
                _manager.CollectionUpdate(_typeDetail);
                if (_manager.ModelOfPageDetailsInProduct != null) _manager.ModelOfPageDetailsInProduct.GetDetail();
                MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null);
                _applicationMain.DialogWindowShow(new InfoNotificationMessage(String.Format("{0} {1} добавлено!!!", NumberDetail, NameDetail)), "MainWindow");
                _model.CleaningFields();
            }
            catch (Exception ex) { _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "AddDetail"); }
            finally { ProgressIsIndeterminate = false; }
        }
    }
}
