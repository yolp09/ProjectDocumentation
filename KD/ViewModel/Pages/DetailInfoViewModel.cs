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
using System.Windows;

namespace KD.ViewModel.Pages
{
    public class DetailInfoViewModel : PageViewModelBase
    {
        private Manager _manager;
        private ApplicationMain _applicationMain;
        private DetailModel _model;

        public override string Title { get { return _model.Title; } }
        public string Number { get { return _model.Number; } }
        public string Name { get { return _model.Name; } }
        public string Developer { get { return _model.Developer; } }
        public string Adress { get { return _model.Adress; } }
        public string InfoScan { get { return _model.InfoScan; } }
        public int Id { get { return _model.Id; } }
        public bool IsEnabledFile { get { return _model.IsEnabledFile; } }
        public bool ProgressIsIndeterminate { get { return _model.ProgressIsIndeterminate; } set { _model.ProgressIsIndeterminate = value; OnPropertyChanged("ProgressIsIndeterminate"); } }
        public Visibility VisibilityCardNotice { get { return _model.ApplicabilityNotices.Count == 0 ? Visibility.Collapsed : Visibility.Visible; } }
        public Visibility VisibilityAllScan { get { return _model.ApplicabilityFs.Count == 0 ? Visibility.Collapsed : Visibility.Visible; } }
        public Visibility VisibilityAccessLeve { get { return App.AccessLeve == 1 ? Visibility.Collapsed : Visibility.Visible; } }
        public ObservableCollection<ApplicabilityModel> Applicabilities { get { return _model.Applicabilities; } set { _model.Applicabilities = value; } }
        public ObservableCollection<NoticeModel> ApplicabilityNotices { get { return _model.ApplicabilityNotices; } set { _model.ApplicabilityNotices = value; } }
        public ObservableCollection<ApplicabilityFileModel> ApplicabilityFs { get { return _model.ApplicabilityFs; } set { _model.ApplicabilityFs = value; } }

        public RelayCommand AddApplicabilityCommand { get { return new RelayCommand((obj) => { if (obj != null) _applicationMain.DialogWindowShow(new AddApplicability(_applicationMain, _manager, (int)obj, _model.TypeDetaill.Value, _model), "MainWindow"); }); } }
        public RelayCommand AddAdressCommand { get { return new RelayCommand((obj) => { if (obj != null) _applicationMain.DialogWindowShow(new AddAddress(_applicationMain, _manager, (int)obj, true, _model.TypeDetaill.Value, _model), "MainWindow"); }); } }
        public RelayCommand ChangeDetailCommand { get { return new RelayCommand((obj) => { if (obj != null) _applicationMain.DialogWindowShow(new ChangeDetail(_applicationMain, _manager, (int)obj, _model.TypeDetaill.Value, _model), "MainWindow"); }); } }
        public RelayCommand DeleteDetailCommand { get { return new RelayCommand((obj) => { if (obj != null) { _applicationMain.DialogWindowShow(new DINMessageDeleteDetail("Удалить деталь?", _manager, (int)obj, _model.TypeDetaill.Value, _applicationMain), "MainWindow"); } }); } }
        public RelayCommand SelectApplicabilityNotice { get { return new RelayCommand((obj) => { if (obj != null) { _applicationMain.NextPage(new PageNoticeInfo((int)obj, _manager, _applicationMain)); } }); } }
        public RelayCommand SelectApplicability { get { return new RelayCommand((obj) => { if (obj != null) { Selectapplicability(obj); } }); } }
        public RelayCommand DeleteApplicabilityNoticeCommand { get { return new RelayCommand((obj) => { if (obj != null) { DeleteApplicabNotice(obj); } }); } }
        public RelayCommand DeleteApplicabilityCommand { get { return new RelayCommand((obj) => { if (obj != null) { DeleteApplicab(obj); } }); } }
        public RelayCommand OpenFileCommand { get { return new RelayCommand((obj) => { if (obj != null) { int id = (int)obj; OpenFile(id); } }); } }
        public RelayCommand OpenFileCommandApplicability { get { return new RelayCommand((obj) => { if (obj != null) { int id = (int)obj; OpenFileApplicability(id); } }); } }
        public RelayCommand DeleteAddressCommand { get { return new RelayCommand((obj) => { DeleteAddress(obj); }); } }
        public RelayCommand CopyNumberCommand { get { return new RelayCommand((obj) => { Clipboard.SetText(Number); }); } }
        public RelayCommand CopyNameCommand { get { return new RelayCommand((obj) => { Clipboard.SetText(Name); }); } }

        public DetailInfoViewModel(int idDetail, Manager manager, ApplicationMain applicationMain)
        {
            using (Loading lw = new Loading(() =>
            {
                this._manager = manager;
                this._applicationMain = applicationMain;
                this._model = new DetailModel(idDetail);
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
                _applicationMain.DialogWindowShow(new DeleteAddress(_applicationMain, _manager, (int)obj, true, _model.TypeDetaill.Value, true), "MainWindow");
            }
        }

        private void Selectapplicability(object obj)
        {
            ApplicabilitySBOrNotSb item = obj as ApplicabilitySBOrNotSb;
            if (item != null)
            {
                TypeDetail typeD = item.IsSB ? TypeDetail.Detail : TypeDetail.Product;
                _applicationMain.NextPage(new PageDetailsInProduct(_applicationMain, _manager, item.IdProduct, typeD));
            }
        }

        private void DeleteApplicabNotice(object obj)
        {
            ApplicabilityN item = obj as ApplicabilityN;
            if (item != null) _applicationMain.DialogWindowShow(new DINApplicabilityNoticeDelete_Detail("Удалить извещение из детали?", _manager, item, _model.TypeDetaill.Value, _model), "MainWindow");
        }

        private void DeleteApplicab(object obj)
        {
            ApplicabilitySBOrNotSb item = obj as ApplicabilitySBOrNotSb;
            if (item != null) _applicationMain.DialogWindowShow(new DINMApplicabilityDelete("Удалить применяемость?", _manager, item, _model.TypeDetaill.Value, _model), "MainWindow");
        }

        private async void OpenFile(int id)
        {
            try
            {
                ProgressIsIndeterminate = true;
                await _model.OpenFileAssync(id);
            }
            catch (Exception ex) { _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "MainWindow"); }
            finally { ProgressIsIndeterminate = false; }
        }

        private async void OpenFileApplicability(int id)
        {
            try
            {
                await _model.OpenFileApplicabilityAssync(id);
            }
            catch (Exception ex) { _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "MainWindow"); }
            finally { _model.ChangeProgressApplicability(id, false); }
        }
    }
}
