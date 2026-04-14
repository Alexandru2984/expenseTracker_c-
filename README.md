# Expense Tracker

Aplicație full-stack pentru urmărirea abonamentelor și cheltuielilor recurente.

## Stack

| Layer | Tehnologie |
|---|---|
| Backend | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 (Code-First) |
| Baza de date | PostgreSQL 16 |
| Frontend | Vue 3 + Vite + Tailwind CSS |

## Structura proiectului

```
expense_tracker/
├── ExpenseTracker.Api/          # Backend ASP.NET Core
│   ├── Controllers/
│   │   └── SubscriptionsController.cs
│   ├── Data/
│   │   └── AppDbContext.cs
│   ├── Models/
│   │   └── SubscriptionItem.cs
│   ├── Program.cs
│   └── appsettings.json
└── expense-tracker-ui/          # Frontend Vue 3
    ├── src/
    │   ├── components/
    │   │   ├── SubscriptionForm.vue
    │   │   └── SubscriptionList.vue
    │   ├── App.vue
    │   ├── api.js
    │   └── main.js
    └── vite.config.js
```

## API Endpoints

| Method | Endpoint | Descriere |
|---|---|---|
| GET | `/api/subscriptions` | Lista tuturor abonamentelor |
| GET | `/api/subscriptions/{id}` | Un abonament după ID |
| POST | `/api/subscriptions` | Adaugă abonament nou |
| PUT | `/api/subscriptions/{id}` | Actualizează abonament |
| DELETE | `/api/subscriptions/{id}` | Șterge abonament |
| GET | `/api/subscriptions/summary` | Sumar costuri lunare/anuale |

## Setup local

### Prerequisite

- .NET 8 SDK
- PostgreSQL 16
- Node.js 18+

### Backend

```bash
cd ExpenseTracker.Api

# Actualizează connection string în appsettings.json
# "DefaultConnection": "Host=...;Database=expense_tracker;Username=...;Password=..."

dotnet ef database update
dotnet run --urls "http://localhost:5000"
```

### Frontend

```bash
cd expense-tracker-ui
npm install
npm run dev
# → http://localhost:5173
```

## Model de date

```csharp
public class SubscriptionItem
{
    public Guid Id { get; set; }
    public string Name { get; set; }       // "Netflix", "VPS"
    public decimal Cost { get; set; }
    public string Currency { get; set; }   // "RON", "EUR", "USD"
    public BillingPeriod BillingPeriod { get; set; }  // Monthly | Yearly
    public DateTime NextBillingDate { get; set; }
    public string Category { get; set; }
    public bool IsActive { get; set; }
}
```
