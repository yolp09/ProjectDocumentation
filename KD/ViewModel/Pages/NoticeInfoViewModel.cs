using KD.Command;
using KD.Model;
using KD.Model.PageModel;
using KD.View;
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
    public class NoticeInfoViewModel : PageViewModelBase
    {
        private Manager _manager;
        private ApplicationMain _applicationMain;
        private NoticeModel _model;

        public override string Title { get { return _model.Title; } }
        public string Number { get { return _model.Number; } }
        public string Date { get { return _model.Date; } }
        public string Developer { get { return _model.Developer; } }
        public string ChangeCode { get { return _model.ChangeCode; } }
        public string Adress { get { return _model.Adress; } }
        public int Id { get { return _model.Id; } }
        public int CountSheets { get { return _model.CountSheets; } }
        public bool ProgressIsIndeterminate { get { return _model.ProgressIsIndeterminate; } set { _model.ProgressIsIndeterminate = value; } }
        public Visibility VisibilityAccessLeve { get { return App.AccessLeve == 1 ? Visibility.Collapsed : Visibility.Visible; } }
        public ObservableCollection<DetailModel> ApplicabilityDetails { get { return _model.ApplicabilityDetails; } set { _model.ApplicabilityDetails = value; } }

        public RelayCommand AddApplicabilityCommand { get { return new RelayCommand((obj) => { if (obj != null) _applicationMain.DialogWindowShow(new AddApplicabilityNotice(_applicationMain, _manager, (int)obj, _model), "MainWindow"); }); } }
        public RelayCommand AddAdressCommand { get { return new RelayCommand((obj) => { if (obj != null) _applicationMain.DialogWindowShow(new AddAddress(_applicationMain, _manager, (int)obj, false, TypeDetail.Product, null, _model), "MainWindow"); }); } }
        public RelayCommand ChangeNoticeCommand { get { return new RelayCommand((obj) => { if (obj != null) _applicationMain.DialogWindowShow(new ChangeNotice(_applicationMain, _manager, (int)obj, _model), "MainWindow"); }); } }
        public RelayCommand DeleteNoticeCommand { get { return new RelayCommand((obj) => { if (obj != null) { _applicationMain.DialogWindowShow(new DINMessageDeleteNotice("Удалить извещение?", _manager, (int)obj, _applicationMain), "MainWindow"); } }); } }
        public RelayCommand SelectApplicability { get { return new RelayCommand((obj) => { SelectApplicab(obj); }); } }
        public RelayCommand DeleteApplicabilityNoticeCommand { get { return new RelayCommand((obj) => { DeleteApplicab(obj); }); } }
        public RelayCommand OpenFileCommand { get { return new RelayCommand((obj) => { if (obj != null) { int id = (int)obj; OpenFile(id); } }); } }
        public RelayCommand DeleteAddressCommand { get { return new RelayCommand((obj) => { DeleteAddress(obj); }); } }

        public NoticeInfoViewModel(int idNotice, Manager manager, ApplicationMain applicationMain)
        {
            using (Loading lw = new Loading(() =>
            {
                this._manager = manager;
                this._applicationMain = applicationMain;
                _model = new NoticeModel(idNotice, true);
                _model.PropertyChanged += (s, e) => { OnPropertyChanged(e.PropertyName); };
            }))
            {
                if (System.Windows.Application.Current.Windows.Count > 0)
                {
                    var w = System.Windows.Application.Current.Windows[0];
                    lw.Owner = w;
                }
                lw.ShowDialog();
            }
        }

        private void DeleteAddress(object obj)
        {
            if (obj != null)
            {
                _applicationMain.DialogWindowShow(new DeleteAddress(_applicationMain, _manager, (int)obj, false, TypeDetail.Archive, true), "MainWindow");
            }
        }

        private void SelectApplicab(object obj)
        {
            if (obj != null)
            {
                int id = (int)obj;
                TypeDetail typedetail = _model.ApplicabilityDetails.SingleOrDefault(ad => ad.Id == id).TypeDetaill.Value;
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
                if (item != null) _applicationMain.DialogWindowShow(new DINApplicabilityNoticeDelete_Notice("Удалить деталь из извещения?", _manager, item, _model), "MainWindow");
            }
        }

        private async void OpenFile(int id)
        {
            try
            {
                await _model.OpenFileAssync(id);
            }
            catch (Exception ex) { _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "MainWindow"); }
            finally { _model.ProgressIsIndeterminate = false; }
        }
    }
}
