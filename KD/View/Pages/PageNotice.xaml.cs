using KD.ViewModel.Pages;
using System.Windows.Controls;

namespace KD.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageNotice.xaml
    /// </summary>
    public partial class PageNotice : Page
    {
        private NoticeViewModel _nvm;

        public PageNotice(NoticeViewModel nvm)
        {
            InitializeComponent();
            this._nvm = nvm;
            this.DataContext = _nvm;
        }

        private void scrl_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if ((e.VerticalOffset + e.ViewportHeight) == e.ExtentHeight && e.ExtentHeight > 10 && _nvm.EndScroll == false)
            {
                _nvm.NextScroll();
                scrl.ScrollToVerticalOffset(scrl.ScrollableHeight / 2);
                scrl.ScrollToHorizontalOffset(scrl.ScrollableWidth / 2);
            }
        }
    }
}
