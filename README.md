# 🚗 Vehicle Management API

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-blue?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Redis](https://img.shields.io/badge/Redis-Cache-red?logo=redis&logoColor=white)](https://redis.io/)
[![Swagger](https://img.shields.io/badge/Swagger-Docs-green?logo=swagger&logoColor=white)](https://swagger.io/)
[![Docker](https://img.shields.io/badge/Container-Docker-blue?logo=docker&logoColor=white)](https://www.docker.com/)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg)](LICENSE)

---

API completa para **gerenciamento de veículos**, desenvolvida em **C# .NET 8.0**, com suporte a **cache Redis** e **invalidação automática** após operações de escrita.

---

## 👩‍💻 Desenvolvedores

- **Cíntia Cristina Braga Angelo** — RM552253  
- **Henrique Mosseri** — RM552240  

---

## 📋 Descrição do Projeto

A **Vehicle Management API** permite **criar, listar, atualizar e remover veículos**, implementando **cache inteligente via Redis**, com **fallback automático para MemoryCache**.  
Cada operação de escrita (**POST**, **PUT**, **DELETE**) **invalida automaticamente o cache**, garantindo dados atualizados e melhor desempenho em leitura.

---

## 🚀 Funcionalidades

| Método | Endpoint | Descrição | Cache |
|:-------|:----------|:-----------|:-------|
| **GET** | `/api/vehicles` | Lista todos os veículos | ✅ Redis |
| **GET** | `/api/vehicles/{id}` | Busca veículo por ID | ✅ Redis |
| **POST** | `/api/vehicles` | Cria novo veículo | 🗑️ Invalida cache |
| **PUT** | `/api/vehicles/{id}` | Atualiza veículo existente | 🗑️ Invalida cache |
| **DELETE** | `/api/vehicles/{id}` | Remove veículo | 🗑️ Invalida cache |

🔄 **Cache Automático:** Redis + fallback para MemoryCache  
🧹 **Invalidação Automática:** Após POST, PUT e DELETE  

---

## 🛠️ Tecnologias Utilizadas

- **.NET 8.0**
- **Entity Framework Core (InMemory Database)**
- **Redis** (cache distribuído)
- **Podman / Docker** (para o container Redis)
- **Swagger** (documentação interativa da API)

---

## 📁 Estrutura do Projeto

```
VehicleManagementAPI/
├── Controllers/
│   └── VehiclesController.cs
├── Models/
│   └── Vehicle.cs
├── Data/
│   └── AppDbContext.cs
├── Services/
│   ├── ICacheService.cs
│   ├── CacheService.cs
│   ├── IVehicleService.cs
│   └── VehicleService.cs
├── Program.cs
└── appsettings.json
```

---

## ⚙️ Como Executar o Projeto

### 📦 Pré-requisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download)
- [Podman](https://podman.io/) ou [Docker](https://www.docker.com/)
- Redis em container

### 1️⃣ Clonar o Repositório
```bash
git clone <url-do-repositorio>
cd VehicleManagementAPI
```

### 2️⃣ Iniciar o Redis
**Com Docker:**
```bash
docker run -d -p 6379:6379 --name redis-cache redis
```

**Com Podman (utilizado neste projeto):**
```bash
podman run -d -p 6379:6379 --name redis-cache docker.io/redis
```

### 3️⃣ Executar a API
```bash
dotnet run
```

A API ficará disponível em:
```
http://localhost:5158
```

### 4️⃣ Acessar a Documentação (Swagger)
```
http://localhost:5158/swagger
```

---

## 📊 Modelo de Dados

**Vehicle**
```json
{
  "brand": "string",
  "model": "string",
  "year": 0,
  "plate": "string"
}
```

**Regras de Validação**
- `brand`, `model`, `year` e `plate` são obrigatórios  
- `plate` deve ser única  
- `id` é autoincrementável  

---

## 🔌 Endpoints da API

### **GET /api/vehicles**
Retorna todos os veículos.  
Cache: ✅ Redis (10 minutos)

**Exemplo de resposta:**
```json
[
  {
    "id": 1,
    "brand": "Toyota",
    "model": "Corolla",
    "year": 2023,
    "plate": "ABC1234"
  }
]
```

---

### **GET /api/vehicles/{id}**
Retorna um veículo específico.  
Cache: ✅ Redis (10 minutos)

---

### **POST /api/vehicles**
Cria um novo veículo.  
Cache: 🗑️ Invalida após criação.

**Exemplo:**
```json
{
  "brand": "Honda",
  "model": "Civic",
  "year": 2024,
  "plate": "XYZ5678"
}
```

---

### **PUT /api/vehicles/{id}**
Atualiza um veículo existente.  
Cache: 🗑️ Invalida após atualização.

**Exemplo:**
```json
{
  "id": 1,
  "brand": "Toyota",
  "model": "Corolla XEi",
  "year": 2024,
  "plate": "ABC1234"
}
```

---

### **DELETE /api/vehicles/{id}**
Remove um veículo.  
Cache: 🗑️ Invalida após remoção.

---

## 🧪 Testando a API

Pode-se utilizar **Thunder Client** ou **Postman**.

**Criar veículo**
```
POST http://localhost:5158/api/vehicles
Content-Type: application/json
```
```json
{
  "brand": "Toyota",
  "model": "Corolla",
  "year": 2023,
  "plate": "ABC1234"
}
```

**Listar veículos**
```
GET http://localhost:5158/api/vehicles
```

**Buscar por ID**
```
GET http://localhost:5158/api/vehicles/1
```

**Atualizar veículo**
```
PUT http://localhost:5158/api/vehicles/1
Content-Type: application/json
```
```json
{
  "id": 1,
  "brand": "Toyota",
  "model": "Corolla XEi",
  "year": 2024,
  "plate": "ABC1234"
}
```

**Remover veículo**
```
DELETE http://localhost:5158/api/vehicles/1
```

---

## 🔄 Sistema de Cache

### 🧱 Arquitetura
- **Redis** → Cache primário  
- **MemoryCache** → Fallback automático  
- **Invalidação automática** → Após POST, PUT e DELETE  

### ⚙️ Fluxo de Comportamento
1️⃣ Primeira requisição **GET** → busca no banco e salva no cache  
2️⃣ Próximas requisições → lidas diretamente do cache  
3️⃣ Após **POST/PUT/DELETE** → cache é limpo  
4️⃣ Próxima **GET** → recarrega cache com dados atualizados  
