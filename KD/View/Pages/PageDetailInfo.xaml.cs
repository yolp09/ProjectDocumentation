using KD.Model;
using KD.ViewModel.Common;
using KD.ViewModel.Pages;
using System.Windows.Controls;

namespace KD.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageDetailInfo.xaml
    /// </summary>
    public partial class PageDetailInfo : Page
    {
        public PageDetailInfo(int idDetail, Manager manager, ApplicationMain applicationMain)
        {
            InitializeComponent();
            this.DataContext = new DetailInfoViewModel(idDetail, manager, applicationMain);
        }
    }
}
