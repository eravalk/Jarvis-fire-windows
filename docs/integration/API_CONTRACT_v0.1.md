# JARVIS FIRE — API contract v0.1 (draft)

Status: **design contract only**. No deployed server or authenticated client integration exists yet.

Base URL: `https://<server>/api/v1` (placeholder; do not hardcode production URL).

## Security
All endpoints except health require HTTPS and `Authorization: Bearer <access_token>`. Roles: `dispatcher`, `rtp`, `admin`. Server enforces permissions; Android never connects directly to PostgreSQL. Tokens must not be committed to Git. All incident changes require server-side audit logs.

## Endpoints (planned)
| Method | Route | Purpose |
|---|---|---|
| GET | /health | API health |
| GET | /incidents?updated_since=<UTC ISO8601> | Incremental incident sync |
| POST | /incidents | Create incident (dispatcher) |
| GET | /incidents/{id} | Incident detail |
| PATCH | /incidents/{id}/status | Change status with audit |
| GET | /vehicles | Vehicle list/status |
| PATCH | /vehicles/{id}/status | Update status with audit |
| GET | /water-sources?updated_since=<UTC ISO8601> | Offline cache sync |
| GET | /objects?updated_since=<UTC ISO8601> | Object/passport metadata |
| GET | /messages?updated_since=<UTC ISO8601> | Dispatcher/RTP messages |
| POST | /messages | Send message |

## JSON incident sample
```json
{
  "id": "4eae8d2c-faf8-4bd6-a3f6-09f936118e42",
  "address": "Тестовый адрес",
  "description": "Учебная запись",
  "status": "new",
  "created_at": "2026-11-21T00:00:00Z",
  "updated_at": "2026-11-21T00:00:00Z",
  "version": 1
}
```

## Synchronization
- All timestamps are UTC ISO 8601; IDs are UUID.
- Client persists a sync cursor and queues offline mutations with idempotency keys.
- Server is authoritative for incident status; conflicts return HTTP 409 with current version.
- Never treat queued offline writes as delivered until acknowledged by server.
- Incremental sync must support pagination and tombstones for deletions.
- Sensitive operational data must be encrypted in transit and protected at rest.

## Implementation gate
No real emergency operations until authenticated API, tests, offline recovery, audit trail, access control and end-to-end Windows↔Android tests are complete.
