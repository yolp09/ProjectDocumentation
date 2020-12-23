using KD.Command;
using KD.View.NotificationMessages;
using KD.ViewModel.Common;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace KD.ViewModel
{
    public class ItemMenu
    {
        private ApplicationMain _applicationMain;
        private SubItem _selectedItem;

        public string Header { get; private set; }
        public PackIconKind Icon { get; private set; }
        public List<SubItem> SubItems { get; private set; }
        public Page Screen { get; private set; }
        public Visibility VisibilityExpanderMenu { get; private set; }
        public Visibility VisibilityListViewItemMenu { get; private set; }

        public RelayCommand ClickListViewItemMenu { get { return new RelayCommand((obj) => { ListViewItemMenu(obj); }); } }

        public ItemMenu(ApplicationMain applicationMain, string header, List<SubItem> subItems, PackIconKind icon)
        {
            this._applicationMain = applicationMain;
            this.Header = header;
            this.SubItems = subItems;
            this.Icon = icon;

            this.VisibilityExpanderMenu = SubItems == null ? Visibility.Collapsed : Visibility.Visible;
            this.VisibilityListViewItemMenu = SubItems == null ? Visibility.Visible : Visibility.Collapsed;
        }

        public ItemMenu(ApplicationMain applicationMain, string header, Page screen, PackIconKind icon)
        {
            this._applicationMain = applicationMain;
            this.Header = header;
            this.Screen = screen;
            this.Icon = icon;

            this.VisibilityExpanderMenu = SubItems == null ? Visibility.Collapsed : Visibility.Visible;
            this.VisibilityListViewItemMenu = SubItems == null ? Visibility.Visible : Visibility.Collapsed;
        }

        public SubItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (value != null)
                {
                    _selectedItem = value;
                    if (App.ConnectionOpen || _selectedItem.Name == "Подключение к серверу" || _selectedItem.Name == "Выход из программы")
                    {
                        if (_selectedItem.Screen != null) { _applicationMain.Clear(); _applicationMain.NextPage(_selectedItem.Screen); }
                        if (_selectedItem.DialogWindow != null) _applicationMain.DialogWindowShow(_selectedItem.DialogWindow, "MainWindow");
                    }
                    else
                    {
                        _applicationMain.DialogWindowShow(new ErrorNotificationMessage("Нет подключения!!!"), "MainWindow");
                    }
                    _selectedItem = null;
                }
            }
        }

        private void ListViewItemMenu(object obj)
        {
            if (App.ConnectionOpen)
            {
                var page = (Page)obj;
                if (page != null) _applicationMain.NextPage(page);
            }
            else
            {
                _applicationMain.DialogWindowShow(new ErrorNotificationMessage("Нет подключения!!!"), "MainWindow");
            }
        }
    }
}
