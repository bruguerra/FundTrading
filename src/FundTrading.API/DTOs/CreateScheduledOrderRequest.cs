using System.Text.Json.Serialization;

namespace FundTrading.API.DTOs
{
    public class CreateScheduledOrderRequest : CreateOrderRequest
    {
        [JsonPropertyName("data_agendamento")]
        public DateOnly DataAgendamento { get; set; }
    }
}