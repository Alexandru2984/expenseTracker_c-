# Deploy Guide — Expense Tracker

Ghid pentru deployment pe un VPS Linux cu Docker și Nginx (host).

## Cerințe preliminare

- VPS Linux cu Docker Engine ≥ 24 și Docker Compose CLI (plugin `compose`)
- Un domeniu cu DNS-ul `A` setat pe IP-ul VPS-ului (ex: `expenses.micutu.com`)
- Nginx instalat pe sistemul gazdă (host)
- Porturile 80 și 443 deschise în firewall-ul host-ului

---

## 1. Pregătire server

```bash
# Instalează Docker (Ubuntu/Debian)
curl -fsSL https://get.docker.com | sh
sudo usermod -aG docker $USER

# Instalează Nginx (Ubuntu/Debian)
sudo apt update && sudo apt install nginx certbot python3-certbot-nginx -y
```

---

## 2. Clonează repo-ul

```bash
git clone https://github.com/Alexandru2984/expenseTracker_cs.git
cd expenseTracker_cs
```

---

## 3. Configurează variabilele de mediu

```bash
cp .env.example .env
nano .env
```

Completează toate valorile din `.env`:

| Variabilă | Descriere |
|---|---|
| `ConnectionStrings__DefaultConnection` | String de conexiune PostgreSQL (host=db) |
| `Jwt__Secret` | Cheie secretă JWT (min 32 caractere, `openssl rand -hex 32`) |
| `Cors__AllowedOrigins` | Lasă gol (frontend e same-origin prin Nginx) |
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `POSTGRES_DB` | Numele bazei de date |
| `POSTGRES_USER` | Utilizatorul PostgreSQL |
| `POSTGRES_PASSWORD` | Parola PostgreSQL |

---

## 4. Pornire containere

### 4.1 Prima rulare — migrații baza de date

Rulează migrația EF Core:

```bash
docker compose run --rm migrate
```

### 4.2 Construire UI și pornire servicii

```bash
docker compose up -d ui-builder  # Construiește activele statice în ./frontend_dist
docker compose up -d
```

---

## 5. Configurează Nginx (Host)

Creează o configurație în `/etc/nginx/sites-available/expenses.micutu.com`:

```nginx
server {
    listen 80;
    server_name expenses.micutu.com;

    # Frontend statice
    location / {
        root /calea/catre/expenseTracker_cs/frontend_dist;
        index index.html;
        try_files $uri $uri/ /index.html;
    }

    # API Proxy
    location /api/ {
        proxy_pass http://localhost:8080/api/;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'keep-alive';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    # Health check
    location /health {
        proxy_pass http://localhost:8080/health;
    }
}
```

Activează site-ul și instalează SSL:

```bash
sudo ln -s /etc/nginx/sites-available/expenses.micutu.com /etc/nginx/sites-enabled/
sudo nginx -t && sudo systemctl reload nginx
sudo certbot --nginx -d expenses.micutu.com
```

---

## 6. Update aplicație

```bash
git pull
docker compose build
docker compose run --rm migrate   # dacă există migrații noi
docker compose up -d ui-builder
docker compose up -d api
```

---

## 7. Logs

```bash
docker compose logs -f api     # Log-uri API (Serilog)
docker compose logs -f db      # Log-uri PostgreSQL
sudo tail -f /var/log/nginx/access.log # Log-uri Nginx host
```
