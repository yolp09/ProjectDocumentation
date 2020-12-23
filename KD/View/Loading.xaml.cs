using System;
using System.Threading.Tasks;
using System.Windows;

namespace KD.View
{
    /// <summary>
    /// Логика взаимодействия для Loading.xaml
    /// </summary>
    public partial class Loading : Window, IDisposable
    {
        public Action Worker { get; set; }

        public Loading(Action worker)
        {
            InitializeComponent();
            this.Worker = worker;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(Worker).ContinueWith(t => { Close(); }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void Dispose()
        {
        }
    }
}
