# Software Design Document: Core & Infrastructure Layers

## 1. Architectural Overview
This system is built using Clean Architecture principles, specifically focusing on a strict separation between business logic and data access. The architecture relies on the Dependency Inversion Principle to ensure the business rules remain completely isolated from database technologies.

* **The Dependency Rule:** The Infrastructure layer references the Core layer. The Core layer references nothing.
* **The Communication Bridge:** The Core layer defines repository interfaces. The Infrastructure layer implements them. 

## 2. Core Layer Design (`CleanApi.Core`)
The Core layer is the heart of the application. It contains pure C# code with no dependencies on Entity Framework, SQL Server, or third-party mapping libraries.

### 2.1 Domain Models
Models are divided into two categories based on their required behaviour:
* **Rich Domain Models (e.g., `OrderModel`):** Used for transactional data that carries risk or strict business rules. State is encapsulated using private setters. New instances can only be created via static factory methods (e.g., `OrderModel.CreateNew()`) to guarantee that invalid states cannot materialise.
* **Anemic Models (e.g., `CustomerModel`, `StatusModel`):** Used for simple reference data and lookups. These are standard plain-old CLR objects (POCOs) with public setters, as they do not require strict behavioural validation.

### 2.2 Interfaces (Contracts)
The Core layer dictates exactly what it needs from the outside world using interfaces.
* **Specific Repositories (`IOrderRepository`):** Defines full CRUD operations tailored specifically to the `OrderModel`. 
* **Generic Lookups (`ILookupRepository<TModel>`):** A reusable read-only contract used to fetch reference data without duplicating boilerplate code.

### 2.3 Services (Orchestration)
* **`OrderService`:** Acts as the workflow conductor. It does not perform database queries directly. Instead, it uses the injected interfaces to look up required foreign keys (like Customer and Status), invokes the Rich Domain Model to enforce business rules, and passes the resulting valid model back to the repository for persistence.

## 3. Infrastructure Layer Design (`CleanApi.Infrastructure`)
The Infrastructure layer is responsible for translating the pure concepts of the Core layer into physical database operations. It relies heavily on Entity Framework (EF) Core and Mapster.

### 3.1 Database Entities
* **Entities (e.g., `OrderEntity`, `CustomerEntity`):** These are strictly 1:1 mappings of the physical SQL Server tables. They contain foreign key properties and navigation collections required by EF Core.

### 3.2 Data Context and Configurations
* **Fluent API Configurations (`IEntityTypeConfiguration<T>`):** Database schema rules (primary keys, column names, foreign key constraints) are cleanly separated into individual configuration files rather than cluttering the entity classes with data annotations.
* **`AppDbContext`:** The EF Core session manager that applies the configurations and exposes the `DbSet` properties for querying.

### 3.3 Repository Implementations
Repositories in this layer act as bilingual translators between SQL and C#.
* **Specific Repositories (`OrderRepository`):** Implements `IOrderRepository`. It takes an `OrderModel`, uses Mapster to map it to an `OrderEntity`, and saves it via EF Core.
* **Generic Lookup Repository (`LookupRepository<TEntity, TModel>`):** Implements `ILookupRepository<TModel>`. It uses Mapster's `.ProjectToType<TModel>()` to translate the EF Core `IQueryable` directly into the Core model, ensuring the Core layer never receives a raw database entity.

## 4. Data Flow Example: Creating an Order
To illustrate the separation of concerns, here is the lifecycle of a data creation request within these two layers:

1.  The `OrderService` (Core) receives a request containing raw strings and numbers.
2.  The `OrderService` calls `ILookupRepository<StatusModel>.FindAsync()` (Core contract).
3.  The `LookupRepository` (Infrastructure) executes a SQL `SELECT` statement, maps the `OrderStatusLookupEntity` to a `StatusModel`, and returns it.
4.  The `OrderService` passes the validated data into `OrderModel.CreateNew()` (Core).
5.  The `OrderModel` runs its internal business logic and returns a valid instance of itself.
6.  The `OrderService` passes this valid model to `IOrderRepository.AddAsync()` (Core contract).
7.  The `OrderRepository` (Infrastructure) maps the `OrderModel` to an `OrderEntity`, generates an `INSERT` statement via EF Core, commits the transaction, and returns the model with its new database-generated ID.

