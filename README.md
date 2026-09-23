# Team Project Management API

REST API for teams to manage projects, tasks, members, and progress.

## Tech stack

- ASP.NET Core 10 Web API
- PostgreSQL + Entity Framework Core
- MediatR (CQRS)
- JWT access tokens + HttpOnly refresh cookies
- Serilog
- xUnit + Moq
- Swagger

## Setup

1. Install the .NET 10 SDK and PostgreSQL.
2. Copy `.env.sample` to `.env` next to `TeamProjectManagement/Program.cs` and set your values:

```
DATABASE_URL=Host=localhost;Database=team_projects;Username=postgres;Password=postgres
JWT_SECRET=this-is-a-super-secret-jwt-key-that-should-be-at-least-32-chars-long
```

1. From the `TeamProjectManagement` API folder:

```bash
dotnet run
```

Migrations run on startup. Sample users and projects are seeded if the database is empty.

Swagger: `https://localhost:7224/swagger`  
HTTP file: `TeamProjectManagement/TeamProjectManagement.http`

### Tests

```bash
dotnet test
```



## Seeded users

Password for all: `Test123!`


| Username  | Email                                                 |
| --------- | ----------------------------------------------------- |
| testuser1 | [testuser1@example.com](mailto:testuser1@example.com) |
| testuser2 | [testuser2@example.com](mailto:testuser2@example.com) |
| testuser3 | [testuser3@example.com](mailto:testuser3@example.com) |


Note: testuser1 owns **Alpha Protocol**. testuser2 owns **Beta Migration** & is a member in **Alpha Protocol**. testuser3 is a member in both.

## Endpoints

All payloads use this shape:

```json
{ "success": true, "data": {}, "message": "...", "errors": null, "statusCode": 200 }
```

Creates return **201**. Validation errors return **400** with `errors`.

## Auth

- `POST /api/Auth/register` (hashed password, returns JWT, sets refresh cookie)
- `POST /api/Auth/login`
- `POST /api/Auth/refresh` (uses `refreshToken` cookie)
- `POST /api/Auth/logout`
- `PUT /api/Auth/profile`

Login and register are rate limited (5 requests / minute). Other routes are 60 / minute per IP.

Send `Authorization: Bearer <accessToken>` on protected routes. Missing/invalid token -> `401`.

### Projects

- `GET /api/Projects` (projects you own or joined; includes `progressPercent`)
- `GET /api/Projects/owned`
- `GET /api/Projects/member`
- `GET /api/Projects/{id}`
- `GET /api/Projects/{id}/progress`
- `POST /api/Projects`
- `PUT /api/Projects/{id}` (owner only)
- `DELETE /api/Projects/{id}` (owner only)



### Members

- `GET /api/projects/{projectId}/members` (get members, owner/member)
- `POST /api/projects/{projectId}/members` (add member, owner only)
- `DELETE /api/projects/{projectId}/members/{userId}` (remove member, owner only)



### Tasks

- `GET /api/projects/{projectId}/tasks?status=Done&priority=High&assignedToId=&dueBefore=&dueAfter=&searchTerm=&sortBy=`
- `POST /api/projects/{projectId}/tasks` (owner only)
- `GET /api/projects/{projectId}/tasks/{taskId}`
- `PUT /api/projects/{projectId}/tasks/{taskId}` (owner only)
- `DELETE /api/projects/{projectId}/tasks/{taskId}` (owner only)
- `PATCH /api/projects/{projectId}/tasks/{taskId}/status` (owner, or member if assigned)
- `POST /api/projects/{projectId}/tasks/{taskId}/assign` (owner; assignee must be a member)
- `POST /api/projects/{projectId}/tasks/{taskId}/unassign` (owner)
- `GET /api/tasks/my` (tasks assigned to you (same filters))

Status: `ToDo`, `InProgress`, `Done`  
Priority: `Low`, `Medium`, `High`

## Roles

**Owner** (created the project): edit/delete project, add/remove members, create/edit/delete/assign tasks.

**Member**: view project and tasks, update status of tasks assigned to them. Cannot edit the project or delete tasks.

Users only see projects they own or belong to.

## Database

```
User 1---* Project (Owner)
User *---* Project (ProjectMember, role Owner/Member)
Project 1---* ProjectTask (optional AssignedTo User)
User 1---* RefreshToken
```

Passwords are bcrypt hashes. Refresh tokens are stored hashed.

## Assumptions

- User delete is not required.
- Members cannot create tasks (owner does).
- Enums in JSON are strings (`Done`, not `2`).

## Project layout

- **Domain**: entities, enums
- **Application**: handlers, interfaces, view models
- **Infrastructure**: EF Core, repositories, JWT/bcrypt
- **Api**: controllers, middleware, `Program.cs`

