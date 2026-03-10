
---

# 📏 Quantity Measurement System

A modular, extensible, and object-oriented **Quantity Measurement Console Application** built in C#.

This system allows users to perform:

* ✅ Comparison
* 🔄 Conversion
* ➕ Addition
* ➖ Subtraction
* ➗ Division

Across multiple measurement categories:

* Length
* Weight
* Volume
* Temperature

The project follows clean architecture principles and leverages abstraction, inheritance, generics, and design patterns to ensure scalability and maintainability.

---

# 🏗️ Architecture Overview

The project follows a **layered and hierarchical architecture**:

```
QuantityMeasurementApp
│
├── EntryPoint
│   └── Program.cs
│
├── Menu
│   └── Menu.cs
│
├── FactoryPattern
│   └── FactoryPattern.cs
│
├── Core
│   ├── QuantityAbstractBaseClass.cs
│   └── Unit.cs
│
├── Units
│   ├── Length
│   │   ├── LengthAbstractSubBaseClass.cs
│   │   ├── Feet.cs
│   │   ├── Inches.cs
│   │   ├── Yard.cs
│   │   ├── Meter.cs
│   │   └── Centimeter.cs
│   │
│   ├── Weight
│   │   ├── WeightAbstractSubBaseClass.cs
│   │   ├── Kilogram.cs
│   │   ├── Gram.cs
│   │   └── Pound.cs
│   │
│   ├── Volume
│   │   ├── VolumeAbstractSubBaseClass.cs
│   │   ├── Litre.cs
│   │   ├── Millilitre.cs
│   │   └── Gallon.cs
│   │
│   └── Temperature
│       ├── TemperatureAbstractSubBaseClass.cs
│       ├── Celsius.cs
│       ├── Fahrenheit.cs
│       └── Kelvin.cs
```

---

# 🧩 Interface-Driven Design

This system uses interfaces to enforce contracts and promote loose coupling.

## 🔹 `IMenu`

* Decouples entry point from UI implementation
* Enables alternative UI layers (Console, Web, GUI)
* Supports Dependency Inversion Principle

## 🔹 `IQuantity<T>`

* Enforces immutability
* Requires value + unit
* Requires equality contract
* Ensures hash consistency
* Allows future polymorphic handling of quantities

---

---

# 🎯 Core Design Principles Used

### 1️⃣ Abstraction

All measurement logic is abstracted into the base class:

👉 `Quantity`

Concrete units only define identity — not behavior.

---

### 2️⃣ Open/Closed Principle (OCP)

The system is:

* ✅ Open for extension
* ❌ Closed for modification

You can add new units or categories without modifying core logic.

---

### 3️⃣ Single Responsibility Principle (SRP)

Each class has a clearly defined purpose:

* `Unit` → Conversion metadata
* `Quantity` → Mathematical logic
* Sub-classes → Category-specific typing
* Concrete units → Identity wrappers
* `MenuClass` → User interaction
* `Factory` → Object creation abstraction

---

### 4️⃣ Encapsulation

* Value validation inside constructor
* Conversion logic hidden inside `Unit`
* Arithmetic protected against invalid category mixing

---

# 🏛️ Design Patterns Used

## 🔹 1. Factory Pattern

Implemented in:

```csharp
Factory<K, T>
```

Used in `Program.cs` to create:

```csharp
IMenu menu = Factory<IMenu, MenuClass>.CreateObject();
```

### Why?

* Removes direct instantiation dependency
* Allows easy swapping of implementations
* Supports Dependency Inversion

---

## 🔹 2. Factory Method Pattern

Inside abstract sub-base classes:

```csharp
public override Quantity CreateInstance(double value, Unit unit)
```

Each category (Length, Weight, etc.) controls how its concrete objects are created.

This ensures:

* Correct type returned
* Strong typing
* No external type checking required

---

## 🔹 3. Template Method Pattern

The base class `Quantity` defines:

* ConvertTo()
* Add()
* Subtract()
* Divide()
* Equals()

Subclasses reuse the template while specializing behavior via:

```csharp
CreateInstance()
```

---

# 🧠 Core System Components

---

# 🔷 Entry Point

## `Program.cs`

Namespace: `QuantityMeasurement.EntryPoint`

