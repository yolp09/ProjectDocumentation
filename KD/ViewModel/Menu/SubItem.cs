using KD.View.NotificationMessages;
using System.Windows.Controls;

namespace KD.ViewModel
{
    public class SubItem
    {
        public string Name { get; private set; }
        public Page Screen { get; private set; }
        public NotificationMessage DialogWindow { get; private set; }

        public SubItem(string name, Page screen = null, NotificationMessage dialogWindow = null)
        {
            this.Name = name;
            this.Screen = screen;
            this.DialogWindow = dialogWindow;
        }
    }
}
