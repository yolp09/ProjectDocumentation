using KD.ViewModel;
using System.Windows.Controls;

namespace KD.View
{
    /// <summary>
    /// Логика взаимодействия для PageDetail.xaml
    /// </summary>
    public partial class PageDetail : Page
    {
        private DetailViewModel _dvm;

        public PageDetail(DetailViewModel dvm)
        {
            InitializeComponent();
            this._dvm = dvm;
            this.DataContext = dvm;
        }

        private void scrl_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if ((e.VerticalOffset + e.ViewportHeight) == e.ExtentHeight && e.ExtentHeight > 10 && _dvm.EndScroll == false)
            {
                _dvm.NextScroll();
                scrl.ScrollToVerticalOffset(scrl.ScrollableHeight / 2);
                scrl.ScrollToHorizontalOffset(scrl.ScrollableWidth / 2);
            }
        }
    }
}
