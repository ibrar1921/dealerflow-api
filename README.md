<div align="center">

# 🏘️ DealerFlow API
### A modular dealer and property-listing backend

**C# · ASP.NET Core 8 · Entity Framework Core · SQLite · REST · Docker**

[![.NET](https://img.shields.io/badge/.NET-8-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![CI](https://github.com/ibrar1921/dealerflow-api/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ibrar1921/dealerflow-api/actions/workflows/dotnet.yml)
[![SQLite](https://img.shields.io/badge/Database-SQLite-003B57?logo=sqlite&logoColor=white)](https://www.sqlite.org/)

[Quick start](#quick-start) · [REST API](#api-endpoints) · [Docker](#docker) · [Testing](#testing)

</div>

---

## 💡 Overview

**DealerFlow** is a full-stack-backend portfolio project inspired by dealer-management workflows from Ibrar Pervaiz's professional web-development background. It is a **new, independent implementation**, not a copy of any former employer's application.

This version focuses on strong **backend fundamentals**: relational entities, data validation, REST endpoints, persistence and lifecycle management.

### ✅ Implemented features

- **Dealers:** create, list and retrieve dealer profiles.
- **Property listings:** create and retrieve listings with foreign-key relationships.
- **Search/filter:** filter listings by dealer ID or status.
- **Workflow logic:** validate state transitions `draft → published → closed`, with optional direct closure from draft.
- **Data persistence:** Entity Framework Core using SQLite with indexes for common query paths.
- **Developer ergonomics:** JSON validation errors, health route, HTTP request examples, Docker setup.
- **CI/testing:** .NET build and API-level integration tests (compiled by the workflow).

## 🧱 Architecture

```text
HTTP Client
    │
    ▼
ASP.NET Core Minimal REST API (Program.cs)
    │
    ├── Dealer endpoints
    ├── Listing endpoints
    ├── Validation + state transitions
    │
    ▼
EF Core DbContext + Models
    │
    ▼
SQLite database file (dealerflow.db)
```

## ⚡ Quick start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Optionally Docker

```bash
git clone https://github.com/ibrar1921/dealerflow-api.git
cd dealerflow-api
dotnet restore
dotnet run --urls http://localhost:5070
```

Open another terminal:

```bash
curl http://localhost:5070/health
```

The SQLite database is created locally on first start. It is ignored by Git.

## 🌐 API endpoints

| HTTP | Endpoint | Description |
|:---|:---|:---|
| `GET` | `/health` | Service health |
| `GET` | `/api/dealers` | List dealers |
| `GET` | `/api/dealers/{id}` | Get dealer |
| `POST` | `/api/dealers` | Create dealer |
| `GET` | `/api/listings` | Get listings, optionally filtered |
| `GET` | `/api/listings/{id}` | Get listing |
| `POST` | `/api/listings` | Create listing |
| `PATCH` | `/api/listings/{id}/status` | Change listing state |

### Create a dealer

```bash
curl -X POST http://localhost:5070/api/dealers \
  -H "Content-Type: application/json" \
  -d '{"name":"Northside Property Partners","region":"London"}'
```

The response contains a newly generated **dealer ID**. Use that real ID when creating a listing:

```json
{
  "dealerId": "REPLACE_WITH_DEALER_UUID",
  "title": "Modern two-bedroom flat",
  "price": 285000
}
```

Send that JSON to `POST /api/listings`. Then make it public in the demo workflow with `PATCH /api/listings/{id}/status` and `{"status":"published"}`. See [API examples and validation](docs/api.md).

## 🐳 Docker

```bash
docker build -t dealerflow-api .
docker run --rm -p 8080:8080 dealerflow-api
```

Visit `http://localhost:8080/health`. SQLite data inside the container is ephemeral unless you mount persistent storage.

## 🧪 Testing

A separate xUnit integration-test project covers the health route, invalid input, dealer creation and listing workflow.

```bash
dotnet restore tests/DealerFlow.Tests/DealerFlow.Tests.csproj
dotnet test tests/DealerFlow.Tests/DealerFlow.Tests.csproj --configuration Release
```

A GitHub Actions workflow also restores, builds and runs tests on pushes and pull requests.

## 📁 Project layout

```text
dealerflow-api/
├── Program.cs
├── Models.cs
├── DealerFlow.Api.csproj
├── appsettings.json
├── DealerFlow.http
├── Dockerfile
├── docs/api.md
├── tests/DealerFlow.Tests/
│   ├── DealerFlow.Tests.csproj
│   └── DealerApiTests.cs
└── .github/workflows/dotnet.yml
```

## 🔐 Security, limitations and next steps

**Development prototype only:** there is no authentication, authorization, access-level model, request throttling, tenant isolation or production migration strategy. **Do not deploy publicly with real data.** SQLite is appropriate for this local demo, not a substitute for production architecture decisions.

Planned enhancements:
- [ ] Authentication and role-based dealer/admin authorization
- [ ] EF Core migrations and PostgreSQL support
- [ ] Pagination, sorting, structured logs and OpenAPI docs
- [ ] Angular front end and optional SignalR notifications
- [ ] Integration tests for authorization and concurrent writes

## 👤 Author

**Ibrar Pervaiz** — Software Engineer, K.M.S Business Services Ltd. | MSc Computer Science, York St John University

[GitHub](https://github.com/ibrar1921) · [LinkedIn](https://www.linkedin.com/in/ibrar-pervaiz-0b1a2a158/)

> This is an original public learning project; it does not include private employer code or customer data.
