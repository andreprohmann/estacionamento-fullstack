
using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class Veiculo
    {
        public int Id { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string? Modelo { get; set; }
        public int? VagaId { get; set; }

        [JsonIgnore]
        public Vaga? Vaga { get; set; }
    }
}
