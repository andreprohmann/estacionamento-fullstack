using Backend.Data;
using Backend.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// CORS para o frontend local (Vite padrão 5173)
var frontendOrigin = "http://localhost:5173";
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
{
    p.WithOrigins(frontendOrigin)
    .AllowAnyHeader()
    .AllowAnyMethod();
}));

// DbContext + Pomelo MySQL
var cs = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseMySql(cs, ServerVersion.AutoDetect(cs)));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapVagasEndpoints();
app.MapVeiculosEndpoints();

// Seed/Migrate
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await Seed.InitializeAsync(db);
}

app.Run();
