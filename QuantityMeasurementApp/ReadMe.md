# QuantityMeasurementApp — README.md

*(Composed for humans who like their architectures explicit and their unit conversions exact.)*

---

## Quick summary (elevator pitch)

**QuantityMeasurementApp** is a small, well-layered .NET solution that lets you represent physical quantities (length, weight, volume, temperature), do arithmetic and conversions between units, and persist a tiny conversion history.
It follows a clean separation of concerns:

* **Domain** — value objects & units (business logic, conversions, arithmetic)
* **Application** — application services that orchestrate domain operations and map to DTOs
* **Infrastructure** — persistence (EF Core + SQL Server) and caching (Redis) implementations
* **API** — ASP.NET Core Web API exposing operations to clients (and middleware + DI)
* **Console** — an interactive CLI client that exercises app logic (factory / menu pattern)
* **Tests** — automated unit tests

If you want to jump straight in: the core logic lives in `QuantityMeasurement.Domain`, the conversion orchestration in `QuantityMeasurement.Application`, and the repository & EF context in `QuantityMeasurement.Infrastructure`. The API is the outward-facing layer that wires them together.

---

## Table of contents

1. [Architecture & Layers](#architecture--layers)
2. [Flow of a request (end-to-end)](#flow-of-a-request-end-to-end)
3. [Key projects, important files & core classes](#key-projects-important-files--core-classes)
4. [Commands used to create / build / run / migrate / test the project](#commands-used-to-create--build--run--migrate--test-the-project)
5. [How to add features — example scenarios](#how-to-add-features---example-scenarios)

   * Add new operation (e.g., **Multiplication**)
   * Add new unit (e.g., **Tonn**)
   * Add new category (e.g., **Energy**)
6. [Developer notes, caveats, and suggested improvements](#developer-notes-caveats-and-suggested-improvements)
7. [Appendix: Helpful file map & where to look first](#appendix-helpful-file-map--where-to-look-first)

---

## Architecture & layers

High level:

```
[Client] -> [QuantityMeasurement.API (Controllers)] -> [QuantityMeasurement.Application (Services)]
            -> [QuantityMeasurement.Domain (Quantities, Units)] 
            -> [QuantityMeasurement.Infrastructure (Repository, DbContext, Cache)] -> [SQL Server / Redis]
```

* **Domain** (`QuantityMeasurement.Domain`): pure business logic. `Quantity` abstract base class implements equality, conversion to base, addition/subtraction/division, and converts using `Unit` helper. There are *category-specific* abstract classes (e.g., `Length : Quantity`, `Weight : Quantity`) and concrete unit classes (e.g., `Meter`, `Feet`, `Kilogram`) that inherit those.
* **Application** (`QuantityMeasurement.Application`): DTOs and `QuantityService` which:

  * resolves unit names to `Unit` domain objects,
  * creates concrete `Quantity` instances (e.g., `new Meter(value)`),
  * performs requested operations (`Convert`, `Add`, `Subtract`, `Divide`, **etc.**),
  * persists operation record to history via `IHistoryRepository`.
* **Infrastructure** (`QuantityMeasurement.Infrastructure`): EF Core `QuantityDbContext`, `HistoryRepository` (implements `IHistoryRepository`) which uses SQL Server for storage and Redis (an `IDistributedCache`) for caching history.
* **API** (`QuantityMeasurement.API`): exposes endpoints in `QuantityController` (convert/add/subtract/divide-scalar etc.) and `HistoryController`. Wires services and repositories via DI in `Program.cs`. Includes a small middleware `TimeLogging` for measuring request durations.
* **Console**: a local CLI using a factory + menu to exercise the same `Domain` logic directly for interactive use.
* **Tests**: unit tests verifying domain behavior, services, and likely some integration.

---

## Flow of a request (end-to-end)

Take a simple convert request `POST /api/quantity/convert`:

1. **HTTP client** -> `QuantityController.Convert([FromBody] QuantityRequestDto)`
2. Controller calls `IQuantityService.Convert(request)` (in `QuantityMeasurement.Application`)
3. **QuantityService**:

   * validates `request`,
   * resolves unit strings to `Unit` objects (via `ResolveUnit`),
   * creates a concrete `Quantity` instance (e.g., `new Feet(value)`),
   * calls `quantity.ConvertTo(targetUnit)` (domain logic — see `Quantity` class),
   * builds `QuantityResultDto`.
   * creates a `HistoryRecord` and calls `_historyRepository.Save(record)`.
4. **HistoryRepository** (Infrastructure):

   * saves to `QuantityDbContext.Histories` (SQL Server),
   * updates the Redis cache key `history:all` (so future reads are fast).
5. `QuantityService` returns `QuantityResultDto` to controller, controller returns 200 OK with JSON.
6. Optional: middleware `TimeLogging` logged around the controller call.

Graphically:

```
Client -> Controller -> Application.Service -> Domain.Quantity -> Application.Service -> Infrastructure.Repository -> SQL Server (+Redis cache)
```

---

## Key projects, files & core classes (file-by-file guidance)

> I'll list the most important artifacts to orient a new developer. If you want code pointers I’ll include the exact file names so you can Ctrl+F your way to the code.

### `QuantityMeasurement.Domain`

* `Core/QuantityAbstractBaseClass.cs` — **Most critical**. Declares `abstract class Quantity : IQuantity<Quantity>` and:

  * holds `double value` and `Unit unit` (immutable),
  * `ToBase()` converts to the category base unit,
  * `ConvertTo(Unit)`, `Add`, `Subtract`, `Divide`, `Equals`, `GetHashCode`.
  * Contains arithmetic rules and category checks (e.g., cannot add different categories).
* `Core/Unit.cs` — `sealed class Unit`:

  * static `Unit` instances for each unit (e.g., `Unit.Meter`, `Unit.Feet`, `Unit.Kilogram`, `Unit.Celsius`),
  * properties: `Name`, `ConversionFactorToBase`, `OffsetToBase` (for temperature conversions).
  * `ConvertToBaseUnit(value)` & `ConvertFromBaseUnit(baseValue)` implement conversion math.
* `Interfaces/IQuantity.cs` — small, enforces `value`, `unit`, `Equals`, `GetHashCode`.
* `Units/<Category>/*`:

  * Category sub-base class (e.g., `LengthAbstractSubBaseClass.cs`, `WeightAbstractSubBaseClass.cs`, `TemperatureAbstractSubBaseClass.cs`) inherits `Quantity`.
  * Concrete unit classes — e.g., `Meter.cs`, `Centimeter.cs`, `Feet.cs`, `Inches.cs`, `Kilogram.cs`, `Gram.cs`, `Litre.cs`, `Millilitre.cs`, `Celsius.cs` etc. These wrap the value with their `Unit` (call base constructor with appropriate `Unit.<X>`).
* Typical extension point: add a new concrete unit class and a corresponding `Unit` static instance.

### `QuantityMeasurement.Application`

* `DTOs/QuantityRequestDto.cs` and `DTOs/QuantityResultDto.cs` — request/response shapes used by the API.
* `Interfaces/IQuantityService.cs` — defines methods like `Convert(...)`, `Add(...)`, `Subtract(...)`, `DivideByQuantity(...)`, etc.
* `Services/QuantityService.cs` — orchestration layer:

  * `ResolveUnit(string unitName)` — maps unit name strings to `Unit` static instances,
  * `CreateQuantity(double value, Unit unit)` — maps `Unit` to concrete `Quantity` subclasses,
  * calls domain operations and persists history via injected `IHistoryRepository`.

**Important**: When adding units/categories/operations you usually need to update `ResolveUnit` and `CreateQuantity` here.

### `QuantityMeasurement.Infrastructure`

* `Interfaces/IHistoryRepository.cs` — `Save`, `GetHistory`, `ClearHistory`.
* `Models/HistoryRecord.cs` — DTO/model used by app & repository.
* `Persistence/QuantityDbContext.cs` — EF `DbContext` with `DbSet<History> Histories`.
* `Repositories/HistoryRepository.cs` — EF + Redis caching logic. Stores `History` entries and manages cache key `history:all`.
* `Persistence/Entities/History.cs` — EF entity for the history table.
* `Migrations/` — EF migration files (InitialCreate etc.) — if you change schema or add persistence needs, update here.

### `QuantityMeasurement.API`

* `Controllers/QuantityController.cs` — API endpoints: `convert`, `add`, `subtract`, `divide-scalar`, etc. Each method uses `IQuantityService`.
* `Controllers/HistoryController.cs` — `GET` history and `DELETE` (clear).
* `Program.cs` — DI registrations:

  * `builder.Services.AddScoped<IQuantityService, QuantityService>();`
  * `builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();`
  * `AddDbContext<QuantityDbContext>(options => options.UseSqlServer(...))`
  * Note: Redis cache is registered if configured (StackExchangeRedis).
* `Middleware/TimeLogging.cs` — example custom middleware to measure request time.

### `QuantityMeasurement.Console`

* `Program.cs` — entry point that uses a `Factory` and `Menu`.
* `Menu.cs` — interactive CLI that uses Domain & Application logic to let the user perform conversions locally.

### `QuantityMeasurement.Tests`

* Unit tests (verify equality, conversion factors, service behavior).

---

## Commands used to create / build / run / migrate / test the project

A condensed reproducible list (you’ll need .NET 8 SDK installed; adapt paths & project names as you like):

1. **Restore & build**

```bash
dotnet restore
dotnet build
```

2. **Run the API (development)**

```bash
# from solution root
cd QuantityMeasurementApp/QuantityMeasurement.API
dotnet run
# or use Visual Studio to run the API project
```

3. **Run the Console app**

```bash
cd QuantityMeasurementApp/QuantityMeasurement.Console
dotnet run
```

4. **Run tests**

```bash
cd QuantityMeasurementApp/QuantityMeasurement.Tests
dotnet test
```

5. **Entity Framework migrations (if you change DB model):**

```bash
# Add migration (from solution root or specify project)
dotnet ef migrations add InitialCreate \
  --project QuantityMeasurement.Infrastructure \
  --startup-project QuantityMeasurement.API

# Apply migration to DB
dotnet ef database update \
  --project QuantityMeasurement.Infrastructure \
  --startup-project QuantityMeasurement.API
```

*(Note: `--startup-project` points to API project because it carries configuration and `Program.cs` DI.)*

6. **Useful development commands**:

```bash
dotnet clean
dotnet test --logger "trx;LogFileName=test_results.trx"
dotnet run --project QuantityMeasurementApp/QuantityMeasurement.API/QuantityMeasurement.API.csproj
```

7. **Redis & SQL**

* The app expects a connection string named `DefaultConnection` in `appsettings.json` for SQL Server.
* Redis configuration is read from `builder.Configuration.GetSection("Redis")`. If Redis is not available, either configure in `appsettings.json` or remove the Redis-related registration in `Program.cs`.

8. **Commands used during initial creation**
   (From the included helper text `Cmd_Commands_That_Were_Used_For_Creation(Project).txt`) — examples seen in that file:

```text
dotnet new classlib -n QuantityMeasurement.Application
dotnet new classlib -n QuantityMeasurement.Infrastructure
dotnet new webapi -n QuantityMeasurement.API
dotnet new console -n QuantityMeasurement.Console
dotnet new xunit -n QuantityMeasurement.Tests        # or similar testing template
dotnet sln add QuantityMeasurement.Domain/QuantityMeasurement.Domain.csproj
dotnet sln add QuantityMeasurement.Application/QuantityMeasurement.Application.csproj
# ...and so on
```

Those are creation-time commands; you still use standard `dotnet build` / `dotnet run` / `dotnet test` to operate.

---

## How to add features — step-by-step patterns

Below are practical, tested patterns for adding new units, categories, or operations. Think of it as *where* to change code across layers so your app remains consistent.

> For each pattern: **Domain** (business rules), **Application** (mapping + orchestration), **API/Console** (expose to clients), **Tests** (add unit tests), **Infra** (rarely needed unless you change persisted model).

---

### 1) Add a new operation — **Multiplication** between quantities (or multiply by scalar)

**Goal:** support `Multiply(Quantity other)` or `Multiply(double scalar)`.

**Steps:**

* **Domain**

  * Edit `Core/QuantityAbstractBaseClass.cs`:

    * Add `public Quantity Multiply(double scalar)` — trivial: `return CreateInstance(this.value * scalar, this.unit)`.
    * Add `public Quantity Multiply(Quantity other)` — this is semantic: if multiplication of two quantities yields a different dimension (e.g., *m* × *m* = *m²*), you must decide if you want derived dimensions; existing code doesn't support derived dimensions, so either:

      * implement scalar-only multiplication (recommended), OR
      * design a new system for derived units (more complex — add unit algebra).
  * Add matching wrapper methods in category sub-classes if you want strongly-typed returns (e.g., in `Length` add `public Length Multiply(double scalar)`).
* **Application**

  * Update `IQuantityService` to include a `Multiply` method (e.g., `QuantityResultDto Multiply(QuantityRequestDto request)` or `double MultiplyByScalar(QuantityRequestDto request)`).
  * Implement it in `QuantityService.cs` using the domain `Multiply` method.
* **API**

  * Add a controller route in `QuantityController`:

    ```csharp
    [HttpPost("multiply-scalar")]
    public IActionResult MultiplyScalar([FromBody] QuantityRequestDto request)
    {
        return Ok(_quantityService.MultiplyByScalar(request));
    }
    ```
* **Console**

  * Add a menu entry & wiring to call new service method (or call domain method directly if Console avoids service).
* **Tests**

  * Add unit tests for domain multiply & service multiply scenarios.

**Notes:** If you want derived units (e.g., area, volume from multiplication), you'll need a design extension: unit algebra, dimensional analysis, and new `Category` definitions for derived types.

---

### 2) Add a new **unit** (example: `Tonn` as alias for metric ton)

**Goal:** Add a new concrete unit, reuse existing `Weight` category.

**Steps:**

* **Domain**

  1. In `Core/Unit.cs`:

     * Add a static field, e.g.:

       ```csharp
       public static readonly Unit Tonne = new Unit("Tonne", 1000.0, WEIGHT);
       // or Tonn / Ton depending on naming; conversion factor is how many base units (e.g., base = kilogram)
       ```

       *Important:* The conversion factor must match how `Unit` treats conversion — verify whether the base for weight is `Kilogram` and adjust.
  2. Create a new concrete class file under `Units/Weight`, e.g., `Tonne.cs`:

     ```csharp
     namespace QuantityMeasurement.Domain.Units;
     using QuantityMeasurement.Domain.Core;

     public class Tonne : Weight
     {
         public Tonne(double value) : base(value, Unit.Tonne) {}
     }
     ```
* **Application**

  * Update `Services/QuantityService.cs`:

    * Add `case "tonne": return Unit.Tonne;` in `ResolveUnit`.
    * In `CreateQuantity(...)`, add:

      ```csharp
      if (unit == Unit.Tonne) return new Tonne(value);
      ```
* **Console**

  * In `Menu.cs`, add `Unit.Tonne` to `GetUnitsByCategory("Weight")` list so it displays in the menu.
* **API**

  * No API changes needed if the API accepts unit names — but you may update sample requests or documentation.
* **Tests**

  * Add tests confirming `new Tonne(x).ConvertTo(Unit.Kilogram)` yields `x * 1000`.

**Propagation summary:**
`Unit.cs` ←→ domain unit concrete class ← `Application.ResolveUnit` / `CreateQuantity` ← `Console` and `API` consume the mapping. Tests verify behavior.

---

### 3) Add a new **category** — e.g., `Energy`

**Goal:** Add a whole category like `Energy` with units: `Joule`, `KilowattHour`, `Calorie`.

**Steps:**

* **Domain**

  1. In `Core/Unit.cs`, add a `private const string ENERGY = "Energy";` and static fields:

     ```csharp
     public static readonly Unit Joule = new Unit("Joule", 1.0, ENERGY);
     public static readonly Unit KilowattHour = new Unit("KilowattHour", 3_600_000.0, ENERGY); // if Joule is base
     public static readonly Unit Calorie = new Unit("Calorie", 4.184, ENERGY);
     ```

     Decide a base for the category (e.g., **Joule**).
  2. Add `Units/Energy` folder

     * `EnergyAbstractSubBaseClass.cs` : `public abstract class Energy: Quantity { protected Energy(double v, Unit u): base(v,u){} }`
     * `Joule.cs`, `KilowattHour.cs`, `Calorie.cs` — concrete classes similar to `Meter`.
* **Application**

  * Update `ResolveUnit` to map names to `Unit.Joule` / `Unit.KilowattHour` / `Unit.Calorie`.
  * Update `CreateQuantity` to return new `Joule(value)`, etc.
* **Console & API**

  * Add menu items (Console) and update documentation/examples for API to show energy units.
* **Tests**

  * Write domain tests for conversions (e.g., `1 kWh -> 3_600_000 Joule`).
* **Infrastructure**

  * No DB schema changes required (history stores operation as generic fields), unless you want to store category-specific columns.

**Propagation summary:**
Add constants & classes in Domain → update Application mapping → update UI/API → test. Very localized.

---

## How to run locally (recommended dev setup)

1. Ensure you have .NET SDK (8.x) installed.
2. Ensure a SQL Server instance is available. Update `QuantityMeasurement.API/appsettings.json` or environment variables with `DefaultConnection`. Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=QuantityDb;Trusted_Connection=True;"
}
```

3. If using Redis caching, set `Redis:Configuration` and `Redis:InstanceName` in `appsettings.json` or disable the call in `Program.cs`.
4. From solution root:

```bash
dotnet restore
dotnet build

# apply migrations once
dotnet ef database update --project QuantityMeasurement.Infrastructure --startup-project QuantityMeasurement.API

# run API
cd QuantityMeasurementApp/QuantityMeasurement.API
dotnet run
```

Browse to `https://localhost:<port>/swagger` (if dev & Swagger enabled) to try endpoints.

---

## Developer notes, caveats & suggested improvements

* **Unit constants & base units**: The `Unit` class encodes conversion factor to a "base unit" per category. When adding a unit, make sure you understand which base is used (e.g., `Length` base may be *feet* or *inches* depending on code). Use consistent base units across category.
* **Temperature conversions**: Temperature uses `OffsetToBase` (e.g., Fahrenheit offset -32) to support affine transforms. Be careful with `ConvertToBaseUnit` and rounding errors.
* **Derived units**: The current domain model is conservative — arithmetic returns same category or scalars (e.g., divide two same-category quantities yields scalar). If you want derived dimensions (area, density), you'll need a richer unit algebra model (units having dimensions and exponents).
* **String matching for unit names**: `ResolveUnit` uses string switches — consider centralizing unit metadata into a registry/dictionary to avoid brittle code and to support localization / synonyms (e.g., "kg", "kilogram", "kilograms").
* **Validation & Error Handling**: Application and domain throw exceptions for invalid operations. For API-level resilience, you may want global exception handling middleware that returns proper HTTP status codes and descriptive error messages.
* **Caching & concurrency**: `HistoryRepository` writes DB and updates Redis; ensure consistent cache invalidation under concurrency.
* **Precision & tolerances**: Equality uses `Math.Abs(thisBase - otherBase) < 0.0001` — consider making epsilon configurable or using `decimal` for financial/very precise domains.
* **Automate with CI**: Add `dotnet test` step in CI and `dotnet ef` migration verification if you plan to deploy migrations automatically.

---

## Appendix — Helpful file map & where to look first

* `QuantityMeasurement.Domain/`

  * `Core/QuantityAbstractBaseClass.cs` ← **start here** (core conversion logic)
  * `Core/Unit.cs` ← unit definitions & conversion math
  * `Units/` ← concrete units by category
* `QuantityMeasurement.Application/`

  * `Services/QuantityService.cs` ← orchestration, unit string mapping, creating concrete quantities
  * `DTOs/` ← request/response shapes
* `QuantityMeasurement.Infrastructure/`

  * `Persistence/QuantityDbContext.cs` ← EF Core context
  * `Repositories/HistoryRepository.cs` ← persistence + cache
  * `Interfaces/IHistoryRepository.cs`
* `QuantityMeasurement.API/`

  * `Controllers/QuantityController.cs` ← HTTP endpoints
  * `Program.cs` ← DI, DB & Redis wiring, middleware
* `QuantityMeasurement.Console/`

  * `Menu.cs` ← CLI interactions (useful to see example flows)
* `QuantityMeasurement.Tests/`

  * Unit tests — good examples of expected behavior & edge cases.

---

## Minimal checklist for adding a unit / category / operation (copy-paste)

* [ ] Add `Unit` static field in `Domain/Core/Unit.cs`.
* [ ] Create concrete unit class under `Domain/Units/<Category>/NewUnit.cs`.
* [ ] Update `QuantityService.ResolveUnit(string)` to map unit name -> `Unit.*`.
* [ ] Update `QuantityService.CreateQuantity(double, Unit)` to return new concrete class.
* [ ] Update Console (menu) to show the unit in the category list.
* [ ] Add unit tests verifying conversion & arithmetic behavior.
* [ ] Update API examples/docs and (optionally) swagger descriptions.

---
