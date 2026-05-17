using System.Text.Json.Serialization;

namespace FundTrading.API.DTOs
{
    public class CreateInvestmentFundRequest
    {
        [JsonPropertyName("nome")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("horario_corte")]
        public TimeOnly CutoffTime { get; set; }

        [JsonPropertyName("valor_cota")]
        public decimal SharePrice { get; set; }

        [JsonPropertyName("valor_minimo_aporte")]
        public decimal MinimumContributionAmount { get; set; }

        [JsonPropertyName("valor_minimo_permanencia")]
        public decimal MinimumRemainingBalance { get; set; }

        [JsonPropertyName("limite_capacidade")]
        public decimal CapacityLimit { get; set; }

        [JsonPropertyName("status_captacao")]
        public string Status { get; set; } = string.Empty;
    }
}