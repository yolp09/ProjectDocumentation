using KD.ViewModel.Common;
using System.Collections.ObjectModel;

namespace KD.ViewModel
{
    public abstract class PageViewModelBase : ViewModelBase
    {
        public abstract string Title { get; }
        public string SelectedCombobox { get; set; }
        public ObservableCollection<string> ComboBoxItems { get; set; }

        public PageViewModelBase()
        {
            ComboBoxItems = new ObservableCollection<string>();
        }
    }
}
