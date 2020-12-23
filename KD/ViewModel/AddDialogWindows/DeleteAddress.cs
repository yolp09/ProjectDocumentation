using KD.Command;
using KD.Model;
using KD.Model.ModelsForAdd;
using KD.Model.PageModel;
using KD.View.NotificationMessages;
using KD.View.Pages;
using KD.ViewModel.Common;
using System;
using System.Collections.ObjectModel;

namespace KD.ViewModel.AddDialogWindows
{
    public class DeleteAddress : NotificationMessage
    {
        private ApplicationMain _applicationMain;
        private ModelOfDeleteAddress _model;
        private Manager _manager;
        private TypeDetail _typeDetail;
        private int _idDetail;
        private bool _isDetail;
        private bool _isInfoPage;

        public ObservableCollection<AddressModel> Adresses { get { return _model.Adresses; } set { _model.Adresses = value; } }

        public RelayCommand DeleteCommand { get { return new RelayCommand((obj) => { Delete(); }); } }
        public RelayCommand CloseCommand { get { return new RelayCommand((obj) => { MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null); }); } }

        public DeleteAddress(ApplicationMain applicationMain, Manager manager, int idDetail, bool isDetail, TypeDetail typeDetail, bool isInfoPage)
        {
            this._applicationMain = applicationMain;
            this._manager = manager;
            this._idDetail = idDetail;
            this._isDetail = isDetail;
            this._typeDetail = typeDetail;
            this._isInfoPage = isInfoPage;

            this._model = new ModelOfDeleteAddress(idDetail, isDetail);
            _model.PropertyChanged += (s, e) => { OnPropertyChanged(e.PropertyName); };

            Title = "Адресаты детали";
        }

        private void Delete()
        {
            try
            {
                _model.DeleteAdress();
                MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null);
                if(_isDetail)
                {
                    _manager.CollectionUpdate(_typeDetail);
                    if (_isInfoPage) _applicationMain.CurrentPage = new PageDetailInfo(_idDetail, _manager, _applicationMain);
                }
                else
                {
                    _manager.ModelOfPageNotice.GetNotices();
                    if (_isInfoPage) _applicationMain.CurrentPage = new PageNoticeInfo(_idDetail, _manager, _applicationMain);
                }
            }
            catch (Exception ex) { _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "DeleteAddress"); }
        }
    }
}
