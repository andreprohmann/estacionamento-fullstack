
using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class Vaga
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public bool Ocupada { get; set; }
        public int? VeiculoAtualId { get; set; }
        public Veiculo? VeiculoAtual { get; set; }
    }
}
