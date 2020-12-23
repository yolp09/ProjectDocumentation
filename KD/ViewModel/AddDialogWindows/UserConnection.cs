using KD.Command;
using KD.Model.ModelsForAdd;
using KD.View.NotificationMessages;
using KD.ViewModel.Common;
using System;

namespace KD.ViewModel.AddDialogWindows
{
    public class UserConnection : NotificationMessage
    {
        private ModelOfUserConnection _model;
        private ApplicationMain _applicationMain;
        private MainWindowViewModel _mainWibdowViewModel;
        private bool _progressIsIndeterminate;

        public string UserName { get { return _model.UserName; } set { _model.UserName = value; } }
        public string UserPassword { get { return _model.UserPassword; } set { _model.UserPassword = value;  } }
        public bool ProgressIsIndeterminate { get { return _progressIsIndeterminate; } set { _progressIsIndeterminate = value; OnPropertyChanged("ProgressIsIndeterminate"); } }

        public RelayCommand AddCommand { get { return new RelayCommand((obj) => { Add(); }); } }
        public RelayCommand CloseCommand { get { return new RelayCommand((obj) => { MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null); }); } }

        public UserConnection(ApplicationMain applicationMain, MainWindowViewModel mainWindowViewModel)
        {
            this._applicationMain = applicationMain;
            this._mainWibdowViewModel = mainWindowViewModel;
            this._model = new ModelOfUserConnection();
            _model.PropertyChanged += (s, e) => { OnPropertyChanged(e.PropertyName); };
        }

        public async void AddStart()
        {
            try
            {
                int accessLevel = await _model.CheckUserAssync();
                App.ConnectionOpen = true; _mainWibdowViewModel.IsEnabledSaveUser = true;
                string level = String.Empty;
                App.AccessLeve = accessLevel;
                _mainWibdowViewModel.MenuUpdate();
                switch (accessLevel)
                {
                    case 1: { level = "Пользователь для просмотра"; break; }
                    case 2: { level = "Пользователь для добавления"; _mainWibdowViewModel.Menu.Add(_mainWibdowViewModel.MenuAdding); _mainWibdowViewModel.Menu.Add(_mainWibdowViewModel.MenuUnusedFiles); break; }
                    case 3: { level = "Админ"; _mainWibdowViewModel.Menu.Add(_mainWibdowViewModel.MenuAdding); _mainWibdowViewModel.Menu.Add(_mainWibdowViewModel.MenuUnusedFiles); break; }
                }
                _mainWibdowViewModel.Header = String.Format("Учет КД ({0}: {1})", level, UserName);
                _model.SaveHistory();
                _model.UserPassword = String.Empty;
            }
            catch (Exception ex) { _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "MainWindow"); }
        }

        private async void Add()
        {
            try
            {
                ProgressIsIndeterminate = true;
                int accessLevel = await _model.CheckUserAssync();
                App.ConnectionOpen = true; _mainWibdowViewModel.IsEnabledSaveUser = true;
                string level = String.Empty;
                App.AccessLeve = accessLevel;
                _mainWibdowViewModel.MenuUpdate();
                switch(accessLevel)
                {
                    case 1: { level = "Пользователь для просмотра"; break; }
                    case 2: { level = "Пользователь для добавления"; _mainWibdowViewModel.Menu.Add(_mainWibdowViewModel.MenuAdding); _mainWibdowViewModel.Menu.Add(_mainWibdowViewModel.MenuUnusedFiles); break; }
                    case 3: { level = "Админ"; _mainWibdowViewModel.Menu.Add(_mainWibdowViewModel.MenuAdding); _mainWibdowViewModel.Menu.Add(_mainWibdowViewModel.MenuUnusedFiles); break; }
                }
                _mainWibdowViewModel.Header = String.Format("Учет КД ({0}: {1})", level, UserName);
                _applicationMain.CurrentPage = new System.Windows.Controls.Page();
                _applicationMain.Clear();
                Properties.Settings.Default.SettingUserName = UserName;
                Properties.Settings.Default.SettingPassword = UserPassword;
                Properties.Settings.Default.Save();
                _model.SaveHistory();
                _model.UserPassword = String.Empty;
                MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null);
            }
            catch (Exception ex) { _applicationMain.DialogWindowShow(new ErrorNotificationMessage(ex.Message), "UserConnection"); }
            finally { ProgressIsIndeterminate = false; }
        }
    }
}
