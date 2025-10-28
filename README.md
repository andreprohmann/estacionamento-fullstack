# Estacionamento (Fullstack)

Projeto completo **do zero** com back-end (.NET 8, EF Core, MySQL) e front-end (React + Vite + TS), cobrando **R$ 8,00 por cada 30 minutos** com **arredondamento para cima**.

## Rodando
1. `docker compose up -d` (sobe MySQL)
2. `cd backend && dotnet ef migrations add InitialCreate --output-dir Data/Migrations --context AppDbContext && dotnet ef database update --context AppDbContext && dotnet run`
3. `cd ../frontend && npm install && npm run dev`

Ajuste `frontend/.env` se a URL/porta da API mudar.
