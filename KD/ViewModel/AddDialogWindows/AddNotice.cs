using KD.Command;
using KD.Model;
using KD.Model.Common;
using KD.Model.ModelsForAdd;
using KD.Model.PageModel;
using KD.View.NotificationMessages;
using KD.ViewModel.Common;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;

namespace KD.ViewModel.AddDialogWindows
{
    public class AddNotice : NotificationMessage
    {
        private ModelOfAddNotice _model;
        private Manager _manager;
        private ApplicationMain _applicationMain;

        public string UploadedFileInformation { get { return _model.UploadedFileInformation; } set { _model.UploadedFileInformation = value; } }
        public string NumberNotice { get { return _model.NumberNotice; } set { _model.NumberNotice = value; } }
        public string Developer { get { return _model.Developer; } set { _model.Developer = value; } }
        public string ChangeCode { get { return _model.ChangeCode; } set { _model.ChangeCode = value; } }
        public int? SelectedIdSection { set { _model.SelectedSection = value; } }
        public int? SelectedIdProduct { set { _model.SelectedProducts = value; } }
        public int? SelectedIdDetail { set { _model.SelectedDetail = value; } }
        public int? CountSheets { get { return _model.CountSheets; } set { _model.CountSheets = value; } }
        public bool ProgressIsIndeterminateFileDetail { get { return _model.ProgressIsIndeterminateFileDetail; } set { _model.ProgressIsIndeterminateFileDetail = value; } }
        public bool ProgressIsIndeterminateFile { get { return _model.ProgressIsIndeterminateFile; } set { _model.ProgressIsIndeterminateFile = value; } }
        public bool ProgressIsIndeterminate { get { return _model.ProgressIsIndeterminate; } set { _model.ProgressIsIndeterminate = value; OnPropertyChanged("ProgressIsIndeterminate"); } }
        public DateTime? DateNotice { get { return _model.DateNotice; } set { _model.DateNotice = value; } }
        public DetailModel SelectNoticeDetail { get { return _model.SelectNoticeDetail; } set { _model.SelectNoticeDetail = value; } }
        public ObservableCollection<DetailModel> NoticeDetails { get { return _model.NoticeDetails; } set { _model.NoticeDetails = value; } }
        public ReadOnlyObservableCollection<ModelComboBox> ComboSections { get; set; }
        public ObservableCollection<ModelComboBox> ComboProducts { get { return _model.ComboProducts; } set { _model.ComboProducts = value; } }
        public ObservableCollection<ModelComboBox> ComboDetails { get { return _model.ComboDetails; } set { _model.ComboDetails = value; } }

        public RelayCommand PlusDetail { get { return new RelayCommand((obj) => { try { _model.PlusDetail(); } catch (Exception ex) { _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "AddNotice"); } }); } }
        public RelayCommand MinusDetail { get { return new RelayCommand((obj) => { _model.MinusDetail(); }); } }
        public RelayCommand AddScanDetail { get { return new RelayCommand((obj) => { ScanDetail(); }); } }
        public RelayCommand AddScanNotice { get { return new RelayCommand((obj) => { ScanNotice(); }); } }
        public RelayCommand AddCommand { get { return new RelayCommand((obj) => { Add(); }); } }
        public RelayCommand CloseCommand { get { return new RelayCommand((obj) => { MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null); _model.CleaningFields(); }); } }

        public AddNotice(ApplicationMain applicationMain, Manager manager)
        {
            this._manager = manager;
            this._applicationMain = applicationMain;

            this._model = new ModelOfAddNotice();
            _model.PropertyChanged += (s, e) => { OnPropertyChanged(e.PropertyName); };

            ComboSections = new ReadOnlyObservableCollection<ModelComboBox>(_model.ComboSectoins);
        }

        private async void ScanNotice()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Файлы изображений (*.bmp, *.jpg, *.jpeg, *.png, *.pdf)|*.bmp;*.jpg;*.jpeg;*.png;*.pdf";
                if (ofd.ShowDialog().Value) { if (ofd.FileName != null) { await _model.AddFileNoticeAsync(ofd.FileName); } }
            }
            catch (Exception ex)
            {
                _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "AddNotice");
                ProgressIsIndeterminateFileDetail = false;
            }
        }

        private async void Add()
        {
            try
            {
                await _model.AddNoticeAssync();
                _manager.ModelOfPageNotice.GetNotices();
                _manager.CollectionUpdate(TypeDetail.Product);
                if (_manager.ModelOfPageDetailsInProduct != null) _manager.ModelOfPageDetailsInProduct.GetDetail();
                MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null);
                _applicationMain.DialogWindowShow(new InfoNotificationMessage(String.Format("Извещение {0} добавлено!!!", NumberNotice)), "MainWindow");
                _model.CleaningFields();
            }
            catch (Exception ex) { _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "AddNotice"); }
            finally { ProgressIsIndeterminate = false; }
        }

        private async void ScanDetail()
        {
            try
            {
                if (SelectNoticeDetail == null) throw new Exception("Не выбрана деталь для добавления скана!!!");
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Файлы изображений (*.bmp, *.jpg, *.jpeg, *.png, *.pdf)|*.bmp;*.jpg;*.jpeg;*.png;*.pdf";
                if (ofd.ShowDialog().Value) { if (ofd.FileName != null) { await _model.AddFileDetailAsync(ofd.FileName); } }
            }
            catch (Exception ex)
            {
                _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "AddNotice");
                ProgressIsIndeterminateFileDetail = false;
            }
        }
    }
}
