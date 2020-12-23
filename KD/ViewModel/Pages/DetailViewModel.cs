using KD.Command;
using KD.Model;
using KD.Model.PageModel;
using KD.ViewModel.Common;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using KD.View.NotificationMessages;
using KD.View.Pages;
using KD.ViewModel.AddDialogWindows;
using System.Windows;

namespace KD.ViewModel
{
    public class DetailViewModel : PageViewModelBase
    {
        private ApplicationMain _applicationMain;
        private Manager _manager;
        private ModelOfPageDetail _model;
        private TypeDetail _typeDetail;
        private readonly string _title;
        private string _serachText;

        public override string Title { get { return _title; } }
        public string SearchText { get { return _serachText; } set { _serachText = value; _model.SearcDetail(value, SelectedCombobox, _typeDetail); OnPropertyChanged("SearchText"); } }
        public bool EndScroll { get { return _model.EndScroll; } }
        public Visibility VisibilityButtonArchive { get { if (App.AccessLeve == 1) return Visibility.Collapsed; if (_typeDetail == TypeDetail.StandardProduct || _typeDetail == TypeDetail.OthresProduct || _typeDetail == TypeDetail.Material) return Visibility.Visible; else return Visibility.Collapsed; } }
        public Visibility VisibilityAccessLeve { get { return App.AccessLeve == 1 ? Visibility.Collapsed : Visibility.Visible; } }
        public ObservableCollection<DetailModel> Details { get { return _model.Details; } set { _model.Details = value; } }

        public RelayCommand ChangeDetailCommand { get { return new RelayCommand((obj) => { if (obj != null) _applicationMain.DialogWindowShow(new ChangeDetail(_applicationMain, _manager, (int)obj, _typeDetail), "MainWindow"); }); } }
        public RelayCommand AddApplicabilityCommand { get { return new RelayCommand((obj) => { if (obj != null) _applicationMain.DialogWindowShow(new AddApplicability(_applicationMain, _manager, (int)obj, _typeDetail), "MainWindow"); }); } }
        public RelayCommand AddAdressCommand { get { return new RelayCommand((obj) => { if (obj != null) _applicationMain.DialogWindowShow(new AddAddress(_applicationMain, _manager, (int)obj, true, _typeDetail), "MainWindow"); }); } }
        public RelayCommand DeleteApplicabilityCommand { get { return new RelayCommand((obj) => { DeleteApplicab(obj); }); } }
        public RelayCommand OpenFileCommand { get { return new RelayCommand((obj) => { if (obj != null) { int id = (int)obj; OpenFile(id); } }); } }
        public RelayCommand OpenFileCommandApplicability { get { return new RelayCommand((obj) => { if (obj != null) { int id = (int)obj; OpenFileApplicability(id); } }); } }
        public RelayCommand DeleteApplicabilityNoticeCommand { get { return new RelayCommand((obj) => { DeleteApplicabNotice(obj); }); } }
        public RelayCommand DeleteDetailCommand { get { return new RelayCommand((obj) => { if (obj != null) { _applicationMain.DialogWindowShow(new DINMessageDeleteDetail("Удалить деталь?", _manager, (int)obj, _typeDetail), "MainWindow"); } }); } }
        public RelayCommand SelectDetail { get { return new RelayCommand((obj) => { SelectDet(obj); }); } }
        public RelayCommand SelectApplicabilityNotice { get { return new RelayCommand((obj) => { if (obj != null) { _applicationMain.NextPage(new PageNoticeInfo((int)obj, _manager, _applicationMain)); } }); } }
        public RelayCommand SelectApplicability { get { return new RelayCommand((obj) => { SelectApplicab(obj); }); } }
        public RelayCommand DeleteAddressCommand { get { return new RelayCommand((obj) => { DeleteAddress(obj); }); } }
        public RelayCommand ArchiveCommand { get { return new RelayCommand((obj) => { Arrchive(obj); }); } }

