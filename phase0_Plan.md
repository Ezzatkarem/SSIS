Phase 0: Project Setup & Infrastructure - Implementation Plan

Project: Smart Student Management System (SSIS)
Phase: 0 - Setup & Foundation
Duration: 1 day
Dependencies: None

---

📋 Table of Contents

1. Overview
2. Solution Structure
3. Technology Stack
4. Task Breakdown
5. Project Templates & Configurations
6. NuGet Packages
7. Database Setup
8. Git & Version Control
9. Environment Configuration
10. Checklist Summary

---

Overview

Goal

Establish the foundation for the entire SSIS project:

· Create clean architecture solution (Domain, DAL, BLL, PL)
· Configure all necessary NuGet packages
· Set up database (SQL Server) and connection
· Configure Git, .gitignore, and initial commit
· Set up environment variables and appsettings
· Enable Swagger/OpenAPI for API documentation

---

Solution Structure (Clean Architecture)

```
SSIS/
├── SSIS.sln
├── SSIS.Domain/                 (Class Library - .NET 8)
│   ├── Entities/
│   ├── Enum/
│   ├── Interfaces/
│   └── Common/
├── SSIS.DAL/                    (Class Library - .NET 8)
│   ├── Context/
│   ├── Reposatory/              (Repository implementations)
│   ├── Migrations/
│   └── Configurations/
├── SSIS.BLL/                    (Class Library - .NET 8)
│   ├── DTOs/
│   ├── Services/
│   ├── Interfaces/
│   ├── Validators/
│   ├── Mappings/
│   └── Helpers/
├── SSIS.PL/                     (ASP.NET Core Web API - .NET 8)
│   ├── Controllers/
│   ├── Middleware/
│   ├── Attributes/
│   └── Program.cs
└── SSIS.Tests/                  (xUnit Test Project - optional)
    ├── UnitTests/
    └── IntegrationTests/
```

---

Technology Stack

Layer Technology Version
Framework .NET 8.0
ORM Entity Framework Core 8.0.x
Database SQL Server 2019+ / LocalDB
Authentication JWT Bearer 8.0.x
Mapping AutoMapper 13.0.1
Validation FluentValidation 11.9.x
API Documentation Swashbuckle (Swagger) 6.5.x
Logging Serilog (optional) 5.0.x
Testing xUnit, Moq Latest
Version Control Git -
Containerization Docker (optional) 24+

---

Task Breakdown

0.1 Solution & Project Creation

Task Command / Action Description
T0.1 dotnet new sln -n SSIS Create blank solution
T0.2 dotnet new classlib -n SSIS.Domain -f net8.0 Create Domain layer
T0.3 dotnet new classlib -n SSIS.DAL -f net8.0 Create DAL layer
T0.4 dotnet new classlib -n SSIS.BLL -f net8.0 Create BLL layer
T0.5 dotnet new webapi -n SSIS.PL -f net8.0 Create Web API project
T0.6 dotnet new xunit -n SSIS.Tests -f net8.0 Create Test project (optional)
T0.7 Add project references (see Project References) Link layers correctly

0.2 Folder Structure & Cleanup

Task Action Description
T0.8 Delete Class1.cs from each class library Remove default files
T0.9 Create folders inside each project Domain: Entities, Enum, Interfaces, Common; DAL: Context, Reposatory, Migrations, Configurations; BLL: DTOs, Services, Interfaces, Validators, Mappings, Helpers; PL: Controllers, Middleware, Attributes
T0.10 Add README.md to solution root Project documentation

0.3 Install NuGet Packages

Domain Layer (SSIS.Domain)

Package Command
None (pure .NET) -

DAL Layer (SSIS.DAL)

Package Command
Microsoft.EntityFrameworkCore dotnet add package Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer dotnet add package Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools dotnet add package Microsoft.EntityFrameworkCore.Tools
Microsoft.EntityFrameworkCore.Design dotnet add package Microsoft.EntityFrameworkCore.Design

BLL Layer (SSIS.BLL)

Package Command
AutoMapper dotnet add package AutoMapper
AutoMapper.Extensions.Microsoft.DependencyInjection dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
FluentValidation.AspNetCore dotnet add package FluentValidation.AspNetCore
BCrypt.Net-Next dotnet add package BCrypt.Net-Next
System.IdentityModel.Tokens.Jwt dotnet add package System.IdentityModel.Tokens.Jwt
Microsoft.IdentityModel.Tokens dotnet add package Microsoft.IdentityModel.Tokens

PL Layer (SSIS.PL)

