using KD.Command;
using KD.Model;
using KD.Model.ModelsForAdd;
using KD.Model.PageModel;
using KD.View.NotificationMessages;
using KD.View.Pages;
using KD.ViewModel.Common;
using Microsoft.Win32;
using System;

namespace KD.ViewModel.AddDialogWindows
{
    public class ChangeNotice : NotificationMessage
    {
        private ModelOfChangeNotice _model;
        private Manager _manager;
        private ApplicationMain _applicationMain;
        private NoticeModel _noticeModel;

        public string NumberNotice { get { return _model.NumberNotice; } set { _model.NumberNotice = value; } }
        public string Developer { get { return _model.Developer; } set { _model.Developer = value; } }
        public string ChangeCode { get { return _model.ChangeCode; } set { _model.ChangeCode = value; } }
        public string UploadedFileInformation { get { return _model.UploadedFileInformation; } set { _model.UploadedFileInformation = value; } }
        public int? CountSheets { get { return _model.CountSheets; } set { _model.CountSheets = value; } }
        public bool ProgressIsIndeterminate { get { return _model.ProgressIsIndeterminate; } set { _model.ProgressIsIndeterminate = value; } }
        public bool ProgressIsIndeterminateFile { get { return _model.ProgressIsIndeterminateFile; } set { _model.ProgressIsIndeterminateFile = value; } }
        public DateTime? DateNotice { get { return _model.DateNotice; } set { _model.DateNotice = value; } }

        public RelayCommand ChangeCommand { get { return new RelayCommand((obj) => { Change(); }); } }
        public RelayCommand AddScanNotice { get { return new RelayCommand((obj) => { AddScan(); }); } }
        public RelayCommand CloseCommand { get { return new RelayCommand((obj) => { MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null); }); } }

        public ChangeNotice(ApplicationMain applicationMain, Manager manager, int idNotice, NoticeModel noticeModel = null)
        {
            this._applicationMain = applicationMain;
            this._manager = manager;
            this._noticeModel = noticeModel;

            this._model = new ModelOfChangeNotice(idNotice);
            _model.PropertyChanged += (s, e) => { OnPropertyChanged(e.PropertyName); };
            Title = _model.Title;
        }

        private async void Change()
        {
            try
            {
                string result = await _model.ChangeNoticeAssync();
                _manager.ModelOfPageNotice.GetNotices();
                if (_noticeModel != null) _applicationMain.CurrentPage = new PageNoticeInfo(_noticeModel.Id, _manager, _applicationMain);
                if (result != String.Empty) _applicationMain.DialogWindowShow(new InfoNotificationMessage(result), "ChangeNotice");
            }
            catch (Exception ex) { _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "ChangeNotice"); }
            finally { ProgressIsIndeterminate = false; }
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
                _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "ChangeNotice");
                ProgressIsIndeterminateFile = false;
                UploadedFileInformation = "";
            }
        }
    }
}
