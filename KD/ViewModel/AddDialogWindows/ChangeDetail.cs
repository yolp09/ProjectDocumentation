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
    public class ChangeDetail : NotificationMessage
    {
        private ModelOfChangeDetail _model;
        private Manager _manager;
        private ApplicationMain _applicationMain;
        private DetailModel _detailModel;
        private TypeDetail _typeDetail;

        public string NumberDetail { get { return _model.NumberDetail; } set { _model.NumberDetail = value; } }
        public string NameDetail { get { return _model.NameDetail; } set { _model.NameDetail = value; } }
        public string Developer { get { return _model.Developer; } set { _model.Developer = value; } }
        public string UploadedFileInformation { get { return _model.UploadedFileInformation; } set { _model.UploadedFileInformation = value; } }
        public bool ProgressIsIndeterminate { get { return _model.ProgressIsIndeterminate; } set { _model.ProgressIsIndeterminate = value; } }
        public bool ProgressIsIndeterminateFile { get { return _model.ProgressIsIndeterminateFile; } set { _model.ProgressIsIndeterminateFile = value; } }
        public bool CheckedScan { get { return _model.CheckedScan; } set { _model.CheckedScan = value; } }

        public RelayCommand ChangeCommand { get { return new RelayCommand((obj) => { Change(); }); } }
        public RelayCommand AddScanCommand { get { return new RelayCommand((obj) => { AddScan(); }); } }
        public RelayCommand CloseCommand { get { return new RelayCommand((obj) => { MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null); }); } }

        public ChangeDetail(ApplicationMain applicationMain, Manager manager, int idDetail, TypeDetail typeDetail, DetailModel detailModel = null)
        {
            this._applicationMain = applicationMain;
            this._manager = manager;
            this._typeDetail = typeDetail;
            this._detailModel = detailModel;

            this._model = new ModelOfChangeDetail(idDetail, _manager);
            _model.PropertyChanged += (s, e) => { OnPropertyChanged(e.PropertyName); };

            Title = _model.Title;
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
                _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "ChangeDetail");
                ProgressIsIndeterminateFile = false;
                UploadedFileInformation = "";
            }
        }

        private async void Change()
        {
            try
            {
                string result = await _model.ChangeDetailAssync();
                _manager.CollectionUpdate(_typeDetail);
                if (_manager.ModelOfPageDetailsInProduct != null) _manager.ModelOfPageDetailsInProduct.GetDetail();
                if (_detailModel != null) _applicationMain.CurrentPage = new PageDetailInfo(_detailModel.Id, _manager, _applicationMain);
                if (result != String.Empty) _applicationMain.DialogWindowShow(new InfoNotificationMessage(result), "ChangeDetail");
            }
            catch (Exception ex) { _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "ChangeDetail"); }
            finally { ProgressIsIndeterminate = false; }
        }
    }
}
