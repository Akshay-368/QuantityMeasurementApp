# QuantityMeasurementApp

A layered ASP.NET Core Web API for quantity operations (convert, add, subtract, divide), with JWT-based auth, operation history persistence, and Redis-backed history caching.

## Table of Contents

1. Overview
2. Tech Stack
3. Solution Structure
4. Architecture and Layer Responsibilities
5. Major Classes and Their Roles
6. Design Principles and Paradigms
7. Request Lifecycle (How the API Travels)
8. Domain Model and Unit System
9. Data and Persistence Model
10. Security Model
11. Caching Strategy
12. Rate Limiting and Middleware
13. API Surface
14. Configuration
15. Local Setup and Run
16. Database Migrations
17. Testing
18. Current Constraints and Improvement Opportunities

## 1. Overview

This project implements a Quantity Measurement API following a layered architecture:

- API layer for HTTP concerns and endpoint exposure.
- Business layer for use-case orchestration and domain-level operations.
- Infrastructure layer for persistence and caching.
- Model layer for DTOs, entities, core quantity abstractions, and unit implementations.
- Test layer for domain behavior validation.

The system supports operations across categories such as Length, Weight, Volume, and Temperature using a base-unit conversion strategy.

## 2. Tech Stack

- .NET 8 (ASP.NET Core Web API)
- C#
- Entity Framework Core (SQL Server provider)
- SQL Server (primary persistence)
- JWT Bearer authentication
- Redis via `IDistributedCache` (history caching)
- Swashbuckle / Swagger (OpenAPI UI)
- NUnit (unit testing)

## 3. Solution Structure

- `QuantityMeasurement.API`
  - API host, middleware pipeline, controllers, configuration bootstrap.
- `QuantityMeasurement.BusinessLayer`
  - Service interfaces + concrete services (auth, quantity, history), DI registration.
- `QuantityMeasurement.Infrastructure`
  - EF Core DbContext, repository implementations, migrations.
- `QuantityMeasurement.ModelLayer`
  - Core quantity abstractions, units, DTOs, persistence entities, contracts.
- `QuantityMeasurement.Tests`
  - NUnit tests focused on quantity behavior/use-cases.

## 4. Architecture and Layer Responsibilities

### Layered / N-tier Design

The solution follows a classic layered architecture:

- Presentation/API Layer (`QuantityMeasurement.API`)
  - Handles transport protocol (HTTP), routing, authorization attributes, action results, and middleware composition.
- Application/Business Layer (`QuantityMeasurement.BusinessLayer`)
  - Implements use cases and orchestration logic:
    - convert/add/subtract/division operations
    - auth/register/login
    - history retrieval and clearing
- Infrastructure Layer (`QuantityMeasurement.Infrastructure`)
  - Implements technical concerns:
    - SQL persistence with EF Core
    - repository implementation
    - distributed cache integration for history reads
- Domain/Model Layer (`QuantityMeasurement.ModelLayer`)
  - Encodes quantity behavior and unit conversion semantics.
  - Defines DTO and entity contracts shared across layers.

### Dependency Direction

High-level dependency direction is inward toward abstractions:

- API depends on Business interfaces
- Business depends on Infrastructure interfaces + Model types
- Infrastructure depends on Model types and implements Infrastructure interfaces
- Model has no dependency on API or Business orchestration

## 5. Major Classes and Their Roles

### API Layer

- `Program`
  - Application composition root.
  - Configures auth, JWT validation, rate limiter, Swagger, middleware pipeline, and DI registration.
- `AuthController`
  - Endpoints for register and login.
  - Marked `AllowAnonymous`.
- `QuantityController`
  - Protected quantity operations (`Authorize`).
- `HistoryController`
  - Protected history retrieval and deletion (`Authorize`).
- `TimeLogging` middleware
  - Measures and logs request duration around downstream middleware execution.

### Business Layer

- `ServiceCollectionExtensions`
  - Registers application services, repositories, EF DbContext, in-memory cache, and Redis distributed cache.
- `QuantityService : IQuantityService`
  - Core use-case orchestrator for quantity operations.
  - Resolves units, constructs typed quantity objects, executes operations, and writes operation history.
- `HistoryService : IHistoryService`
  - Reads and clears operation history through repository abstraction.
- `AuthService : IAuthService`
  - User registration, password verification, and JWT creation.
- `PasswordHasher : IPasswordHasher`
  - Password hashing + verification using HMACSHA256 with per-user salt.

### Infrastructure Layer

- `QuantityDbContext : DbContext, IQuantityDbContext`
  - EF model root exposing `Histories` and `Users` sets.
