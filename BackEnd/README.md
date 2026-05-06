# README.md - Smart Student Management System (SSIS)

```markdown
# 🎓 Smart Student Management System (SSIS)

A comprehensive **Student Management System** built with **.NET 10**, **Clean Architecture**, and **JWT Authentication**.

---

## 🛠 Technology Stack

| Layer | Technology |
|-------|------------|
| Framework | .NET 10 |
| Database | SQL Server |
| ORM | Entity Framework Core 10 |
| Authentication | ASP.NET Core Identity + JWT |
| Payment Gateway | Paymob |
| Email | MailKit + MimeKit |
| Validation | FluentValidation |
| Mapping | AutoMapper |
| API Docs | Swagger / OpenAPI |

---

## 🏗 Architecture

```
API Layer (SSIS.PL) → BLL Layer (SSIS.BLL) → DAL Layer (SSIS.DAL) → Domain Layer (SSIS.Domain)
```

---

## 📁 Project Structure

```
SSIS/
├── SSIS.Domain/          # Entities, Enums, Interfaces
├── SSIS.DAL/             # Identity, Repositories, UnitOfWork, DbContext
├── SSIS.BLL/             # Services, DTOs, Validators, Mappings
└── SSIS.PL/              # Controllers, Middleware, Program.cs
```

---

## ✨ Features

| Phase | Features |
|-------|----------|
| **1** | Authentication, JWT, Roles, Email Confirmation |
| **2** | Courses, Enrollments, Schedules |
| **3** | Grades, GPA Calculation |
| **4** | Attendance Management |
| **5** | Fees, Payments (Paymob Integration) |
| **6** | Notifications (Admin/Doctor Broadcast) |
| **7** | Reports & Dashboards |

---

## 🚀 Quick Start

### 1. Clone & Restore

```bash
git clone https://github.com/Ezzatkarem/SSIS.git
cd SSIS
dotnet restore
```

### 2. Update Database

```bash
dotnet ef database update --context AppIdentityDbContext --project SSIS.DAL --startup-project SSIS.PL
```

### 3. Run

```bash
dotnet run --project SSIS.PL
```

### 4. Open Swagger

```
https://localhost:44394/swagger
```

---

## 🔐 Default Users (SeedData)

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@system.com | Admin@123 |
| Doctor | doctor@system.com | Doctor@123 |
| Student | student@system.com | Student@123 |

---

## 💳 Paymob Test Card

| Field | Value |
|-------|-------|
| Card Number | `4242 4242 4242 4242` |
| Expiry Date | `12/30` (any future date) |
| CVV | `123` |

---

## 📚 Main API Endpoints

| Category | Base URL |
|----------|----------|
| Auth | `/api/Auth` |
| Users | `/api/v1/Users` |
| Courses | `/api/v1/Courses` |
| Grades | `/api/v1/Grades` |
| Attendance | `/api/v1/Attendance` |
| Fees | `/api/v1/Fees` |
| Payments | `/api/v1/Payments` |
| Notifications | `/api/v1/Notifications` |
| Reports | `/api/v1/Reports` |

---

## 🔧 Common Commands

### Add Migration

```bash
dotnet ef migrations add MigrationName --context AppIdentityDbContext --project SSIS.DAL --startup-project SSIS.PL
```

### Update Database

```bash
dotnet ef database update --context AppIdentityDbContext --project SSIS.DAL --startup-project SSIS.PL
```

### Remove Last Migration

```bash
dotnet ef migrations remove --context AppIdentityDbContext --project SSIS.DAL --startup-project SSIS.PL
```

---

## 🐛 Troubleshooting

| Error | Solution |
|-------|----------|
| `Role ADMIN does not exist` | Run app once (SeedData creates roles) |
| `Paymob callback not reaching` | Use `ngrok http https://localhost:44394` |
| `Cannot insert NULL into NationalIdImagePath` | Use `multipart/form-data`, not JSON |

---

## 👨‍💻 Author

**Ezzat Karem**

GitHub: [@Ezzatkarem](https://github.com/Ezzatkarem)

---

**Happy Coding! 🚀**
```