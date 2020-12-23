using KD.Model;
using KD.ViewModel.Common;
using KD.ViewModel.Pages;
using System.Windows.Controls;

namespace KD.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageDetailsInProduct.xaml
    /// </summary>
    public partial class PageDetailsInProduct : Page
    {
        public PageDetailsInProduct(ApplicationMain applicationMain, Manager manager, int idProduct, TypeDetail typeDetail)
        {
            InitializeComponent();
            this.DataContext = new DetailsInProductViewModel(applicationMain, manager, idProduct, typeDetail);
        }
    }
}
