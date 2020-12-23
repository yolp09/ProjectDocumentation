using KD.View.NotificationMessages;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace KD.ViewModel.Common
{
    public class ApplicationMain : IApplicationMain
    {
        private Stack<Page> _backStack;
        private Page _currentPage;
        private Visibility _visibilityButtonBack;

        public ApplicationMain()
        {
            _backStack = new Stack<Page>();
        }

        #region IApplicationMain

        public Page CurrentPage { get { return _currentPage; } set { _currentPage = value; PageChanged(this, EventArgs.Empty); } }
        public Visibility VisibilityButtonBack { get { return _visibilityButtonBack; } private set { _visibilityButtonBack = value; VisibilityButtonBackChanged(this, EventArgs.Empty); } }
        public event EventHandler PageChanged;
        public event EventHandler VisibilityButtonBackChanged;

        #endregion

        public void BackPage()
        {
            if (_backStack.Count > 0) ChangePage(_backStack.Pop());
            if (_backStack.Count > 1) VisibilityButtonBack = Visibility.Visible;
            else VisibilityButtonBack = Visibility.Collapsed;
        }

        public void NextPage(Page page)
        {
            if (page == null || CurrentPage == page) 
                return;
            if (CurrentPage != null) _backStack.Push(CurrentPage);

            if (_backStack.Count > 1) VisibilityButtonBack = Visibility.Visible;
            else VisibilityButtonBack = Visibility.Collapsed;

            ChangePage(page);
        }

        public void Clear()
        {
            VisibilityButtonBack = Visibility.Collapsed;
            _backStack.Clear();
        }

        public void ChangeVisibilityButton(Visibility value)
        {
            VisibilityButtonBack = value;
        }

        public async void DialogWindowShow(NotificationMessage notification, string identifier)
        {
            await MaterialDesignThemes.Wpf.DialogHost.Show(notification, identifier);
        }

        private void ChangePage(Page page)
        {
            CurrentPage = page;
        }
    }
}