Package Command
Microsoft.AspNetCore.Authentication.JwtBearer dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
Microsoft.EntityFrameworkCore.Design dotnet add package Microsoft.EntityFrameworkCore.Design
Swashbuckle.AspNetCore dotnet add package Swashbuckle.AspNetCore
Serilog.AspNetCore (optional) dotnet add package Serilog.AspNetCore

Tests (SSIS.Tests)

Package Command
Microsoft.EntityFrameworkCore.InMemory dotnet add package Microsoft.EntityFrameworkCore.InMemory
Moq dotnet add package Moq
FluentAssertions (optional) dotnet add package FluentAssertions

0.4 Project References

Project References
SSIS.DAL → SSIS.Domain
SSIS.BLL → SSIS.Domain, SSIS.DAL
SSIS.PL → SSIS.BLL, SSIS.DAL, SSIS.Domain
SSIS.Tests → SSIS.PL, SSIS.BLL, SSIS.DAL, SSIS.Domain

Add using CLI:

```bash
dotnet add SSIS.DAL reference SSIS.Domain
dotnet add SSIS.BLL reference SSIS.Domain
dotnet add SSIS.BLL reference SSIS.DAL
dotnet add SSIS.PL reference SSIS.BLL
dotnet add SSIS.PL reference SSIS.DAL
dotnet add SSIS.PL reference SSIS.Domain
dotnet add SSIS.Tests reference SSIS.PL
```

0.5 Database Setup

Task Action Description
T0.11 Install SQL Server (or use LocalDB) Ensure instance is running
T0.12 Create database manually or via code first Name: SSISDb
T0.13 Set connection string in appsettings.json See Configuration

0.6 Configuration Files

appsettings.json (in SSIS.PL)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SSISDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "JWT": {
    "SecretKey": "YOUR_VERY_LONG_SECRET_KEY_HERE_AT_LEAST_32_CHARS",
    "Issuer": "SmartStudentSystem",
    "Audience": "SmartStudentSystemAPI",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

appsettings.Development.json (override for dev)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=SSISDev;Trusted_Connection=True;TrustServerCertificate=true;"
  }
}
```

0.7 Program.cs Base Configuration

Minimal setup with Swagger, controllers, and dependency injection container.

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories, services (will be added in later phases)
// builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

0.8 Git & .gitignore

Task Action
T0.14 Initialize Git: git init
T0.15 Create .gitignore (use gitignore.io for VisualStudio, .NET, Windows, macOS)
T0.16 First commit: git add . && git commit -m "Initial setup: Phase 0"
T0.17 Create remote repository (GitHub/GitLab) and push

0.9 Docker Support (Optional)

Task File Content
T0.18 Dockerfile Multi-stage build for API
T0.19 docker-compose.yml Define API + SQL Server container

---

Project References Diagram

```
┌─────────────┐
│  SSIS.PL    │ (Web API)
└──────┬──────┘
       │ references
┌──────▼──────┐
│  SSIS.BLL   │ (Services, DTOs)
└──────┬──────┘
       │ references
┌──────▼──────┐      ┌─────────────┐
│  SSIS.DAL   │─────►│ SSIS.Domain │
└─────────────┘      └─────────────┘
       │                     ▲
       │ references          │ (no dependency)
       └─────────────────────┘
```

---

Environment Configuration

Required Environment Variables (for production)

· DB_CONNECTION_STRING
· JWT_SECRET_KEY
· JWT_ISSUER
· JWT_AUDIENCE

Development Settings

· Use User Secrets for sensitive data:
    dotnet user-secrets init
    dotnet user-secrets set "JWT:SecretKey" "your-secret"

---

Checklist Summary

Solution & Projects

· Solution created (SSIS.sln)
· Domain, DAL, BLL, PL, Tests projects created
· Project references configured
· Folder structure created in each project

NuGet Packages

· EF Core + SQL Server in DAL
· AutoMapper, FluentValidation, BCrypt, JWT in BLL
· JwtBearer, Swashbuckle in PL
· Testing packages (xUnit, Moq) in Tests

Configuration

· appsettings.json with connection string and JWT settings
· Program.cs with DbContext, Swagger, Controllers
· (Optional) User Secrets configured

Database

· SQL Server instance ready
· Database created (or will be created via migration)

Version Control

· Git initialized
· .gitignore added
· Initial commit done

Optional

· Dockerfile and docker-compose
· Serilog logging configured

---

Next Steps

1. Execute all tasks in order: solution → projects → packages → references → config → git.
2. Verify that dotnet build succeeds without errors.
3. Run the API (dotnet run --project SSIS.PL) and navigate to /swagger to see the default template.
4. Proceed to Phase 1 (Authentication & User Management) once setup is complete.

---

Generated for Smart Student Management System Phase 0 – Project Setup & Infrastructure