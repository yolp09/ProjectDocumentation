
namespace KD.View.NotificationMessages
{
    public class InfoNotificationMessage : NotificationMessage
    {
        public InfoNotificationMessage(string msg)
        {
            Title = "Сообщение";
            Message = msg;
        }
    }
}
