# TaskFlow Backend

ASP.NET Core Web API for a Jira-inspired task and workspace management system.

This repository contains the **backend** of TaskFlow (polyrepo). The API focuses on clean layering, multi-tenant workspaces, and permission-based authorization suitable for a portfolio / production-oriented learning project.

## Features

- **Authentication:** Register / login / current user (`/me`) with BCrypt password hashing and JWT Bearer tokens
- **Workspaces:** Create, update, delete (personal workspaces protected), list memberships
- **Members:** Add / remove members, change roles, last-owner safeguards
- **Invitations:** Invite by email, list, cancel, and accept with a token. Membership is created only when the invited account accepts. No outbound email yet; the token is returned by the API for testing.
- **RBAC:** System roles (Owner, Administrator, Member, Viewer) plus workspace-scoped custom roles with permission assignment (create, update, delete)
- **Tasks:** Workspace and personal task lists, create / update / delete, assign / unassign, status and deadline updates, summary vs detail DTOs
- **Cross-cutting:** Global exception middleware (401 / 403 / 404 / 400 / 409), EF Core + PostgreSQL, Docker Compose for local database

## Tech stack

| Layer | Choice |
|--------|--------|
| Runtime | .NET 10 |
| API | ASP.NET Core Web API |
| ORM | Entity Framework Core (Code-First) |
| Database | PostgreSQL 16 (Docker) |
| Auth | JWT Bearer + BCrypt |
| Authorization | Permission-based RBAC (`RolePermission` seed) |

## Solution layout

- `docker-compose.yml` — PostgreSQL (+ optional pgAdmin)
- `.gitignore`
- `TaskFlow.WebApi/TaskFlow.WebApi/` — Controllers, Core, Infrastructure

Typical flow: **Controller → Service → DbContext / entities**, with DTOs at the API boundary.

## Prerequisites

- .NET 10 SDK
- Docker Desktop (for PostgreSQL)
- A REST client (e.g. Postman) for manual API checks

## Local setup

### 1. Database (Docker)

Create a `.env` file in the repository root (gitignored):

    DB_USER=your_user
    DB_PASSWORD=your_password
    DB_NAME=taskflowdb
    PGADMIN_EMAIL=admin@example.com
    PGADMIN_PASSWORD=your_pgadmin_password

Start services:

    docker compose up -d

PostgreSQL: `localhost:5432`. pgAdmin (optional): `localhost:5050`.

### 2. API configuration

Create `TaskFlow.WebApi/TaskFlow.WebApi/appsettings.Development.json` (gitignored). Example:

    {
      "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Port=5432;Database=taskflowdb;Username=your_user;Password=your_password"
      },
      "Jwt": {
        "SecretKey": "use-a-long-random-secret-at-least-32-chars",
        "Issuer": "TaskFlow",
        "Audience": "TaskFlowUsers",
        "ExpiryDays": 7
      }
    }

Do **not** commit real secrets. `appsettings.json` may contain placeholders only.

### 3. Run the API

    cd TaskFlow.WebApi/TaskFlow.WebApi
    dotnet restore
    dotnet ef database update
    dotnet run

Use the HTTPS URL printed by Kestrel (often `https://localhost:7xxx`). In Development, OpenAPI is available.

## Example API surface

| Area | Examples |
|------|----------|
| Auth | `POST /api/auth/register`, `POST /api/auth/login`, `GET /api/auth/me` |
| Workspaces | `GET/POST /api/workspaces`, `GET/PUT/DELETE /api/workspaces/{id}` |
| Members | `GET/POST /api/workspaces/{id}/members`, role update / remove |
| Roles | `GET/POST /api/workspaces/{id}/roles`, `GET/PUT/DELETE .../roles/{roleId}` |
| Invitations | `POST/GET /api/workspaces/{id}/invitations`, `DELETE .../invitations/{invitationId}`, `POST /api/workspaces/invitations/accept` |
| Tasks | `GET /api/tasks`, workspace task CRUD, assign / unassign, status, deadline, detail |

Protected endpoints need `Authorization: Bearer <token>`.

## Security notes

- Passwords hashed with BCrypt; JWTs validated (issuer, audience, lifetime, signing key)
- Authorization enforced in services via permission codes
- Secrets stay in `.env` / `appsettings.Development.json` (ignored by Git)
- Custom roles cannot receive owner-only `workspace.delete`

## Status / roadmap

Implemented: auth, workspaces, members, custom roles (create / list / get / update / delete), invitations (create / list / cancel / accept), task API (detail + unassign).

Next: projects, production hardening (rate limiting, hosting). Outbound invitation email is not implemented.

## Manual API coverage (7 Oct 2026)

Checked over HTTPS against a local API. Re-run this set before a large portfolio push so older areas are not skipped.

- Auth: register, duplicate register 409, password mismatch 400, login success and bad password 401, `GET /me` with and without a token
- Workspaces: list, get, update, create, delete non-personal, block delete of a personal workspace, outsider get/update/delete 404
- Members: list, add unknown email 404, add member, block demoting or removing the last owner, outsider list 403
- Invitations: create (email normalized), duplicate pending 409, invite an existing member 400, bad role 404, bad email 400, list, cancel pending 204, cancel twice 400, cancelled row stays in the list, re-invite after cancel, accept by the invited account, accept twice 400, cancel after accept 400, stolen token 403, unknown token 404, missing token 401, viewer cannot create/list/cancel 403, outsider cannot list or cancel 404
- Roles: create, list, get by id, update keeping the same name, name clash 409, block `workspace.delete` on a custom role, block update/delete of system roles, block delete while the role is assigned, viewer 403, outsider 404
- Tasks: list mine and workspace tasks, get summary and detail, update, status, invalid status 400, deadline, assign a member, block assign of a non-member, unassign, delete, outsider 404, viewer can read and cannot create

Known response issue: `POST /api/tasks/{workspaceId}/tasks` saves the task, then `CreatedAtAction` fails to build the location URL and the HTTP result is 400. Read, update, and delete of that task still work.

## License

No license file yet. For learning/forks, please credit this repository. A formal license (e.g. MIT) can be added later.