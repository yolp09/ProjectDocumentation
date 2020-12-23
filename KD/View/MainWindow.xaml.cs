using KD.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace KD
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isWiden = false;
        private bool _stateClose = false;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
            uiScaleSlider.MouseDoubleClick += new MouseButtonEventHandler(RestoreScalingFactor);
            this.MouseLeftButtonDown += TitleBar_MouseLeftButtonDown;
            this.Height = Properties.Settings.Default.SettingHeight;
            this.Width = Properties.Settings.Default.SettingWidth;
            uiScaleSlider.Value = Properties.Settings.Default.SettingScale;
            CheckIsSave.IsChecked = Properties.Settings.Default.SettingIsSave;
        }

        private void RestoreScalingFactor(object sender, MouseButtonEventArgs e)
        {
            ((Slider)sender).Value = 1.0;
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs args)
        {
            base.OnPreviewMouseWheel(args);
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) { uiScaleSlider.Value += (args.Delta > 0) ? 0.1 : -0.1; }
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs args)
        {
            base.OnPreviewMouseDown(args);
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (args.MiddleButton == MouseButtonState.Pressed) { RestoreScalingFactor(uiScaleSlider, args); }
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_initiateWiden(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _isWiden = true;
            this.MouseLeftButtonDown -= TitleBar_MouseLeftButtonDown;
        }

        private void Window_endWiden(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _isWiden = false;
            Rectangle rect = (Rectangle)sender;
            rect.ReleaseMouseCapture();
            this.MouseLeftButtonDown += TitleBar_MouseLeftButtonDown;
        }

        //Изменение окна в ширину
        private void Window_Widen(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            if (_isWiden)
            {
                rect.CaptureMouse();
                double newWidth = e.GetPosition(this).X + 5;
                if (newWidth > 0) this.Width = newWidth;
            }
        }

        //Изменение окна в высоту
        private void Window_Heighten(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            if (_isWiden)
            {
                rect.CaptureMouse();
                double newHeight = e.GetPosition(this).Y + 5;
                if (newHeight > 0) this.Height = newHeight;
            }
        }

        //Изменение окна и в выстоу и в ширину
        private void Window_HeightenAndWiden(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            if (_isWiden)
            {
                rect.CaptureMouse();
                double newWidth = e.GetPosition(this).X + 5;
                double newHeight = e.GetPosition(this).Y + 5;
                if (newHeight > 0 && newWidth > 0) { this.Height = newHeight; this.Width = newWidth; }
            }
        }

        //Открытие/Закрытие меню
        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            if (_stateClose)
            {
                System.Windows.Media.Animation.Storyboard sb = this.FindResource("OpenMenu") as System.Windows.Media.Animation.Storyboard;
                sb.Begin();
                _stateClose = !_stateClose;
            }
            else
            {
                System.Windows.Media.Animation.Storyboard sb = this.FindResource("CloseMenu") as System.Windows.Media.Animation.Storyboard;
                sb.Begin();
                _stateClose = !_stateClose;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                Properties.Settings.Default.SettingHeight = this.Height;
                Properties.Settings.Default.SettingWidth = this.Width;
                Properties.Settings.Default.SettingScale = uiScaleSlider.Value;
                Properties.Settings.Default.Save();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SettingIsSave = CheckIsSave.IsChecked.Value;
            Properties.Settings.Default.Save();
        }
    }
}
