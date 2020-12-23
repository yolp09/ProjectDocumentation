
using KD.Command;
using KD.Model;
using KD.Model.PageModel;
using KD.ViewModel;
using KD.ViewModel.Common;
using System;

namespace KD.View.NotificationMessages
{
    public class DialogInfoNotificationMessageExit : NotificationMessage
    {
        private ApplicationMain _applicationMain;

        public RelayCommand DialogInfoCommandExit { get { return new RelayCommand((obj) => { Exit(obj); }); } }

        public DialogInfoNotificationMessageExit(string msg, ApplicationMain applicationMain)
        {
            Title = "Сообщение";
            Message = msg;
            this._applicationMain = applicationMain;
        }

        private void Exit(object obj)
        {
            bool? result = obj as bool?;
            if (result != null && result.Value)
            {
                var _dir = System.IO.Path.GetTempPath();
                string _del = @"*.pdf";
                string[] _files = System.IO.Directory.GetFiles(_dir, _del);
                try
                {
                    foreach (string fl in _files)
                    {
                        System.IO.File.Delete(fl);
                    }
                    System.Windows.Application.Current.Shutdown();
                }
                catch
                {
                    _applicationMain.DialogWindowShow(new ErrorNotificationMessage("Закройте все .pdf файлы!!!"), "Exit");
                }
            }
        }
    }

    public class DINMExitConnection : NotificationMessage
    {
        private ApplicationMain _applicationMain;
        private MainWindowViewModel _mainWibdowViewModel;

        public RelayCommand DialogInfoCommandExit { get { return new RelayCommand((obj) => { ExitConnection(obj); }); } }

        public DINMExitConnection(string msg, ApplicationMain applicationMain, MainWindowViewModel mainWindowViewModel)
        {
            Title = "Сообщение";
            Message = msg;
            this._applicationMain = applicationMain;
            this._mainWibdowViewModel = mainWindowViewModel;
        }

