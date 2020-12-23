using System;
using System.Windows;
using System.Windows.Controls;

namespace KD.ViewModel.Common
{
    public interface IApplicationMain
    {
        Page CurrentPage { get; }
        Visibility VisibilityButtonBack { get; }
        event EventHandler PageChanged;
        event EventHandler VisibilityButtonBackChanged;
    }
}
