# 📦 Enterprise Inventory Management System (ERP) - .NET 10

A high-performance, enterprise-grade **Inventory & Multi-Warehouse Management System** built with the cutting-edge **.NET 10** stack. This project showcases a sophisticated **Clean Architecture** implementation, focusing on scalability, type safety, and rigorous automated testing.

## 🏗️ Architectural Excellence

The solution is engineered with a strict separation of concerns across four primary layers:

* **Domain:** Contains pure business entities (`Product`, `Warehouse`, `Category`, `ProductStock`) and domain-driven logic.
* **Application:** Implements the core business orchestration via services (`ProductService`, `CategoryService`, etc.) using the **Unit of Work** and **Repository** patterns.
* **Shared Project (The Bridge):** A centralized project shared between the **Web API** and **Blazor UI**. 
    * **Unified DTOs:** Eliminates code duplication across the network.
    * **FluentValidation:** Shared validation logic ensuring "Write Once, Validate Anywhere" consistency on both Client and Server.
* **Infrastructure:** Handles data persistence using **EF Core**, SQL Server, and automated image lifecycle management.

## 🚀 Key Technical Implementation

### 🔹 Advanced Stock Engine
* **Multi-Warehouse Support:** Real-time stock tracking across various geographical locations.
* **Lookup Intelligence:** Specialized services for fetching comprehensive warehouse/product data, enabling "First-time Stock Entry" even for empty warehouses.
* **Optimized Projections:** Utilizing AutoMapper's `ProjectTo<T>` for high-performance SQL execution, retrieving only necessary fields.

### 🔹 Blazor UI & UX Intelligence
* **Dynamic Filtering:** Enterprise-level search and filtering by SKU, Product Name, or Warehouse directly within the Blazor UI.
* **Unified Action Bar:** A reactive UI component for Stock Movements (Adjustments, Transfers, Opening Balances) that adapts dynamically to user input.

### 🔹 Intelligent Media Management
* **Automated Image Lifecycle:** Handles unique file naming (Guids), physical storage on the server, and automatic cleanup of old assets during updates or deletions.

## 🧪 Quality Assurance (Test-Driven Mindset)

We ensure 100% reliable business flows through a robust **Unit Test Suite** using **xUnit** and **Moq**:

* **Isolated Service Tests:** Mocking `IUnitOfWork` and Repositories to verify business logic without side effects.
* **Conflict Handling:** Rigorous testing of edge cases like duplicate SKUs, category name conflicts, and soft-deactivation logic.
* **Performance-Aware Logic:** Tests verify that basic updates (e.g., changing a description) do not trigger redundant and expensive database lookups.

## 🛠️ Tech Stack

* **Framework:** .NET 10 (LTS Ready)
* **Frontend:** Blazor WebAssembly
* **Backend:** ASP.NET Core Web API
* **Database:** SQL Server + EF Core
* **Mapping:** AutoMapper
* **Validation:** FluentValidation (Shared Logic)
* **Testing:** xUnit, Moq, FluentAssertions

## ⚙️ Quick Start

1.  **Clone the Repository:**
    ```bash
    git clone [https://github.com/samwel314/ERP-Inventory-Management-System)
    ```
2.  **Database Migration:**
    Update your connection string in `appsettings.json` and run:
    ```bash
    dotnet ef database update --project InventoryManagement.Infrastructure
    ```
3.  **Run:**
    Set both **API** and **Client** as startup projects and launch.

---
*Developed by Samuel — Focused on Clean Code & Scalable Architecture.*
