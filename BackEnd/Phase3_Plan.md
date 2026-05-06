# Phase 3: Grade Management — Implementation Plan

**Project:** Smart Student Management System (SSIS)  
**Phase:** 3 - Grade Management  
**Duration:** 2–3 days  
**Dependencies:** Phase 1 (Auth) + Phase 2 (Courses, Enrollments)

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

Implement the complete grade management module including:

- **Grades** — Record, update, and view student grades per course enrollment
- **Grade Calculation** — Automatic grade letter based on score
- **Transcript** — View student's academic record (GPA, course grades)
- **Statistics** — Course grade distribution *(optional for Phase 3)*

### New Entities

| Entity | Description |
|--------|-------------|
| **Grade** | Stores student's grade for a specific enrollment (course) |
| **GradeAudit** *(optional)* | Logs changes to grades for tracking |

### Reused Entities

| Entity | Usage |
|--------|-------|
| User (Student) | Grade belongs to a student |
| Course | Grade is associated via Enrollment |
| Enrollment | Link between student and course |
| GradeLetter | Enum (A, B, C, D, F) — already defined in Phase 2 |

---

## Prerequisites

- ✅ JWT Authentication & Role-based authorization
- ✅ `UserService` (Student, Doctor, Admin)
- ✅ `CourseService` and `CourseRepository`
- ✅ `EnrollmentService` and `EnrollmentRepository`
- ✅ UnitOfWork pattern
- ✅ AutoMapper configurations for Courses and Enrollments

---

## Entity Relationship Diagram

```
┌─────────────────┐       ┌─────────────────┐       ┌─────────────────┐
│     Student     │       │   Enrollment    │       │     Grade       │
│     (User)      │       │                 │       │                 │
│                 │       │ PK: Id          │       │ PK: Id          │
│ PK: Id          │───────│ StudentId (FK)  │───────│ EnrollmentId(FK)│
│ FullName        │       │ CourseId (FK)   │       │ Score (decimal) │
└─────────────────┘       │ IsActive        │       │ GradeLetter     │
                          └────────┬────────┘       │ Remarks         │
                                   │                │ EnteredById (FK)│
                          ┌────────┴────────┐       │ EnteredAt       │
                          │     Course      │       │ UpdatedAt       │
                          │                 │       └─────────────────┘
                          │ PK: Id          │
                          │ Name, Code, etc.│
                          └─────────────────┘

┌─────────────────┐       ┌─────────────────┐
│    GradeAudit   │       │     Doctor      │
│  (optional)     │       │     (User)      │
│ PK: Id          │       │                 │
│ GradeId (FK)    │───────│ PK: Id          │
│ OldScore        │       └─────────────────┘
│ NewScore        │
│ ChangedBy       │
│ ChangedAt       │
└─────────────────┘
```

---

## Task Breakdown

### 3.1 Domain Layer (`SSIS.Domain`)

#### 3.1.1 Enums

| Task | File | Path | Content |
|------|------|------|---------|
| T3.1 | `GradeLetter.cs` | `SSIS.Domain/Enum/` | Already exists in Phase 2 (A=1, B=2, C=3, D=4, F=5) — no change needed |

#### 3.1.2 Entities

| Task | File | Path | Properties |
|------|------|------|------------|
| T3.2 | `Grade.cs` | `SSIS.Domain/Entities/` | `Id`, `EnrollmentId`, `Score` (decimal 0–100), `GradeLetter`, `Remarks` (max 200), `EnteredById`, `EnteredAt`, `UpdatedAt?` |
| T3.3 | `GradeAudit.cs` *(optional)* | `SSIS.Domain/Entities/` | `Id`, `GradeId`, `OldScore`, `NewScore`, `ChangedByUserId`, `ChangedAt` |

#### 3.1.3 Update Existing Entities

| Task | File | Updates |
|------|------|---------|
| T3.4 | `Enrollment.cs` | Add navigation: `Grade? Grade` (one-to-one) |
| T3.5 | `User.cs` | Add navigation: `ICollection<Grade> EnteredGrades` |

