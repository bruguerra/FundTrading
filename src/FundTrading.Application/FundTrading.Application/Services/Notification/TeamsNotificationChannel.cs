using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace FundTrading.Application.Services.Notification
{
    public class TeamsNotificationChannel : INotificationChannel
    {
        private readonly HttpClient _httpClient;
        private readonly TeamsSettings _settings;

        public TeamsNotificationChannel(HttpClient httpClient, TeamsSettings settings)
        {
            _httpClient = httpClient;
            _settings = settings;
        }

        public async Task SendAsync(NotificationMessage message)
        {
            var url = _settings.WebhookUrl;

            var payload = new
            {
                text =
                    $"[{message.Type}] {message.Title}\n" +
                    $"{message.Message}\n" +
                    $"CorrelationId: {message.CorrelationId}"
            };

            await _httpClient.PostAsJsonAsync(url, payload);
        }
    }

    public interface INotificationChannel
    {
        Task SendAsync(NotificationMessage message);
    }
}