- `HistoryRepository : IHistoryRepository`
  - Persists operation history.
  - Reads history with Redis cache-aside strategy.
  - Invalidates cache on writes/clear.
- Migrations (`Migrations/*`)
  - Schema evolution and DB-side audit trigger setup.

### Model Layer

- `Unit` (sealed)
  - Canonical unit metadata: name, category, conversion factor, optional offset.
  - Provides conversion to/from category base unit.
- `Quantity` (abstract base)
  - Category-safe operations: convert, add, subtract, divide by quantity/scalar.
- Category abstractions: `Length`, `Weight`, `Volume`, `Temperature`
  - Type-specific wrappers and factory behavior for creating concrete quantities.
- Concrete units under `Units/*`
  - `Feet`, `Inches`, `Yard`, `Meter`, `Centimeter`
  - `Kilogram`, `Gram`, `Pound`
  - `Litre`, `Millilitre`, `Gallon`
  - `Celsius`, `Fahrenheit`, `Kelvin`
- DTOs
  - `QuantityRequestDto`, `QuantityResultDto`, `HistoryDto`
- Entities
  - `History`, `User`
- Internal model
  - `HistoryRecord` as domain/persistence transfer model for history repository.

## 6. Design Principles and Paradigms

### 1) Separation of Concerns

Each project has a focused responsibility:

- API is transport-facing.
- Business is use-case-facing.
- Infrastructure is integration-facing.
- Model is domain-facing.

### 2) Dependency Inversion + Interface-driven Design

Business code depends on abstractions (`IHistoryRepository`, `IQuantityDbContext`, service interfaces), while Infrastructure provides implementations.

### 3) Repository Pattern

`HistoryRepository` isolates data access concerns and caching behavior from business logic.

### 4) Domain-centric Quantity Modeling

Quantity behavior lives in domain abstractions rather than controllers. This keeps domain rules reusable and testable.

### 5) Strategy-like conversion model

`Unit` instances encapsulate conversion rules (factor/offset), while `Quantity` orchestrates operation logic consistently for all categories.

### 6) Middleware Pipeline (Chain of Responsibility)

The HTTP pipeline composes reusable middleware units in ordered sequence.

## 7. Request Lifecycle (How the API Travels)

At runtime, the request path is:

1. Kestrel receives HTTP request.
2. Middleware pipeline executes in configured order:
   - `UseSwagger` / `UseSwaggerUI` in Development
   - `UseTimeLogging`
   - `UseHttpsRedirection`
   - `UseRateLimiter`
   - `UseAuthentication`
   - `UseAuthorization`
   - `MapControllers`
3. Routing maps request to controller action.
4. Controller delegates to business service interface.
5. Business service executes domain logic and/or persistence calls.
6. Repository/DbContext interacts with SQL and cache where applicable.
7. Result DTO is returned to controller and serialized as HTTP response.

### Example: Convert Flow

1. Client sends `POST /api/quantity/convert` with JSON body.
2. JWT is validated before controller execution.
3. `QuantityController.Convert` calls `IQuantityService.Convert`.
4. `QuantityService`:
   - validates request fields,
   - resolves source/target `Unit`,
   - creates typed quantity object,
   - performs conversion,
   - saves history via repository.
5. `HistoryRepository.Save` writes to `Histories` table and invalidates history cache.
6. API returns `QuantityResultDto`.

### Example: History Read Flow

1. Client sends `GET /api/history`.
2. Auth and authorization are enforced.
3. `HistoryService.GetHistory` calls `HistoryRepository.GetHistory`.
4. Repository checks Redis cache key `history:all`:
   - hit: returns cached list,
   - miss: queries SQL, stores serialized result in Redis, returns list.

## 8. Domain Model and Unit System

The quantity system is category-safe:

- Operations across different categories are blocked (for example Length vs Weight).
- Values are normalized to base units for consistent computation.
- Temperature uses offset-aware conversion path.

Core conversion equations used by `Unit`:

- To base: $base = (value + offset) \times factor$
- From base: $value = \left(\frac{base}{factor}\right) - offset$

For additive categories (length/weight/volume), these rules provide straightforward conversions. Temperature-specific offsets are represented in unit metadata.

## 9. Data and Persistence Model

### Tables/Entity Sets

- `Histories`
  - Stores all quantity operation metadata and outputs.
- `Users`
  - Stores credentials (hashed + salted), unique username, token metadata fields.

### Auditing

Migration `AddAuditTrigger` creates:

