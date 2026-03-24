## Data Flow Across Project Layers

Based on the analysis of your Quantity Measurement App, here's a comprehensive overview of how data flows through the layered architecture:

### **Layer Hierarchy & Dependencies**
The app follows a **clean layered architecture** with these dependencies:
- **Domain** (foundation - no dependencies)
- **Infrastructure** → Domain
- **Application** → Domain + Infrastructure  
- **API** → Application + Infrastructure
- **Console** → Domain only
- **Tests** → Domain only

### **Key Data Flow Patterns**

#### **1. API Request Flow (Web Clients)**
```
Client HTTP Request → API Layer → Application Layer → Domain Layer → Infrastructure Layer → Database/Cache → Response
```

**What flows:**
- **Client → API**: `QuantityRequestDto` (JSON) with values, units, operation type
- **API → Application**: Same DTO, plus JWT authentication context
- **Application → Domain**: Domain objects (`Quantity`, `Unit`) created from DTO data
- **Domain → Domain**: Business logic execution (conversions, arithmetic)
- **Application → Infrastructure**: `HistoryRecord` for persistence
- **Infrastructure → Database**: EF Core entities (`History`) saved to SQL Server
- **Infrastructure → Cache**: History data cached in Redis (30min TTL)
- **Response Path**: Results flow back as `QuantityResultDto` → JSON response

**Cross-cutting concerns:**
- JWT authentication at API layer
- Rate limiting (5 requests/10s window)
- Custom time-logging middleware

#### **2. Console Application Flow (Desktop Users)**
```
User Input → Console Layer → Domain Layer → Display Results
```

**What flows:**
- **User → Console**: Raw text input (values, menu selections)
- **Console → Domain**: Direct instantiation of `Quantity` objects (e.g., `new Feet(value)`)
- **Domain → Domain**: Same business logic as API
- **Domain → Console**: Result values displayed to console
- **No persistence** - console app doesn't use database or services

#### **3. History Retrieval Flow (with Caching)**
```
Client → API → Infrastructure → Cache/DB → Response
```

**What flows:**
- **Cache-first**: Redis checked for `"history:all"` key
- **Cache miss**: SQL Server queried via EF Core
- **Data mapping**: `History` entities → `HistoryRecord` DTOs
- **Cache storage**: Results cached for 30 minutes
- **Response**: List of history records as JSON

### **Layer Connections & Interfaces**

| Connection | Interface/Contract | Data Flowing |
|------------|-------------------|--------------|
| **API ↔ Application** | `IQuantityService` | `QuantityRequestDto` → `QuantityResultDto` |
| **Application ↔ Domain** | Direct instantiation | Primitive values → `Quantity` objects |
| **Application ↔ Infrastructure** | `IHistoryRepository` | `HistoryRecord` → void (persistence) |
| **Infrastructure ↔ Database** | `IQuantityDbContext` | EF Core entities ↔ SQL Server |
| **Infrastructure ↔ Cache** | `IDistributedCache` | JSON serialized history data |
| **Console ↔ Domain** | Direct instantiation | User input → `Quantity` objects |

### **Key Data Transformation Points**

1. **DTO → Domain Objects**: Application layer parses string units and creates type-safe `Quantity` instances
2. **Domain Results → DTOs**: Application layer converts results back to serializable DTOs
3. **Domain → Persistence**: History records mapped to EF entities for database storage
4. **Database → Domain**: Cached/invalidated data flows back through repository interfaces

### **Special Flow Characteristics**

- **Immutability**: All `Quantity` and `Unit` objects are immutable once created
- **Type Safety**: Specialized quantity classes (`Length`, `Weight`, etc.) prevent invalid operations
- **Caching Strategy**: Cache-aside pattern with Redis for history data
- **Authentication**: JWT tokens flow through API middleware but don't reach domain layer
- **Separation**: Console bypasses application/infrastructure layers entirely for simplicity

The architecture ensures clean separation while allowing different entry points (web API vs console) to access the same core business logic in the domain layer.