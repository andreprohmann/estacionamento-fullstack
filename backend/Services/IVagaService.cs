
using Estacionamento.Api.Dtos;

namespace Estacionamento.Api.Services
{
    public interface IVagaService
    {
        Task<VagaResponseDto> CreateAsync(VagaCreateDto dto);
        Task<IEnumerable<VagaResponseDto>> GetAllAsync(bool somenteAbertas = false);
        Task<VagaResponseDto?> GetByIdAsync(int id);
        Task<VagaResponseDto?> UpdateAsync(int id, VagaUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<VagaResponseDto?> CheckoutAsync(int id, VagaCheckoutDto dto);
    }
}
