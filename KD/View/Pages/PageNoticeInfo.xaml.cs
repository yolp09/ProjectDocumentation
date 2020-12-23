using KD.Model;
using KD.ViewModel.Common;
using KD.ViewModel.Pages;
using System.Windows.Controls;

namespace KD.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageNoticeInfo.xaml
    /// </summary>
    public partial class PageNoticeInfo : Page
    {
        public PageNoticeInfo(int idDetail, Manager manager, ApplicationMain applicationMain)
        {
            InitializeComponent();
            this.DataContext = new NoticeInfoViewModel(idDetail, manager, applicationMain);
        }
    }
}
