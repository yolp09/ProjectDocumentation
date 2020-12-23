using KD.Command;
using KD.Model;
using KD.Model.Common;
using KD.Model.ModelsForAdd;
using KD.Model.PageModel;
using KD.View.NotificationMessages;
using KD.ViewModel.Common;
using System;
using System.Collections.ObjectModel;

namespace KD.ViewModel.AddDialogWindows
{
    public class AddApplicability : NotificationMessage
    {
        private ModelOfAddApplicability _model;
        private Manager _manager;
        private ApplicationMain _applicationMain;
        private DetailModel _detailModel;
        private TypeDetail _typeDetail;

        public int? SelectedSection { set { _model.SelectedSection = value; } }
        public int? SelectedProducts { get { return _model.SelectedProducts; } set { _model.SelectedProducts = value; } }
        public bool ProgressIsIndeterminate { get { return _model.ProgressIsIndeterminate; } set { _model.ProgressIsIndeterminate = value; } }
        public ObservableCollection<ModelComboBox> ComboProducts { get { return _model.ComboProducts; } set { _model.ComboProducts = value; } }
        public ReadOnlyObservableCollection<ModelComboBox> ComboSections { get; set; }

        public RelayCommand AddCommand { get { return new RelayCommand((obj) => { Add(); }); } }
        public RelayCommand CloseCommand { get { return new RelayCommand((obj) => { MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null); }); } }

        public AddApplicability(ApplicationMain applicationMain, Manager manager, int idDetail, TypeDetail typeDetail, DetailModel detailModel = null)
        {
            this._applicationMain = applicationMain;
            this._manager = manager;
            this._typeDetail = typeDetail;
            this._detailModel = detailModel;

            this._model = new ModelOfAddApplicability(idDetail, _manager);
            _model.PropertyChanged += (s, e) => { OnPropertyChanged(e.PropertyName); };

            Title = _model.Title;
            ComboSections = new ReadOnlyObservableCollection<ModelComboBox>(_model.ComboSectoins);
        }

        private async void Add()
        {
            try
            {
                await _model.AddApplicabilityAssync();
                _manager.CollectionUpdate(_typeDetail);
                if (_manager.ModelOfPageDetailsInProduct != null) _manager.ModelOfPageDetailsInProduct.GetDetail();
                if (_detailModel != null) _detailModel.GetApplicability();
                _applicationMain.DialogWindowShow(new InfoNotificationMessage(String.Format("Применяемость добавлена!!!")), "AddApplicability");
            }
            catch (Exception ex) { _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "AddApplicability"); }
            finally { ProgressIsIndeterminate = false; }
        }
    }
}
