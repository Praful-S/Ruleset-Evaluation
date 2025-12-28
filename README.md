# Ruleset-Evaluation
Production Plant Ruleset Evaluation Project

A production plant ruleset evaluation system built with .NET using Clean Architecture principles. This API enables rule-based evaluation and validation for production plant operations.

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Project Structure](#project-structure)
- [Setup Instructions](#setup-instructions)
- [Configuration](#configuration)
- [Running the Application](#running-the-application)
- [API Documentation](#api-documentation)
- [Database](#database)
- [Testing](#testing)

## 🎯 Overview

The Ruleset Evaluation System is designed to evaluate and validate production plant operations against predefined rulesets. Built with Clean Architecture, it ensures separation of concerns, maintainability, and testability.

## 🏗️ Architecture

This project follows **Clean Architecture** principles with clear separation of layers:

- **Core Layer**: Domain entities, business logic, and interfaces
- **Application Layer**: Use cases, DTOs, and application services
- **Infrastructure Layer**: Data access, external services, and Entity Framework implementation
- **API Layer**: RESTful API endpoints and presentation logic

For detailed architecture diagrams, see [`/docs/architecture-diagram.md`](./docs/)

## ✅ Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- [SQL Server 2019](https://www.microsoft.com/sql-server/sql-server-downloads) or later (Express Edition is sufficient)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/downloads)
- [Postman](https://www.postman.com/downloads/) or similar API testing tool (optional)

## 📁 Project Structure

```
Ruleset-Evaluation/
│
├── src/                                    # Source code
│   ├── RulesetEvaluation.Api/             # Web API project
│   ├── RulesetEvaluation.Application/     # Application layer (use cases, DTOs)
│   ├── RulesetEvaluation.Core/            # Core domain layer (entities, interfaces)
│   └── RulesetEvaluation.Infrastructure/  # Infrastructure layer (EF Core, repositories)
│
├── db/                                     # Database files
│   └── ER-Diagram.png                     # Entity-Relationship diagram
│
├── tests/                                  # Test projects
│   └── RulesetEvaluation.Tests/           # Unit and integration tests
│
├── docs/                                   # Documentation
│   ├── architecture-diagram.md            # System architecture documentation
│   └── api-documentation.md               # API endpoint documentation
│
├── README.md                              # This file
└── RulesetEvaluationSystem.slnx           # Solution file
```

## 🚀 Setup Instructions

### 1. Clone the Repository

```bash
git clone https://github.com/Praful-S/Ruleset-Evaluation.git
cd Ruleset-Evaluation
```

### 2. Restore Dependencies

Open a terminal in the project root directory and run:

```bash
dotnet restore
```

### 3. Configure Database Connection

Update the connection string in `src/RulesetEvaluation.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=RulesetEvaluationDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**For SQL Server Authentication:**
```json
"DefaultConnection": "Server=localhost;Database=RulesetEvaluationDB;User Id=your_username;Password=your_password;TrustServerCertificate=True;"
```

### 4. Apply Database Migrations

Navigate to the API project directory and run:

```bash
cd src/RulesetEvaluation.Api
dotnet ef database update
```

**If Entity Framework tools are not installed:**
```bash
dotnet tool install --global dotnet-ef
```
