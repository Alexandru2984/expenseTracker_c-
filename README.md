# Expense Tracker

Aplicație full-stack pentru urmărirea abonamentelor și cheltuielilor recurente.
Backend ASP.NET Core 8 + EF Core + PostgreSQL. Frontend Vue 3 + Vite + Tailwind CSS.
Deployabil cu `docker compose` pe orice VPS Linux cu Docker, folosind Nginx ca reverse proxy.

---

## Cuprins

1. [Ce face aplicația](#ce-face-aplicația)
2. [Quickstart local](#quickstart-local)
3. [Deployment producție](#deployment-producție)
4. [Variabile de mediu](#variabile-de-mediu)
5. [API Endpoints](#api-endpoints)
6. [Backup & Restore](#backup--restore)

---

## Ce face aplicația

Expense Tracker îți permite să urmărești abonamentele lunare sau anuale (Netflix, hosting, SaaS etc.).
Funcționalități principale:
- Autentificare securizată cu User și Parolă (JWT)
- Adaugă, editează și șterge abonamente
- Vizualizează totaluri lunare și anuale **per valută** (RON, EUR, USD etc.)
- Filtrează abonamente active / inactive

---

## Quickstart local

### Cerințe

- .NET 8 SDK
- PostgreSQL 16 (local sau Docker)
- Node.js 20+

### Backend

```bash
cd ExpenseTracker.Api

# Copiază și editează variabilele de dev
# Actualizează connection string-ul dacă PostgreSQL-ul tău ascultă pe alt port/user

dotnet ef database update        # creează schema
dotnet run --urls http://localhost:5000
# → http://localhost:5000/swagger
```

### Frontend

```bash
cd expense-tracker-ui
npm install
npm run dev
# → http://localhost:5173
```

La primul acces va trebui să te înregistrezi pe pagina de Login.

### Script convenabil (rulează ambele)

```bash
./dev.sh
```

---

## Deployment producție

Urmărește ghidul detaliat în [deploy.md](deploy.md).

### Pași rapizi

```bash
# 1. Clonează repo-ul pe VPS
git clone https://github.com/Alexandru2984/expenseTracker_cs.git && cd expenseTracker_cs

# 2. Completează variabilele de mediu
cp .env.example .env && nano .env

# 3. Rulează migrațiile
docker compose run --rm migrate

# 4. Construiește UI-ul și pornește serviciile
docker compose up -d ui-builder
docker compose up -d
```

Configurarea Nginx de pe host trebuie să pointeze către portul `8080` pentru API și către folderul `frontend_dist` pentru fișierele statice.

---

## Variabile de mediu

Copiază `.env.example` în `.env` și completează valorile. **Nu comite niciodată `.env`.**

| Variabilă | Descriere | Exemplu |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string | `Host=db;Port=5432;Database=expense_tracker;Username=expense_user;Password=...` |
| `Jwt__Secret` | Cheie secretă pentru semnarea token-urilor JWT | `openssl rand -hex 32` |
| `Cors__AllowedOrigins` | Origini CORS permise (virgulă, gol în prod) | `http://localhost:5173` |
| `ASPNETCORE_ENVIRONMENT` | Mediu .NET | `Production` |
| `POSTGRES_DB` | Numele bazei de date | `expense_tracker` |
| `POSTGRES_USER` | Utilizatorul PostgreSQL | `expense_user` |
| `POSTGRES_PASSWORD` | Parola PostgreSQL | *(secret)* |

---

## API Endpoints

Endpoint-urile `/api/auth/*` și `/health` sunt publice.
Endpoint-urile `/api/subscriptions/*` necesită header `Authorization: Bearer <JWT_TOKEN>`.

| Method | Endpoint | Descriere |
|---|---|---|
| `GET` | `/health` | Status aplicație + baza de date |
| `POST` | `/api/auth/register` | Înregistrare user nou |
| `POST` | `/api/auth/login` | Autentificare și primire token |
| `GET` | `/api/subscriptions` | Lista paginată a abonamentelor userului |
| `POST` | `/api/subscriptions` | Adaugă abonament |
| `PUT` | `/api/subscriptions/{id}` | Actualizează abonament |
| `DELETE` | `/api/subscriptions/{id}` | Șterge abonament |
| `GET` | `/api/subscriptions/summary` | Totaluri per valută |

---

## Backup & Restore

### Backup manual

```bash
./scripts/backup-db.sh
```

### Restore

```bash
./scripts/restore-db.sh backups/expense_tracker_YYYY-MM-DD.sql.gz
```

---

## Tech Stack

| Layer | Tehnologie |
|---|---|
| Backend | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Baza de date | PostgreSQL 16 |
| Frontend | Vue 3 + Vite + Tailwind CSS |
| Reverse proxy | Nginx (Host) |
| Containerizare | Docker + Docker Compose |

## Licență

[MIT](LICENSE)
