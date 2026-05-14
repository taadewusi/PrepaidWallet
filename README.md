# Prepaid Wallet API

A **.NET 8 Web API** for managing prepaid user accounts and balances, backed by **MS SQL Server**.
Operators can credit (increase) and debit (reduce) user balances with full audit trail.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 8 |
| Framework | ASP.NET Core Web API |
| ORM | Entity Framework Core 8 |
| Database | MS SQL Server |
| Docs | Swagger / OpenAPI |

---

## Project Structure

```
PrepaidWallet/
├── Controllers/
│   ├── UsersController.cs          # CRUD for prepaid users
│   ├── BalanceController.cs        # Credit / debit operations
│   └── TransactionsController.cs   # Audit trail / history
├── Data/
│   └── AppDbContext.cs             # EF Core DbContext + seed data
├── DTOs/
│   └── PrepaidDTOs.cs              # Request/Response models
├── Migrations/                     # EF Core migrations
├── Models/
│   ├── PrepaidUser.cs              # PrepaidUsers table
│   └── BalanceTransaction.cs       # BalanceTransactions audit table
├── Services/
│   ├── IPrepaidUserService.cs      # Service interface
│   └── PrepaidUserService.cs       # Business logic
├── appsettings.json
└── Program.cs
```

---

## Database Schema

### `PrepaidUsers`
| Column | Type | Notes |
|---|---|---|
| Id | int | PK, identity |
| FullName | nvarchar(150) | Required |
| PhoneNumber | nvarchar(20) | Required, unique |
| Email | nvarchar(150) | Unique (nullable) |
| Balance | decimal(18,2) | Default 0.00 |
| IsActive | bit | Default true |
| CreatedAt | datetime2 | Auto |
| UpdatedAt | datetime2 | Auto |

### `BalanceTransactions` *(audit log)*
| Column | Type | Notes |
|---|---|---|
| Id | int | PK, identity |
| PrepaidUserId | int | FK → PrepaidUsers |
| TransactionType | int | 1=Credit, 2=Debit |
| Amount | decimal(18,2) | |
| BalanceBefore | decimal(18,2) | |
| BalanceAfter | decimal(18,2) | |
| Remark | nvarchar(500) | Optional |
| OperatorName | nvarchar(100) | Required |
| CreatedAt | datetime2 | Auto |

---

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- MS SQL Server (Express or full)
- Visual Studio 2022 / VS Code / Rider

### 1. Clone & configure

```bash
git clone <repo-url>
cd PrepaidWallet
```

Edit `appsettings.json` with your SQL Server connection:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=PrepaidWalletDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**SQL Server Authentication example:**
```json
"DefaultConnection": "Server=localhost;Database=PrepaidWalletDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
```

### 2. Run (migrations apply automatically on startup)

```bash
dotnet run
```

The app will:
1. Create the `PrepaidWalletDb` database
2. Run migrations (create tables)
3. Seed 3 sample users
4. Start on `https://localhost:5001`

### 3. Open Swagger UI

Navigate to `https://localhost:5001` — Swagger loads at the root.

---

## API Endpoints

### Users

| Method | URL | Description |
|---|---|---|
| GET | `/api/users` | List all users |
| GET | `/api/users/{id}` | Get user by ID |
| POST | `/api/users` | Create new user |
| PUT | `/api/users/{id}` | Update user profile |
| DELETE | `/api/users/{id}` | Deactivate user |

### Balance Operations *(operator actions)*

| Method | URL | Description |
|---|---|---|
| POST | `/api/users/{id}/balance/credit` | **Increase** balance |
| POST | `/api/users/{id}/balance/debit` | **Reduce** balance |

### Transactions

| Method | URL | Description |
|---|---|---|
| GET | `/api/transactions` | All transactions |
| GET | `/api/users/{id}/transactions` | Transactions for one user |

---

## Example Requests

### Create a user
```http
POST /api/users
Content-Type: application/json

{
  "fullName": "John Doe",
  "phoneNumber": "08011223344",
  "email": "john.doe@email.com",
  "initialBalance": 1000.00
}
```

### Credit (increase) balance
```http
POST /api/users/1/balance/credit
Content-Type: application/json

{
  "amount": 5000.00,
  "operatorName": "Admin",
  "remark": "Top-up via bank transfer"
}
```

### Debit (reduce) balance
```http
POST /api/users/1/balance/debit
Content-Type: application/json

{
  "amount": 200.00,
  "operatorName": "Admin",
  "remark": "Service charge"
}
```

### Sample response
```json
{
  "success": true,
  "message": "Balance credited successfully. New balance: 6,000.00",
  "data": {
    "userId": 1,
    "fullName": "Adaeze Okonkwo",
    "previousBalance": 1000.00,
    "newBalance": 6000.00,
    "transactionType": "Credit",
    "amount": 5000.00,
    "remark": "Top-up via bank transfer",
    "operatorName": "Admin",
    "timestamp": "2024-05-12T10:30:00Z"
  }
}
```

---

## Manual Migrations (optional)

If you prefer to run migrations manually instead of auto-apply:

```bash
dotnet ef database update
```

To add a new migration after model changes:
```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

---

## Business Rules

- Balance **cannot go below zero** — debit will return 400 if insufficient
- **Inactive users** cannot be credited or debited
- Phone number and email are **unique** across all users
- Every balance change creates an **immutable audit record** in `BalanceTransactions`
- Amount must always be **greater than zero**
- `OperatorName` is **required** for all balance adjustments

---

## Running with SQL Server in Docker (optional)

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1433:1433 --name mssql -d mcr.microsoft.com/mssql/server:2022-latest
```

Then update connection string:
```
Server=localhost,1433;Database=PrepaidWalletDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;
```
