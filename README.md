# Estacionamento — Fullstack (Minimal API .NET 8 + React + Vite + TS)

Este projeto implementa um **sistema de estacionamento** com:

- **Back-end**: .NET 8 **Minimal API**, **EF Core** (Pomelo/MySQL), Swagger, CORS  
- **Front-end**: **React + Vite + TypeScript**  
- **Regra de cobrança**: **R$ 8,00** a cada **30 minutos**, com **arredondamento para cima**  
- **Vínculo obrigatório** da **Vaga** com um **Veículo** (FK `VeiculoId`)

> **Observação**: As datas são persistidas em **UTC** no back-end. O front trabalha com `DateTimeOffset` gerado a partir do horário local e exibe no fuso do navegador.

---

## Sumário

1. Arquitetura e Fluxo  
2. Como rodar  
3. Banco de Dados e Modelo  
4. CRUD — O que cada endpoint faz  
5. Back-end — Estrutura de arquivos  
6. Front-end — Estrutura de arquivos  
7. Exemplos de requisições (cURL)  
8. Boas práticas, extensões e troubleshooting

---

## Arquitetura e Fluxo

- **Veículos** são cadastrados separadamente (placa única).  
- **Vagas** representam o **registro de estadia** (check-in/check-out) de **um veículo**.  
- No **check-in** criamos uma **Vaga** vinculada a um **Veículo** (`VeiculoId`).  
- No **check-out**, calculamos e gravamos `MinutosEstadia` e `ValorCobrado` (blocos de 30 min).  
- Enquanto a vaga está aberta, o **front** exibe o **valor previsto** (cálculo “live” feito no back-end).

---

## Como rodar

### 1) Banco de dados (MySQL via Docker)

```bash
docker compose up -d
```

Valores padrão (edite em `docker-compose.yml` e `backend/appsettings.json` se necessário):
- usuário root: `YourStrong@Password`  
- database: `estacionamento_db`  
- porta externa: `3306`

### 2) Back-end (Minimal API .NET 8)

```bash
cd backend
# (se precisar) dotnet tool install --global dotnet-ef
dotnet restore
dotnet ef migrations add InitialCreate --output-dir Data/Migrations --context AppDbContext
dotnet ef database update --context AppDbContext
dotnet run
```

- API: `http://localhost:5087` e `https://localhost:7087`  
- Swagger: `https://localhost:7087/swagger`

### 3) Front-end (React + Vite + TS)

```bash
cd ../frontend
npm install
npm run dev
```

- App: `http://localhost:5173`

> Se mudar a URL/porta da API, ajuste `frontend/.env`:
```env
VITE_API_BASE=http://localhost:5087
```

---

## Banco de Dados e Modelo

### Tabelas principais

- **Veiculos**
  - `Id` *(PK)*  
  - `Placa` *(única, obrigatória)*  
  - `Marca`, `Modelo` *(obrigatórias)*  
  - `Ano` *(1950–2100)*

- **Vagas**
  - `Id` *(PK)*  
  - `VeiculoId` *(FK para Veiculos)*  
  - `CheckInUtc` *(obrigatório, UTC)*  
  - `CheckOutUtc` *(nulo enquanto aberta)*  
  - `MinutosEstadia` *(preenchido no checkout)*  
  - `ValorCobrado` *(preenchido no checkout; precision 10,2)*

### Regra de faturamento

- **Preço por bloco**: R$ **8,00**  
- **Tamanho do bloco**: **30 minutos**  
- **Arredondamento** para cima (ex.: 31 min = 2 blocos)  
- **Fórmula**:
  - `minutosTotais = ceil((CheckOutUtc - CheckInUtc).TotalMinutes)`
  - `blocos = ceil(minutosTotais / 30)`
  - `valor = blocos * 8`

---

## CRUD — O que cada endpoint faz

### Veículos (`/api/veiculos`)
- `GET /api/veiculos?placa=ABC` — lista veículos, opcionalmente filtrando por placa (contém).  
- `GET /api/veiculos/{id}` — obtém um veículo.  
- `POST /api/veiculos` — cria veículo **(placa única)**.  
- `PUT /api/veiculos/{id}` — atualiza veículo; impede duplicidade de placa.  
- `DELETE /api/veiculos/{id}` — exclui veículo **sem histórico** de vagas (se tiver, bloqueia).

