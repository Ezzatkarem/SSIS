# Phase 5: Fee & Payment Management — Implementation Plan

**Project:** Smart Student Management System (SSIS)  
**Phase:** 5 - Fee & Payment Management  
**Duration:** 2–3 days  
**Dependencies:** Phase 1 (Users) must be completed first

---

## 📋 Table of Contents

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Entity Relationship Diagram](#entity-relationship-diagram)
- [Task Breakdown](#task-breakdown)
- [Authorization Matrix](#authorization-matrix)
- [API Endpoints](#api-endpoints)
- [Database Schema](#database-schema)
- [Checklist Summary](#checklist-summary)

---

## Overview

### Goal

Implement complete fee and payment management including:

- **Fees** — Create and manage student fee invoices
- **Payments** — Record payments (manual + Paymob integration)
- **Payment Gateway** — Integrate with Paymob for online payments
- **Reports** — Fee collection reports and overdue tracking

### New Entities

| Entity | Description |
|--------|-------------|
| **Fee** | Student fee invoice per semester/year |
| **Payment** | Individual payment transactions |

### New Enums

| Enum | Values |
|------|--------|
| **FeeStatus** | Unpaid (1), Partial (2), Paid (3), Overdue (4) |
| **PaymentStatus** | Pending (1), Completed (2), Failed (3), Refunded (4) |

---

## Prerequisites

- ✅ Phase 1 (Users, Authentication) must be functional
- ✅ Admin role must exist

---

## Entity Relationship Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         ENTITY RELATIONSHIPS                            │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────┐       ┌─────────────┐       ┌─────────────┐
│    User     │1      │     Fee     │1      │   Payment   │
│  (Student)  │───────│             │───────│             │
│             │       │ PK: Id      │       │ PK: Id      │
│ PK: Id      │       │ StudentId   │       │ FeeId       │
│ FullName    │       │ TotalAmount │       │ Amount      │
│ Email       │       │ PaidAmount  │       │ PaymentDate │
└─────────────┘       │ DueDate     │       │ Status      │
                      │ Status      │       │ TransactionId│
                      │ Semester    │       └─────────────┘
                      │ AcademicYear│
                      └─────────────┘
```

---

## Task Breakdown

### 5.1 Domain Layer (`SSIS.Domain`)

#### 5.1.1 Enums

| Task | File | Path | Content |
|------|------|------|---------|
| T5.1 | `FeeStatus.cs` | `SSIS.Domain/Enums/` | Unpaid=1, Partial=2, Paid=3, Overdue=4 |
| T5.2 | `PaymentStatus.cs` | `SSIS.Domain/Enums/` | Pending=1, Completed=2, Failed=3, Refunded=4 |

#### 5.1.2 Entities

| Task | File | Path | Properties |
|------|------|------|------------|
| T5.3 | `Fee.cs` | `SSIS.Domain/Entities/` | `Id`, `StudentId`, `Semester`, `AcademicYear`, `TotalAmount`, `PaidAmount`, `DueDate`, `Status`, `CreatedAt`, `UpdatedAt` |
| T5.4 | `Payment.cs` | `SSIS.Domain/Entities/` | `Id`, `FeeId`, `StudentId`, `Amount`, `PaymentDate`, `PaymentMethod`, `TransactionId`, `Status`, `PaymobOrderId`, `ReceiptUrl`, `CreatedAt` |
| T5.5 | Update `User.cs` | `SSIS.Domain/Entities/` | Add navigation: `ICollection<Fee> Fees`, `ICollection<Payment> Payments` |

#### 5.1.3 Repository Interfaces

| Task | File | Path | Methods |
|------|------|------|---------|
| T5.6 | `IFeeRepository.cs` | `SSIS.Domain/Interfaces/` | `GetByStudentIdAsync`, `GetByStatusAsync`, `GetOverdueFeesAsync`, `GetBySemesterAsync` |
| T5.7 | `IPaymentRepository.cs` | `SSIS.Domain/Interfaces/` | `GetByStudentIdAsync`, `GetByFeeIdAsync`, `GetByTransactionIdAsync`, `GetByPaymobOrderIdAsync` |

---

### 5.2 DAL Layer (`SSIS.DAL`)

#### 5.2.1 Database Context

| Task | File | Updates |
|------|------|---------|
| T5.8 | `AppDbContext.cs` | Add `DbSet<Fee>`, `DbSet<Payment>` |

#### 5.2.2 Repository Implementations

| Task | File | Path | Description |
|------|------|------|-------------|
| T5.9 | `FeeRepository.cs` | `SSIS.DAL/Repositories/` | Implement `IFeeRepository` |
| T5.10 | `PaymentRepository.cs` | `SSIS.DAL/Repositories/` | Implement `IPaymentRepository` |

#### 5.2.3 Unit of Work Updates

| Task | File | Updates |
|------|------|---------|
| T5.11 | `IUnitOfWork.cs` | Add: `IFeeRepository Fees`, `IPaymentRepository Payments` |
| T5.12 | `UnitOfWork.cs` | Implement new repository properties |

#### 5.2.4 Service Extensions

| Task | File | Updates |
|------|------|---------|
| T5.13 | `ServiceExtensions.cs` | Register: `IFeeRepository`, `IPaymentRepository`, `IFeeService`, `IPaymentService`, `PaymobClient` |

---

### 5.3 BLL Layer (`SSIS.BLL`)

#### 5.3.1 DTOs — Fee

| Task | File | Path | Properties |
|------|------|------|------------|
| T5.14 | `FeeDto.cs` | `SSIS.BLL/DTOs/Fees/` | `Id`, `StudentId`, `StudentName`, `Semester`, `AcademicYear`, `TotalAmount`, `PaidAmount`, `RemainingAmount`, `DueDate`, `Status` |
| T5.15 | `CreateFeeDto.cs` | `SSIS.BLL/DTOs/Fees/` | `StudentId`, `Semester`, `AcademicYear`, `TotalAmount`, `DueDate` |
| T5.16 | `UpdateFeeDto.cs` | `SSIS.BLL/DTOs/Fees/` | `TotalAmount`, `DueDate`, `Reason` |
| T5.17 | `FeeSettingsDto.cs` | `SSIS.BLL/DTOs/Fees/` | `AcademicYear`, `Semester`, `AmountPerStudent`, `DueDate` |
| T5.18 | `BulkUpdateFeesDto.cs` | `SSIS.BLL/DTOs/Fees/` | `AcademicYear`, `Semester`, `AcademicLevel`, `Department`, `NewTotalAmount`, `NewDueDate`, `Reason` |

#### 5.3.2 DTOs — Payment

| Task | File | Path | Properties |
|------|------|------|------------|
| T5.19 | `PaymentDto.cs` | `SSIS.BLL/DTOs/Payments/` | `Id`, `FeeId`, `StudentId`, `Amount`, `PaymentDate`, `PaymentMethod`, `Status`, `TransactionId`, `ReceiptUrl` |
| T5.20 | `InitiatePaymentDto.cs` | `SSIS.BLL/DTOs/Payments/` | `FeeId`, `Amount`, `ReturnUrl` |
| T5.21 | `ManualPaymentDto.cs` | `SSIS.BLL/DTOs/Payments/` | `FeeId`, `StudentId`, `Amount`, `PaymentMethod`, `ReferenceNumber`, `PaymentDate` |
| T5.22 | `PaymentResponseDto.cs` | `SSIS.BLL/DTOs/Payments/` | `PaymentUrl`, `PaymentId`, `Amount` |
| T5.23 | `PaymobCallbackDto.cs` | `SSIS.BLL/DTOs/Payments/` | `Hmac`, `Data` (`OrderId`, `TransactionId`, `Success`) |

#### 5.3.3 Service Interfaces

| Task | File | Path | Methods |
|------|------|------|---------|
| T5.24 | `IFeeService.cs` | `SSIS.BLL/Interfaces/` | `CreateFeeAsync`, `UpdateFeeAsync`, `DeleteFeeAsync`, `GetFeeByIdAsync`, `GetAllFeesAsync`, `GetFeesByStudentAsync`, `BulkUpdateFeesAsync`, `AutoGenerateFeesAsync` |
| T5.25 | `IPaymentService.cs` | `SSIS.BLL/Interfaces/` | `InitiatePaymentAsync`, `RecordManualPaymentAsync`, `HandlePaymobCallbackAsync`, `GetPaymentByIdAsync`, `GetPaymentsByStudentAsync` |

#### 5.3.4 Service Implementations

| Task | File | Path | Description |
|------|------|------|-------------|
| T5.26 | `FeeService.cs` | `SSIS.BLL/Services/` | Business logic for fees |
| T5.27 | `PaymentService.cs` | `SSIS.BLL/Services/` | Business logic for payments + Paymob integration |

#### 5.3.5 Validators

| Task | File | Path | Validation Rules |
|------|------|------|-----------------|
| T5.28 | `CreateFeeValidator.cs` | `SSIS.BLL/Validators/` | `StudentId` valid; `TotalAmount` > 0; `DueDate` in future |
| T5.29 | `InitiatePaymentValidator.cs` | `SSIS.BLL/Validators/` | `FeeId` valid; `Amount` > 0; `Amount` ≤ Remaining |

#### 5.3.6 Mapping Profiles

| Task | File | Path | Mappings |
|------|------|------|----------|
| T5.30 | `FeeMappingProfile.cs` | `SSIS.BLL/Mappings/` | `Fee ↔ FeeDto`; `CreateFeeDto → Fee` |
| T5.31 | `PaymentMappingProfile.cs` | `SSIS.BLL/Mappings/` | `Payment ↔ PaymentDto`; `ManualPaymentDto → Payment` |

#### 5.3.7 Paymob Integration

| Task | File | Path | Description |
|------|------|------|-------------|
| T5.32 | `appsettings.json` | `SSIS.PL/` | Add section: `Paymob: ApiKey, IntegrationId, HmacSecret, IframeId` |
| T5.33 | `Program.cs` | `SSIS.PL/` | `builder.Services.AddPaymob(config["Paymob:ApiKey"])` |

---

### 5.4 API Layer (`SSIS.PL`)

#### 5.4.1 Controllers

| Task | File | Path | Endpoints |
|------|------|------|-----------|
| T5.34 | `FeesController.cs` | `SSIS.PL/Controllers/v1/` | See [Fees Endpoints](#fees-endpoints) |
| T5.35 | `PaymentsController.cs` | `SSIS.PL/Controllers/v1/` | See [Payments Endpoints](#payments-endpoints) |

---

### 5.5 Database Migration

| Task | Command | Description |
|------|---------|-------------|
| T5.36 | `Add-Migration AddFeeAndPaymentTables` | Create migration for Fee, Payment |
| T5.37 | `Update-Database` | Apply migration |

---

## Authorization Matrix

| Operation | Admin | Doctor | Student | Notes |
|-----------|:-----:|:------:|:-------:|-------|
| **Fees** | | | | |
| Create Fee | ✅ | ❌ | ❌ | |
| Update Fee | ✅ | ❌ | ❌ | |
| Delete Fee | ✅ | ❌ | ❌ | Only if no payments |
| View All Fees | ✅ | ❌ | ❌ | |
| View Student Fees | ✅ | ❌ | ✅ (own) | |
| Bulk Update Fees | ✅ | ❌ | ❌ | |
| Auto Generate Fees | ✅ | ❌ | ❌ | Background job |
| **Payments** | | | | |
| Initiate Payment | ❌ | ❌ | ✅ | Student only |
| Manual Payment | ✅ | ❌ | ❌ | Treasurer/Admin |
| View Payment | ✅ | ❌ | ✅ (own) | |

---

## API Endpoints

### Fees Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| `GET` | `/api/v1/fees` | Get all fees | Admin |
| `GET` | `/api/v1/fees/{id}` | Get fee by ID | Admin, Student (own) |
| `GET` | `/api/v1/fees/student/{studentId}` | Get student's fees | Admin, Student (own) |
| `GET` | `/api/v1/fees/my-fees` | Get current student's fees | Student |
| `POST` | `/api/v1/fees` | Create fee | Admin |
| `PUT` | `/api/v1/fees/{id}` | Update fee | Admin |
| `DELETE` | `/api/v1/fees/{id}` | Delete fee | Admin |
| `PUT` | `/api/v1/fees/bulk` | Bulk update fees | Admin |
| `POST` | `/api/v1/fees/auto-generate` | Auto generate fees | Admin |

### Payments Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| `POST` | `/api/v1/payments/initiate` | Initiate Paymob payment | Student |
| `POST` | `/api/v1/payments/manual` | Record manual payment | Admin |
| `POST` | `/api/v1/payments/callback` | Paymob webhook | Public |
| `GET` | `/api/v1/payments/{id}` | Get payment by ID | Admin, Student (own) |
| `GET` | `/api/v1/payments/student/{studentId}` | Get student's payments | Admin, Student (own) |
| `GET` | `/api/v1/payments/my-payments` | Get current student's payments | Student |

---

## Database Schema

### Fees Table

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `Id` | UNIQUEIDENTIFIER | PK | Primary key |
| `StudentId` | UNIQUEIDENTIFIER | FK -> Users, NOT NULL | Student reference |
| `Semester` | INT | NOT NULL | Semester number (1, 2) |
| `AcademicYear` | INT | NOT NULL | Year (e.g., 2025) |
| `TotalAmount` | DECIMAL(18,2) | NOT NULL | Total fee amount |
| `PaidAmount` | DECIMAL(18,2) | NOT NULL, DEFAULT 0 | Amount paid |
| `DueDate` | DATETIME2 | NOT NULL | Due date |
| `Status` | INT | NOT NULL | 1=Unpaid, 2=Partial, 3=Paid, 4=Overdue |
| `CreatedAt` | DATETIME2 | NOT NULL | Creation timestamp |
| `UpdatedAt` | DATETIME2 | NULL | Last update |

### Payments Table

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `Id` | UNIQUEIDENTIFIER | PK | Primary key |
| `FeeId` | UNIQUEIDENTIFIER | FK -> Fees, NOT NULL | Fee reference |
| `StudentId` | UNIQUEIDENTIFIER | FK -> Users, NOT NULL | Student reference |
| `Amount` | DECIMAL(18,2) | NOT NULL | Payment amount |
| `PaymentDate` | DATETIME2 | NOT NULL | Payment date |
| `PaymentMethod` | NVARCHAR(50) | NOT NULL | Paymob, Cash, BankTransfer |
| `TransactionId` | NVARCHAR(100) | NULL | Gateway transaction ID |
| `Status` | INT | NOT NULL | 1=Pending, 2=Completed, 3=Failed, 4=Refunded |
| `PaymobOrderId` | NVARCHAR(100) | NULL | Paymob order ID |
| `ReceiptUrl` | NVARCHAR(500) | NULL | Receipt URL |
| `CreatedAt` | DATETIME2 | NOT NULL | Creation timestamp |

---

## Checklist Summary

### Domain Layer
- [ ] `FeeStatus` enum
- [ ] `PaymentStatus` enum
- [ ] `Fee` entity
- [ ] `Payment` entity
- [ ] Update `User` with `Fees`, `Payments` navigation
- [ ] `IFeeRepository` interface
- [ ] `IPaymentRepository` interface

### DAL Layer
- [ ] Update `AppDbContext` with `Fees`, `Payments` DbSets
- [ ] `FeeRepository` implementation
- [ ] `PaymentRepository` implementation
- [ ] Update `IUnitOfWork`
- [ ] Update `UnitOfWork`
- [ ] Update `ServiceExtensions`

### BLL Layer
- [ ] Fee DTOs: `FeeDto`, `CreateFeeDto`, `UpdateFeeDto`, `FeeSettingsDto`, `BulkUpdateFeesDto`
- [ ] Payment DTOs: `PaymentDto`, `InitiatePaymentDto`, `ManualPaymentDto`, `PaymentResponseDto`, `PaymobCallbackDto`
- [ ] `IFeeService` interface
- [ ] `IPaymentService` interface
- [ ] `FeeService` implementation
- [ ] `PaymentService` implementation
- [ ] Validators: `CreateFeeValidator`, `InitiatePaymentValidator`
- [ ] Mapping Profiles: `FeeMappingProfile`, `PaymentMappingProfile`
- [ ] Paymob integration setup

### API Layer
- [ ] `FeesController`
- [ ] `PaymentsController`

### Database
- [ ] `Add-Migration AddFeeAndPaymentTables`
- [ ] `Update-Database`