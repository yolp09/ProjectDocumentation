
namespace KD.View.NotificationMessages
{
    public class ErrorNotificationMessage : NotificationMessage
    {
        public ErrorNotificationMessage(string msg)
        {
            Title = "Ошибка";
            Message = msg;
        }
    }
}
