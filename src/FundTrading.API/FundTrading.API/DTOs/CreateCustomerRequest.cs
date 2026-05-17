using System.Text.Json.Serialization;

namespace FundTrading.API.DTOs
{
    public class CreateCustomerRequest
    {
        [JsonPropertyName("nome")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("documento")]
        public string Document { get; set; } = string.Empty;

        [JsonPropertyName("saldo_disponivel")]
        public decimal AvailableBalance { get; set; }
    }
}