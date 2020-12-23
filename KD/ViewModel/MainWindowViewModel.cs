using KD.Command;
using KD.Model;
using KD.View;
using KD.View.NotificationMessages;
using KD.View.Pages;
using KD.ViewModel.AddDialogWindows;
using KD.ViewModel.Common;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace KD.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ApplicationMain _applicationMain;
        private Manager _manager;
        private Page _currentPage;
        private System.Windows.WindowState _windowState;
        private RelayCommand _windowsStateMaximizeCommand;
        private RelayCommand _windowsStateMinimizeCommand;
        private Visibility _visibilityButtonBack;
        private ManagerViewModel _managerViewModel;
        private string _header;
        private bool _IsDialogOpen;
        private bool _isEnabledSaveUser;

        public string Header { get { return _header; } set { _header = value; OnPropertyChanged("Header"); } }
        public bool IsDialogOpen { get { return _IsDialogOpen; } set { _IsDialogOpen = value; OnPropertyChanged("IsDialogOpen"); } }
        public bool IsEnabledSaveUser { get { return _isEnabledSaveUser; } set { _isEnabledSaveUser = value; OnPropertyChanged("IsEnabledSaveUser"); } }
        public Visibility VisibilityButtonBack { get { return _visibilityButtonBack; } set { _visibilityButtonBack = value; OnPropertyChanged("VisibilityButtonBack"); } }
        public Page CurrentPage { get { return _currentPage; } set { _currentPage = value; OnPropertyChanged("CurrentPage"); } }
        public System.Windows.WindowState WindowState { get { return _windowState; } set { _windowState = value; OnPropertyChanged("WindowState"); } }
        public ItemMenu MenuAdding;
        public ItemMenu MenuConnection;
        public ItemMenu MenuDirectory;
        public ItemMenu MenuUnusedFiles;
        public ItemMenu MenuArchive;
        public ObservableCollection<ItemMenu> Menu { get; set; }

        #region Commands

        public RelayCommand ChangeSkinCommand { get { return new RelayCommand((obj) => { var item = obj as string; if (item != null) Change_Skin(item); }); } }
        public RelayCommand ExitCommand { get { return new RelayCommand((obj) => { _applicationMain.DialogWindowShow(new DialogInfoNotificationMessageExit("Закрыть программу?", _applicationMain), "MainWindow"); }); } }
        public RelayCommand BackCommand { get { return new RelayCommand((obj) => { _applicationMain.BackPage(); }); } }
        public RelayCommand WindowsStateMaximizeCommand
        {
            get
            {
                if (this._windowsStateMaximizeCommand == null)
                {
                    this._windowsStateMaximizeCommand = new RelayCommand(param =>
                    {
                        if (this.WindowState == System.Windows.WindowState.Normal)
                        { this.WindowState = System.Windows.WindowState.Maximized; }
                        else if (this.WindowState == System.Windows.WindowState.Maximized)
                        { this.WindowState = System.Windows.WindowState.Normal; }
                    }, param => true);
                }
                return this._windowsStateMaximizeCommand;
            }
        }
        public RelayCommand WindowsStateMinimizeCommand
        {
            get
            {
                if (this._windowsStateMinimizeCommand == null)
                { this._windowsStateMinimizeCommand = new RelayCommand(param => this.WindowState = System.Windows.WindowState.Minimized, param => true); }
                return this._windowsStateMinimizeCommand;
            }
        }

        #endregion

        public MainWindowViewModel()
        {
            try
            {
                this._managerViewModel = new ManagerViewModel();

                this._manager = _managerViewModel.Manager;
                this._applicationMain = _managerViewModel.ApplicationMain;
                _applicationMain.PageChanged += Application_PageChanged;
                _applicationMain.VisibilityButtonBackChanged += ApplicationMain_VisibilityButtonBackChanged;
                _applicationMain.NextPage(new Page());
                _applicationMain.ChangeVisibilityButton(Visibility.Visible);

                Menu = new ObservableCollection<ItemMenu>();
                InitializeMenu();
                MenuUpdate();
                VisibilityButtonBack = Visibility.Collapsed;
                if (!App.ConnectionOpen) { Header = "Учет КД (без подключения)"; IsEnabledSaveUser = false; }

                Change_Skin(Properties.Settings.Default.SettingApplicationColor);
                IsEnabledSaveUser = false;

                if (Properties.Settings.Default.SettingIsSave)
                {
                    UserConnection uc = new UserConnection(_applicationMain, this);
                    uc.UserName = Properties.Settings.Default.SettingUserName;
                    uc.UserPassword = Properties.Settings.Default.SettingPassword;
                    uc.AddStart();
                }
            }
            catch (Exception ex) { VisibilityButtonBack = Visibility.Collapsed; this._applicationMain = new ApplicationMain(); MessageBox.Show("Сервер не найден. Обратитесь к разработчику для решения проблемы.","Ошибка"); }
        }

        public void MenuUpdate()
        {
            Menu.Clear();
            InitializeMenu();
            Menu.Add(MenuConnection);
            Menu.Add(MenuDirectory);
            Menu.Add(MenuArchive);
        }

        private void ApplicationMain_VisibilityButtonBackChanged(object sender, EventArgs e)
        {
            VisibilityButtonBack = _applicationMain.VisibilityButtonBack;
        }

        private void Application_PageChanged(object sender, System.EventArgs e)
        {
            CurrentPage = _applicationMain.CurrentPage;
        }

        private void Change_Skin(string item)
        {
            ResourceDictionary dict = Application.Current.Resources.MergedDictionaries[2];
            ResourceDictionary dictFore = Application.Current.Resources.MergedDictionaries[3];
            switch (item)
            {
                case "grey":
                    {
                        dict.Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Grey.xaml");
                        dictFore.Source = new Uri("pack://application:,,,/View/ResourceDictionaries/Themes/BrushesBlack.xaml");
                        Properties.Settings.Default.SettingApplicationColor = item;
                        break;
                    }
                case "blue":
                    {
                        dict.Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Blue.xaml");
                        dictFore.Source = new Uri("pack://application:,,,/View/ResourceDictionaries/Themes/BrushesWhite.xaml");
                        Properties.Settings.Default.SettingApplicationColor = item;
                        break;
                    }
                case "yellow":
                    {
                        dict.Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Yellow.xaml");
                        dictFore.Source = new Uri("pack://application:,,,/View/ResourceDictionaries/Themes/BrushesBlack.xaml");
                        Properties.Settings.Default.SettingApplicationColor = item;
                        break;
                    }
                case "orange":
                    {
                        dict.Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Orange.xaml");
                        dictFore.Source = new Uri("pack://application:,,,/View/ResourceDictionaries/Themes/BrushesBlack.xaml");
                        Properties.Settings.Default.SettingApplicationColor = item;
                        break;
                    }
                case "deeppurple":
                    {
                        dict.Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.DeepPurple.xaml");
                        dictFore.Source = new Uri("pack://application:,,,/View/ResourceDictionaries/Themes/BrushesWhite.xaml");
                        Properties.Settings.Default.SettingApplicationColor = item;
                        break;
                    }
                case "green":
                    {
                        dict.Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.LightGreen.xaml");
                        dictFore.Source = new Uri("pack://application:,,,/View/ResourceDictionaries/Themes/BrushesBlack.xaml");
                        Properties.Settings.Default.SettingApplicationColor = item;
                        break;
                    }
                case "red":
                    {
                        dict.Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Red.xaml");
                        dictFore.Source = new Uri("pack://application:,,,/View/ResourceDictionaries/Themes/BrushesWhite.xaml");
                        Properties.Settings.Default.SettingApplicationColor = item;
                        break;
                    }
                case "purple":
                    {
                        dict.Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Purple.xaml");
                        dictFore.Source = new Uri("pack://application:,,,/View/ResourceDictionaries/Themes/BrushesWhite.xaml");
                        Properties.Settings.Default.SettingApplicationColor = item;
                        break;
                    }
            }
            Properties.Settings.Default.Save();
        }

        private void InitializeMenu()
        {
            var menuConnection = new List<SubItem>();
            menuConnection.Add(new SubItem("Подключение к серверу", null, new UserConnection(_applicationMain, this)));
            menuConnection.Add(new SubItem("Закрыть подключение", null, new DINMExitConnection("Закрыть подключение?", _applicationMain, this)));
            menuConnection.Add(new SubItem("Выход из программы", null, new DialogInfoNotificationMessageExit("Закрыть программу?", _applicationMain)));
            MenuConnection = new ItemMenu(_applicationMain, "Подключение", menuConnection, PackIconKind.TransitConnectionVariant); 

            var menuDirectory = new List<SubItem>();
            menuDirectory.Add(new SubItem("Изделия", new PageProducts(_applicationMain, _manager)));
            menuDirectory.Add(new SubItem("Документация", new PageDetail(_managerViewModel.DocomentsViewModel)));
            menuDirectory.Add(new SubItem("Комплексы", new PageDetail(_managerViewModel.ComplexsViewModel)));
            menuDirectory.Add(new SubItem("Сборочные единицы", new PageDetail(_managerViewModel.AssemblyUnitsViewModel)));
            menuDirectory.Add(new SubItem("Детали", new PageDetail(_managerViewModel.DetailsViewModel)));
            menuDirectory.Add(new SubItem("Стандартные изделия", new PageDetail(_managerViewModel.StandardProductsViewModel)));
            menuDirectory.Add(new SubItem("Прочие изделия", new PageDetail(_managerViewModel.OtherProductsViewModel)));
            menuDirectory.Add(new SubItem("Материалы", new PageDetail(_managerViewModel.MaterialsViewModel)));
            menuDirectory.Add(new SubItem("Комплекты", new PageDetail(_managerViewModel.KomplectsViewModel)));
            menuDirectory.Add(new SubItem("Извещения", new PageNotice(_managerViewModel.NoticesViewModel)));
            MenuDirectory = new ItemMenu(_applicationMain, "Справочник", menuDirectory, PackIconKind.Book);

            var menuAdding = new List<SubItem>();
            menuAdding.Add(new SubItem("Изделия", null, new AddProduct(_applicationMain, _manager)));
            menuAdding.Add(new SubItem("Документации", null, new AddDetail(_applicationMain, "Добавление документации", TypeDetail.Document, _manager)));
            menuAdding.Add(new SubItem("Комплекса", null, new AddDetail(_applicationMain, "Добавление комплекса", TypeDetail.Complex, _manager)));
            menuAdding.Add(new SubItem("Сборочной единицы", null, new AddDetail(_applicationMain, "Добавление сборочной единицы", TypeDetail.AssemblyUnit, _manager)));
            menuAdding.Add(new SubItem("Детали", null, new AddDetail(_applicationMain, "Добавление детали", TypeDetail.Detail, _manager)));
            menuAdding.Add(new SubItem("Стандартного изделия", null, new AddStandardProduct(_applicationMain, "Добавление стандартного изделия",TypeDetail.StandardProduct, _manager)));
            menuAdding.Add(new SubItem("Прочего изделия", null, new AddStandardProduct(_applicationMain, "Добавление прочего изделия", TypeDetail.OthresProduct, _manager)));
            menuAdding.Add(new SubItem("Материала", null, new AddStandardProduct(_applicationMain, "Добавление материала", TypeDetail.Material, _manager)));
            menuAdding.Add(new SubItem("Комплекта", null, new AddDetail(_applicationMain, "Добавление комплекта", TypeDetail.Komplect, _manager)));
            menuAdding.Add(new SubItem("Извещения", null, new AddNotice(_applicationMain, _manager)));
            MenuAdding = new ItemMenu(_applicationMain, "Добавление", menuAdding, PackIconKind.Add);

            MenuArchive = new ItemMenu(_applicationMain, "Архив", new PageDetail(_managerViewModel.ArchiveViewModel), PackIconKind.Archive);
            MenuUnusedFiles = new ItemMenu(_applicationMain, "Неиспользуемые\r\nфайлы", new PageUnusedFiles(_manager), PackIconKind.FileDocumentBoxRemove);
        }
    }
}
