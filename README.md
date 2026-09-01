# LedgerLite

A small, full-stack finance app for invoices, double-entry journals, and profit-and-loss reporting. Built as a readable reference project—not an accounting system pretending to be enterprise software.

![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-512BD4) ![React](https://img.shields.io/badge/React-19-149ECA) ![SQLite](https://img.shields.io/badge/SQLite-3-003B57) ![CI](https://github.com/erikstruga/ledgerlite/actions/workflows/ci.yml/badge.svg)

## What it does

- Tracks draft, sent, paid, and overdue invoices
- Records balanced debit/credit journal entries
- Calculates revenue, expenses, and net income by date range
- Ships with useful sample data, Swagger docs, and a responsive React UI
- Includes API integration tests, a UI smoke test, SQL DDL, Docker, and GitHub Actions

## Architecture

```text
React + TypeScript  ──HTTP──▶  ASP.NET Core minimal API  ──EF Core──▶  SQLite
     :5173 / :3000                 :5050 / :8080                       file
```

The API owns accounting validation and reporting. The UI uses seeded fallback data when opened without the API, which makes design work and demos painless.

## Quick start

The shortest route is Docker:

```bash
docker compose up --build
```

Open [http://localhost:3000](http://localhost:3000). Swagger is available through the API container at `/swagger` if you expose port `8080`, or run locally as below.

### Local development

Requirements: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0), Node 22+, and pnpm.

```bash
# terminal 1 — API on http://localhost:5050
dotnet run --project src/LedgerLite.Api

# terminal 2 — React on http://localhost:5173
cd src/LedgerLite.Web
pnpm install
pnpm dev
```

The Vite server proxies `/api` to the ASP.NET app. SQLite is created and seeded automatically on first run.

## API

| Method | Route | Purpose |
| --- | --- | --- |
| `GET` | `/api/invoices` | List invoices |
| `POST` | `/api/invoices` | Create an invoice |
| `PATCH` | `/api/invoices/{id}/status?status=Paid` | Change invoice status |
| `GET` | `/api/journal-entries` | List entries and lines |
| `POST` | `/api/journal-entries` | Create a balanced entry |
| `GET` | `/api/reports/profit-and-loss?from=2026-01-01&to=2026-12-31` | Calculate P&L |
| `GET` | `/health` | Health check |

Example journal entry:

```json
{
  "entryDate": "2026-09-01",
  "memo": "Invoice paid",
  "lines": [
    { "accountId": 1, "debit": 1200, "credit": 0 },
    { "accountId": 2, "debit": 0, "credit": 1200 }
  ]
}
```

## Database

EF Core creates the development database. A portable schema is also provided in [`database/schema.sql`](database/schema.sql). Money uses fixed-precision decimals; dates use ISO `DateOnly` values; foreign keys and accounting-side checks live close to the data.

## Tests

```bash
dotnet test
cd src/LedgerLite.Web && pnpm test
```

The API suite checks health, seeded invoices, rejection of unbalanced journals, and the P&L result. CI runs tests and the production web build on every push and pull request.

## Project layout

```text
src/LedgerLite.Api/       ASP.NET Core API + EF Core models
src/LedgerLite.Web/       React + TypeScript + Vite UI
tests/LedgerLite.Api.Tests/  xUnit integration tests
database/schema.sql       Standalone SQLite schema
.github/workflows/ci.yml  Build and test pipeline
```

## Scope

LedgerLite is intentionally small. Before using it for real books, add authentication, tenant isolation, audit history, migrations, tax handling, currency rules, invoice line items, and period locking.

Released under the [MIT License](LICENSE).