Responsibilities:

* Application starting point
* Uses generic factory
* Creates `IMenu`
* Calls `StartMenu()`

---

# 🔷 Menu Layer

## `MenuClass`

Namespace: `QuantityMeasurement.Menu`

Responsibilities:

* User interface
* Category selection
* Operation selection
* Input reading
* Display formatting

Features:

* Generic category handling
* Shared helpers:

  * `ReadQuantity()`
  * `ShowUnits()`
  * `GetUnit()`
  * `CreateQuantity()`

---

# 🔷 Core Layer

---

## 📌 `Unit` Class

Namespace: `QuantityMeasurement.Core`

### Purpose:

Represents a measurement unit.

### Contains:

* `Name`
* `ConversionFactorToBase`
* `OffsetToBase` (for temperature)
* `Category`

### Static Predefined Units:

Length:

* Feet
* Inch
* Yard
* Meter
* Centimeter

Weight:

* Kilogram
* Gram
* Pound

Volume:

* Litre
* Millilitre
* Gallon

Temperature:

* Celsius
* Fahrenheit
* Kelvin

### Key Methods:

```csharp
ConvertToBaseUnit(double value)
ConvertFromBaseUnit(double baseValue)
```

Temperature conversion is handled using:

```
(value + offset) * factor
```

This avoids circular dependencies.

---

## 📌 `Quantity` (Abstract Base Class)

Namespace: `QuantityMeasurement.Core`

### Core Responsibilities:

* Equality logic
* Conversion logic
* Arithmetic operations
* Validation
* Enforces category safety

### Properties:

```csharp
double value
Unit unit
```

### Key Methods:

| Method           | Purpose                  |
| ---------------- | ------------------------ |
| ConvertTo()      | Convert to target unit   |
| Add()            | Add two quantities       |
| Subtract()       | Subtract quantities      |
| Divide(Quantity) | Return scalar            |
| Divide(double)   | Return new Quantity      |
| Equals()         | Category-safe comparison |

### Safety Guards:

* Prevents cross-category math
* Prevents division by zero
* Rejects NaN and Infinity
* Enforces category consistency

---

# 🔷 Units Layer (Hierarchy)

All categories follow this structure:

```
IQuantity<T>
    ↓
Quantity (abstract)
    ↓
Length / Weight / Volume / Temperature ( Category abstract sub-base)
    ↓
Feet / Kilogram / Litre / Celsius (concrete classes)
```

---

## 🔹 Length

Base class: `Length : Quantity`

Concrete Units:

* Feet
* Inches
* Yard
* Meter
* Centimeter

Supports:

* Convert
* Add
* Subtract
* Divide

---

## 🔹 Weight

Base class: `Weight : Quantity`

Concrete Units:

* Kilogram
* Gram
* Pound

---

## 🔹 Volume

Base class: `Volume : Quantity`

Concrete Units:

* Litre
* Millilitre
* Gallon

---

## 🔹 Temperature

Base class: `Temperature : Quantity`

Concrete Units:

* Celsius
* Fahrenheit
* Kelvin

Special Handling:

Temperature supports offset-based conversion.

---

# 🔢 Supported Operations

| Operation | Description               |
| --------- | ------------------------- |
| Compare   | Uses base unit equality   |
| Convert   | Converts to selected unit |
| Add       | Optional target unit      |
| Subtract  | Optional target unit      |
| Divide    | Quantity or scalar        |

---

# ⚙️ How Conversion Works Internally

1️⃣ Convert value to base unit
2️⃣ Perform math in base unit
3️⃣ Convert result to target unit

This ensures:

* Mathematical correctness
* Avoids floating point drift
* Category isolation

---

# 🚀 How to Extend the System

The architecture is intentionally expandable.

---

## ✅ Adding a New Unit (Example: Kilometer)

### Step 1:

Add in `Unit.cs`

```csharp
public static readonly Unit Kilometer = 
    new Unit("Kilometer", 3280.84, LENGTH);
```

### Step 2:

Create concrete class:

```csharp
public class Kilometer : Length
{
    public Kilometer(double value)
        : base(value, Unit.Kilometer)
    {
    }
}
```

### Step 3:

Update `CreateInstance()` in `Length`

Done. 🎉