## 5. System Architecture Diagrams

To visualise the separation of concerns, we rely on logical architecture diagrams rather than physical database schemas. This highlights the direction of dependencies and the flow of data through the application.



### 5.1 The Dependency Inversion Flow
* **The Core:** Sits at the absolute centre of the system. It contains no external references.
* **The Infrastructure:** Forms the outer ring. It points inward, meaning it relies on the Core to define the rules of engagement (interfaces and models) before it can do its job connecting to the database.



### 5.2 The Request Lifecycle
When a command is executed, the flow follows a strict path to ensure business logic is never bypassed:
1.  **Request Input:** Hits the Core `Service`.
2.  **Orchestration:** The `Service` asks the `Repository` interface for required reference data.
3.  **Data Access:** The concrete `Repository` translates the request into SQL, fetches the database `Entity`, maps it to a `Model`, and returns it.
4.  **Business Logic:** The `Service` uses the `Model` to execute strict domain rules.
5.  **Persistence:** The `Service` passes the validated `Model` back to the `Repository` to be saved as an `Entity`.

## 6. Entity versus Model Properties

A common question when adopting Clean Architecture is why we duplicate properties across Entities and Models. The answer lies in their fundamentally different responsibilities. 

An Entity is designed for **persistence and relational data mapping**. A Model is designed for **business logic and encapsulation**.



### 6.1 The Entity Properties (Infrastructure Layer)
The Entity exists solely to satisfy Entity Framework Core and mirror your SQL tables. Its properties are purely structural.

* **Public Setters:** Every property must be completely open (`{ get; set; }`) so Entity Framework can easily write data to it when materialising records from the database.
* **Foreign Keys:** It contains raw integer IDs (like `CustomerId` and `StatusId`) to explicitly define SQL relationships.
* **Navigation Properties:** It uses complex types (like `public CustomerEntity Customer { get; set; }`) and collections (`ICollection<OrderEntity>`) to allow Entity Framework to perform SQL `JOIN` operations.
* **Zero Behaviour:** It contains no methods, no validation, and no business rules.

**Example Entity:**
```csharp
public class OrderEntity : IBaseEntity
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int StatusId { get; set; } // Raw database key
    public decimal TotalAmount { get; set; }

    public CustomerEntity Customer { get; set; } // SQL Join mapping
    public OrderStatusLookupEntity Status { get; set; } // SQL Join mapping
}
```

### 6.2 The Model Properties (Core Layer)
The Model exists to enforce the rules of your business. Its properties are heavily protected.

* **Private Setters:** Properties are locked down ({ get; private set; }). The rest of the application cannot arbitrarily change an order's total amount. It must pass through a specific method that enforces your business rules.
* **Flattened Data:** It rarely uses foreign key IDs. Instead of a StatusId, it holds the actual StatusName. The business logic cares about the word "Pending", not the arbitrary integer 1 assigned by the database.
* **No Navigation Collections:** A Rich Domain Model does not expose lists of child objects unless the root model explicitly controls them.
* **Rich Behaviour:** It contains methods like UpdateStatus(string newStatus) to ensure state changes are legal.

**Example Model**
```csharp
public class OrderModel : IBaseModel
{
    public int Id { get; set; } 
    public int CustomerId { get; private set; }
    public string StatusName { get; private set; } // Flattened meaningful data
    public decimal TotalAmount { get; private set; }

    private OrderModel() { }

    public void UpdateStatus(string newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
            throw new ArgumentException("Status cannot be empty.");
            
        StatusName = newStatus;
    }
}
```

### The Mapping Bridge
The Infrastructure repository uses Mapster to bridge the gap between these two structures. When an OrderEntity is retrieved from SQL, Mapster looks at the Status navigation property and automatically maps the Status.StatusName string directly into the Model's StatusName property. This keeps the Core layer completely unaware of the relational database structure.