### Vagas (`/api/vagas`)
- `GET /api/vagas?abertas=true` — lista vagas (todas ou só abertas).  
  - Para vagas abertas, retorna **valor previsto** (cálculo até agora).  
- `GET /api/vagas/{id}` — obtém uma vaga + dados do veículo vinculado.  
- `POST /api/vagas` — cria **check-in**:
  - **Modo A**: informar `veiculoId` de um veículo existente.  
  - **Modo B (opcional)**: enviar `veiculoNovo` (placa única) para cadastrar veículo e já vincular.  
  - Impede **estadia aberta duplicada** para a mesma **placa**.  
- `PUT /api/vagas/{id}` — atualizar (opcional, regra mínima; não permite alterar depois do checkout).  
- `PUT /api/vagas/{id}/checkout` — **check-out**:
  - Valida `CheckOutUtc >= CheckInUtc`  
  - Gera `MinutosEstadia` e `ValorCobrado`  
- `DELETE /api/vagas/{id}` — remove o registro da vaga.

---

## Back-end — Estrutura de arquivos

```
backend/
  Estacionamento.Api.csproj         # Projeto .NET 8 (Estilo SDK)
  Program.cs                        # Minimal API, DI, CORS, Swagger, mapping de endpoints
  appsettings.json                  # ConnectionStrings e logging
  appsettings.Development.json      # Overrides para dev (conn string, etc.)

  Data/
    AppDbContext.cs                 # DbContext EF Core (Veiculos, Vagas)
    Migrations/                     # Gerado pelo EF (após migrations)

  Models/
    Veiculo.cs                      # Entidade Veiculo (Placa única)
    Vaga.cs                         # Entidade Vaga (FK VeiculoId, datas, valor)

  Dtos/
    VeiculoDtos.cs                  # DTOs: Create/Update/Response para Veiculo
    VagaDtos.cs                     # DTOs: Create/Update/Checkout/Response para Vaga

  Services/
    IVeiculoService.cs              # Interface para operações de veículos
    VeiculoService.cs               # Regras: placa única, bloqueios de exclusão, mapeamento DTO
    IVagaService.cs                 # Interface para operações de vagas
    VagaService.cs                  # Regras: check-in/out, cálculo blocos, validações e respostas
```

---

## Front-end — Estrutura de arquivos

```
frontend/
  package.json                 # Dependências, scripts Vite
  tsconfig.json                # Configuração TypeScript
  vite.config.ts               # Plugin React, porta 5173
  index.html                   # HTML raiz
  .env                         # VITE_API_BASE=http://localhost:5087

  src/
    main.tsx                   # Bootstrap React + Router
    App.tsx                    # Layout + rotas
    styles.css                 # Estilos básicos

    api/
      client.ts                # HTTP genérico (fetch) + endpoints da API

    types.ts                   # Tipos TS (Veiculo, Vaga, etc.)

    pages/
      VagasList.tsx            # Lista vagas; mostra abertas/fechadas, valor previsto/cobrado
      VagaForm.tsx             # Check-in (seleciona Veículo) / Editar
      VagaCheckout.tsx         # Checkout da estadia
```

---

## Exemplos de requisições (cURL)

### Criar veículo
```bash
curl -X POST http://localhost:5087/api/veiculos   -H "Content-Type: application/json"   -d '{
    "placa": "ABC1D23",
    "marca": "Fiat",
    "modelo": "Argo",
    "ano": 2022
  }'
```

### Check-in vinculando veículo existente
```bash
curl -X POST http://localhost:5087/api/vagas   -H "Content-Type: application/json"   -d '{ "veiculoId": 1 }'
```

### Checkout
```bash
curl -X PUT http://localhost:5087/api/vagas/1/checkout   -H "Content-Type: application/json"   -d '{}'
```

---

## Boas práticas e extensões
- Preço configurável via `appsettings.json`
- Validação de placa (Mercosul)
- Paginação e busca por placa
- Relatórios (período, receita)

---

## Troubleshooting
- **Access denied MySQL**: alinhar senha no `docker-compose.yml` e `appsettings.json`
- **EJSONPARSE no front**: confirmar `package.json` válido, sem BOM
- **Erro .csproj**: arquivo deve começar com `<Project ...>` sem caracteres invisíveis

---
