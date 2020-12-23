using KD.Model;
using KD.ViewModel;
using KD.ViewModel.Common;
using System.Windows.Controls;

namespace KD.View
{
    /// <summary>
    /// Логика взаимодействия для PageProducts.xaml
    /// </summary>
    public partial class PageProducts : Page
    {
        public PageProducts(ApplicationMain applicationMain, Manager manager)
        {
            InitializeComponent();
            this.DataContext = new ProductsViewModel(applicationMain, manager);
        }
    }
}
