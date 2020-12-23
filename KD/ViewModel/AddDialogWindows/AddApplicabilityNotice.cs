using KD.Command;
using KD.Model;
using KD.Model.Common;
using KD.Model.ModelsForAdd;
using KD.Model.PageModel;
using KD.View.NotificationMessages;
using KD.View.Pages;
using KD.ViewModel.Common;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;

namespace KD.ViewModel.AddDialogWindows
{
    public class AddApplicabilityNotice : NotificationMessage
    {
        private ModelOfAddApplicabilityNotice _model;
        private Manager _manager;
        private ApplicationMain _applicationMain;
        private NoticeModel _noticeModel;
        private int _idNotice;

        public int? SelectedIdSection { set { _model.SelectedSection = value; } }
        public int? SelectedIdProduct { set { _model.SelectedProducts = value; } }
        public int? SelectedIdDetail { set { _model.SelectedDetail = value; } }
        public bool ProgressIsIndeterminateFileDetail { get { return _model.ProgressIsIndeterminateFileDetail; } set { _model.ProgressIsIndeterminateFileDetail = value; } }
        public bool ProgressIsIndeterminate { get { return _model.ProgressIsIndeterminate; } set { _model.ProgressIsIndeterminate = value; } }
        public DetailModel SelectNoticeDetail { get { return _model.SelectNoticeDetail; } set { _model.SelectNoticeDetail = value; } }
        public ObservableCollection<DetailModel> NoticeDetails { get { return _model.NoticeDetails; } set { _model.NoticeDetails = value; } }
        public ReadOnlyObservableCollection<ModelComboBox> ComboSections { get; set; }
        public ObservableCollection<ModelComboBox> ComboProducts { get { return _model.ComboProducts; } set { _model.ComboProducts = value; } }
        public ObservableCollection<ModelComboBox> ComboDetails { get { return _model.ComboDetails; } set { _model.ComboDetails = value; } }

        public RelayCommand PlusDetail { get { return new RelayCommand((obj) => { try { _model.PlusDetail(); } catch (Exception ex) { _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "AddApplicabilityNotice"); } }); } }
        public RelayCommand MinusDetail { get { return new RelayCommand((obj) => { _model.MinusDetail(); }); } }
        public RelayCommand AddScanDetail { get { return new RelayCommand((obj) => { ScanDetail(); }); } }
        public RelayCommand CloseCommand { get { return new RelayCommand((obj) => { MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null); }); } }
        public RelayCommand AddCommand { get { return new RelayCommand((obj) => { Add(); }); } }

        public AddApplicabilityNotice(ApplicationMain applicationMain, Manager manager, int idNotice, NoticeModel noticeModel = null)
        {
            this._applicationMain = applicationMain;
            this._manager = manager;
            this._idNotice = idNotice;
            this._noticeModel = noticeModel;

            this._model = new ModelOfAddApplicabilityNotice(idNotice);
            _model.PropertyChanged += (s, e) => { OnPropertyChanged(e.PropertyName); };

            ComboSections = new ReadOnlyObservableCollection<ModelComboBox>(_model.ComboSectoins);
            Title = _model.Title;
        }

        private async void Add()
        {
            try
            {
                await _model.AddAssync();
                _manager.ModelOfPageNotice.GetNotices();
                if (_noticeModel != null) _applicationMain.CurrentPage = new PageNoticeInfo(_idNotice, _manager, _applicationMain);
                _applicationMain.DialogWindowShow(new InfoNotificationMessage(String.Format("Применяемость добавлена!!!")), "AddApplicabilityNotice");
            }
            catch (Exception ex) { _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "AddApplicabilityNotice"); }
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
                _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "AddApplicabilityNotice");
                ProgressIsIndeterminateFileDetail = false;
            }
        }
    }
}