- `SystemAudit` table
- Trigger `trg_SystemAudit` on `Histories` (insert/update/delete)
- DML protection (`DENY UPDATE, DELETE` on `SystemAudit` to `public`)

Audit rows capture action type and JSON snapshots of old/new values.

## 10. Security Model

### Authentication

- JWT Bearer auth is configured in API bootstrap.
- Token validation checks issuer, audience, signature, and lifetime.
- `ClockSkew` is set to zero.

### Authorization

- `QuantityController` and `HistoryController` are protected with `Authorize`.
- `AuthController` allows anonymous access for register/login.

### Password Handling

- Passwords are hashed with HMACSHA256 and random salt (HMAC key).
- Login verifies computed hash against stored hash with stored salt.

## 11. Caching Strategy

`HistoryRepository` applies cache-aside strategy:

- Read path: cache first, database fallback, then cache populate.
- Write path (`Save`, `ClearHistory`): write DB first, then cache invalidation.
- Cache key: `history:all`
- TTL: 30 minutes absolute expiration.

## 12. Rate Limiting and Middleware

A fixed-window limiter named `fixedWindowLimiter` is enabled on controllers:

- Window: 10 seconds
- Permit limit: 5
- Queue limit: 5
- Queue order: oldest first
- Rejection status: 429

Custom middleware `TimeLogging` logs per-request elapsed time.

## 13. API Surface

Base routes are convention-based `api/[controller]`.

### Auth

- `POST /api/auth/register`
  - Inputs: `username`, `password` (action parameters)
  - Auth: anonymous
- `POST /api/auth/login`
  - Inputs: `username`, `password`
  - Auth: anonymous
  - Output: JWT token string

### Quantity (requires JWT)

- `POST /api/quantity/convert`
- `POST /api/quantity/add`
- `POST /api/quantity/subtract`
- `POST /api/quantity/divide-scalar`
- `POST /api/quantity/divide-quantity`

Body contract uses `QuantityRequestDto`.

### History (requires JWT)

- `GET /api/history`
  - Output: list of `HistoryDto`
- `DELETE /api/history`
  - Clears all history

## 14. Configuration

### appsettings values

From API config:

- Connection string: `ConnectionStrings:DefaultConnection`
- Redis: `Redis:Configuration`, `Redis:InstanceName`
- JWT section exists in config, but runtime auth setup primarily reads environment variables.

### Environment Variables Required for JWT

The runtime expects:

- `Jwt__Key`
- `Jwt__Issuer`
- `Jwt__Audience`

Because `DotNetEnv.Env.Load()` is called, these can be supplied via environment or `.env` file.

## 15. Local Setup and Run

Prerequisites:

- .NET SDK 8.x
- SQL Server instance reachable by configured connection string
- Redis server reachable by configured Redis settings

Typical run:

```bash
dotnet restore
 dotnet build QuantityMeasurementApp.sln
 dotnet run --project QuantityMeasurement.API
```

Open Swagger UI at:

- `https://localhost:7051/swagger`
- or `http://localhost:5228/swagger`

## 16. Database Migrations

From Infrastructure project, using API as startup project:

```bash
dotnet ef migrations add <MigrationName> --startup-project ../QuantityMeasurement.API
 dotnet ef database update --startup-project ../QuantityMeasurement.API
```

The startup project is required so EF can read runtime configuration and connection settings.

## 17. Testing

Test project: `QuantityMeasurement.Tests` (NUnit).

Current tests are organized by use case and category with files such as:

- `UnitTestFeetUC-1.cs`
- `UnitTestInchesUC-2.cs`
- `UnitTestLengthEqualityUC-3.cs`
- `UnitTestConverionUC-5.cs`
- `UnitTestTemperatureUC-14.cs`
- and additional UC-focused files.

Run tests:

```bash
dotnet test QuantityMeasurement.Tests
```

## 18. Current Constraints and Improvement Opportunities

Observed opportunities from current implementation:

- Input model validation can be expanded with FluentValidation or data annotations for richer API errors.
- Auth endpoints currently use action parameters for credentials; a request DTO would improve contract clarity.
- Error handling can be centralized using exception-handling middleware for consistent HTTP problem responses.
- Logging currently uses `Console.WriteLine` in middleware; structured logging via `ILogger` would be production-friendlier.
- `DivideByQuantity` computes the result twice in service logic; returning the already-computed value would avoid duplicate work.
- Consider refresh token lifecycle completion since user model contains refresh token fields.

---

 this README can be split further into dedicated docs:

- `docs/architecture.md` (C4-level and sequence diagrams)
- `docs/api-reference.md` (request/response examples)
- `docs/security.md` (threat model and hardening checklist)