---

## ✅ Adding a New Category (Example: Time)

### Step 1:

Add new category constant in `Unit`

### Step 2:

Create `Time : Quantity`

### Step 3:

Create concrete units:

* Second
* Minute
* Hour

### Step 4:

Update `Menu`

No core modification needed.

---

# 🛡️ Error Handling

The system handles:

* Invalid category mixing
* Invalid unit selection
* Division by zero
* NaN / Infinity values
* Null references

---

# 💡 Why This Architecture Is Powerful

✔ Fully extensible

✔ Strong type safety

✔ Zero duplication of math logic

✔ Category isolation

✔ Already validated with 247 passing NUnit test cases

✔ Console UI decoupled from business logic

✔ Future ready for ASP.NET / Web API integration

---

# 🧪 Example Usage

```
=== Quantity Measurement System ===

1. Length
2. Weight
3. Volume
4. Temperature
```

You can:

* Compare 12 Inch and 1 Foot
* Convert 5 Meter to Yard
* Add 2 Litre + 500 Millilitre
* Divide 10 Kilogram / 2 Kilogram
* Convert 100 Celsius to Fahrenheit

---

# 🧪 Testing

This project includes a dedicated test project:

```
QuantityMeasurement.Tests
```

Built using **NUnit** on **.NET 8.0**.

### ✔ Test Execution Result

```bash
dotnet test BusinessLogic/QuantityMeasurementApp.sln
```

**Result:**

* ✅ Total Tests: 247
* ✅ Passed: 247
* ❌ Failed: 0
* ⏱ Duration: 23 ms
* 🎯 Framework: .NET 8.0
* 🧪 Test Framework: NUnit

The system has been thoroughly validated for:

* Equality comparisons
* Cross-unit conversions
* Addition (with and without target unit)
* Subtraction (with and without target unit)
* Division by quantity
* Division by scalar
* Category mismatch protection
* Zero division protection
* Offset-based temperature conversions
* Floating-point tolerance handling

---

# 🤔 Is This an Adapter Pattern?

Short answer:

👉 No — this is not an Adapter pattern.

Let’s break it down carefully.

---

## 🔹 What Adapter Pattern Actually Does

The Adapter pattern:

* Converts one interface into another
* Allows incompatible interfaces to work together
* Wraps an existing class

Example scenario:
You have a third-party library with a weird API, and you create a wrapper to make it match your expected interface.

---

## 🔹 What I Actually Built



✔ Interface-based abstraction
✔ Template Method pattern
✔ Factory Method pattern
✔ Generic Factory pattern
✔ Strong domain hierarchy

But not Adapter.

My abstract classes are not adapting incompatible interfaces.
They are **providing shared behavior to concrete implementations**.

This is classic:

* **Layered OOP architecture**
* **Polymorphism**
* **Domain modeling**

---

# 💎 What Pattern This Actually Resembles More

If we categorize it correctly:

### 🔹 1. Template Method Pattern

`Quantity` defines algorithm structure.
Subclasses customize via `CreateInstance()`.

---

### 🔹 2. Factory Method Pattern

Sub-base classes decide concrete object creation.

---

### 🔹 3. Dependency Inversion Principle (DIP)

`Program` depends on `IMenu`.

---

### 🔹 4. Strategy-like Behavior (Implicitly)

Because:

* Conversion behavior is delegated to `Unit`
* Arithmetic is centralized in `Quantity`
* Category enforcement is embedded

But still not Adapter.




# 🎯 Why This Layered Approach Matters

The hierarchy:

```
Interface → Abstract Base → Sub-Base → Concrete Class
```

Provides:

✔ Compile-time safety
✔ Strong domain typing
✔ Code reuse
✔ Extension without modification
✔ Clean separation of concerns
✔ High testability


---

# 🧭 Future Improvements

* Add multiplication
* Add unit parsing from string
* Add JSON serialization support
* Add REST API layer
* Add GUI (WPF / ASP.NET Core)
* Add precision configuration strategy

---

# 📌 Conclusion

This project demonstrates:

* Advanced OOP
* Clean layered architecture
* Proper abstraction
* Design pattern implementation
* Expandable engineering mindset

It is not just a unit converter — it is a scalable measurement framework.

---
