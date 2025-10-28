
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Estacionamento.Api.Models
{
    [Index(nameof(Placa))]
    public class Vaga
    {
        public int Id { get; set; }

        [Required, StringLength(10)]
        public string Placa { get; set; } = default!;

        [Required, StringLength(60)]
        public string Marca { get; set; } = default!;

        [Required, StringLength(60)]
        public string Modelo { get; set; } = default!;

        [Range(1950, 2100)]
        public int Ano { get; set; }

        [Required]
        public DateTime CheckInUtc { get; set; }
        public DateTime? CheckOutUtc { get; set; }

        public int? MinutosEstadia { get; set; }

        [Precision(10, 2)]
        public decimal? ValorCobrado { get; set; }
    }
}
