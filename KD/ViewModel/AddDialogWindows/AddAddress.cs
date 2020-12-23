using KD.Command;
using KD.Model;
using KD.Model.ModelsForAdd;
using KD.Model.PageModel;
using KD.View.NotificationMessages;
using KD.View.Pages;
using KD.ViewModel.Common;
using System;

namespace KD.ViewModel.AddDialogWindows
{
    public class AddAddress : NotificationMessage
    {
        private ApplicationMain _applicationMain;
        private ModelOfAddAddress _model;
        private Manager _manager;
        private DetailModel _detailModel;
        private NoticeModel _noticeModel;
        private TypeDetail _typeDetail;
        private bool _isDetail;

        public string NumberAndNameDetail { get { return _model.NumberAndNameDetail; } set { _model.NumberAndNameDetail = value; } }
        public string Address { get { return _model.Address; } set { _model.Address = value; } }
        public bool CheckedSubscription { get { return _model.CheckedSubscription; } set { _model.CheckedSubscription = value; } }
        public bool ProgressIsIndeterminate { get { return _model.ProgressIsIndeterminate; } set { _model.ProgressIsIndeterminate = value; } }

        public RelayCommand AddCommand { get { return new RelayCommand((obj) => { Add(); }); } }
        public RelayCommand CloseCommand { get { return new RelayCommand((obj) => { MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null); }); } }

        public AddAddress(ApplicationMain applicationMain, Manager manager, int idDetail, bool isDetail, TypeDetail typeDetail, DetailModel detailModel = null, NoticeModel noticeModel = null)
        {
            Title = "Добавление адресата";
            this._applicationMain = applicationMain;
            this._manager = manager;
            this._isDetail = isDetail;
            this._typeDetail = typeDetail;
            this._detailModel = detailModel;
            this._noticeModel = noticeModel;

            this._model = new ModelOfAddAddress(idDetail, isDetail, _manager);
            _model.PropertyChanged += (s, e) => { OnPropertyChanged(e.PropertyName); };
        }

        private async void Add()
        {
            try
            {
                await _model.AddAddressAsync();
                if (_isDetail) { _manager.CollectionUpdate(_typeDetail); } else { _manager.ModelOfPageNotice.GetNotices(); }
                if (_manager.ModelOfPageDetailsInProduct != null) _manager.ModelOfPageDetailsInProduct.GetDetail();
                if (_detailModel != null) _applicationMain.CurrentPage = new PageDetailInfo(_detailModel.Id, _manager, _applicationMain);
                if (_noticeModel != null) _applicationMain.CurrentPage = new PageNoticeInfo(_noticeModel.Id, _manager, _applicationMain);

                _applicationMain.DialogWindowShow(new InfoNotificationMessage("Адрес добавлен!!!"), "AddAddress");
            }
            catch (Exception ex) { _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "AddAddress"); }
            finally { ProgressIsIndeterminate = false; }
        }
    }
}
