using KD.Command;
using KD.Model;
using KD.Model.PageModel;
using KD.ViewModel.Common;
using System.Collections.ObjectModel;

namespace KD.ViewModel.Pages
{
    public class UnusedFileViewModel : ViewModelBase
    {
        private Manager _manager;
        private ModelOfPageUnusedFiles _model;

        public ObservableCollection<FileModel> UnusedFiles { get { return _model.UnusedFiles; } set { _model.UnusedFiles = value; } }

        public RelayCommand DeleteUnusedFile { get { return new RelayCommand((obj) => { if (obj != null) { _model.DeleteFile((int)obj); } }); } }

        public UnusedFileViewModel(Manager manager)
        {
            this._manager = manager;
            _model = this._manager.ModelOfPageUnusedFiles;
            _model.PropertyChanged += (s, e) => { OnPropertyChanged(e.PropertyName); };
        }
    }
}