#### 3.1.4 Repository Interfaces

| Task | File | Path | Methods |
|------|------|------|---------|
| T3.6 | `IGradeRepository.cs` | `SSIS.Domain/Interfaces/` | `GetByIdAsync`, `GetByEnrollmentIdAsync`, `GetByStudentIdAsync`, `GetByCourseIdAsync`, `GetByDoctorIdAsync`, `UpdateAsync`, `AddAsync`, `DeleteAsync`, `GetWithDetailsAsync` |

---

### 3.2 DAL Layer (`SSIS.DAL`)

#### 3.2.1 Database Context

| Task | File | Updates |
|------|------|---------|
| T3.7 | `AppDbContext.cs` | Add `DbSet<Grade>`, `DbSet<GradeAudit>` (if auditing) |

#### 3.2.2 Repository Implementation

| Task | File | Path | Description |
|------|------|------|-------------|
| T3.8 | `GradeRepository.cs` | `SSIS.DAL/Reposatory/` | Implement `IGradeRepository` with eager loading for `Enrollment → Student`, `Course` |

#### 3.2.3 Unit of Work Updates

| Task | File | Updates |
|------|------|---------|
| T3.9 | `IUnitOfWork.cs` | Add: `IGradeRepository Grades` |
| T3.10 | `UnitOfWork.cs` | Implement new property |

#### 3.2.4 Service Extensions

| Task | File | Updates |
|------|------|---------|
| T3.11 | `ServiceExtensions.cs` | Register: `IGradeRepository`, `GradeRepository` |

---

### 3.3 BLL Layer (`SSIS.BLL`)

#### 3.3.1 DTOs — Grade

| Task | File | Path | Properties |
|------|------|------|------------|
| T3.12 | `GradeDto.cs` | `SSIS.BLL/DTOs/Grades/` | `Id`, `EnrollmentId`, `StudentId`, `StudentName`, `CourseId`, `CourseName`, `CourseCode`, `Score`, `GradeLetter`, `Remarks`, `EnteredByDoctorName`, `EnteredAt`, `UpdatedAt` |
| T3.13 | `CreateGradeDto.cs` | `SSIS.BLL/DTOs/Grades/` | `EnrollmentId` (or `StudentId+CourseId`), `Score`, `Remarks` |
| T3.14 | `UpdateGradeDto.cs` | `SSIS.BLL/DTOs/Grades/` | `Score`, `Remarks` |
| T3.15 | `GradeBulkCreateDto.cs` | `SSIS.BLL/DTOs/Grades/` | List of `GradeEntryDto` (`StudentId`, `Score`, `Remarks`) for a course |
| T3.16 | `StudentTranscriptDto.cs` | `SSIS.BLL/DTOs/Grades/` | `StudentId`, `StudentName`, `TotalCredits`, `GPA`, `Courses` (list of `CourseGradeDto`) |
| T3.17 | `CourseGradeStatisticsDto.cs` | `SSIS.BLL/DTOs/Grades/` | `CourseId`, `CourseName`, `AverageScore`, `MinScore`, `MaxScore`, `GradeDistribution` (A/B/C/D/F counts) |

#### 3.3.2 Service Interfaces

| Task | File | Path | Methods |
|------|------|------|---------|
| T3.18 | `IGradeService.cs` | `SSIS.BLL/Interfaces/` | `CreateOrUpdateGradeAsync`, `UpdateGradeAsync`, `DeleteGradeAsync`, `GetGradeByIdAsync`, `GetGradesByStudentAsync`, `GetGradesByCourseAsync`, `GetCourseStatisticsAsync`, `GetStudentTranscriptAsync`, `BulkCreateGradesAsync` |

#### 3.3.3 Service Implementation

| Task | File | Path | Description |
|------|------|------|-------------|
| T3.19 | `GradeService.cs` | `SSIS.BLL/Services/` | Auto-calc `GradeLetter` from score, validate doctor–course assignment, check enrollment, prevent duplicates, handle audit trail |

#### 3.3.4 Helper — Grade Calculator

