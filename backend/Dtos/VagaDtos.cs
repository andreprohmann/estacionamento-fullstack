
using System.ComponentModel.DataAnnotations;

namespace Estacionamento.Api.Dtos
{
    public class VagaCreateDto
    {
        [Required, StringLength(10)] public string Placa { get; set; } = default!;
        [Required, StringLength(60)] public string Marca { get; set; } = default!;
        [Required, StringLength(60)] public string Modelo { get; set; } = default!;
        [Range(1950, 2100)] public int Ano { get; set; }
        public DateTimeOffset? CheckIn { get; set; }
    }

    public class VagaUpdateDto
    {
        [Required, StringLength(10)] public string Placa { get; set; } = default!;
        [Required, StringLength(60)] public string Marca { get; set; } = default!;
        [Required, StringLength(60)] public string Modelo { get; set; } = default!;
        [Range(1950, 2100)] public int Ano { get; set; }
    }

    public class VagaCheckoutDto
    {
        public DateTimeOffset? CheckOut { get; set; }
    }

    public class VagaResponseDto
    {
        public int Id { get; set; }
        public string Placa { get; set; } = default!;
        public string Marca { get; set; } = default!;
        public string Modelo { get; set; } = default!;
        public int Ano { get; set; }
        public DateTime CheckInUtc { get; set; }
        public DateTime? CheckOutUtc { get; set; }
        public int? MinutosEstadia { get; set; }
        public decimal? ValorCobrado { get; set; }
        public int? BlocosDe30Min { get; set; }
        public decimal? ValorPrevisto { get; set; }
    }
}
