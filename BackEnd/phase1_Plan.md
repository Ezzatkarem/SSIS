# Phase 1: Authentication & User Management — Implementation Plan

**Project:** Smart Student Management System (SSIS)  
**Phase:** 1 - Authentication & User Management  
**Duration:** 3–4 days  
**Dependencies:** None (foundation phase)

---

## 📋 Table of Contents

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Entity Relationship Diagram](#entity-relationship-diagram)
- [Task Breakdown](#task-breakdown)
- [Authorization Matrix](#authorization-matrix)
- [API Endpoints](#api-endpoints)
- [Database Schema](#database-schema)
- [Business Rules & Validations](#business-rules--validations)
- [Checklist Summary](#checklist-summary)

---

## Overview

### Goal

Implement the complete authentication and user management module including:

- **User Registration & Onboarding** — Create users (Admin, Doctor, Student) with different roles
- **Authentication** — Login with JWT tokens (access + refresh)
- **User Management** — CRUD operations, profile management, role assignment
- **Password Security** — Hashing with BCrypt or ASP.NET Core Identity
- **Authorization** — Role-based access control (Admin, Doctor, Student)

### New Entities

| Entity | Description |
|--------|-------------|
| **User** | Base user with common properties (Id, Email, FullName, Role, etc.) |
| **RefreshToken** | Stores refresh tokens for JWT renewal |

### New Enums

| Enum | Values |
|------|--------|
| **UserRole** | Admin (1), Doctor (2), Student (3) |
| **Gender** *(optional)* | Male (1), Female (2) |

---

## Prerequisites

Before starting Phase 1, ensure the following tools are installed:

- ✅ .NET 8 SDK
- ✅ SQL Server (or SQL Server Express)
- ✅ Visual Studio 2022 / VS Code / Rider
- ✅ Postman or Swagger for testing

---

## Entity Relationship Diagram

```
┌─────────────────────────────────────────┐
│                 User                     │
├─────────────────────────────────────────┤
│ Id (PK, Guid)                           │
│ Email (unique)                          │
│ PasswordHash                            │
│ FullName                                │
│ Role (Admin, Doctor, Student)           │
│ Gender (optional)                       │
│ DateOfBirth (optional)                  │
│ PhoneNumber (optional)                  │
│ IsActive                                │
│ CreatedAt                               │
│ UpdatedAt                               │
│ LastLoginAt                             │
└─────────────────────────────────────────┘
                    │
                    │ 1
                    │
                    ▼
┌─────────────────────────────────────────┐
│            RefreshToken                  │
├─────────────────────────────────────────┤
│ Id (PK, Guid)                           │
│ Token (string, unique)                  │
│ UserId (FK)                             │
│ ExpiresAt                               │
│ CreatedAt                               │
│ RevokedAt (nullable)                    │
│ ReplacedByToken (nullable)              │
└─────────────────────────────────────────┘
```

---

## Task Breakdown

### 1.1 Domain Layer (`SSIS.Domain`)

#### 1.1.1 Enums

| Task | File | Path | Content |
|------|------|------|---------|
| T1.1 | `UserRole.cs` | `SSIS.Domain/Enum/` | Admin=1, Doctor=2, Student=3 |
| T1.2 | `Gender.cs` *(optional)* | `SSIS.Domain/Enum/` | Male=1, Female=2 |

#### 1.1.2 Entities

| Task | File | Path | Properties |
|------|------|------|------------|
| T1.3 | `User.cs` | `SSIS.Domain/Entities/` | `Id`, `Email`, `PasswordHash`, `FullName`, `Role`, `Gender?`, `DateOfBirth?`, `PhoneNumber`, `IsActive`, `CreatedAt`, `UpdatedAt?`, `LastLoginAt?` |
| T1.4 | `RefreshToken.cs` | `SSIS.Domain/Entities/` | `Id`, `Token`, `UserId`, `ExpiresAt`, `CreatedAt`, `RevokedAt?`, `ReplacedByToken` |

#### 1.1.3 Repository Interfaces

| Task | File | Path | Methods |
|------|------|------|---------|
| T1.5 | `IUserRepository.cs` | `SSIS.Domain/Interfaces/` | `GetByIdAsync`, `GetByEmailAsync`, `GetAllAsync` (paginated), `GetByRoleAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` (soft), `ExistsByEmailAsync` |
| T1.6 | `IRefreshTokenRepository.cs` | `SSIS.Domain/Interfaces/` | `GetByTokenAsync`, `AddAsync`, `UpdateAsync`, `RevokeTokenAsync`, `RemoveExpiredTokensAsync` |

---

### 1.2 DAL Layer (`SSIS.DAL`)

#### 1.2.1 Database Context

| Task | File | Updates |
|------|------|---------|
| T1.7 | `AppDbContext.cs` | Add `DbSet<User>`, `DbSet<RefreshToken>`; configure unique index on Email |

#### 1.2.2 Repository Implementations

| Task | File | Path | Description |
|------|------|------|-------------|
| T1.8 | `UserRepository.cs` | `SSIS.DAL/Reposatory/` | Implement `IUserRepository` with soft delete (`IsActive` flag) |
| T1.9 | `RefreshTokenRepository.cs` | `SSIS.DAL/Reposatory/` | Implement `IRefreshTokenRepository` |

#### 1.2.3 Unit of Work

| Task | File | Path | Description |
|------|------|------|-------------|
| T1.10 | `IUnitOfWork.cs` | `SSIS.DAL/Interfaces/` | `SaveChangesAsync`, `Users`, `RefreshTokens` |
| T1.11 | `UnitOfWork.cs` | `SSIS.DAL/Implementations/` | Implement UnitOfWork with repository instances |

#### 1.2.4 Service Extensions

| Task | File | Updates |
|------|------|---------|
| T1.12 | `ServiceExtensions.cs` | Register repositories, UnitOfWork, and DbContext |

---

### 1.3 BLL Layer (`SSIS.BLL`)

#### 1.3.1 DTOs — Authentication

| Task | File | Path | Properties |
|------|------|------|------------|
| T1.13 | `LoginDto.cs` | `SSIS.BLL/DTOs/Auth/` | `Email`, `Password` |
| T1.14 | `RegisterDto.cs` | `SSIS.BLL/DTOs/Auth/` | `Email`, `Password`, `FullName`, `Role?`, `Gender?`, `DateOfBirth?`, `PhoneNumber?` |
| T1.15 | `TokenDto.cs` | `SSIS.BLL/DTOs/Auth/` | `AccessToken`, `RefreshToken`, `ExpiresIn` |
| T1.16 | `RefreshTokenRequestDto.cs` | `SSIS.BLL/DTOs/Auth/` | `RefreshToken` |

#### 1.3.2 DTOs — User Management

| Task | File | Path | Properties |
|------|------|------|------------|
| T1.17 | `UserDto.cs` | `SSIS.BLL/DTOs/Users/` | `Id`, `Email`, `FullName`, `Role`, `Gender`, `DateOfBirth`, `PhoneNumber`, `IsActive`, `CreatedAt`, `LastLoginAt` |
| T1.18 | `CreateUserDto.cs` | `SSIS.BLL/DTOs/Users/` | `Email`, `Password`, `FullName`, `Role`, `Gender?`, `DateOfBirth?`, `PhoneNumber?` |
| T1.19 | `UpdateUserDto.cs` | `SSIS.BLL/DTOs/Users/` | `FullName`, `PhoneNumber`, `Gender`, `DateOfBirth`, `IsActive` |
| T1.20 | `ChangePasswordDto.cs` | `SSIS.BLL/DTOs/Users/` | `CurrentPassword`, `NewPassword` |
| T1.21 | `ResetPasswordDto.cs` | `SSIS.BLL/DTOs/Users/` | `Email`, `NewPassword` (Admin-only or with token) |

#### 1.3.3 Service Interfaces

| Task | File | Path | Methods |
|------|------|------|---------|
| T1.22 | `IAuthService.cs` | `SSIS.BLL/Interfaces/` | `LoginAsync`, `RegisterAsync`, `RefreshTokenAsync`, `LogoutAsync` |
| T1.23 | `IUserService.cs` | `SSIS.BLL/Interfaces/` | `GetByIdAsync`, `GetAllAsync`, `CreateUserAsync`, `UpdateUserAsync`, `DeleteUserAsync`, `ChangePasswordAsync`, `ResetPasswordAsync` |
| T1.24 | `ICurrentUserService.cs` | `SSIS.BLL/Interfaces/` | `GetCurrentUserId()`, `GetCurrentUserRole()`, `IsAuthenticated()` |

#### 1.3.4 Service Implementations

| Task | File | Path | Description |
|------|------|------|-------------|
| T1.25 | `AuthService.cs` | `SSIS.BLL/Services/` | JWT generation, password hashing (BCrypt), refresh token logic |
| T1.26 | `UserService.cs` | `SSIS.BLL/Services/` | Business logic for user management, role validation |
| T1.27 | `CurrentUserService.cs` | `SSIS.BLL/Services/` | Access `HttpContext` to get current user claims |

#### 1.3.5 Helper Classes

| Task | File | Path | Methods |
|------|------|------|---------|
| T1.28 | `JwtHelper.cs` | `SSIS.BLL/Helpers/` | `GenerateAccessToken(User)`, `GenerateRefreshToken()`, `ValidateToken()` |
| T1.29 | `PasswordHasher.cs` | `SSIS.BLL/Helpers/` | `HashPassword(string)`, `VerifyPassword(string hash, string password)` |

#### 1.3.6 Validators (FluentValidation)

| Task | File | Path | Validation Rules |
|------|------|------|-----------------|
| T1.30 | `LoginValidator.cs` | `SSIS.BLL/Validators/` | Email: not empty, valid format; Password: not empty |
| T1.31 | `RegisterValidator.cs` | `SSIS.BLL/Validators/` | Email: unique, valid; Password: min 6 chars, letter+number; FullName: 3–100 chars; Role: valid if provided |
| T1.32 | `CreateUserValidator.cs` | `SSIS.BLL/Validators/` | Similar to Register but role required |
| T1.33 | `UpdateUserValidator.cs` | `SSIS.BLL/Validators/` | FullName: 3–100 chars; PhoneNumber: optional, valid format |

#### 1.3.7 Mapping Profiles (AutoMapper)

| Task | File | Path | Mappings |
|------|------|------|----------|
| T1.34 | `UserMappingProfile.cs` | `SSIS.BLL/Mappings/` | `User ↔ UserDto`; `RegisterDto → User`; `CreateUserDto → User`; `UpdateUserDto → User` (partial) |

---

### 1.4 API Layer (`SSIS.PL`)

#### 1.4.1 Controllers

| Task | File | Path | Endpoints |
|------|------|------|-----------|
| T1.35 | `AuthController.cs` | `SSIS.PL/Controllers/v1/` | `POST /login`, `POST /register`, `POST /refresh-token`, `POST /logout` |
| T1.36 | `UsersController.cs` | `SSIS.PL/Controllers/v1/` | `GET /users`, `GET /users/{id}`, `POST /users`, `PUT /users/{id}`, `DELETE /users/{id}`, `POST /users/{id}/change-password`, `POST /users/{id}/reset-password` |
| T1.37 | `ProfileController.cs` | `SSIS.PL/Controllers/v1/` | `GET /profile`, `PUT /profile`, `POST /profile/change-password` |

#### 1.4.2 Middleware & Configuration

| Task | File | Path | Description |
|------|------|------|-------------|
| T1.38 | `JwtMiddleware.cs` *(optional)* | `SSIS.PL/Middleware/` | Or use built-in `app.UseAuthentication()` + `AddAuthentication` |
| T1.39 | `Program.cs` | Root | Add JWT Bearer auth, Swagger with JWT support, authorization policies |

#### 1.4.3 Custom Attributes

| Task | File | Path | Description |
|------|------|------|-------------|
| T1.40 | `AuthorizeRoleAttribute.cs` | `SSIS.PL/Attributes/` | Custom authorization filter to check roles |

---

### 1.5 Database Migration

| Task | Command | Description |
|------|---------|-------------|
| T1.41 | `Add-Migration InitIdentity` | Create initial migration for Users and RefreshTokens |
| T1.42 | `Update-Database` | Apply migration to database |
| T1.43 | `SeedData.cs` *(optional)* | Seed default Admin user (`admin@ssis.com` / `Admin@123`) |

---

## Authorization Matrix

| Operation | Admin | Doctor | Student | Notes |
|-----------|:-----:|:------:|:-------:|-------|
| **Authentication** | | | | |
| Login | ✅ | ✅ | ✅ | Everyone can log in |
| Register (self) | ❌ | ❌ | ✅ | Usually admin creates users |
| Refresh Token | ✅ | ✅ | ✅ | All authenticated |
| Logout | ✅ | ✅ | ✅ | All authenticated |
| **User Management** | | | | |
| Get all users | ✅ | ❌ | ❌ | Admin only |
| Get user by ID | ✅ | ✅ | ✅ (own) | |
| Create user | ✅ | ❌ | ❌ | Admin only |
| Update user | ✅ | ✅ (own) | ✅ (own) | Admin can update anyone |
| Delete user (soft) | ✅ | ❌ | ❌ | Admin only |
| Change own password | ✅ | ✅ | ✅ | All authenticated |
| Reset password | ✅ | ❌ | ❌ | Admin only |

---

## API Endpoints

### Auth Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| `POST` | `/api/v1/auth/login` | Login, returns tokens | Public |
| `POST` | `/api/v1/auth/register` | Register new user | Admin only (or Public) |
| `POST` | `/api/v1/auth/refresh-token` | Get new access token | Public (with valid refresh token) |
| `POST` | `/api/v1/auth/logout` | Revoke refresh token | Authenticated |

### Users Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| `GET` | `/api/v1/users` | Get all users (paginated, filter by role) | Admin |
| `GET` | `/api/v1/users/{id}` | Get user by ID | Admin, Doctor, Student (own) |
| `POST` | `/api/v1/users` | Create new user | Admin |
| `PUT` | `/api/v1/users/{id}` | Update user | Admin (any) or User (own, limited) |
| `DELETE` | `/api/v1/users/{id}` | Soft delete user | Admin |
| `POST` | `/api/v1/users/{id}/change-password` | Change password | User (own) or Admin |
| `POST` | `/api/v1/users/{id}/reset-password` | Admin reset password | Admin |

### Profile Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| `GET` | `/api/v1/profile` | Get current user profile | Authenticated |
| `PUT` | `/api/v1/profile` | Update own profile | Authenticated |
| `POST` | `/api/v1/profile/change-password` | Change own password | Authenticated |

---

## Database Schema

### Users Table

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `Id` | UNIQUEIDENTIFIER | PK | Primary key |
| `Email` | NVARCHAR(256) | NOT NULL, UNIQUE | User email (login) |
| `PasswordHash` | NVARCHAR(255) | NOT NULL | Hashed password (BCrypt) |
| `FullName` | NVARCHAR(100) | NOT NULL | Full name |
| `Role` | INT | NOT NULL | 1=Admin, 2=Doctor, 3=Student |
| `Gender` | INT | NULL | 1=Male, 2=Female |
| `DateOfBirth` | DATE | NULL | Birth date |
| `PhoneNumber` | NVARCHAR(20) | NULL | Contact number |
| `IsActive` | BIT | NOT NULL, DEFAULT 1 | Soft delete flag |
| `CreatedAt` | DATETIME2 | NOT NULL | Creation timestamp |
| `UpdatedAt` | DATETIME2 | NULL | Last update |
| `LastLoginAt` | DATETIME2 | NULL | Last login timestamp |

### RefreshTokens Table

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `Id` | UNIQUEIDENTIFIER | PK | Primary key |
| `Token` | NVARCHAR(500) | NOT NULL, UNIQUE | Refresh token string |
| `UserId` | UNIQUEIDENTIFIER | FK -> Users, NOT NULL | User owning token |
| `ExpiresAt` | DATETIME2 | NOT NULL | Expiration time (7 days) |
| `CreatedAt` | DATETIME2 | NOT NULL | Creation time |
| `RevokedAt` | DATETIME2 | NULL | If revoked, not usable |
| `ReplacedByToken` | NVARCHAR(500) | NULL | Token that replaced this one |

---

## Business Rules & Validations

### User Registration & Management

1. **Email Uniqueness** — Email must be unique across all users.
2. **Password Strength** — Minimum 6 characters, at least one letter and one number.
3. **Role Validation** — Only Admin can create users with Admin role.
4. **Soft Delete** — Deleting a user sets `IsActive = false`; email remains reserved.
5. **Email Format** — Valid email format using `EmailAddressAttribute`.

### Authentication Rules

1. **JWT Expiry** — Access token expires in 15 minutes (configurable).
2. **Refresh Token Expiry** — Refresh token expires in 7 days.
3. **Token Rotation** — On refresh, old token is revoked and a new one is issued.
4. **Single Use** — Refresh token can be used only once.
5. **Revocation** — Logout revokes the refresh token.

### Authorization Rules

- **Admin** — Full access to all user management endpoints.
- **Doctor** — Can view list of students (Phase 2) and own profile.
- **Student** — Can view/edit own profile only.

---

## Checklist Summary

### Domain Layer
- [ ] `UserRole` enum
- [ ] `Gender` enum *(optional)*
- [ ] `User` entity
- [ ] `RefreshToken` entity
- [ ] `IUserRepository` interface
- [ ] `IRefreshTokenRepository` interface

### DAL Layer
- [ ] `AppDbContext` with `DbSet<User>`, `DbSet<RefreshToken>`
- [ ] `UserRepository` implementation
- [ ] `RefreshTokenRepository` implementation
- [ ] `IUnitOfWork` and `UnitOfWork`
- [ ] `ServiceExtensions` registration

### BLL Layer
- [ ] DTOs: `LoginDto`, `RegisterDto`, `TokenDto`, `UserDto`, `CreateUserDto`, `UpdateUserDto`, `ChangePasswordDto`, `ResetPasswordDto`
- [ ] `IAuthService`, `IUserService`, `ICurrentUserService`
- [ ] `AuthService`, `UserService`, `CurrentUserService`
- [ ] `JwtHelper`, `PasswordHasher`
- [ ] Validators: `LoginValidator`, `RegisterValidator`, `CreateUserValidator`, `UpdateUserValidator`
- [ ] `UserMappingProfile`

### API Layer
- [ ] `AuthController`
- [ ] `UsersController`
- [ ] `ProfileController`
- [ ] JWT authentication configuration in `Program.cs`
- [ ] Swagger JWT support

### Database
- [ ] `Add-Migration InitIdentity`
- [ ] `Update-Database`
- [ ] Seed admin user

---

## Next Steps

1. Start with **Domain Layer** (entities, enums, repository interfaces).
2. Implement **DAL** (DbContext, repositories, UnitOfWork).
3. Implement **BLL** (services, helpers, validators, AutoMapper).
4. Implement **API** (controllers, JWT config, Swagger).
5. Run **Migrations** and seed initial admin.
6. **Test** using Swagger or Postman.