using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Endpoints;

public static class VeiculosEndpoints
{
    public static IEndpointRouteBuilder MapVeiculosEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/veiculos");

        group.MapGet("/", async (AppDbContext db) =>
            Results.Ok(await db.Veiculos
                .AsNoTracking()
                .Include(v => v.Vaga)
                .ToListAsync()));

        group.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var veiculo = await db.Veiculos
                .Include(v => v.Vaga)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id);
            return veiculo is null ? Results.NotFound() : Results.Ok(veiculo);
        });

        group.MapPost("/", async (Veiculo veiculo, AppDbContext db) =>
        {
            veiculo.Placa = veiculo.Placa.Trim().ToUpperInvariant();
            var exists = await db.Veiculos.AnyAsync(x => x.Placa == veiculo.Placa);
            if (exists) return Results.Conflict($"Placa {veiculo.Placa} já cadastrada.");
            veiculo.VagaId = null;
            db.Veiculos.Add(veiculo);
            await db.SaveChangesAsync();
            return Results.Created($"/api/veiculos/{veiculo.Id}", veiculo);
        });

        group.MapPut("/{id:int}", async (int id, Veiculo input, AppDbContext db) =>
        {
            var veiculo = await db.Veiculos.FindAsync(id);
            if (veiculo is null) return Results.NotFound();

            var newPlaca = input.Placa.Trim().ToUpperInvariant();
            if (veiculo.Placa != newPlaca)
            {
                var plateInUse = await db.Veiculos.AnyAsync(x => x.Placa == newPlaca && x.Id != id);
                if (plateInUse) return Results.Conflict($"Placa {newPlaca} já cadastrada.");
                veiculo.Placa = newPlaca;
            }
            veiculo.Modelo = input.Modelo;
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
        {
            var veiculo = await db.Veiculos.FindAsync(id);
            if (veiculo is null) return Results.NotFound();
            if (veiculo.VagaId is not null) return Results.BadRequest("Não é possível excluir veículo que está ocupando uma vaga.");
            db.Veiculos.Remove(veiculo);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapPost("/{id:int}/ocupar/{vagaId:int}", async (int id, int vagaId, AppDbContext db) =>
        {
            var veiculo = await db.Veiculos.FindAsync(id);
            if (veiculo is null) return Results.NotFound("Veículo não encontrado.");

            var vaga = await db.Vagas.FindAsync(vagaId);
            if (vaga is null) return Results.NotFound("Vaga não encontrada.");
            if (vaga.Ocupada) return Results.BadRequest("Vaga já está ocupada.");
            if (veiculo.VagaId is not null) return Results.BadRequest("Veículo já está ocupando uma vaga.");

            veiculo.VagaId = vagaId;
            vaga.Ocupada = true;
            await db.SaveChangesAsync();
            return Results.Ok(new { message = $"Veículo {veiculo.Placa} ocupou a vaga {vaga.Numero}." });
        });

        group.MapPost("/{id:int}/liberar", async (int id, AppDbContext db) =>
        {
            var veiculo = await db.Veiculos.Include(v => v.Vaga).FirstOrDefaultAsync(v => v.Id == id);
            if (veiculo is null) return Results.NotFound("Veículo não encontrado.");
            if (veiculo.VagaId is null) return Results.BadRequest("Veículo não está ocupando vaga.");

            var vaga = await db.Vagas.FindAsync(veiculo.VagaId);
            if (vaga is not null) vaga.Ocupada = false;

            veiculo.VagaId = null;
            await db.SaveChangesAsync();
            return Results.Ok(new { message = $"Veículo {veiculo.Placa} liberou a vaga {vaga?.Numero}." });
        });

        return app;
    }
}
