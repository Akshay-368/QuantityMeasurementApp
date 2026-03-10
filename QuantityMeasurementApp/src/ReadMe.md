# QuantityMeasurementApp — Detailed Architecture & Codebase README

**Generated:** 2026-03-09

---

## Purpose
This README documents the architecture, project layout, core classes, functions and dependencies of the `QuantityMeasurementApp` codebase (focus: `src/` projects). It is intended to be used as a developer-facing reference for understanding how the system is structured and how the pieces interact.

> **Note:** There is a legacy `BusinessLogic/` project at the repo root. The authoritative, active implementation lives under `src/` — this README focuses on `src/` as requested.

---

## High-level Architecture

The solution is arranged as a small, layered application following a clean-ish separation of concerns:

- **Domain** — core domain model: `Quantity`, `Unit`, category sub-bases (Length, Weight, Volume, Temperature), and concrete unit definitions (Meter, Inch, Kilogram, Litre, etc.). Domain is a library with no external dependencies.
- **Application** — application services / use-cases and DTOs. Implements MediatR-style request/handler pairs for operations (convert, add, subtract, divide, compare). Contains `QuantityFactory` for constructing domain instances and retrieving available units.
- **Infrastructure** — placeholder for infra concerns (currently minimal / stubbed).
- **Web** — ASP.NET Core Web API exposing endpoints that forward requests to Application handlers via `IMediator` (MediatR).
- **QuantityMeasurement.Tests** — NUnit test project with a suite of unit tests validating the domain behavior and use-cases.

Diagram (text):

```
Client -> Web API (Controllers) -> Application (MediatR Handlers) -> Domain (Quantity & Unit)
                                             ^
                                             |
                                       Infrastructure (optional)
```

---

## Projects (summary)

All paths are relative to `src/`.

### 1) `src/Domain` — core model
**Purpose:** Provide immutable, well-tested domain types and conversion logic.

Key files and responsibilities:

- `Core/IQuantity.cs` — interface contract for `Quantity` types (generic). Declares properties and operation signatures expected from a `Quantity`.
- `Core/QuantityAbstractBaseClass.cs` — **`Quantity` (abstract)** — central class implementing `IQuantity<Quantity>`.
  - Properties: `double value`, `Unit unit`.
  - Key methods:
    - `ConvertTo(Unit targetUnit)` — convert this quantity into a `Quantity` expressed using `targetUnit`.
    - `Add(Quantity other)` / `Add(Quantity other, Unit targetUnit)` — add two quantities (enforcing category equivalence) and return a `Quantity` in the appropriate unit.
    - `Subtract(...)` — subtract two quantities (similar semantics to Add).
    - `Divide(Quantity other)` — divide two quantities to return a scalar.
    - `Divide(double divisor)` — divide by scalar and return `Quantity` (preserves unit).
    - Equality/Hashing: `Equals`, `GetHashCode`, `ToString` — the class enforces reflexive, null, type and value equality and **explicitly enforces same category** (you cannot meaningfully compare Length to Weight).
  - `CreateInstance(double value, Unit unit)` (factory-pattern style) — *protected/abstract* method implemented by concrete subclasses to create a concrete typed `Quantity` instance.

- `Core/Unit.cs` — **`Unit`** representation for units used by quantities.
  - Immutable `Unit` class storing at least `Name`, `Category`, `ConversionFactorToBase`, and `OffsetToBase`.
  - Provides static readonly instances for supported units: `Feet`, `Inch`, `Yard`, `Meter`, `Centimeter`, `Kilogram`, `Gram`, `Pound`, `Litre`, `Millilitre`, `Gallon`, `Celsius`, `Fahrenheit`, `Kelvin`.
  - Methods:
    - `ConvertToBaseUnit(double value)` — apply factor/offset to convert a unit's value into category base unit.
    - `ConvertFromBaseUnit(double baseValue)` — reverse operation.

- `Units/*AbstractSubBaseClass.cs` (e.g. `LengthAbstractSubBaseClass.cs`, `WeightAbstractSubBaseClass.cs`, `VolumeAbstractSubBaseClass.cs`, `TemperatureAbstractSubBaseClass.cs`) — small abstract subclasses of `Quantity` which provide a typed base for concrete units in that category. These implement `CreateInstance` and category-specific behavior where necessary.

- `Units/<Category>/*` — concrete classes for each unit (e.g. `Meter`, `Centimeter`, `Feet`, `Inches`, `Yard`, `Kilogram`, `Gram`, `Pound`, `Litre`, `Millilitre`, `Gallon`, `Celsius`, `Fahrenheit`, `Kelvin`).
  - Each concrete class typically inherits from its category abstract base and provides a constructor like `public Meter(double value) : base(value, Unit.Meter) { }`.

**Notes & behaviors**
- Category enforcement: Operations that combine two quantities check `unit.Category` and throw `InvalidOperationException` when attempting to combine different categories (e.g., Length with Weight).
- Temperature: Units with offsets (Celsius/Fahrenheit) are modeled with `OffsetToBase` inside `Unit` and conversion logic handles offsets (important for temperature conversions).


### 2) `src/Application` — application/use-case layer
**Purpose:** Encapsulate application logic and expose use-cases via request/response DTOs and MediatR `IRequestHandler`s.

Key folders:
- `DTOs/QuantityDto.cs` — DTO representing an input quantity (likely `Value` and `UnitName`).
- `DTOs/ResultDto.cs` — DTO used as a standardized result payload (value, unit name, maybe a textual representation).

- `Features/Quantities/*` — the main feature set for quantity operations. Files present:
  - `ConvertQuantityQuery.cs` and `ConvertQuantityHandler.cs` — convert one quantity to another unit.
  - `AddQuantitiesCommand.cs` and `AddQuantitiesHandler.cs` — add two quantities and return a `ResultDto`.
  - `SubtractQuantitiesCommand.cs` and `SubtractQuantitiesHandler.cs` — subtract two quantities.
  - `DivideQuantitiesQuery.cs` and `DivideQuantitiesHandler.cs` — divide two quantities to obtain a scalar.
  - `DivideByScalarCommand.cs` and `DivideByScalarHandler.cs` — divide a quantity by a scalar (result is a `Quantity`).
  - `CompareQuantitiesQuery.cs` and `CompareQuantitiesHandler.cs` — compare two quantities (e.g. less/equal/greater) and return a standardized result (probably boolean/int/result DTO).

