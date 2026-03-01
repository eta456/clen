# Clean Architecture Order API

This repository is a production-ready demonstration of Clean Architecture in .NET. It enforces strict separation of concerns, utilises Rich Domain Models to protect business logic, and replaces generic repositories with highly focused, specific repositories.

## Project Structure

The solution is divided into three distinct layers:
1. **CleanApi.Core**: Pure business logic, domain models, and interfaces. Zero dependencies on databases or web frameworks.
2. **CleanApi.Infrastructure**: Database connectivity, Entity Framework Core configurations, and repository implementations.
3. **CleanApi.Api**: The HTTP entry point, controllers, DTOs, and input validation.

---

## How to Build This Project (Step-by-Step)

When adopting a robust, data-conscious architecture, you must build from the inside out. You never start by writing an API controller. Instead, you define your business rules, establish your database schema mappings, and build the front door last.

Here is the exact order of execution and the reasoning behind it.

### Step 1: Define the Core Models and Interfaces
**Where:** `CleanApi.Core/Models` and `CleanApi.Core/Interfaces`
**What you do:** Write your `OrderModel`, `CustomerModel`, and the repository contracts like `IOrderRepository`.
**Why start here?** In software engineering, you must define "what" your application does before you define "how" it does it. By creating your Rich Domain Models first, you establish the strict rules of your business (like ensuring an order's initial amount is greater than zero). By writing the interfaces, you create a strict contract that your database layer must eventually fulfil. 

### Step 2: Establish the Infrastructure and Database Setup
**Where:** `CleanApi.Infrastructure/Entities`, `Configurations`, and `AppDbContext.cs`
**What you do:** Create your plain database entities (e.g., `OrderEntity`), write the EF Core Fluent API configurations to map them to exact SQL columns, and build the `DbContext`.
**Why do this second?** Since we are highly focused on the database structure, we immediately translate our pure domain models into a physical SQL reality. Doing this early ensures your database schema is perfectly tuned (with correct foreign keys and column names) before you start writing complex business workflows. 



### Step 3: Implement the Specific Repositories
**Where:** `CleanApi.Infrastructure/Repositories`
**What you do:** Write classes like `OrderRepository` that implement `IOrderRepository`. Use Mapster to translate the database `OrderEntity` back into the pure `OrderModel`.
**Why do this third?**
You now have your Core contracts (Step 1) and your Database context (Step 2). The repository simply bridges the gap between them. You build out your full CRUD operations here, completely isolating Entity Framework from the rest of the application.

### Step 4: Build the Core Services (Business Orchestration)
**Where:** `CleanApi.Core/Services/OrderService.cs`
**What you do:** Write the code that coordinates the workflows, like looking up a customer, creating a new order model, and telling the repository to save it.
**Why do this fourth?**
Now that the database can actually save and retrieve data, you can build the workflows that use it. The Service acts as the conductor of the orchestra. It does not talk to the database directly; it relies entirely on the interfaces from Step 1.

### Step 5: Construct the API Layer
**Where:** `CleanApi.Api/Controllers`, `Dtos`, and `Validators`
**What you do:** Define the JSON payloads your frontend will send (`CreateOrderDto`), write FluentValidation rules to secure them, configure Mapster to map DTOs to Core Models, and build the `OrdersController`.
**Why do this last?**
The API is just a delivery mechanism. It is the least important part of your core business. By building it last, the controller remains incredibly thin. Its only job is to receive an HTTP request, validate the payload, and hand it straight over to the `OrderService`.

---

## Tech Stack
* **Framework:** .NET 8.0
* **Database Access:** Entity Framework Core
* **Object Mapping:** Mapster
* **Validation:** FluentValidation