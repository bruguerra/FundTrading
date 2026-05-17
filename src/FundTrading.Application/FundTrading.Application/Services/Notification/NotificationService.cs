namespace FundTrading.Application.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly IEnumerable<INotificationChannel> _channels;

        public NotificationService(IEnumerable<INotificationChannel> channels)
        {
            _channels = channels;
        }

        public async Task NotifyAsync(NotificationMessage message)
        {
            foreach (var channel in _channels)
            {
                await channel.SendAsync(message);
            }
        }
    }

    public interface INotificationService
    {
        Task NotifyAsync(NotificationMessage message);
    }
}
