using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public static class Seed
{
    public static async Task InitializeAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();

        if (!await db.Vagas.AnyAsync())
        {
            var vagas = Enumerable.Range(1, 20)
                .Select(n => new Vaga { Numero = n, Ocupada = false })
                .ToList();
            await db.Vagas.AddRangeAsync(vagas);
            await db.SaveChangesAsync();
        }
    }
}
