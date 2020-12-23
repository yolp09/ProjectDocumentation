using KD.Command;
using KD.Model;
using KD.Model.PageModel;
using KD.View.NotificationMessages;
using KD.View.Pages;
using KD.ViewModel.AddDialogWindows;
using KD.ViewModel.Common;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace KD.ViewModel.Pages
{
    public class NoticeViewModel : PageViewModelBase
    {
        private ApplicationMain _applicationMain;
        private Manager _manager;
        private ModelOfPageNotice _model;
        private string _serachText;

        public override string Title { get { return "Извещения"; } }
        public string SearchText { get { return _serachText; } set { _serachText = value; _model.SearchNotice(value, SelectedCombobox); OnPropertyChanged("SearchText"); } }
        public bool EndScroll { get { return _model.EndScroll; } }
        public Visibility VisibilityAccessLeve { get { return App.AccessLeve == 1 ? Visibility.Collapsed : Visibility.Visible; } }
        public ObservableCollection<NoticeModel> Notices { get { return _model.Notices; } set { _model.Notices = value; } }

        public RelayCommand SelectNotice { get { return new RelayCommand((obj) => { if (obj != null) { _applicationMain.NextPage(new PageNoticeInfo((int)obj, _manager, _applicationMain)); } }); } }
        public RelayCommand SelectApplicability { get { return new RelayCommand((obj) => { SelectApplicab(obj); }); } }
        public RelayCommand ChangeNoticeCommand { get { return new RelayCommand((obj) => { if (obj != null) _applicationMain.DialogWindowShow(new ChangeNotice(_applicationMain, _manager, (int)obj), "MainWindow"); }); } }
        public RelayCommand AddApplicabilityCommand { get { return new RelayCommand((obj) => { if (obj != null) _applicationMain.DialogWindowShow(new AddApplicabilityNotice(_applicationMain, _manager, (int)obj), "MainWindow"); }); } }
        public RelayCommand AddAdressCommand { get { return new RelayCommand((obj) => { if (obj != null) _applicationMain.DialogWindowShow(new AddAddress(_applicationMain, _manager, (int)obj, false, TypeDetail.Product), "MainWindow"); }); } }
        public RelayCommand DeleteNoticeCommand { get { return new RelayCommand((obj) => { if (obj != null) { _applicationMain.DialogWindowShow(new DINMessageDeleteNotice("Удалить извещение?", _manager, (int)obj), "MainWindow"); } }); } }
        public RelayCommand DeleteApplicabilityNoticeCommand { get { return new RelayCommand((obj) => { DeleteApplicab(obj); }); } }
        public RelayCommand OpenFileCommand { get { return new RelayCommand((obj) => { if (obj != null) { int id = (int)obj; OpenFile(id); } }); } }
        public RelayCommand DeleteAddressCommand { get { return new RelayCommand((obj) => { DeleteAddress(obj); }); } }

        public NoticeViewModel(ApplicationMain applicationMain, Manager manager)
        {
            this._applicationMain = applicationMain;
            this._manager = manager;
            _model = this._manager.ModelOfPageNotice;
            _model.PropertyChanged += (s, e) => { OnPropertyChanged(e.PropertyName); };

            ComboBoxItems.Add("По обозначению");
            SelectedCombobox = ComboBoxItems[0];
        }

        public void NextScroll()
        {
            _model.NextScroll();
        }

        private void DeleteAddress(object obj)
        {
            if (obj != null)
            {
                _applicationMain.DialogWindowShow(new DeleteAddress(_applicationMain, _manager, (int)obj, false, TypeDetail.Archive, false), "MainWindow");
            }
        }

        private void SelectApplicab(object obj)
        {
            if (obj != null)
            {
                int id = (int)obj;
                TypeDetail typedetail = _model.Notices.FirstOrDefault(n => (n.ApplicabilityDetails.SingleOrDefault(ad => ad.Id == id)) != null).ApplicabilityDetails.SingleOrDefault(ad => ad.Id == id).TypeDetaill.Value;
                if (typedetail == TypeDetail.AssemblyUnit || typedetail == TypeDetail.Complex || typedetail == TypeDetail.Komplect)
                    _applicationMain.NextPage(new PageDetailsInProduct(_applicationMain, _manager, id, typedetail));
                else
                    _applicationMain.NextPage(new PageDetailInfo(id, _manager, _applicationMain));
            }
        }

        private void DeleteApplicab(object obj)
        {
            if (obj != null)
            {
                ApplicabilityN item = obj as ApplicabilityN;
                if (item != null) _applicationMain.DialogWindowShow(new DINApplicabilityNoticeDelete_Notice("Удалить деталь из извещения?", _manager, item), "MainWindow");
            }
        }

        private async void OpenFile(int id)
        {
            try
            {
                await _model.OpenFileAssync(id);
            }
            catch (Exception ex) { _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "MainWindow"); }
            finally { _model.ChangeProgress(id, false); }
        }
    }
}