        private void ExitConnection(object obj)
        {
            bool? result = obj as bool?;
            if (result != null && result.Value)
            {
                App.ConnectionOpen = false; _mainWibdowViewModel.IsEnabledSaveUser = false; Properties.Settings.Default.SettingIsSave = false;
                _applicationMain.Clear();
                _applicationMain.CurrentPage = new System.Windows.Controls.Page();
                _mainWibdowViewModel.Header = String.Format("Учет КД (без подключения)");
                _mainWibdowViewModel.MenuUpdate();
                Properties.Settings.Default.SettingPassword = String.Empty;
                Properties.Settings.Default.Save();
                MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null);
            }
        }
    }

    public class DINMApplicabilityDelete : NotificationMessage
    {
        private Manager _manager;
        private ApplicabilitySBOrNotSb _applicabilitySBOrNotSb;
        private TypeDetail _typeDetail;
        private DetailModel _model;

        public RelayCommand DialogInfoCommandApplicability { get { return new RelayCommand((obj) => { bool? result = obj as bool?; if (result != null && result.Value) { ApplicabilityDelete(); } }); } }

        public DINMApplicabilityDelete(string msg, Manager manager, ApplicabilitySBOrNotSb applicabilitySBOrNotSb, TypeDetail typeDetail, DetailModel model = null)
        {
            this._manager = manager;
            this._applicabilitySBOrNotSb = applicabilitySBOrNotSb;
            this._typeDetail = typeDetail;
            this._model = model;
            Title = "Сообщение";
            Message = msg;
        }

        private void ApplicabilityDelete()
        {
            if (_model != null) { _model.DeleteApplicability(_applicabilitySBOrNotSb); }
            else
            {
                switch (_typeDetail)
                {
                    case TypeDetail.Product: { _manager.ModelOfPageDetailsInProduct.DeleteApplicabilityDetail(_applicabilitySBOrNotSb); break; }
                    case TypeDetail.Detail: { _manager.ModelOfPageDetails.DeleteApplicabilityDetail(_applicabilitySBOrNotSb); break; }
                    case TypeDetail.Document: { _manager.ModelOfPageDocuments.DeleteApplicabilityDetail(_applicabilitySBOrNotSb); break; }
                    case TypeDetail.AssemblyUnit: { _manager.ModelOfPageAssemblyUnits.DeleteApplicabilityDetail(_applicabilitySBOrNotSb); break; }
                    case TypeDetail.StandardProduct: { _manager.ModelOfPageStandardProducts.DeleteApplicabilityDetail(_applicabilitySBOrNotSb); break; }
                    case TypeDetail.OthresProduct: { _manager.ModelOfPageOtherProducts.DeleteApplicabilityDetail(_applicabilitySBOrNotSb); break; }
                    case TypeDetail.Complex: { _manager.ModelOfPageComplexs.DeleteApplicabilityDetail(_applicabilitySBOrNotSb); break; }
                    case TypeDetail.Komplect: { _manager.ModelOfPageKomplekts.DeleteApplicabilityDetail(_applicabilitySBOrNotSb); break; }
                    case TypeDetail.Material: { _manager.ModelOfPageMaterials.DeleteApplicabilityDetail(_applicabilitySBOrNotSb); break; }
                }
            }
            if (_model != null || _typeDetail == TypeDetail.Product) { _manager.CollectionUpdate(_typeDetail); }
            MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }

    public class DINApplicabilityNoticeDelete_Notice : NotificationMessage
    {
        private Manager _manager;
        private ApplicabilityN _appN;
        private NoticeModel _noticeModel;

        public RelayCommand DialogInfoCommandDeleteApplicabilityNotice { get { return new RelayCommand((obj) => { bool? result = obj as bool?; if (result != null && result.Value) { ApplicabilityNoticeDelete(); } }); } }

        public DINApplicabilityNoticeDelete_Notice(string msg, Manager manager, ApplicabilityN appN, NoticeModel noticeModel = null)
        {
            this._manager = manager;
            this._appN = appN;
            this._noticeModel = noticeModel;
            Title = "Сообщение";
            Message = msg;
        }

        private void ApplicabilityNoticeDelete()
        {
            if (_noticeModel != null) _noticeModel.DeleteApplicabilityNotice(_appN);
            else _manager.ModelOfPageNotice.DeleteApplicabilityNotice(_appN);

            _manager.ModelOfPageNotice.GetNotices();
            MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }

    public class DINApplicabilityNoticeDelete_Detail : NotificationMessage
    {
        private Manager _manager;
        private ApplicabilityN _appN;
        private DetailModel _detailModel;
        private TypeDetail _typeDetail;

        public RelayCommand DialogInfoCommandDeleteApplicabilityNotice { get { return new RelayCommand((obj) => { bool? result = obj as bool?; if (result != null && result.Value) { ApplicabilityNoticeDelete(); } }); } }

        public DINApplicabilityNoticeDelete_Detail(string msg, Manager manager, ApplicabilityN appN, TypeDetail typeDetail, DetailModel detailModel = null)
        {
            this._manager = manager;
            this._appN = appN;
            this._typeDetail = typeDetail;
            this._detailModel = detailModel;
            Title = "Сообщение";
            Message = msg;
        }

        private void ApplicabilityNoticeDelete()
        {
            if (_detailModel != null) _detailModel.DeleteApplicabilityNotice(_appN);
            else
            {
                switch (_typeDetail)
                {
                    case TypeDetail.Product: { _manager.ModelOfPageDetailsInProduct.DeleteApplicabilityNotice(_appN); break; }
                    case TypeDetail.Detail: { _manager.ModelOfPageDetails.DeleteApplicabilityNotice(_appN); break; }
                    case TypeDetail.Document: { _manager.ModelOfPageDocuments.DeleteApplicabilityNotice(_appN); break; }
                    case TypeDetail.AssemblyUnit: { _manager.ModelOfPageAssemblyUnits.DeleteApplicabilityNotice(_appN); break; }
                    case TypeDetail.StandardProduct: { _manager.ModelOfPageStandardProducts.DeleteApplicabilityNotice(_appN); break; }
                    case TypeDetail.OthresProduct: { _manager.ModelOfPageOtherProducts.DeleteApplicabilityNotice(_appN); break; }
                    case TypeDetail.Complex: { _manager.ModelOfPageComplexs.DeleteApplicabilityNotice(_appN); break; }
                    case TypeDetail.Komplect: { _manager.ModelOfPageKomplekts.DeleteApplicabilityNotice(_appN); break; }
                    case TypeDetail.Material: { _manager.ModelOfPageMaterials.DeleteApplicabilityNotice(_appN); break; }
                }
            }
            if (_detailModel != null || _typeDetail == TypeDetail.Product) { _manager.CollectionUpdate(_typeDetail); }
            _manager.ModelOfPageNotice.GetNotices();
            MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }

    public class DINMessageDeleteDetail : NotificationMessage
    {
        private Manager _manager;
        private int _idDetail;
        private TypeDetail _typeDetail;
        private ApplicationMain _applicationMain;

        public RelayCommand DialogInfoCommandDelete { get { return new RelayCommand((obj) => { bool? result = obj as bool?; if (result != null && result.Value) { DeleteDetail(); } }); } }

        public DINMessageDeleteDetail(string msg, Manager manager, int idDetail, TypeDetail typeDetail, ApplicationMain applicationMain = null)
        {
            this._manager = manager;
            this._idDetail = idDetail;
            this._typeDetail = typeDetail;
            this._applicationMain = applicationMain;
            Title = "Сообщение";
            Message = msg;
        }

        private void DeleteDetail()
        {
            switch (_typeDetail)
            {
                case TypeDetail.Detail: { _manager.ModelOfPageDetails.DeleteDetail(_idDetail); break; }
                case TypeDetail.Document: { _manager.ModelOfPageDocuments.DeleteDetail(_idDetail); break; }
                case TypeDetail.AssemblyUnit: { _manager.ModelOfPageAssemblyUnits.DeleteDetail(_idDetail); break; }
                case TypeDetail.StandardProduct: { _manager.ModelOfPageStandardProducts.DeleteDetail(_idDetail); break; }
                case TypeDetail.OthresProduct: { _manager.ModelOfPageOtherProducts.DeleteDetail(_idDetail); break; }
                case TypeDetail.Complex: { _manager.ModelOfPageComplexs.DeleteDetail(_idDetail); break; }
                case TypeDetail.Komplect: { _manager.ModelOfPageKomplekts.DeleteDetail(_idDetail); break; }
                case TypeDetail.Material: { _manager.ModelOfPageMaterials.DeleteDetail(_idDetail); break; }
            }
            MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null);
            if (_applicationMain != null) _applicationMain.BackPage();
        }
    }

    public class DINMessageDeleteNotice : NotificationMessage
    {
        private Manager _manager;
        private int _idNotice;
        private ApplicationMain _applicationMain;

        public RelayCommand DialogInfoCommandDelete { get { return new RelayCommand((obj) => { bool? result = obj as bool?; if (result != null && result.Value) { DeleteNotice(); } }); } }

        public DINMessageDeleteNotice(string msg, Manager manager, int idNotice, ApplicationMain applicationMain = null)
        {
            this._manager = manager;
            this._idNotice = idNotice;
            this._applicationMain = applicationMain;
            Title = "Сообщение";
            Message = msg;
        }

        private void DeleteNotice()
        {
            _manager.ModelOfPageNotice.DeleteNotice(_idNotice); 
            MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null);
            if (_applicationMain != null) _applicationMain.BackPage();
        }
    }
}
