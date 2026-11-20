# Development API setup (NOT production ready)
1. Start PostgreSQL: `cd database && POSTGRES_PASSWORD=<strong-password> docker compose up -d`.
2. Apply `database/001_initial.sql` to the `jarvisfire` database.
3. Set `JARVISFIRE_POSTGRES` to a PostgreSQL connection string and `JARVISFIRE_DEV_API_KEY` to a random 32+ character secret.
4. Start `dotnet run --project src/JarvisFire.Api`.
5. Development endpoints: GET `/api/v1/health`, GET/POST `/api/v1/incidents` (latter require X-Dev-Api-Key).

**Important:** This is a proof of concept only. Do not expose the server to the Internet or use for live dispatch. Production requires TLS, JWT role-based authorization, audit logging, UUID identifiers, migrations, tests and Android integration. This development API intentionally differs from the planned final contract.