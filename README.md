# 📦 Stockyo — Multi-Tenant SaaS Inventory & Batch Analytics Platform

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blueviolet.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean_Architecture-blue.svg)]()
[![SaaS Multi-Tenant](https://img.shields.io/badge/SaaS-Multi--Tenant-orange.svg)]()
[![Repository Pattern](https://img.shields.io/badge/Pattern-Repository_%26_UoW-green.svg)]()

Stockyo is an Enterprise-grade, Multi-Tenant Cloud-Ready Software-as-a-Service (SaaS) platform engineered to optimize supply chains and resolve critical retail inefficiencies for small and medium-sized businesses (SMBs). Built on **.NET 8** following **Clean Architecture** principles, the platform acts as an intelligent assistant that eliminates capital stagnation (Overstocking) and prevents missed revenue opportunities (Out-of-Stock) through algorithmic inventory control and real-time data auditing.

---

## 🚀 Project Overview

### ❌ The Problem
Many local shop and retail owners struggle with two major financial leaks that directly drain their profits:
1. **Overstocking:** Buying excessive inventory that sits dead on shelves, tying up vital business capital.
2. **Out of Stock:** Missing out on immediate sales and suffering customer churn because high-demand products are unavailable when requested.

### ✔️ The Solution
**Stockyo** bridges this structural gap by providing a smart, data-driven system. Instead of just recording baseline sales, the backend continuously logs and analyzes transaction patterns to generate an automated "Purchase Plan." By tracking precise product batches and expiry metrics, the system minimizes waste, logs lost demands dynamically, and empowers owners to make purchasing decisions based on real telemetry rather than guesswork.

---

## ✨ System Features

* **Multi-Tenant SaaS Data Isolation:** Implements strict logical tenant segmentation at the database level, ensuring complete data privacy, encryption, and structural isolation of transactional records across distinct subscriber stores.
* **Granular Enterprise RBAC:** Robust Role-Based Access Control enforcing secure operational boundaries and separation of duties among Store Owners, Inventory Managers, and Cashiers.
* **Advanced Batch & Expiry Tracking Lifecycle:** Multi-batch profiling capable of mapping distinct procurement lots per product, keeping tight audits on `Cost_Price`, `Quantity`, `Production_Date`, and `Expiry_Date` to prevent decay loss.
* **Reactive Shortage & Lost Sales Logger:** Dedicated internal analytics logging framework that captures instances where consumer requests exceed available stock capacities, providing empirical evidence of unfulfilled market demands.
* **Real-Time Threshold Alerting Engine:** High-performance messaging layer driven by **SignalR Hubs**, broadcasting immediate threshold warning notifications to active store clients when a product hits its `reorder_level` or a batch nears its obsolescence threshold.
* **AI-Driven Forecasting Ingestion:** Optimized ingestion contract mapping to receive daily statistical patterns and automated prediction parameters, dynamically transforming raw data inputs into an actionable, proactive retail "Purchase Plan".

---

## 🛠️ Tech Stack & Tooling

### Core Frameworks
* **Backend Runtime:** .NET 8 (ASP.NET Core Web API)
* **Database & ORM:** Microsoft SQL Server | Entity Framework Core (Code-First Approach)
* **Asynchronous Mediation:** MediatR (In-Process Memory Bus)
* **Real-Time Middleware:** ASP.NET Core SignalR (WebSockets Protocol)

### Security & Architecture Components
* **Authentication:** ASP.NET Core Identity Architecture
* **Token Protocol:** JSON Web Tokens (JWT) with secure, stateless Refresh Token Rotation workflows
* **Validation:** FluentValidation pipeline intercepting incoming Command/Query HTTP payloads

### Utilities & Version Control
* **Version Control:** Git & GitHub for team collaboration and branch merging
* **API Testing & Engineering:** Postman & Swagger OpenAPI documentation

---

## 🏗️ Backend Architecture

The backend strictly adheres to **Clean Architecture** boundaries, enforcing a definitive **Separation of Concerns (SoC)** to ensure that the core domain remains completely isolated from external frameworks, infrastructure components, or database breaking changes.

### Architectural Layers Mapping

| Layer Name | Project Namespace | Primary Responsibilities & Components |
| :--- | :--- | :--- |
| **Presentation** | `Stockyo.WebAPI` | API Controllers, SignalR Hub Configurations, Custom Middleware, App Settings |
| **Infrastructure** | `Stockyo.Infrastructure` | EF Core Tenant DbContext, Migrations, Repository Implementations |
| **Application** | `Stockyo.Application` | Use-Case Services, DTOs, Repository Interfaces, AutoMapper Profiles |
| **Domain** | `Stockyo.Domain` | Enterprise Entities (Stores, Products, Batches), Core Business Rules, Enums |

### Key Design Patterns & Practices Implemented

1. **Result Pattern:** to manage responses away from exceptions in an unified way.
    
2. **Repository & Unit of Work Patterns:** Abstracts data persistence mechanisms away from application use cases, presenting a clean collection-like layer while wrapping multi-entity updates inside safe transactional blocks.
    
3. **Global Tenant Query Filtering:** Configured automatically inside the persistence engine to seamlessly inject tenant boundaries on all operations, eliminating any chance of accidental cross-store data leakage.

---

## 🔒 Authentication & Security Architecture

* **Stateless Claims Exchange:** Identity verification is asserted through secure **JWT** tokens issued upon authentication, carrying implicit store and role claims.
* **Sliding Refresh Token Rotation:** Implements defense-in-depth against replay attacks. Upon access token expiration, the client rotates identity parameters through a one-time-use refresh token that automatically invalidates compromised sessions.
* **Declarative Attribute Authorization:** Crucial management and configuration API endpoints are strictly guarded via metadata attributes (e.g., `[Authorize(Roles = "Owner")]`), insulating infrastructure configurations from general cashier access.

---

## 🔌 API Architecture Overview

All endpoints expose standard RESTful semantics utilizing proper HTTP verbs, consistent status codes, and uniform response envelopes.

| HTTP Method | Endpoint Path | Primary Purpose | Role Authorization |
| :--- | :--- | :--- | :--- |
| **POST** | `/api/auth/register` | Registers a new Merchant Store profile (New Tenant) | Public |
| **POST** | `/api/auth/login` | Validates credentials; returns JWT, Tenant ID & Refresh Token | Public |
| **GET** | `/api/products` | Lists store-specific catalog (Automated tenant filter) | Authenticated |
| **POST** | `/api/batches` | Ingests a new incoming stock lot with production/expiry data | Inventory Manager |
| **POST** | `/api/sales` | Commits sales order (Evaluates batch stock & triggers alerts) | Owner |
| **POST** | `/api/lost-sales` | Records missed consumer demand parameters manually/automatically | Owner |

---

## 📂 Project Folder Structure

* **Stockyo.Domain/**: Core Business Models, Aggregate Roots, Relational Multi-Tenant Domain Entities
* **Stockyo.Application/**: Use-Case Services, DTOs, Repository Interfaces, AutoMapper , Profiles, Validation & Business Use Cases
* **Stockyo.Infrastructure/**: Tenant-Scoped EF Core Context, Migrations, Identity Service Layer, SignalR Hub implementations
* **Stockyo.WebAPI/**: REST Controllers, Middleware pipelines, Routing configurations, Program.cs setup

---

## 💻 Local Execution & Setup

### Prerequisites
* .NET 8.0 SDK
* SQL Server (Express or LocalDB instance)

### Setup Steps

1. **Clone Project Repository:**
   ```bash
   git clone [https://github.com/shahdayman315315/Stockyo.git](https://github.com/shahdayman315315/Stockyo.git)
   cd Stockyo

2. **Configure Connection String:**
Navigate to Stockyo.WebAPI/appsettings.json and adjust the ConnectionStrings:DefaultConnection to target your local SQL Server instance.

3. **Apply Database Migrations:**
Run the following EF Core CLI command to generate tables and map constraints:
  ```bash
  dotnet ef database update --project Stockyo.Infrastructure --startup-project Stockyo.WebAPI
```

4. **Launch Application:**
  ```bash
  dotnet run --project Stockyo.WebAPI
Once executed, open your browser and navigate to https://localhost:7001/swagger to interact with the OpenAPI dashboard.
```

## 📈 Future System Roadmap

* **Distributed Caching Layer**: Integrating **Redis Cache** to optimize high-frequency read operations on product catalogs and store dashboards.
* **Message Broker Decoupling**: Introducing **RabbitMQ** or **Azure Service Bus** to handle asynchronous AI background ingestion and bulk forecasting tasks.
* **Containerization**: Authoring multi-stage **Dockerfiles** to simplify orchestration and microservice preparedness.

---
*Developed by Shahd Ayman – Backend Software Engineer*
