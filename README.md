
# Estacionamento Fullstack 🚗

Sistema completo de gerenciamento de estacionamento, desenvolvido com tecnologias modernas de front-end e back-end.

## 📋 Descrição

Este projeto tem como objetivo oferecer uma solução para controle de entrada e saída de veículos em um estacionamento, com funcionalidades como cadastro de veículos, controle de vagas, cálculo de tempo de permanência e geração de cobranças.

## 🛠️ Tecnologias Utilizadas

### Backend
- **C# com ASP.NET Core**
- **Entity Framework Core**
- **MySQL** (banco de dados relacional)
- **API RESTful**

### Frontend
- **React.js**
- **Axios** (para requisições HTTP)
- **Bootstrap** (estilização responsiva)

## ⚙️ Funcionalidades

- Cadastro de veículos
- Registro de entrada e saída
- Cálculo automático de tempo de permanência
- Geração de cobrança
- Visualização de histórico
- Integração entre front-end e back-end via API

## 📦 Estrutura do Projeto

```
estacionamento-fullstack/
├── backEnd/
│   ├── Controllers/         # Implementação das funções de CRUD
│   │   └── VeiculoController.cs
│   ├── Models/              # Definição das entidades e classes de dados
│   │   └── Veiculo.cs
│   ├── Data/                # Configuração da conexão com o banco de dados
│   │   └── AppDbContext.cs
│   └── Program.cs           # Inicialização da aplicação e serviços
├── frontEnd/
│   ├── src/
│   │   ├── components/      # Componentes reutilizáveis da interface
│   │   ├── pages/           # Páginas principais da aplicação
│   │   ├── services/        # Integração com a API via Axios
│   │   │   └── api.js
│   │   └── App.js           # Composição geral da aplicação React
└── README.md
```

## 🔄 CRUD e Banco de Dados

### Backend
- As operações de **CRUD** estão implementadas em `backEnd/Controllers/VeiculoController.cs`, utilizando rotas HTTP como `GET`, `POST`, `PUT` e `DELETE`.
- A conexão com o banco de dados MySQL é configurada em `backEnd/Data/AppDbContext.cs`, utilizando o **Entity Framework Core** para mapeamento objeto-relacional.

### Frontend
- As chamadas à API são feitas em `frontEnd/src/services/api.js` usando **Axios**.
- Os dados recebidos da API são exibidos e manipulados nos componentes React localizados em `frontEnd/src/pages/`.

## 🚀 Como Executar

### Backend
```bash
cd backEnd
dotnet restore
dotnet run
```

### Frontend
```bash
cd frontEnd
npm install
npm start
```

## 🧪 Testes

Os testes podem ser adicionados utilizando frameworks como xUnit para o back-end e Jest para o front-end.

## 📄 Licença

Este projeto é de uso livre para fins educacionais.
