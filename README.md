# 🔐 AuthService

![.NET](https://img.shields.io/badge/.NET-8-blue)
![License](https://img.shields.io/badge/license-MIT-green)
![Status](https://img.shields.io/badge/status-em%20desenvolvimento-yellow)

Serviço centralizado de autenticação desenvolvido em **.NET 8**, projetado para gerenciar usuários, perfis e segurança com **JWT** em múltiplas aplicações.

---

## 🚀 Objetivo

Este projeto tem como objetivo fornecer uma **API reutilizável de autenticação**, evitando a necessidade de recriar login, controle de usuários e geração de tokens em cada sistema.

Também serve como base de estudo para:

- ASP.NET Core Web API
- Entity Framework Core
- Migrations
- SQL Server
- JWT Authentication
- Refresh Token
- CORS
- Swagger

---

## 🛠️ Tecnologias

- .NET 8
- ASP.NET Core
- Entity Framework Core
- SQL Server
- JWT Bearer Authentication
- Swagger / OpenAPI

---

## 📁 Estrutura

```text
AuthService
└── AuthService.API
    ├── Controllers
    ├── Data
    ├── DTOs
    ├── Entities
    ├── Migrations
    ├── Program.cs
    ├── appsettings.json
    └── appsettings.Development.json
	
---

```markdown
## 🚀 Primeiros passos

### 1. Clonar o repositório

```bash
git clone https://github.com/SEU_USUARIO/auth-service-dotnet.git
cd auth-service-dotnet
	
dotnet restore	

dotnet run --project AuthService.API

dotnet run --project AuthService.API

https://localhost:xxxx/swagger

---

```markdown
## 🤝 Como contribuir

Contribuições são bem-vindas!

Siga os passos abaixo:

### 1. Faça um fork do projeto

### 2. Crie uma branch para sua alteração

```bash
git checkout -b minha-feature

dotnet ef migrations add NomeDaMigration --project AuthService.API
dotnet ef database update --project AuthService.API

