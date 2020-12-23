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
    public class DetailsInProductViewModel : PageViewModelBase
    {
        private ApplicationMain _applicationMain;
        private Manager _manager;
        private ModelOfPageDetailsInProduct _model;
        private TypeDetail _detailView;
        private string _serachText;

        public override string Title { get { return _model.Title; } }
        public string SearchText { get { return _serachText; } set { _serachText = value; _model.SearcDetail(value, SelectedCombobox); OnPropertyChanged("SearchText"); } }
        public TypeDetail DetailView
        {
            get { return _detailView; }
            set
            {
                if (_detailView == value)
                    return;

                _detailView = value;
                OnPropertyChanged("IsCheckedAll");
                OnPropertyChanged("IsCheckedStandard");
                OnPropertyChanged("IsCheckedProchie");
                OnPropertyChanged("IsCheckedComplex");
                OnPropertyChanged("IsCheckedDetail");
                OnPropertyChanged("IsCheckedAssembly");
                OnPropertyChanged("IsCheckedDocument");
                OnPropertyChanged("IsCheckedMaterial");
                OnPropertyChanged("IsCheckedKomplect");
                _model.DetailViewSelection(_detailView);
            }
        }
        public bool IsCheckedAll
        {
            get { return DetailView == TypeDetail.Product; }
            set { DetailView = value ? TypeDetail.Product : DetailView; }
        }
        public bool IsCheckedStandard
        {
            get { return DetailView == TypeDetail.StandardProduct; }
            set { DetailView = value ? TypeDetail.StandardProduct : DetailView; }
        }
        public bool IsCheckedProchie
        {
            get { return DetailView == TypeDetail.OthresProduct; }
            set { DetailView = value ? TypeDetail.OthresProduct : DetailView; }
        }

        public bool IsCheckedComplex
        {
            get { return DetailView == TypeDetail.Complex; }
            set { DetailView = value ? TypeDetail.Complex : DetailView; }
        }
        public bool IsCheckedDetail
        {
            get { return DetailView == TypeDetail.Detail; }
            set { DetailView = value ? TypeDetail.Detail : DetailView; }
        }
        public bool IsCheckedAssembly
        {
            get { return DetailView == TypeDetail.AssemblyUnit; }
            set { DetailView = value ? TypeDetail.AssemblyUnit : DetailView; }
        }

        public bool IsCheckedDocument
        {
            get { return DetailView == TypeDetail.Document; }
            set { DetailView = value ? TypeDetail.Document : DetailView; }
        }
        public bool IsCheckedMaterial
        {
            get { return DetailView == TypeDetail.Material; }
            set { DetailView = value ? TypeDetail.Material : DetailView; }
        }
        public bool IsCheckedKomplect
        {
            get { return DetailView == TypeDetail.Komplect; }
            set { DetailView = value ? TypeDetail.Komplect : DetailView; }
        }
        public Visibility VisibilityAccessLeve { get { return App.AccessLeve == 1 ? Visibility.Collapsed : Visibility.Visible; } }
        public ObservableCollection<DetailModel> Details { get { return _model.Details; } set { _model.Details = value; } }

        public RelayCommand SelectDetail { get { return new RelayCommand((obj) => { SelectDet(obj); }); } }
        public RelayCommand SelectApplicability { get { return new RelayCommand((obj) => { SelectApplicab(obj); }); } }
        public RelayCommand SelectApplicabilityNotice { get { return new RelayCommand((obj) => { if (obj != null) { _applicationMain.NextPage(new PageNoticeInfo((int)obj, _manager, _applicationMain)); } }); } }
        public RelayCommand ChangeDetailCommand { get { return new RelayCommand((obj) => { if (obj != null) _applicationMain.DialogWindowShow(new ChangeDetail(_applicationMain, _manager, (int)obj, TypeDetail.Product), "MainWindow"); }); } }
        public RelayCommand AddApplicabilityCommand { get { return new RelayCommand((obj) => { if (obj != null) _applicationMain.DialogWindowShow(new AddApplicability(_applicationMain, _manager, (int)obj, TypeDetail.Product), "MainWindow"); }); } }
        public RelayCommand AddAdressCommand { get { return new RelayCommand((obj) => { if (obj != null) _applicationMain.DialogWindowShow(new AddAddress(_applicationMain, _manager, (int)obj, true, TypeDetail.Product), "MainWindow"); }); } }
        public RelayCommand OpenFileCommand { get { return new RelayCommand((obj) => { if (obj != null) { int id = (int)obj; OpenFile(id); } }); } }
        public RelayCommand OpenFileCommandApplicability { get { return new RelayCommand((obj) => { if (obj != null) { int id = (int)obj; OpenFileApplicability(id); } }); } }
        public RelayCommand DeleteApplicabilityCommand { get { return new RelayCommand((obj) => { DeleteApplicab(obj); }); } }
        public RelayCommand DeleteApplicabilityNoticeCommand { get { return new RelayCommand((obj) => { DeleteApplicabNotice(obj); }); } }

        public DetailsInProductViewModel(ApplicationMain applicationMain, Manager manager, int idProduct, TypeDetail typeDetail)
        {
            using (Loading lw = new Loading(() =>
            {
                this._applicationMain = applicationMain;
                this._manager = manager;

                _manager.ModelOfPageDetailsInProduct = new ModelOfPageDetailsInProduct(idProduct, typeDetail);
                _model = _manager.ModelOfPageDetailsInProduct;

                ComboBoxItems.Add("По обозначению");
                ComboBoxItems.Add("По наименованию");
                SelectedCombobox = ComboBoxItems[0];
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

        private void SelectDet(object obj)
        {
            if (obj != null)
            {
                TypeDetail typedetail = _model.Details.SingleOrDefault(d => d.Id == (int)obj).TypeDetaill.Value;
                if (typedetail == TypeDetail.AssemblyUnit || typedetail == TypeDetail.Complex || typedetail == TypeDetail.Komplect)
                    _applicationMain.NextPage(new PageDetailsInProduct(_applicationMain, _manager, (int)obj, typedetail));
                else
                    _applicationMain.NextPage(new PageDetailInfo((int)obj, _manager, _applicationMain));
            }
        }

        private void SelectApplicab(object obj)
        {
            if (obj != null)
            {
                ApplicabilitySBOrNotSb item = obj as ApplicabilitySBOrNotSb;
                TypeDetail typeD = item.IsSB ? TypeDetail.Detail : TypeDetail.Product;
                _applicationMain.NextPage(new PageDetailsInProduct(_applicationMain, _manager, item.IdProduct, typeD));
            }
        }

        private void DeleteApplicab(object obj)
        {
            if (obj != null)
            {
                ApplicabilitySBOrNotSb item = obj as ApplicabilitySBOrNotSb;
                _applicationMain.DialogWindowShow(new DINMApplicabilityDelete("Удалить применяемость?", _manager, item, TypeDetail.Product), "MainWindow");
            }
        }

        private void DeleteApplicabNotice(object obj)
        {
            if (obj != null)
            {
                ApplicabilityN item = obj as ApplicabilityN;
                _applicationMain.DialogWindowShow(new DINApplicabilityNoticeDelete_Detail("Удалить извещение из детали?", _manager, item, TypeDetail.Product), "MainWindow");
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