        public DetailViewModel(ApplicationMain applicationMain, Manager manager, string title, TypeDetail typedetail)
        {
            this._applicationMain = applicationMain;
            this._manager = manager;
            this._title = title;
            this._typeDetail = typedetail;
            switch(_typeDetail)
            {
                case TypeDetail.Detail: { _model = this._manager.ModelOfPageDetails; break; }
                case TypeDetail.Document: { _model = this._manager.ModelOfPageDocuments; break; }
                case TypeDetail.AssemblyUnit: { _model = this._manager.ModelOfPageAssemblyUnits; break; }
                case TypeDetail.StandardProduct: { _model = this._manager.ModelOfPageStandardProducts; break; }
                case TypeDetail.OthresProduct: { _model = this._manager.ModelOfPageOtherProducts; break; }
                case TypeDetail.Complex: { _model = this._manager.ModelOfPageComplexs; break; }
                case TypeDetail.Komplect: { _model = this._manager.ModelOfPageKomplekts; break; }
                case TypeDetail.Material: { _model = this._manager.ModelOfPageMaterials; break; }
                case TypeDetail.Archive: { _model = this._manager.ModelOfPageArchive; break; }
            }
            _model.PropertyChanged += (s, e) => { OnPropertyChanged(e.PropertyName); };

            if (_typeDetail == TypeDetail.Detail || _typeDetail == TypeDetail.Document || _typeDetail == TypeDetail.AssemblyUnit || _typeDetail == TypeDetail.Complex || _typeDetail == TypeDetail.Komplect) ComboBoxItems.Add("По обозначению");
            ComboBoxItems.Add("По наименованию");
            SelectedCombobox = ComboBoxItems[0];
        }

        public void NextScroll()
        {
            _model.NextScroll();
        }

        private void Arrchive(object obj)
        {
            if (obj != null)
            {
                _model.Archive((int)obj);
            }
        }

        private void DeleteAddress(object obj)
        {
            if (obj != null)
            {
                _applicationMain.DialogWindowShow(new DeleteAddress(_applicationMain, _manager, (int)obj, true, _typeDetail, false), "MainWindow");
            }
        }

        private async void OpenFile(int id)
        {
            DetailModel detail = null;
            try
            {
                detail = Details.SingleOrDefault(d => d.Id == id);

                detail.ProgressIsIndeterminate = true;
                await _model.OpenFileAssync(detail.IdFile.Value);
            }
            catch (Exception ex) { _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "MainWindow"); }
            finally { if (detail != null) detail.ProgressIsIndeterminate = false; }
        }

        private async void OpenFileApplicability(int id)
        {
            ApplicabilityFileModel applicabilityFileModel = null;
            try
            {
                applicabilityFileModel = Details.SingleOrDefault(d => d.ApplicabilityFs.SingleOrDefault(da => da.IdFile == id) != null).ApplicabilityFs.SingleOrDefault(da => da.IdFile == id);
                applicabilityFileModel.ProgressIsIndeterminate = true;
                await _model.OpenFileAssync(applicabilityFileModel.IdFile.Value);
            }
            catch (Exception ex) { _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "MainWindow"); }
            finally { if (applicabilityFileModel != null) applicabilityFileModel.ProgressIsIndeterminate = false; }
        }

        private void DeleteApplicab(object obj)
        {
            if (obj != null)
            {
                ApplicabilitySBOrNotSb item = obj as ApplicabilitySBOrNotSb;
                if(item != null)_applicationMain.DialogWindowShow(new DINMApplicabilityDelete("Удалить применяемость?", _manager, item, _typeDetail), "MainWindow");
            }
        }

        private void DeleteApplicabNotice(object obj)
        {
            if (obj != null)
            {
                ApplicabilityN item = obj as ApplicabilityN;
                if(item != null)_applicationMain.DialogWindowShow(new DINApplicabilityNoticeDelete_Detail("Удалить извещение из детали?", _manager, item, _typeDetail), "MainWindow");
            }
        }

        private void SelectDet(object obj)
        {
            if (obj != null)
            {
                if (_typeDetail == TypeDetail.AssemblyUnit || _typeDetail == TypeDetail.Complex || _typeDetail == TypeDetail.Komplect)
                    _applicationMain.NextPage(new PageDetailsInProduct(_applicationMain, _manager, (int)obj, _typeDetail));
                else
                    _applicationMain.NextPage(new PageDetailInfo((int)obj, _manager, _applicationMain));
            }
        }

        private void SelectApplicab(object obj)
        {
            if (obj != null)
            {
                ApplicabilitySBOrNotSb item = obj as ApplicabilitySBOrNotSb;
                if (item != null)
                {
                    TypeDetail typeD = item.IsSB ? TypeDetail.Detail : TypeDetail.Product;
                    _applicationMain.NextPage(new PageDetailsInProduct(_applicationMain, _manager, item.IdProduct, typeD));
                }
            }
        }
    }
}