| Task | File | Path | Method |
|------|------|------|--------|
| T3.20 | `GradeCalculator.cs` | `SSIS.BLL/Helpers/` | `static GradeLetter GetGradeLetter(decimal score)` → A: ≥90, B: 80–89, C: 70–79, D: 60–69, F: <60 |

#### 3.3.5 Validators (FluentValidation)

| Task | File | Path | Validation Rules |
|------|------|------|-----------------|
| T3.21 | `CreateGradeValidator.cs` | `SSIS.BLL/Validators/` | `EnrollmentId`: valid enrollment; Score: 0–100; Remarks: max 200 chars; check if grade already exists |
| T3.22 | `UpdateGradeValidator.cs` | `SSIS.BLL/Validators/` | Score: 0–100; Remarks: max 200 |
| T3.23 | `BulkCreateGradeValidator.cs` | `SSIS.BLL/Validators/` | Validate each entry, check doctor authorization for the course |

#### 3.3.6 Mapping Profiles (AutoMapper)

| Task | File | Path | Mappings |
|------|------|------|----------|
| T3.24 | `GradeMappingProfile.cs` | `SSIS.BLL/Mappings/` | `Grade ↔ GradeDto`; `CreateGradeDto → Grade`; `UpdateGradeDto → Grade` |

---

### 3.4 API Layer (`SSIS.PL`)

#### 3.4.1 Controllers

