using System.Text.Json.Serialization;

namespace FundTrading.API.DTOs
{
    public class CreateOrderRequest
    {
        [JsonPropertyName("id_cliente")]
        public int IdCliente { get; set; }

        [JsonPropertyName("id_fundo")]
        public int IdFundo { get; set; }

        [JsonPropertyName("tipo_operacao")]
        public string TipoOperacao { get; set; } = string.Empty;

        [JsonPropertyName("quantidade_cotas")]
        public int QuantidadeCotas { get; set; }
    }
}