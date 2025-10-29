using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Endpoints;

public static class VagasEndpoints
{
    
    public static IEndpointRouteBuilder MapVagasEndpoints(this IEndpointRouteBuilder app)
    {
        
        var group = app.MapGroup("/api/vagas");

        
        group.MapGet("/", async (AppDbContext db) =>
        {
            try
            {
                var vagas = await db.Vagas
                    .AsNoTracking()
                    .Include(v => v.VeiculoAtual)
                    .ToListAsync();

                return Results.Ok(vagas);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Erro ao buscar vagas: {ex.Message}");
            }
        });


        group.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var vaga = await db.Vagas
                .Include(v => v.VeiculoAtual)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id);
            return vaga is null ? Results.NotFound() : Results.Ok(vaga);
        });

        group.MapPost("/", async (Vaga vaga, AppDbContext db) =>
        {
            var exists = await db.Vagas.AnyAsync(x => x.Numero == vaga.Numero);
            if (exists) return Results.Conflict($"Já existe a vaga número {vaga.Numero}.");
            vaga.Ocupada = false;
            db.Vagas.Add(vaga);
            await db.SaveChangesAsync();
            return Results.Created($"/api/vagas/{vaga.Id}", vaga);
        });

        group.MapPut("/{id:int}", async (int id, Vaga input, AppDbContext db) =>
        {
            var vaga = await db.Vagas.FindAsync(id);
            if (vaga is null) return Results.NotFound();

            if (vaga.Numero != input.Numero)
            {
                var numberInUse = await db.Vagas.AnyAsync(x => x.Numero == input.Numero && x.Id != id);
                if (numberInUse) return Results.Conflict($"Número {input.Numero} já em uso.");
            }

            vaga.Numero = input.Numero;
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
        {
            var vaga = await db.Vagas.FindAsync(id);
            if (vaga is null) return Results.NotFound();
            if (vaga.Ocupada) return Results.BadRequest("Não é possível excluir vaga ocupada.");
            db.Vagas.Remove(vaga);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        return app;
    }
}