| Task | File | Path | Endpoints |
|------|------|------|-----------|
| T3.25 | `GradesController.cs` | `SSIS.PL/Controllers/v1/` | See [Grades Endpoints](#grades-endpoints) |
| T3.26 | `TranscriptController.cs` | `SSIS.PL/Controllers/v1/` | Student transcripts and statistics |

---

### 3.5 Database Migration

| Task | Command | Description |
|------|---------|-------------|
| T3.27 | `Add-Migration AddGradeTables` | Create migration for Grade (and GradeAudit if included) |
| T3.28 | `Update-Database` | Apply migration to database |

---

## Authorization Matrix

| Operation | Admin | Doctor | Student | Notes |
|-----------|:-----:|:------:|:-------:|-------|
| Create/Update Grade | ✅ | ✅ (own courses) | ❌ | Doctor can only grade enrolled students in their courses |
| Delete Grade | ✅ | ❌ | ❌ | Admin only |
| View Grade by ID | ✅ | ✅ (own course) | ✅ (own) | |
| View all grades of a student | ✅ | ✅ (if in their course) | ✅ (own) | |
| View all grades of a course | ✅ | ✅ (if they teach it) | ❌ | |
| Bulk upload grades | ✅ | ✅ (own course) | ❌ | |
| View own transcript | ✅ | ✅ *(optional)* | ✅ | Student: own only |
| View course grade statistics | ✅ | ✅ (if they teach it) | ❌ | |

---

## API Endpoints

### Grades Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| `GET` | `/api/v1/grades/{id}` | Get grade by ID | Admin, Doctor (own course), Student (own) |
| `GET` | `/api/v1/grades/student/{studentId}` | Get all grades for a student | Admin, Doctor (if student in their course), Student (own) |
| `GET` | `/api/v1/grades/course/{courseId}` | Get all grades for a course | Admin, Doctor (if they teach it) |
| `POST` | `/api/v1/grades` | Create or update grade (upsert) | Admin, Doctor (own course) |
| `PUT` | `/api/v1/grades/{id}` | Update existing grade | Admin, Doctor (own course) |
| `DELETE` | `/api/v1/grades/{id}` | Delete grade | Admin only |
| `POST` | `/api/v1/grades/bulk` | Bulk create/update grades for a course | Admin, Doctor (own course) |
| `GET` | `/api/v1/grades/course/{courseId}/statistics` | Get grade statistics for a course | Admin, Doctor (own course) |

### Transcript Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| `GET` | `/api/v1/transcript/me` | Get logged-in student's transcript | Student |
| `GET` | `/api/v1/transcript/student/{studentId}` | Get transcript for any student | Admin, Doctor |
| `GET` | `/api/v1/transcript/course/{courseId}/export` | Export course grades as CSV/Excel | Admin, Doctor (own course) |

---

## Database Schema

### Grades Table

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `Id` | UNIQUEIDENTIFIER | PK | Primary key |
| `EnrollmentId` | UNIQUEIDENTIFIER | FK -> Enrollments, NOT NULL, UNIQUE | Each enrollment has at most one grade |
| `Score` | DECIMAL(5,2) | NOT NULL, CHECK (0–100) | Numerical score |
| `GradeLetter` | INT | NOT NULL | 1=A, 2=B, 3=C, 4=D, 5=F |
| `Remarks` | NVARCHAR(200) | NULL | Optional comments |
| `EnteredById` | UNIQUEIDENTIFIER | FK -> Users, NOT NULL | Doctor or Admin who entered |
| `EnteredAt` | DATETIME2 | NOT NULL | When first entered |
| `UpdatedAt` | DATETIME2 | NULL | Last update timestamp |

> **Unique Constraint:** `EnrollmentId` — one grade per enrollment.

### GradeAudit Table *(optional)*

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `Id` | UNIQUEIDENTIFIER | PK | Primary key |
| `GradeId` | UNIQUEIDENTIFIER | FK -> Grades, NOT NULL | Referenced grade |
| `OldScore` | DECIMAL(5,2) | NULL | Previous score |
| `NewScore` | DECIMAL(5,2) | NOT NULL | New score |
| `ChangedByUserId` | UNIQUEIDENTIFIER | FK -> Users, NOT NULL | Who made the change |
| `ChangedAt` | DATETIME2 | NOT NULL | Timestamp of change |

---

## Business Rules & Validations

### Grade Entry Rules

1. **Enrollment Must Exist** — Grade can only be added for an active enrollment.
2. **Unique Grade per Enrollment** — A student can have only one grade per course.
3. **Doctor Authorization** — Only the assigned doctor (or Admin) can enter/update grades.
4. **Score Range** — Score must be between 0 and 100 inclusive.
5. **Auto Grade Letter** — Automatically derived from score: A: 90–100 / B: 80–89 / C: 70–79 / D: 60–69 / F: 0–59.
6. **Remarks Optional** — Max 200 characters.
7. **Update Tracking** — If auditing enabled, any score change logs old/new values.
8. **No Grade After Finalization** *(optional)* — Grades locked after semester deadline.

### Transcript Calculation

- **GPA** = Sum of (Grade points × Credits) / Total Credits
- Grade points: A=4.0, B=3.0, C=2.0, D=1.0, F=0.0
- Only include courses with grades (exclude withdrawals/incomplete)

---

## Checklist Summary

### Domain Layer
- [ ] `Grade` entity
- [ ] `GradeAudit` entity *(optional)*
- [ ] Update `Enrollment` entity with `Grade` navigation
- [ ] Update `User` entity with `EnteredGrades`
- [ ] `IGradeRepository` interface

### DAL Layer
- [ ] Update `AppDbContext` with `DbSet<Grade>`
- [ ] `GradeRepository` implementation
- [ ] Update `IUnitOfWork`
- [ ] Update `UnitOfWork`
- [ ] Update `ServiceExtensions`

### BLL Layer
- [ ] DTOs: `GradeDto`, `CreateGradeDto`, `UpdateGradeDto`, `BulkCreateGradeDto`, `StudentTranscriptDto`, `CourseGradeStatisticsDto`
- [ ] `IGradeService` interface
- [ ] `GradeService` implementation
- [ ] `GradeCalculator` helper
- [ ] Validators: `CreateGradeValidator`, `UpdateGradeValidator`, `BulkCreateGradeValidator`
- [ ] `GradeMappingProfile`

### API Layer
- [ ] `GradesController`
- [ ] `TranscriptController`

### Database
- [ ] `Add-Migration AddGradeTables`
- [ ] `Update-Database`

---

## Next Steps

1. Complete **Phase 2** first — ensures Enrollments exist.
2. Implement in order: **Domain → DAL → BLL → API → Migration**.