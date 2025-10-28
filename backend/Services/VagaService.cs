
using Estacionamento.Api.Data;
using Estacionamento.Api.Dtos;
using Estacionamento.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Estacionamento.Api.Services
{
    public class VagaService : IVagaService
    {
        private const decimal PRECO_POR_BLOCO = 8m;
        private const int TAMANHO_BLOCO_MIN = 30;
        private readonly AppDbContext _db;
        public VagaService(AppDbContext db) { _db = db; }

        public async Task<VagaResponseDto> CreateAsync(VagaCreateDto dto)
        {
            var existeAberta = await _db.Vagas.AnyAsync(v => v.Placa == dto.Placa && v.CheckOutUtc == null);
            if (existeAberta) throw new InvalidOperationException($"Já existe uma estadia aberta para a placa {dto.Placa}.");

            var checkInUtc = (dto.CheckIn?.UtcDateTime) ?? DateTime.UtcNow;
            var vaga = new Vaga
            {
                Placa = dto.Placa.Trim().ToUpperInvariant(),
                Marca = dto.Marca.Trim(),
                Modelo = dto.Modelo.Trim(),
                Ano = dto.Ano,
                CheckInUtc = checkInUtc
            };
            _db.Vagas.Add(vaga);
            await _db.SaveChangesAsync();
            return ToResponseDto(vaga, false);
        }

        public async Task<IEnumerable<VagaResponseDto>> GetAllAsync(bool somenteAbertas = false)
        {
            var query = _db.Vagas.AsNoTracking().OrderByDescending(v => v.Id).AsQueryable();
            if (somenteAbertas) query = query.Where(v => v.CheckOutUtc == null);
            var list = await query.ToListAsync();
            return list.Select(v => ToResponseDto(v, true));
        }

        public async Task<VagaResponseDto?> GetByIdAsync(int id)
        {
            var vaga = await _db.Vagas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
            return vaga is null ? null : ToResponseDto(vaga, true);
        }

        public async Task<VagaResponseDto?> UpdateAsync(int id, VagaUpdateDto dto)
        {
            var vaga = await _db.Vagas.FirstOrDefaultAsync(v => v.Id == id);
            if (vaga is null) return null;
            if (vaga.CheckOutUtc is not null) throw new InvalidOperationException("Não é possível alterar uma estadia já encerrada.");

            vaga.Placa = dto.Placa.Trim().ToUpperInvariant();
            vaga.Marca = dto.Marca.Trim();
            vaga.Modelo = dto.Modelo.Trim();
            vaga.Ano = dto.Ano;
            await _db.SaveChangesAsync();
            return ToResponseDto(vaga, true);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var vaga = await _db.Vagas.FirstOrDefaultAsync(v => v.Id == id);
            if (vaga is null) return false;
            _db.Vagas.Remove(vaga);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<VagaResponseDto?> CheckoutAsync(int id, VagaCheckoutDto dto)
        {
            var vaga = await _db.Vagas.FirstOrDefaultAsync(v => v.Id == id);
            if (vaga is null) return null;
            if (vaga.CheckOutUtc is not null) throw new InvalidOperationException("Esta estadia já foi encerrada.");

            var checkOutUtc = (dto.CheckOut?.UtcDateTime) ?? DateTime.UtcNow;
            if (checkOutUtc < vaga.CheckInUtc) throw new InvalidOperationException("CheckOut não pode ser anterior ao CheckIn.");
            var (minutos, blocos, valor) = CalcularEstadia(vaga.CheckInUtc, checkOutUtc);
            vaga.CheckOutUtc = checkOutUtc;
            vaga.MinutosEstadia = minutos;
            vaga.ValorCobrado = valor;
            await _db.SaveChangesAsync();
            return ToResponseDto(vaga, false);
        }

        private static (int minutos, int blocos, decimal valor) CalcularEstadia(DateTime checkInUtc, DateTime checkOutUtc)
        {
            var totalMin = (int)Math.Ceiling((checkOutUtc - checkInUtc).TotalMinutes);
            var blocos = (int)Math.Ceiling(totalMin / (double)TAMANHO_BLOCO_MIN);
            var valor = blocos * PRECO_POR_BLOCO;
            return (totalMin, blocos, valor);
        }

        private VagaResponseDto ToResponseDto(Vaga v, bool incluirCalculoPrevisto)
        {
            int? blocosPrev = null; decimal? valorPrev = null;
            if (incluirCalculoPrevisto)
            {
                var fim = v.CheckOutUtc ?? DateTime.UtcNow;
                var (min, blocos, valor) = CalcularEstadia(v.CheckInUtc, fim);
                blocosPrev = blocos;
                valorPrev = v.CheckOutUtc is null ? valor : v.ValorCobrado;
            }
            return new VagaResponseDto
            {
                Id = v.Id,
                Placa = v.Placa,
                Marca = v.Marca,
                Modelo = v.Modelo,
                Ano = v.Ano,
                CheckInUtc = v.CheckInUtc,
                CheckOutUtc = v.CheckOutUtc,
                MinutosEstadia = v.MinutosEstadia,
                ValorCobrado = v.ValorCobrado,
                BlocosDe30Min = blocosPrev,
                ValorPrevisto = valorPrev
            };
        }
    }
}
