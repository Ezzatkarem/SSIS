# Software Requirements Specification (SRS)
## Smart Student Management System

---

## Document Information

| Field | Value |
|-------|-------|
| **Project Name** | Smart Student Management System |
| **Document Type** | Software Requirements Specification (SRS) |
| **Version** | 1.0 |
| **Date** | March 22, 2026 |
| **Status** | Approved |
| **Architecture** | 3-Tier (Domain, DAL, BLL, API) |
| **Technology Stack** | .NET 10, SQL Server, Entity Framework Core, Identity, JWT |

---

## 📋 Table of Contents

- [1. Introduction](#1-introduction)
- [2. Actors and Roles](#2-actors-and-roles)
- [3. Functional Requirements](#3-functional-requirements)
- [4. Non-Functional Requirements](#4-non-functional-requirements)
- [5. API Endpoints](#5-api-endpoints-initial)
- [6. Database Design](#6-database-design-initial-tables)
- [7. Technology Stack](#7-technology-stack)
- [8. Project Structure](#8-project-structure)
- [9. Success Criteria](#9-success-criteria)

---

## 1. Introduction

### 1.1 Purpose

The Smart Student Management System is a comprehensive web API solution designed to manage academic operations including student records, courses, grades, attendance, fees, and notifications.

### 1.2 Scope

The system will serve three types of users:

- **Admin** — Full system control
- **Doctor** — Course and student management
- **Student** — View personal academic information

### 1.3 Definitions

| Term | Definition |
|------|------------|
| **API** | Application Programming Interface |
| **JWT** | JSON Web Token |
| **GPA** | Grade Point Average |
| **CRUD** | Create, Read, Update, Delete |
| **DTO** | Data Transfer Object |

---

## 2. Actors and Roles

| Actor | Description | Permissions |
|-------|-------------|-------------|
| **Admin** | System Administrator | Full access: user management, course management, fee setup, reports |
| **Doctor** | Faculty Member | Manage assigned courses, enter grades, record attendance |
| **Student** | Learner | View grades, attendance, fees, profile |

---

## 3. Functional Requirements

### 3.1 Module 1: Authentication & User Management

| ID | Requirement | Description |
|----|-------------|-------------|
| FR-01 | User Registration | Admin can create new users (Admin, Doctor, Student) |
| FR-02 | User Login | Users login with email and password, receive JWT token |
| FR-03 | Get All Users | Admin can view paginated list of all users |
| FR-04 | Get User by ID | View specific user details (based on role) |
| FR-05 | Update User | Users can update their own profile |
| FR-06 | Change Password | Users can change their password |
| FR-07 | Delete User | Admin can soft delete users |
| FR-08 | Roles | Admin, Doctor, Student roles with different permissions |

### 3.2 Module 2: Academic Management

| ID | Requirement | Description |
|----|-------------|-------------|
| FR-09 | Create Course | Admin can create courses (name, code, credits) |
| FR-10 | Update Course | Admin can modify course details |
| FR-11 | Delete Course | Admin can delete courses with no enrolled students |
| FR-12 | View Courses | All users can view courses |
| FR-13 | Assign Doctor | Admin can assign doctor to course |
| FR-14 | Enroll Student | Admin can enroll students in courses |
| FR-15 | Unenroll Student | Admin can remove students from courses |
| FR-16 | Create Schedule | Admin can create course schedule (day, time, room) |
| FR-17 | View Schedule | Users can view relevant schedules |

### 3.3 Module 3: Grade Management

| ID | Requirement | Description |
|----|-------------|-------------|
| FR-18 | Enter Grades | Doctors can enter grades for students in their courses |
| FR-19 | Update Grades | Doctors can update grades; Admin can override |
| FR-20 | View Grades | Students view own grades; Doctors view course grades |
| FR-21 | Calculate GPA | System calculates semester and cumulative GPA |
| FR-22 | Grade Reports | View grade statistics per course |

### 3.4 Module 4: Attendance Management

| ID | Requirement | Description |
|----|-------------|-------------|
| FR-23 | Take Attendance | Doctors can record attendance for lectures |
| FR-24 | View Attendance | Students view own attendance; Doctors view course attendance |
| FR-25 | Attendance Percentage | Calculate attendance percentage per course |
| FR-26 | Attendance Reports | View attendance reports with filters |

### 3.5 Module 5: Fee Management

| ID | Requirement | Description |
|----|-------------|-------------|
| FR-27 | Set Semester Fees | Admin can define fees per semester |
| FR-28 | Record Payment | Admin can record student payments |
| FR-29 | View Fee Status | Students view payment status; Admin views all |
| FR-30 | Payment History | View payment history |

### 3.6 Module 6: Notification Management

| ID | Requirement | Description |
|----|-------------|-------------|
| FR-31 | Send Notification | Admin sends to users; Doctors send to their students |
| FR-32 | View Notifications | Users view their notifications |
| FR-33 | Mark as Read | Users can mark notifications as read |

### 3.7 Module 7: Reports & Dashboard

| ID | Requirement | Description |
|----|-------------|-------------|
| FR-34 | Admin Dashboard | Statistics: students count, courses, attendance %, top students |
| FR-35 | Doctor Dashboard | Course statistics, student performance |
| FR-36 | Student Dashboard | GPA, courses, attendance %, unread notifications |

---

## 4. Non-Functional Requirements

| ID | Requirement | Target |
|----|-------------|--------|
| NFR-01 | Response Time | < 200ms for 95% of requests |
| NFR-02 | Security | JWT authentication, role-based authorization |
| NFR-03 | Password Security | BCrypt hashing |
| NFR-04 | API Documentation | Swagger/OpenAPI |
| NFR-05 | Error Handling | Global exception handling |
| NFR-06 | Validation | FluentValidation for all inputs |
| NFR-07 | Scalability | Support 1000 concurrent users |

---

## 5. API Endpoints (Initial)

### 5.1 Authentication

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| `POST` | `/api/v1/auth/login` | User login | All |
| `POST` | `/api/v1/auth/register` | Create new user | Admin |

### 5.2 Users

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| `GET` | `/api/v1/users` | Get all users | Admin |
| `GET` | `/api/v1/users/{id}` | Get user by ID | Role-based |
| `PUT` | `/api/v1/users/{id}` | Update user | Role-based |
| `DELETE` | `/api/v1/users/{id}` | Delete user | Admin |
| `POST` | `/api/v1/users/{id}/change-password` | Change password | Owner |

---

## 6. Database Design (Initial Tables)

### 6.1 Users Table

| Column | Type | Description |
|--------|------|-------------|
| `Id` | Guid | Primary key |
| `FullName` | NVARCHAR(100) | User's full name |
| `Email` | NVARCHAR(200) | Unique email |
| `PasswordHash` | NVARCHAR(255) | Hashed password |
| `Role` | INT | 1=Admin, 2=Doctor, 3=Student |
| `PhoneNumber` | NVARCHAR(20) | Optional |
| `IsActive` | BIT | Account status |

---

## 7. Technology Stack

| Layer | Technology |
|-------|------------|
| **Framework** | .NET 10 |
| **Database** | SQL Server |
| **ORM** | Entity Framework Core |
| **Authentication** | ASP.NET Core Identity + JWT |
| **Validation** | FluentValidation |
| **Mapping** | AutoMapper |
| **API Documentation** | Swagger |

---

## 8. Project Structure

```
SmartStudentSystem/
│
├── SmartStudentSystem.Domain/              # Shared between DAL & BLL
│   ├── Common/
│   │   └── BaseEntity.cs
│   ├── Entities/
│   │   └── User.cs
│   └── Enums/
│       └── UserRole.cs
│
├── SmartStudentSystem.DAL/                 # Data Access Layer
│   ├── Identity/
│   │   ├── ApplicationUser.cs
│   │   └── AppIdentityDbContext.cs
│   ├── Repositories/
│   │   ├── IUserRepository.cs
│   │   └── UserRepository.cs
│   ├── UnitOfWork/
│   │   ├── IUnitOfWork.cs
│   │   └── UnitOfWork.cs
│   ├── Data/
│   │   ├── Configurations/
│   │   │   └── UserConfiguration.cs
│   │   └── Migrations/
│   └── Extensions/
│       └── ServiceExtensions.cs
│
├── SmartStudentSystem.BLL/                 # Business Logic Layer
│   ├── Interfaces/
│   │   ├── IUserService.cs
│   │   └── IJwtService.cs
│   ├── Services/
│   │   ├── UserService.cs
│   │   └── JwtService.cs
│   ├── DTOs/
│   │   ├── Auth/
│   │   │   ├── LoginRequestDto.cs
│   │   │   └── LoginResponseDto.cs
│   │   ├── Users/
│   │   │   └── UserResponseDto.cs
│   │   └── Common/
│   │       └── PaginatedResult.cs
│   ├── Validators/
│   │   └── LoginRequestValidator.cs
│   └── Mappings/
│       └── UserMappingProfile.cs
│
└── SmartStudentSystem.API/                 # Presentation Layer
    ├── Controllers/
    │   └── v1/
    │       ├── AuthController.cs
    │       ├── UsersController.cs
    │       └── DashboardController.cs
    ├── Middleware/
    │   └── GlobalExceptionHandler.cs
    ├── Extensions/
    │   └── SwaggerExtensions.cs
    ├── appsettings.json
    ├── appsettings.Development.json
    └── Program.cs
```

---

## 9. Success Criteria

1. Admin can create users and courses
2. Doctor can enter grades and attendance
3. Student can view grades and attendance
4. JWT authentication works correctly
5. API documented with Swagger
6. Response time under 200ms