- `Features/QuantityFactory.cs` — utility class that maps category strings into lists of `Unit` static instances and creates `Quantity` instances from a `Unit` (factory mapping:
  - `GetUnitsByCategory(string category)` returns supported units for that category.
  - `CreateQuantity(double value, Unit unit)` dispatches by `unit.Category` to concrete domain classes (e.g. `new Feet(value)`, `new Meter(value)`, etc.).

**How handlers work (typical flow):**
1. Handler receives a request DTO.
2. Handler uses `QuantityFactory.GetUnitByName(...)` (or similar) to resolve unit static instances.
3. Create domain `Quantity` using `QuantityFactory.CreateQuantity(value, unit)`.
4. Use domain methods (e.g. `ConvertTo`, `Add`, `Subtract`) to perform the operation.
5. Return `ResultDto` with the numeric result, unit name and string representation.


### 3) `src/Infrastructure`
**Purpose:** Intended for cross-cutting or external concerns (persistence, adapters). The current content is minimal (a `Class1.cs` placeholder) — appears to be a scaffold for future infra code.

Files:
- `Class1.cs` — stub. No production logic at this time.


### 4) `src/Web` — ASP.NET Core Web API
**Purpose:** HTTP API layer that maps routes to `MediatR` requests.

Key files:
- `Program.cs` — ASP.NET startup wiring (DI for `IMediator`, controllers, swagger etc. — typical ASP.NET Core minimal setup).
- `Controllers/QuantitiesController.cs` — controller exposing endpoints for the main quantity operations. Routes observed:
  - `POST /api/quantities/convert` — convert quantity
  - `POST /api/quantities/add` — add two quantities
  - `POST /api/quantities/subtract` — subtract
  - `POST /api/quantities/compare` — compare two quantities
  - `POST /api/quantities/divide` — divide two quantities (scalar result)
  - `POST /api/quantities/divide-scalar` — divide quantity by scalar

Controller behaviour: it accepts the appropriate request DTO (command/query) and calls `_mediator.Send(...)` to dispatch to Application layer handlers.


### 5) `QuantityMeasurement.Tests` — unit tests
**Purpose:** NUnit tests for the domain and probably several UC (use-case) tests.

Notable test files:
- `UnitTestFeetUC-1.cs`
- `UnitTestInchesUC-2.cs`
- `UnitTestLengthEqualityUC-3.cs`
- `UnitTestConverionUC-5.cs`
- `UnitTestTemperatureUC-14.cs`
- `UnitTestVolumeUC-11.cs`
- ... plus more UC tests (subtraction, division, etc.)

Run tests with:
```bash
cd src/.. (or repo root)
dotnet test
```

---

## Important code-level details & walkthrough (selected files)

Below are concise, actionable descriptions of the most important classes and methods so you can navigate and extend the code quickly.

### Domain/Core/QuantityAbstractBaseClass.cs (abstract `Quantity`)
- Implements arithmetic operations and conversions.
- **Equality semantics:** enforces same category, reflexive, and value equality after normalizing to base unit.
- **Conversion:** uses `Unit.ConvertToBaseUnit(...)` and `Unit.ConvertFromBaseUnit(...)` for all category conversions (so category base unit acts as an intermediate).
- **Factory hook:** `CreateInstance(double value, Unit unit)` lets concrete subclasses produce instances of their concrete type.
- **Operations provided:** `ConvertTo`, `Add`, `Subtract`, `Divide(Quantity) -> double`, `Divide(double) -> Quantity`.


### Domain/Core/Unit.cs
- Centralized unit metadata and static instances.
- Each `Unit` encodes `Category`, `ConversionFactorToBase`, and `OffsetToBase` (important for temperature conversions which include offsets).
- Use `Unit.ConvertToBaseUnit` & `Unit.ConvertFromBaseUnit` for safe, symmetric conversions.
- **Supported static units:** Feet, Inch, Yard, Meter, Centimeter, Kilogram, Gram, Pound, Litre, Millilitre, Gallon, Celsius, Fahrenheit, Kelvin.


### Application/Features/Quantities/ConvertQuantityHandler.cs
- Resolves `sourceUnit` and `targetUnit` via `QuantityFactory`.
- Creates `sourceQty` with `QuantityFactory.CreateQuantity`.
- Calls `sourceQty.ConvertTo(targetUnit)` and wraps result in `ResultDto`.

Patterns used: MediatR `IRequest` and `IRequestHandler<TRequest,TResponse>` for use-cases, making them easy to test and wire into Web via DI.


### Web/Controllers/QuantitiesController.cs
- Simple mapping: every operation corresponds to a `POST` route that accepts a command/query object.
- Controller delegates to `_mediator.Send(...)` and returns `Ok(result)` (200) — minimal presentation logic.


## Dependency & Layering Notes

- **Web -> Application**: Web depends on Application for request/response DTOs and for handler dispatching via `IMediator`.
- **Application -> Domain**: Application uses `QuantityFactory` and domain model (`Quantity`, `Unit`). All business rules and conversions live in Domain.
- **Infrastructure**: currently unused by Application and Domain; set up to host infra code in future (DB, caching, adapters).
- **Tests** depend on Domain (and possibly Application) for functional tests.

Dependency tree (simplified):

```
Web
  -> Application
       -> Domain
Infrastructure (independent placeholder)
Tests -> Domain (+ Application)
```


## How to build, run and test

Prerequisites: .NET 8 SDK (project targets net8.0 based on `obj` output), `dotnet` CLI.

Commands:

- Build solution / projects

```bash
# Build Web project (and through project references it will build others)
cd src/Web
dotnet build

# or from repo root build all projects
# if there is a top-level solution, use it (there is a legacy BusinessLogic solution and a Domain.sln). You can also build projects manually:
dotnet build src/Domain/QuantityMeasurement.Domain.csproj
dotnet build src/Application/QuantityMeasurement.Application.csproj
dotnet build src/Web/QuantityMeasurement.Web.csproj
```

- Run Web API (from `src/Web`):
```bash
cd src/Web
dotnet run
# check logs for URLs or visit https://localhost:<port>/swagger if swagger is configured
```

- Run Tests:
```bash
dotnet test QuantityMeasurement.Tests/QuantityMeasurement.Tests.csproj
```


## API surface (controller endpoints)

Controller: `QuantitiesController` (route `api/quantities`)

| Route | HTTP | Request DTO | Description |
|---|---:|---|---|
| `/api/quantities/convert` | POST | `ConvertQuantityQuery` | Convert a source quantity to target unit. |
| `/api/quantities/add` | POST | `AddQuantitiesCommand` | Add two quantities (same category). |
| `/api/quantities/subtract` | POST | `SubtractQuantitiesCommand` | Subtract two quantities. |
| `/api/quantities/compare` | POST | `CompareQuantitiesQuery` | Compare two quantities (>, <, ==). |
| `/api/quantities/divide` | POST | `DivideQuantitiesQuery` | Divide two quantities -> scalar. |
| `/api/quantities/divide-scalar` | POST | `DivideByScalarCommand` | Divide quantity by scalar -> quantity. |

(Exact DTO field names are in `src/Application/DTOs/*.cs` — `QuantityDto` includes `Value` and `UnitName`.)


## Extension & Contribution Guidelines (developer notes)

- **Add new units or categories:**
  1. Add a new static `Unit` instance in `Domain/Core/Unit.cs` with proper `Category`, `ConversionFactorToBase` and `OffsetToBase` when needed.
  2. Add a concrete `Quantity` type class in appropriate `src/Domain/Units/<Category>` folder inheriting from the category abstract base.
  3. Update `Application/Features/QuantityFactory.cs` to include the new unit(s) in `GetUnitsByCategory` and ensure `CreateQuantity` can construct the correct concrete type for that category.

- **Business rules:** Keep them in `Domain` whenever they are domain rules (e.g., category enforcement, conversion math, numeric tolerances). Keep application orchestration (DTO mapping, request validation) in `Application`.

- **Infrastructure:** If you add persistence or adapters (e.g., store historical conversions), add appropriate interfaces in `Application` and concrete implementations in `Infrastructure`, then inject implementations in `Web` at startup.


## Tests & UC Coverage

The test project contains a variety of UC-named tests that map to functional requirements (e.g., UC-1 Feet, UC-2 Inches, UC-3 Length equality, UC-5 Conversion, UC-11 Volume). You can inspect these tests under `QuantityMeasurement.Tests/` for input cases and expected results. They are a good guide to the business rules currently encoded in `Domain`.


## Known gaps & observations made during automated inspection

- There is a legacy `BusinessLogic/` project at repo root. The `src/` folder appears to be the newer, modular implementation.
- `src/Infrastructure` is only a scaffold (`Class1.cs`) — no persistence or external adapters currently implemented.
- I observed one file in the legacy `BusinessLogic` containing `...` placeholder(s) (`BusinessLogic/Menu.cs`). If you intended to preserve all behavior from BusinessLogic, cross-check that logic against the newer `src/` implementation; some behaviour may not yet have been migrated.


## Files index (concise)

**Domain**
- `src/Domain/Core/IQuantity.cs` — quantity contract
- `src/Domain/Core/QuantityAbstractBaseClass.cs` — arithmetic & conversion logic
- `src/Domain/Core/Unit.cs` — unit metadata & static unit instances
- `src/Domain/Units/*` — category abstract bases and concrete unit classes (Length/Temperature/Volume/Weight)

**Application**
- `src/Application/DTOs/QuantityDto.cs`, `ResultDto.cs`
- `src/Application/Features/Quantities/*` — Commands/Queries + Handlers (Convert, Add, Subtract, Divide, DivideByScalar, Compare)
- `src/Application/Features/QuantityFactory.cs`

**Web**
- `src/Web/Controllers/QuantitiesController.cs` — API endpoints
- `src/Web/Program.cs`

**Tests**
- `QuantityMeasurement.Tests/*` — NUnit tests for UCs


---

## Codebase Architecture

The system follows **Clean Architecture principles** (as described by Robert C. Martin) implemented using a layered project structure. While the folder layout may resemble a traditional N‑tier architecture, the *design intention and dependency direction* align with **Clean Architecture**.

The key rule followed in this codebase is the **Dependency Rule**:

```
Outer layers depend on inner layers
Inner layers never depend on outer layers
```

This ensures that the **core business logic (Domain)** remains completely independent from frameworks, UI, and infrastructure concerns.

Conceptually the architecture maps like this:

```
Frameworks & Drivers
        │
        ▼
Interface Adapters (Web / Controllers)
        │
        ▼
Application Layer (Use Cases / Handlers / CQRS)
        │
        ▼
Domain Layer (Enterprise Business Rules)
```

### Mapping to this repository

| Clean Architecture Layer | Project Folder |
|---|---|
| Enterprise Business Rules | `src/Domain` |
| Application Business Rules | `src/Application` |
| Interface Adapters | `src/Web` |
| Frameworks / Drivers | `src/Infrastructure` |

Even though the projects are arranged in layers, the **dependency direction strictly follows Clean Architecture**, not traditional N‑tier coupling.

### Why this is not traditional N‑tier

In classic N‑tier systems, layers typically depend on each other sequentially and business logic may leak into service or controller layers.

In this project:

- The **Domain layer has zero dependencies** on any other project.
- The **Application layer orchestrates use cases but delegates all core logic to Domain**.
- The **Web layer only handles HTTP concerns** and forwards requests using MediatR.
- Infrastructure is designed to implement external integrations without affecting Domain.

Because of this separation, the architecture achieves:

- High **testability**
- Strong **separation of concerns**
- Easy **extensibility** for new units and features
- Long‑term **maintainability**

---


#  Extending the System

This section explains **how to extend the system in the future** while keeping the architecture clean and maintainable.

The project follows **Clean Architecture principles**, so all new features must respect the following dependency rule:

```
Web → Application → Domain
                ↑
         Infrastructure
```

* **Domain** contains core business logic.
* **Application** defines use cases and orchestration.
* **Web** exposes HTTP APIs.
* **Infrastructure** integrates external services.

When extending the project, new logic should be placed in the **correct layer** to maintain separation of concerns.

---

# 1. Adding a New Measurement Category

Example: Add a new category like **Area**, **Speed**, or **Energy**.

### Step 1 – Add Category Units in Domain

Create a new folder inside:

```
src/Domain/Units
```

Example:

```
src/Domain/Units/Area
```

---

### Step 2 – Create an Abstract Base Class

Create a new abstract class.

Example:

```
AreaAbstractSubBaseClass.cs
```

Example structure:

```csharp
public abstract class Area : Quantity
{
    protected Area(double value, Unit unit)
        : base(value, unit) { }

    protected override Quantity CreateInstance(double value, Unit unit)
    {
        return QuantityFactory.CreateQuantity(value, unit);
    }
}
```

This ensures all Area units inherit common behavior.

---

### Step 3 – Define Units in `Unit.cs`

Open:

```
src/Domain/Core/Unit.cs
```

Add new units.

Example:

```csharp
public static readonly Unit SquareMeter =
    new Unit("SquareMeter", "Area", 1);

public static readonly Unit SquareKilometer =
    new Unit("SquareKilometer", "Area", 1_000_000);
```

Each unit must include:

```
Unit Name
Category
ConversionFactorToBase
Offset (if applicable)
```

---

### Step 4 – Create Concrete Unit Classes

Inside:

```
src/Domain/Units/Area
```

Create unit classes.

Example:

```
SquareMeter.cs
SquareKilometer.cs
```

Example implementation:

```csharp
public class SquareMeter : Area
{
    public SquareMeter(double value)
        : base(value, Unit.SquareMeter) { }
}
```

---

### Step 5 – Update QuantityFactory

Open:

```
src/Application/Features/QuantityFactory.cs
```

Add the new units.

Example:

```csharp
case "Area":
    return new List<Unit>
    {
        Unit.SquareMeter,
        Unit.SquareKilometer
    };
```

Also update the `CreateQuantity` method to create instances of the new classes.

---

### Step 6 – Add Tests

Create new test cases inside:

```
QuantityMeasurement.Tests
```

Example:

```
UnitTestAreaUC.cs
```

Test:

```
Conversion
Addition
Equality
Division
```

---

# 2. Adding a New Unit to an Existing Category

Example: Add **Kilometer** to the Length category.

---

### Step 1 – Add Unit in `Unit.cs`

```csharp
public static readonly Unit Kilometer =
    new Unit("Kilometer", "Length", 1000);
```

---

### Step 2 – Create Unit Class

Location:

```
src/Domain/Units/Length
```

Example:

```
Kilometer.cs
```

Implementation:

```csharp
public class Kilometer : Length
{
    public Kilometer(double value)
        : base(value, Unit.Kilometer) { }
}
```

---

### Step 3 – Update Factory

Modify:

```
QuantityFactory.cs
```

Add the new unit inside `GetUnitsByCategory`.

---

### Step 4 – Add Tests

Add conversion tests such as:

```
1 km = 1000 meters
```

---

# 3. Adding a New Operation (Example: Multiplication)

Currently the system supports:

```
Add
Subtract
Divide
Compare
Convert
```

To add **multiplication**, follow the steps below.

---

## Step 1 – Add Domain Method

Open:

```
src/Domain/Core/QuantityAbstractBaseClass.cs
```

Add a new method.

Example:

```csharp
public Quantity Multiply(double factor)
{
    double result = this.value * factor;
    return CreateInstance(result, this.unit);
}
```

This allows:

```
5 meters * 3 = 15 meters
```

---

## Step 2 – Create Command in Application Layer

Location:

```
src/Application/Features/Quantities
```

Create:

```
MultiplyQuantityCommand.cs
```

Example:

```csharp
public class MultiplyQuantityCommand : IRequest<ResultDto>
{
    public QuantityDto Quantity { get; set; }
    public double Factor { get; set; }
}
```

---

## Step 3 – Create Handler

Create:

```
MultiplyQuantityHandler.cs
```

Example:

```csharp
public class MultiplyQuantityHandler
    : IRequestHandler<MultiplyQuantityCommand, ResultDto>
{
    public Task<ResultDto> Handle(
        MultiplyQuantityCommand request,
        CancellationToken cancellationToken)
    {
        var unit = QuantityFactory.GetUnitByName(request.Quantity.UnitName);

        var quantity = QuantityFactory.CreateQuantity(
            request.Quantity.Value,
            unit
        );

        var result = quantity.Multiply(request.Factor);

        return Task.FromResult(new ResultDto
        {
            Value = result.Value,
            UnitName = result.Unit.Name
        });
    }
}
```

---

## Step 4 – Add Controller Endpoint

Open:

```
src/Web/Controllers/QuantitiesController.cs
```

Add endpoint.

Example:

```csharp
[HttpPost("multiply")]
public async Task<IActionResult> Multiply(
    MultiplyQuantityCommand command)
{
    var result = await _mediator.Send(command);
    return Ok(result);
}
```

---

# 4. Expanding the Infrastructure Layer

The `Infrastructure` project is currently a **placeholder**.
In the future it can hold integrations with external systems.

Examples include:

```
Database
Redis Cache
RabbitMQ Messaging
Logging
External APIs
```

---

# 5. Adding a Database

Future folder structure:

```
src/Infrastructure
   ├── Persistence
   │     ├── ApplicationDbContext.cs
   │     └── Repositories
   └── Configurations
```

Steps:

### 1 Install Entity Framework

```
dotnet add package Microsoft.EntityFrameworkCore
```

---

### 2 Create DbContext

Example:

```
ApplicationDbContext.cs
```

```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<QuantityRecord> Quantities { get; set; }
}
```

---

### 3 Register in Web Layer

Modify:

```
Program.cs
```

```csharp
builder.Services.AddDbContext<ApplicationDbContext>();
```

---

# 6. Adding Redis (Caching)

Redis can be used for:

```
Caching conversion results
Reducing repeated calculations
Storing temporary data
```

### Install package

```
dotnet add package StackExchange.Redis
```

---

### Add Service in Infrastructure

```
RedisCacheService.cs
```

Example:

```csharp
public class RedisCacheService
{
    private readonly IDatabase _cache;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _cache = redis.GetDatabase();
    }
}
```

---

# 7. Adding RabbitMQ (Messaging)

RabbitMQ can be used for:

```
Event processing
Background tasks
Notification systems
Microservice communication
```

---

### Install RabbitMQ client

```
dotnet add package RabbitMQ.Client
```

---

### Create Messaging Service

Example:

```
RabbitMqPublisher.cs
```

```csharp
public class RabbitMqPublisher
{
    public void Publish(string message)
    {
        // publish message to queue
    }
}
```

---

### Register in Dependency Injection

Inside:

```
Program.cs
```

```csharp
builder.Services.AddSingleton<RabbitMqPublisher>();
```

---

# 8. Future Possible Features

The architecture allows many extensions:

### New Measurement Categories

```
Area
Speed
Energy
Pressure
Time
```

### New Operations

```
Multiplication
Percentage
Scaling
Normalization
```

### Advanced Features

```
Conversion history
User-defined units
Batch conversions
Analytics
```

---

# 9. Key Design Principle for Future Development

Always follow these rules:

```
Business logic → Domain
Use cases → Application
HTTP/API → Web
External services → Infrastructure
```

Maintaining this separation ensures the project remains:

```
Extensible
Maintainable
Testable
Scalable
```

---

# How BusinessLogic/ was extracted and converted to src/
I ran a **whole sequence of real .NET CLI commands** that engineers use to construct a multi-project backend solution. I’ll turn everything I ran into a **clear reference guide** that anyone can revisit later.

I’ll organize it into:

1️⃣ Core .NET concepts (solution vs project)

2️⃣ Project creation commands

3️⃣ Solution management commands

4️⃣ Dependency commands (project references)

5️⃣ NuGet package commands

6️⃣ Build / restore commands

7️⃣ Common mistakes you hit and why they happened

8️⃣ A **recommended command order** cheat sheet

---

# 1️⃣ Core Concepts Behind All These Commands

## Solution (`.sln`)

A **solution** is a container for multiple projects.

Example:

```
QuantityMeasurementApp.sln
```

It tells the .NET tooling:

* which projects belong together
* what to build
* what to open in Visual Studio

It **does not compile code** itself.

---

## Project (`.csproj`)

A **project** is a compilable unit.

Examples from my system:

```
QuantityMeasurement.Domain.csproj
QuantityMeasurement.Application.csproj
QuantityMeasurement.Infrastructure.csproj
QuantityMeasurement.Web.csproj
QuantityMeasurement.Tests.csproj
```

Each project builds into:

```
Domain.dll
Application.dll
Infrastructure.dll
Web.dll
Tests.dll
```

---

# 2️⃣ Creating Projects

## Command

```
dotnet new
```

Creates new projects from templates.

### Syntax

```
dotnet new <template> -n <ProjectName> -o <Folder>
```

### Parameters

| Parameter  | Meaning       |
| ---------- | ------------- |
| `template` | project type  |
| `-n`       | project name  |
| `-o`       | output folder |

---

## Command I Ran

### Create Application layer

```
dotnet new classlib -n QuantityMeasurement.Application -o src/Application
```

Meaning:

| Part       | Meaning                  |
| ---------- | ------------------------ |
| `classlib` | create a class library   |
| `-n`       | project name             |
| `-o`       | folder to generate files |

Result:

```
src/Application/
   QuantityMeasurement.Application.csproj
```

Class libraries compile to **DLLs**.

---

### Create Infrastructure layer

```
dotnet new classlib -n QuantityMeasurement.Infrastructure -o src/Infrastructure
```

Result:

```
src/Infrastructure/
   QuantityMeasurement.Infrastructure.csproj
```

---

### Create Web API

```
dotnet new webapi -n QuantityMeasurement.Web -o src/Web
```

Template used:

ASP.NET Core Web API.

Result:

```
src/Web/
   QuantityMeasurement.Web.csproj
   Program.cs
   appsettings.json
   Controllers/
```

---

# 3️⃣ Adding Projects to a Solution

## Command

```
dotnet sln add
```

### Syntax

```
dotnet sln <solutionPath> add <projectPath>
```

### Meaning

Registers a project inside the solution.

Without this step:

* the solution cannot build the project
* Visual Studio won't show it

---

## Example I Ran

```
dotnet sln BusinessLogic/QuantityMeasurementApp.sln add src/Application/QuantityMeasurement.Application.csproj
```

Meaning:

```
Solution
   + Application project
```

---

### Adding Infrastructure

```
dotnet sln BusinessLogic/QuantityMeasurementApp.sln add src/Infrastructure
```

CLI detects `.csproj` automatically.

---

### Adding Web project

```
dotnet sln BusinessLogic/QuantityMeasurementApp.sln add src/Web/QuantityMeasurement.Web.csproj
```

---

## Listing projects in a solution

```
dotnet sln <solution> list
```

Example:

```
dotnet sln BusinessLogic/QuantityMeasurementApp.sln list
```

Output:

```
Project(s)
----------
Domain
Application
Infrastructure
Web
Tests
```

---

# 4️⃣ Project References (Dependency Graph)

This is **the most important command in multi-project architecture**.

Projects **cannot access each other automatically**.

You must add references.

---

## Command

```
dotnet add reference
```

### Syntax

```
dotnet add <project> reference <otherProject>
```

Meaning:

```
Project A depends on Project B
```

---

## Example

```
dotnet add src/Application/QuantityMeasurement.Application.csproj reference src/Domain/QuantityMeasurement.Domain.csproj
```

Meaning:

```
Application → Domain
```

Now Application can use:

```
Quantity
Length
Weight
Volume
Temperature
```

---

## Infrastructure references

```
dotnet add src/Infrastructure/QuantityMeasurement.Infrastructure.csproj reference src/Application/QuantityMeasurement.Application.csproj
```

Meaning:

```
Infrastructure → Application
```

---

Another reference:

```
dotnet add src/Infrastructure/QuantityMeasurement.Infrastructure.csproj reference src/Domain/QuantityMeasurement.Domain.csproj
```

Meaning:

```
Infrastructure → Domain
```

---

## Web references

```
dotnet add src/Web/QuantityMeasurement.Web.csproj reference src/Application/QuantityMeasurement.Application.csproj
```

```
dotnet add src/Web/QuantityMeasurement.Web.csproj reference src/Infrastructure/QuantityMeasurement.Infrastructure.csproj
```

Meaning:

```
Web → Application
Web → Infrastructure
```

---

# 5️⃣ NuGet Package Installation

Packages come from **NuGet**.

NuGet is the .NET ecosystem’s package manager.

---

## Command

```
dotnet add package
```

### Syntax

```
dotnet add <project> package <packageName>
```

---

## Example

Installing **MediatR**

```
dotnet add src/Application package MediatR.Extensions.Microsoft.DependencyInjection
```

Purpose:

Implements the **Mediator pattern**.

Used heavily in Clean Architecture.

---

### Installing Swagger

```
dotnet add src/Web package Swashbuckle.AspNetCore
```

Swagger generates API documentation.

Tool: **Swashbuckle**

---

### Installing EF Core

```
dotnet add src/Infrastructure package Microsoft.EntityFrameworkCore.SqlServer
```

ORM library:

**Entity Framework Core**

---

### Why your EF install failed

You saw:

```
Package supports net10.0
Your project is net8.0
```

Solution:

```
dotnet add src/Infrastructure package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
```

---

# 6️⃣ Build Command

## Command

```
dotnet build
```

### Syntax

```
dotnet build <solutionOrProject>
```

Example:

```
dotnet build BusinessLogic/QuantityMeasurementApp.sln
```

Build process:

1️⃣ Restore packages
2️⃣ Compile code
3️⃣ Generate DLLs

Output example:

```
Domain.dll
Application.dll
Infrastructure.dll
Web.dll
```

---

# 7️⃣ Restore Command (Implicit)

Whenever you install packages:

```
dotnet restore
```

This downloads dependencies from NuGet.

But CLI automatically runs restore when needed.

---

# 8️⃣ Common Errors You Encountered

## Typo in package name

You typed:

```
MediatR.Extensions.Mircrosoft.DependencyInjection
```

Correct:

```
Microsoft
```

NuGet couldn't find it.

---

## Wrong path

You typed:

```
src/Application/QuantityMeasurement/Application.csproj
```

Correct:

```
src/Application/QuantityMeasurement.Application.csproj
```

---

## Solution path mistake

You typed:

```
dotnet build BusinessLogicQuantityMeasurementApp.sln
```

Correct:

```
dotnet build BusinessLogic/QuantityMeasurementApp.sln
```

---

# 9️⃣ Clean Architecture Dependency Rule

Your references now follow this structure:

```
Web → Application → Domain
Infrastructure → Application → Domain
```

Rule:

```
Outer layers depend on inner layers
Inner layers depend on nothing
```

Domain is the **most independent layer**.

---

# 🔟 Recommended Command Order (Cheat Sheet)

Create solution:

```
dotnet new sln -n QuantityMeasurementApp
```

Create projects:

```
dotnet new classlib -n QuantityMeasurement.Domain -o src/Domain
dotnet new classlib -n QuantityMeasurement.Application -o src/Application
dotnet new classlib -n QuantityMeasurement.Infrastructure -o src/Infrastructure
dotnet new webapi -n QuantityMeasurement.Web -o src/Web
```

Add projects to solution:

```
dotnet sln add src/Domain/QuantityMeasurement.Domain.csproj
dotnet sln add src/Application/QuantityMeasurement.Application.csproj
dotnet sln add src/Infrastructure/QuantityMeasurement.Infrastructure.csproj
dotnet sln add src/Web/QuantityMeasurement.Web.csproj
```

Add references:

```
dotnet add src/Application reference src/Domain
dotnet add src/Infrastructure reference src/Application
dotnet add src/Infrastructure reference src/Domain
dotnet add src/Web reference src/Application
dotnet add src/Web reference src/Infrastructure
```

Install packages:

```
dotnet add src/Application package MediatR.Extensions.Microsoft.DependencyInjection
dotnet add src/Web package Swashbuckle.AspNetCore
```

Build:

```
dotnet build
```

---

# Then this is how I copied the content from the legacy BusinessLogic/ to src/Domain/
---

# 1️⃣ `mkdir`

### Full Name

**Make Directory**

### Purpose

Creates a **new folder** in the file system.

### Syntax

```bash
mkdir <directory_path>
```

### Example you ran

```bash
mkdir src\Domain\Core
```

### Meaning

Create a folder structure:

```
src/
   Domain/
      Core/
```

Windows automatically creates **intermediate folders** if they don't exist.

### Why you used it

You are restructuring your project so the **Domain layer has its own structure**.

```
src
 └── Domain
      └── Core
```

This is typical in **Clean Architecture**.

---

# 2️⃣ `copy`

### Purpose

Copies files from one location to another.

### Syntax

```bash
copy <source> <destination>
```

### Example

```bash
copy BusinessLogic\Core\QuantityAbstractBaseClass.cs src\Domain\Core\
```

### Meaning

Copy:

```
BusinessLogic/Core/QuantityAbstractBaseClass.cs
```

to

```
src/Domain/Core/
```

### Why

You are **moving domain logic from BusinessLogic → Domain layer**.

Instead of manually dragging files, you used CLI.

---

# 3️⃣ `copy *.cs`

### Purpose

Copy **multiple files using wildcard**.

### Syntax

```bash
copy <source_folder>\*.extension <destination>
```

### Example

```bash
copy BusinessLogic\Units\Length\*.cs src\Domain\Units\Length\
```

### Meaning

Copy **all `.cs` files** from:

```
BusinessLogic/Units/Length/
```

into

```
src/Domain/Units/Length/
```

Wildcard:

```
*.cs
```

means:

```
all C# files
```

---

# 4️⃣ Path Separators

You used two styles:

```
src\Domain\Core
src/Domain/Units
```

### Why the first worked

Windows CMD prefers:

```
\
```

### Why this failed

```
mkdir src/Domain/Units/Length
```

Because CMD sometimes misinterprets `/` as **command option flag**.

Example:

```
dir /s
```

Here `/s` is a flag.

So Windows thought:

```
/Domain
```

was a flag.

Correct version:

```
mkdir src\Domain\Units\Length
```

---

# 5️⃣ Folder Structure You Built

Your commands created this:

```
QuantityMeasurementApp
│
├── src
│   └── Domain
│       │
│       ├── Core
│       │     QuantityAbstractBaseClass.cs
│       │     Unit.cs
│       │     IQuantity.cs
│       │
│       └── Units
│            │
│            ├── Length
│            │     Feet.cs
│            │     Inches.cs
│            │     Meter.cs
│            │     Yard.cs
│            │     Centimeter.cs
│            │
│            ├── Temperature
│            │     Celsius.cs
│            │     Fahrenheit.cs
│            │     Kelvin.cs
│            │
│            ├── Volume
│            │     Litre.cs
│            │     Millilitre.cs
│            │     Gallon.cs
│            │
│            └── Weight
│                  Gram.cs
│                  Kilogram.cs
│                  Pound.cs
```

This is actually **very close to a real DDD structure**.

---

# 6️⃣ Why You Deleted `Class1.cs`

When you create a new **.NET class library**, it generates:

```
Class1.cs
```

Example:

```csharp
public class Class1
{
}
```

It’s just a placeholder.

Since your project already has real domain classes:

```
Quantity
Unit
Length
Weight
```

You removed it.

Correct move. 👍

---

# 7️⃣ Why This Step Matters Architecturally

You basically did:

```
BusinessLogic → Domain
```

Which aligns with **Clean Architecture rule**:

```
UI
Application
Domain
Infrastructure
```

Domain must contain:

✔ Entities
✔ Value Objects
✔ Business rules
✔ Domain logic

Which is exactly what your **Quantity system is**.

---

# 8️⃣ What Your Domain Layer Now Contains

Your domain now includes:

### Core

```
Quantity
Unit
IQuantity
```

### Units (Domain Entities / Value Objects)

```
Length
Weight
Volume
Temperature
```

These are **pure business concepts**.

Which means they are **framework independent**.

That is a **huge Clean Architecture rule**.

---

# 9️⃣ One Important Step You Will Need Next

After copying files you must **update namespaces**.

Example:

Before:

```csharp
namespace QuantityMeasurementApp.BusinessLogic.Core;
```

After:

```csharp
namespace QuantityMeasurementApp.Domain.Core;
```

Otherwise compilation errors will happen.

---

# 🔟 Why This Refactor Is Actually Advanced

Most beginners accidentally create:

```
Controller
   ↓
Service
   ↓
Repository
```

But you already have a **rich domain model**.

Your model:

```
Quantity
Length
Weight
Temperature
```

is **DDD style modeling**.

Which means your ASP.NET app will become:

```
API
 ↓
Application
 ↓
Domain (your current code)
```

Very professional architecture. 🚀

---

# 1️⃣1️⃣ Quick Command Reference Table

| Command     | Meaning                | Example                 |
| ----------- | ---------------------- | ----------------------- |
| `mkdir`     | create folder          | `mkdir src\Domain\Core` |
| `copy`      | copy file              | `copy A.cs B\`          |
| `copy *.cs` | copy multiple files    | `copy src\*.cs dest\`   |
| `\`         | Windows path separator | `src\Domain`            |
| `*`         | wildcard               | `*.cs`                  |

---

# 1️⃣2️⃣ Small Tip for Future

Instead of `copy`, you can use:

```
move
```

Syntax:

```
move <source> <destination>
```

Example:

```
move BusinessLogic\Core\*.cs src\Domain\Core\
```

Difference:

```
copy → duplicate
move → relocate
```

---

# This is the next step

Now that Domain exists:

Next step is creating the **Application layer**.

You will run:

```
dotnet new classlib -n QuantityMeasurement.Application
```

and move **use cases** there later.

Then the architecture becomes:

```
src
 ├── Domain
 ├── Application
 ├── Infrastructure
 └── API
```

Which is **textbook Clean Architecture**.

---

I executed **three important things here** after that :

1️⃣ Installed **NuGet packages**
2️⃣ Navigated directories
3️⃣ Built the entire solution

---

# 1️⃣ `cd` — Change Directory

### Meaning

Moves the terminal to another folder.

### Syntax

```bash
cd <path>
```

### Example you ran

```bash
cd C:\Users\aksha\OneDrive\Documents\QuantityMeasurementApp\src\Web
```

### What it did

Moved terminal working directory to:

```
QuantityMeasurementApp
   └── src
        └── Web
```

Why?

Because **dotnet commands operate on the current folder’s project**.

---

# 2️⃣ `dotnet add package`

### Purpose

Installs a **NuGet package** into a project.

NuGet = .NET package manager.

Equivalent to:

```
npm install
pip install
maven dependency
```

### Syntax

```bash
dotnet add package <PackageName>
```

or

```bash
dotnet add <project>.csproj package <PackageName>
```

---

# 3️⃣ Installing MediatR

You ran:

```bash
dotnet add package MediatR
```

### What happened internally

Step by step:

1️⃣ CLI contacted **NuGet registry**

```
https://api.nuget.org
```

2️⃣ It downloaded metadata:

```
mediatr/index.json
```

3️⃣ Then downloaded package:

```
mediatr.14.1.0.nupkg
```

4️⃣ Installed into local cache:

```
C:\Users\aksha\.nuget\packages\
```

5️⃣ Updated your project file:

```
QuantityMeasurement.Web.csproj
```

Added this:

```xml
<PackageReference Include="MediatR" Version="14.1.0" />
```

---

# 4️⃣ What is MediatR

### MediatR implements the **Mediator Pattern**

Idea:

```
Controller
   ↓
Mediator
   ↓
Handler
   ↓
Domain
```

Instead of:

```
Controller → Service → Repository
```

I use:

```
Controller → Command → Handler
```

Example:

```
AddLengthCommand
ConvertTemperatureQuery
CompareWeightQuery
```

Each handled by a **Handler class**.

This keeps controllers **very thin**.

Very popular in **Clean Architecture + CQRS**.

---

# 5️⃣ Second Package You Installed

```
dotnet add package MediatR.Extensions.Microsoft.DependencyInjection
```

### Why needed

ASP.NET Core uses **Dependency Injection (DI)**.

This package connects:

```
MediatR → ASP.NET Core DI container
```

So handlers get auto-registered.

---

# 6️⃣ Important Warning You Got

You saw this:

```
warning NU1608
Detected package version outside dependency constraint
```

Meaning:

```
MediatR.Extensions.Microsoft.DependencyInjection 11.1.0
expects MediatR < 12.0
```

But you installed:

```
MediatR 14.1.0
```

So:

```
Extension expects old version
You installed newer version
```

⚠️ It's only a **warning**, not an error.

The build still succeeded.

---

### Best fix (optional)

Install matching versions:

```
dotnet add package MediatR --version 11.1.0
```

or remove extension and use built-in registration.

But for now it's fine.

---

# 7️⃣ `cd ..`

### Meaning

Move **one folder up**.

Example:

```
src/Web
```

becomes

```
src
```

Then again:

```
src
```

becomes

```
QuantityMeasurementApp
```

I did:

```
cd ..
cd ..
```

To go back to **solution root**.

---

# 8️⃣ `dotnet build`

I ran:

```
dotnet build
```

Error:

```
MSBUILD : error MSB1003
Specify a project or solution file
```

Meaning:

Current folder did **not contain**:

```
.csproj
or
.sln
```

So .NET didn't know **what to build**.

---

# 9️⃣ Correct Command You Used

```
dotnet build BusinessLogic/QuantityMeasurementApp.sln
```

### Syntax

```
dotnet build <solution-file>
```

or

```
dotnet build <project-file>
```

Example:

```
dotnet build MyApp.csproj
```

---

# 🔟 What `dotnet build` Does Internally

Build pipeline:

1️⃣ Restore NuGet packages
2️⃣ Compile C# code
3️⃣ Generate DLL files
4️⃣ Check dependencies
5️⃣ Report warnings/errors

Output goes to:

```
bin/Debug/net8.0/
```

---

# 1️⃣1️⃣ Build Output You Saw

```
QuantityMeasurement.Domain -> ...Domain.dll
```

Meaning:

Domain project compiled successfully.

Then:

```
QuantityMeasurement.Application -> ...Application.dll
```

Then:

```
Infrastructure.dll
```

Then:

```
Web.dll
```

This shows **correct dependency order**.

Example dependency graph:

```
Web
 ↓
Application
 ↓
Domain
```

Exactly what **Clean Architecture wants**.

---

# 1️⃣2️⃣ Your Solution Structure Now

The solution currently contains:

```
QuantityMeasurementApp.sln
│
├── BusinessLogic (old console app)
│
├── QuantityMeasurement.Tests
│
└── src
    │
    ├── Domain
    │
    ├── Application
    │
    ├── Infrastructure
    │
    └── Web
```

This is actually **production architecture**.

---

# 1️⃣3️⃣ Where Build Outputs Go

Example:

```
src\Domain\bin\Debug\net8.0\
```

contains:

```
QuantityMeasurement.Domain.dll
```

DLL = compiled code.

Web project then references these.

---

# 1️⃣4️⃣ Important Dotnet CLI Commands

Here is a **reference list** I should remember.

| Command                | Purpose                |
| ---------------------- | ---------------------- |
| `dotnet new`           | create project         |
| `dotnet build`         | compile project        |
| `dotnet run`           | build + run            |
| `dotnet restore`       | download packages      |
| `dotnet add package`   | install NuGet package  |
| `dotnet add reference` | add project dependency |
| `dotnet clean`         | remove build files     |

---

# 1️⃣5️⃣ Example of Project Reference

Later I'll probably run:

```
dotnet add src/Web reference src/Application
```

Meaning:

```
Web depends on Application
```

Then:

```
dotnet add src/Application reference src/Domain
```

Meaning:

```
Application depends on Domain
```

Which enforces architecture rule:

```
Outer layers depend on inner layers
```

---

# 1️⃣6️⃣ What Architecture Is Now

```
Presentation
   │
   ▼
Web (ASP.NET Core)
   │
   ▼
Application
   │
   ▼
Domain
```

Infrastructure attaches later.

This is **Uncle Bob Clean Architecture**.



---

# 1️⃣7️⃣ Small Thing That Shows This is Right

The build output shows:

```
0 Error(s)
```

and only:

```
2 Warning(s)
```

Warnings about package version mismatch.

Which means **your refactor did not break compilation**.

That’s actually a **great sign** when restructuring architecture.

---

