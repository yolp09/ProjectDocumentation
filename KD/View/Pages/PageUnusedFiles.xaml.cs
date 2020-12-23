using KD.Model;
using KD.ViewModel.Pages;
using System.Windows.Controls;

namespace KD.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageUnusedFiles.xaml
    /// </summary>
    public partial class PageUnusedFiles : Page
    {
        public PageUnusedFiles(Manager manager)
        {
            InitializeComponent();
            this.DataContext = new UnusedFileViewModel(manager);
        }
    }
}
