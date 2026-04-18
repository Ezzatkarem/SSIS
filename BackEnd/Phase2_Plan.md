# Phase 2: Academic Management - Implementation Plan

> **Project**: Smart Student Management System (SSIS)  
> **Phase**: 2 - Academic Management  
> **Duration**: 3-4 days  
> **Dependencies**: Phase 1 (Authentication & User Management) must be completed first

---

## 📋 Table of Contents

1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Entity Relationship Diagram](#entity-relationship-diagram)
4. [Task Breakdown](#task-breakdown)
5. [Authorization Matrix](#authorization-matrix)
6. [API Endpoints](#api-endpoints)
7. [Database Schema](#database-schema)

---

## Overview

### Goal
Implement the complete academic management module including:
- **Courses**: Create, manage, and assign doctors to courses
- **Enrollments**: Enroll/unenroll students in courses
- **Schedules**: Manage course schedules (day, time, room)

### New Entities

| Entity | Description |
|--------|-------------|
| **Course** | Academic courses with credits, semester, and doctor assignment |
| **Enrollment** | Junction table linking students to courses |
| **Schedule** | Course schedule with day, start/end time, and room |

### New Enums

| Enum | Values |
|------|--------|
| **DayOfWeekEnum** | Sunday (1), Monday (2), Tuesday (3), Wednesday (4), Thursday (5) |
| **GradeLetter** | A (1), B (2), C (3), D (4), F (5) |

---

## Prerequisites

Before starting Phase 2, the following Phase 1 components must be functional:

- ✅ JWT Authentication middleware
- ✅ UserService (for validating StudentId and DoctorId)
- ✅ Role-based authorization (Admin, Doctor, Student)
- ✅ CurrentUser service (to get logged-in user info)

---

## Entity Relationship Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         ENTITY RELATIONSHIPS                            │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────┐       ┌─────────────┐       ┌─────────────┐
│    User     │1      │   Course    │1      │  Enrollment │
│  (Doctor)   │───────│<───────────>│───────│  (Student)  │
│             │       │             │       │             │
│ PK: Id      │       │ PK: Id      │       │ PK: Id      │
│ FullName    │       │ Name        │       │ StudentId   │
│ Role        │       │ Code        │       │ CourseId    │
└─────────────┘       │ Credits     │       │ Enrollment  │
                      │ DoctorId FK │       │    Date     │
                      └──────┬──────┘       └─────────────┘
                             │
                      ┌──────┴──────┐
                      │   Schedule  │
                      │             │
                      │ PK: Id      │
                      │ CourseId FK │
                      │ DayOfWeek   │
                      │ StartTime   │
                      │ EndTime     │
                      │ Room        │
                      └─────────────┘
```

---

## Task Breakdown

### 2.1 Domain Layer (SSIS.Domain)

#### 2.1.1 Enums

| Task | File | Path | Content |
|------|------|------|---------|
| T2.1 | `DayOfWeekEnum.cs` | `SSIS.Domain/Enum/` | Sunday=1, Monday=2, Tuesday=3, Wednesday=4, Thursday=5 |
| T2.2 | `GradeLetter.cs` | `SSIS.Domain/Enum/` | A=1, B=2, C=3, D=4, F=5 |

#### 2.1.2 Entities

| Task | File | Path | Properties |
|------|------|------|------------|
| T2.3 | `Course.cs` | `SSIS.Domain/Entities/` | Id, Name, Code, Credits, Description, DoctorId, Semester, AcademicYear, IsActive, CreatedAt, UpdatedAt |
| T2.4 | `Enrollment.cs` | `SSIS.Domain/Entities/` | Id, StudentId, CourseId, EnrollmentDate, IsActive, CreatedAt |
| T2.5 | `Schedule.cs` | `SSIS.Domain/Entities/` | Id, CourseId, DayOfWeek, StartTime, EndTime, Room, CreatedAt |

#### 2.1.3 Update Existing Entities

| Task | File | Updates |
|------|------|---------|
| T2.6 | `User.cs` | Add navigation: `ICollection<Course> TaughtCourses`, `ICollection<Enrollment> Enrollments` |

#### 2.1.4 Repository Interfaces

| Task | File | Path | Methods |
|------|------|------|---------|
| T2.7 | `ICourseRepository.cs` | `SSIS.Domain/Interfaces/` | GetByDoctorAsync, GetByCodeAsync, GetActiveCoursesAsync, GetBySemesterAsync |
| T2.8 | `IEnrollmentRepository.cs` | `SSIS.Domain/Interfaces/` | GetByStudentAsync, GetByCourseAsync, GetActiveEnrollmentsAsync, ExistsAsync, GetByStudentAndCourseAsync |

---

### 2.2 DAL Layer (SSIS.DAL)

#### 2.2.1 Database Context

| Task | File | Updates |
|------|------|---------|
| T2.9 | `AppDbContext.cs` | Add DbSet<Course>, DbSet<Enrollment>, DbSet<Schedule> |

#### 2.2.2 Repository Implementations

| Task | File | Path | Description |
|------|------|------|-------------|
| T2.10 | `CourseRepository.cs` | `SSIS.DAL/Reposatory/` | Implement ICourseRepository |
| T2.11 | `EnrollmentRepository.cs` | `SSIS.DAL/Reposatory/` | Implement IEnrollmentRepository |

#### 2.2.3 Unit of Work Updates

| Task | File | Updates |
|------|------|---------|
| T2.12 | `IUnitOfWork.cs` | Add: ICourseRepository Courses, IEnrollmentRepository Enrollments |
| T2.13 | `UnitOfWork.cs` | Implement new repository properties |

#### 2.2.4 Service Extensions

| Task | File | Updates |
|------|------|---------|
| T2.14 | `ServiceExtensions.cs` | Register: ICourseRepository, IEnrollmentRepository |

---

### 2.3 BLL Layer (SSIS.BLL)

#### 2.3.1 DTOs - Course

| Task | File | Path | Properties |
|------|------|------|------------|
| T2.15 | `CourseDto.cs` | `SSIS.BLL/DTOs/Courses/` | Id, Name, Code, Credits, Description, DoctorId, DoctorName, Semester, AcademicYear, IsActive, CreatedAt, UpdatedAt |
| T2.16 | `CreateCourseDto.cs` | `SSIS.BLL/DTOs/Courses/` | Name, Code, Credits, Description, Semester, AcademicYear |
| T2.17 | `UpdateCourseDto.cs` | `SSIS.BLL/DTOs/Courses/` | Name, Credits, Description, IsActive |
| T2.18 | `AssignDoctorDto.cs` | `SSIS.BLL/DTOs/Courses/` | DoctorId |

#### 2.3.2 DTOs - Enrollment

| Task | File | Path | Properties |
|------|------|------|------------|
| T2.19 | `EnrollmentDto.cs` | `SSIS.BLL/DTOs/Enrollments/` | Id, StudentId, StudentName, CourseId, CourseName, CourseCode, EnrollmentDate, IsActive |
| T2.20 | `CreateEnrollmentDto.cs` | `SSIS.BLL/DTOs/Enrollments/` | StudentId, CourseId |
| T2.21 | `StudentCoursesDto.cs` | `SSIS.BLL/DTOs/Enrollments/` | StudentId, Courses (list) |
| T2.22 | `CourseStudentsDto.cs` | `SSIS.BLL/DTOs/Enrollments/` | CourseId, Students (list) |

#### 2.3.3 DTOs - Schedule

| Task | File | Path | Properties |
|------|------|------|------------|
| T2.23 | `ScheduleDto.cs` | `SSIS.BLL/DTOs/Schedules/` | Id, CourseId, CourseName, DayOfWeek, StartTime, EndTime, Room |
| T2.24 | `CreateScheduleDto.cs` | `SSIS.BLL/DTOs/Schedules/` | CourseId, DayOfWeek, StartTime, EndTime, Room |
| T2.25 | `UpdateScheduleDto.cs` | `SSIS.BLL/DTOs/Schedules/` | DayOfWeek, StartTime, EndTime, Room |

#### 2.3.4 Service Interfaces

| Task | File | Path | Methods |
|------|------|------|---------|
| T2.26 | `ICourseService.cs` | `SSIS.BLL/Interfaces/` | CreateAsync, UpdateAsync, DeleteAsync, GetByIdAsync, GetAllAsync, GetByDoctorAsync, AssignDoctorAsync, GetActiveCoursesAsync |
| T2.27 | `IEnrollmentService.cs` | `SSIS.BLL/Interfaces/` | EnrollAsync, UnenrollAsync, GetStudentCoursesAsync, GetCourseStudentsAsync, CheckEnrollmentAsync, GetByIdAsync |
| T2.28 | `IScheduleService.cs` | `SSIS.BLL/Interfaces/` | CreateAsync, UpdateAsync, DeleteAsync, GetByIdAsync, GetByCourseAsync, GetAllAsync |

#### 2.3.5 Service Implementations

| Task | File | Path | Description |
|------|------|------|-------------|
| T2.29 | `CourseService.cs` | `SSIS.BLL/Services/` | Business logic for courses |
| T2.30 | `EnrollmentService.cs` | `SSIS.BLL/Services/` | Business logic for enrollments |
| T2.31 | `ScheduleService.cs` | `SSIS.BLL/Services/` | Business logic for schedules |

#### 2.3.6 Validators (FluentValidation)

| Task | File | Path | Validation Rules |
|------|------|------|------------------|
| T2.32 | `CreateCourseValidator.cs` | `SSIS.BLL/Validators/` | Name: required, 3-100 chars; Code: required, unique format; Credits: 1-10; Semester: required |
| T2.33 | `UpdateCourseValidator.cs` | `SSIS.BLL/Validators/` | Name: 3-100 chars; Credits: 1-10 |
| T2.34 | `CreateEnrollmentValidator.cs` | `SSIS.BLL/Validators/` | StudentId: valid student; CourseId: valid course; Check not already enrolled; Student role must be Student |
| T2.35 | `CreateScheduleValidator.cs` | `SSIS.BLL/Validators/` | CourseId: valid; DayOfWeek: valid enum; StartTime < EndTime; Room: required, max 50 chars |

#### 2.3.7 Mapping Profiles (AutoMapper)

| Task | File | Path | Mappings |
|------|------|------|----------|
| T2.36 | `CourseMappingProfile.cs` | `SSIS.BLL/Mappings/` | Course <-> CourseDto, CreateCourseDto -> Course, UpdateCourseDto -> Course |
| T2.37 | `EnrollmentMappingProfile.cs` | `SSIS.BLL/Mappings/` | Enrollment <-> EnrollmentDto, CreateEnrollmentDto -> Enrollment |
| T2.38 | `ScheduleMappingProfile.cs` | `SSIS.BLL/Mappings/` | Schedule <-> ScheduleDto, CreateScheduleDto -> Schedule |

---

### 2.4 API Layer (SSIS.PL)

#### 2.4.1 Controllers

| Task | File | Path | Endpoints |
|------|------|------|-----------|
| T2.39 | `CoursesController.cs` | `SSIS.PL/Controllers/v1/` | See [Courses Endpoints](#courses-endpoints) below |
| T2.40 | `EnrollmentsController.cs` | `SSIS.PL/Controllers/v1/` | See [Enrollments Endpoints](#enrollments-endpoints) below |
| T2.41 | `SchedulesController.cs` | `SSIS.PL/Controllers/v1/` | See [Schedules Endpoints](#schedules-endpoints) below |

---

### 2.5 Database Migration

| Task | Command | Description |
|------|---------|-------------|
| T2.42 | `Add-Migration AddAcademicTables` | Create migration for Course, Enrollment, Schedule |
| T2.43 | `Update-Database` | Apply migration to database |

---

## Authorization Matrix

| Operation | Admin | Doctor | Student | Notes |
|-----------|-------|--------|---------|-------|
| **Courses** |
| Create Course | ✅ | ❌ | ❌ | Only Admin |
| Update Course | ✅ | ❌ | ❌ | Only Admin |
| Delete Course | ✅ | ❌ | ❌ | Soft delete, only if no enrollments |
| View All Courses | ✅ | ✅ | ✅ | All authenticated users |
| View Course by ID | ✅ | ✅ | ✅ | All authenticated users |
| Assign Doctor | ✅ | ❌ | ❌ | Only Admin |
| **Enrollments** |
| Enroll Student | ✅ | ❌ | ❌ | Only Admin |
| Unenroll Student | ✅ | ❌ | ❌ | Only Admin |
| View Student Courses | ✅ | ✅ | ✅ | Student can view own, Admin/Doctor can view any |
| View Course Students | ✅ | ✅ | ❌ | Doctor can view their courses |
| **Schedules** |
| Create Schedule | ✅ | ❌ | ❌ | Only Admin |
| Update Schedule | ✅ | ❌ | ❌ | Only Admin |
| Delete Schedule | ✅ | ❌ | ❌ | Only Admin |
| View Schedules | ✅ | ✅ | ✅ | All authenticated users |

---

## API Endpoints

### Courses Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | `/api/v1/courses` | Get all courses (paginated) | Admin, Doctor, Student |
| GET | `/api/v1/courses/{id}` | Get course by ID | Admin, Doctor, Student |
| GET | `/api/v1/courses/doctor/{doctorId}` | Get courses by doctor | Admin, Doctor, Student |
| POST | `/api/v1/courses` | Create new course | Admin only |
| PUT | `/api/v1/courses/{id}` | Update course | Admin only |
| DELETE | `/api/v1/courses/{id}` | Soft delete course | Admin only |
| POST | `/api/v1/courses/{id}/assign-doctor` | Assign doctor to course | Admin only |

### Enrollments Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| POST | `/api/v1/enrollments` | Enroll student in course | Admin only |
| DELETE | `/api/v1/enrollments/{id}` | Unenroll student | Admin only |
| GET | `/api/v1/enrollments/student/{studentId}` | Get student's courses | Admin, Doctor, Student (own) |
| GET | `/api/v1/enrollments/course/{courseId}` | Get course's students | Admin, Doctor (own courses) |
| GET | `/api/v1/enrollments/check` | Check if enrolled (query params) | Admin, Doctor, Student |

### Schedules Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | `/api/v1/schedules` | Get all schedules | Admin, Doctor, Student |
| GET | `/api/v1/schedules/{id}` | Get schedule by ID | Admin, Doctor, Student |
| GET | `/api/v1/schedules/course/{courseId}` | Get schedules by course | Admin, Doctor, Student |
| POST | `/api/v1/schedules` | Create schedule | Admin only |
| PUT | `/api/v1/schedules/{id}` | Update schedule | Admin only |
| DELETE | `/api/v1/schedules/{id}` | Delete schedule | Admin only |

---

## Database Schema

### Courses Table

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | UNIQUEIDENTIFIER | PK | Primary key |
| Name | NVARCHAR(100) | NOT NULL | Course name |
| Code | NVARCHAR(20) | NOT NULL, UNIQUE | Course code (e.g., CS101) |
| Credits | INT | NOT NULL | Credit hours (1-10) |
| Description | NVARCHAR(500) | NULL | Course description |
| DoctorId | UNIQUEIDENTIFIER | FK -> Users | Assigned doctor |
| Semester | NVARCHAR(20) | NOT NULL | e.g., "Fall 2025" |
| AcademicYear | NVARCHAR(10) | NOT NULL | e.g., "2025-2026" |
| IsActive | BIT | NOT NULL, DEFAULT 1 | Active status |
| CreatedAt | DATETIME2 | NOT NULL | Creation timestamp |
| UpdatedAt | DATETIME2 | NULL | Last update timestamp |

### Enrollments Table

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | UNIQUEIDENTIFIER | PK | Primary key |
| StudentId | UNIQUEIDENTIFIER | FK -> Users, NOT NULL | Enrolled student |
| CourseId | UNIQUEIDENTIFIER | FK -> Courses, NOT NULL | Enrolled course |
| EnrollmentDate | DATETIME2 | NOT NULL | When enrolled |
| IsActive | BIT | NOT NULL, DEFAULT 1 | Active status |
| CreatedAt | DATETIME2 | NOT NULL | Creation timestamp |
| **Unique** | StudentId + CourseId | | Prevent duplicate enrollments |

### Schedules Table

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | UNIQUEIDENTIFIER | PK | Primary key |
| CourseId | UNIQUEIDENTIFIER | FK -> Courses, NOT NULL | Course reference |
| DayOfWeek | INT | NOT NULL | 1=Sunday to 5=Thursday |
| StartTime | TIME | NOT NULL | Lecture start time |
| EndTime | TIME | NOT NULL | Lecture end time |
| Room | NVARCHAR(50) | NOT NULL | Room number/location |
| CreatedAt | DATETIME2 | NOT NULL | Creation timestamp |

---

## Business Rules & Validations

### Course Management
1. **Code Uniqueness**: Course code must be unique across the system
2. **Doctor Assignment**: Only users with Doctor role can be assigned as course doctors
3. **Delete Restriction**: Cannot delete a course that has active enrollments
4. **Credits Range**: Credits must be between 1 and 10

### Enrollment Management
1. **Student Validation**: Only users with Student role can be enrolled
2. **Duplicate Prevention**: A student cannot be enrolled in the same course twice
3. **Active Course**: Can only enroll in active courses
4. **Soft Delete**: Unenrolling soft-deletes the enrollment record

### Schedule Management
1. **Time Validation**: End time must be after start time
2. **Day Validation**: Only Sunday-Thursday (1-5) are valid
3. **Conflict Detection**: (Optional) Detect room/time conflicts
4. **Course Validation**: Schedule must reference an existing course

---

## Checklist Summary

### Domain Layer
- [ ] DayOfWeekEnum
- [ ] GradeLetter enum
- [ ] Course entity
- [ ] Enrollment entity
- [ ] Schedule entity
- [ ] Update User entity with navigation properties
- [ ] ICourseRepository interface
- [ ] IEnrollmentRepository interface

### DAL Layer
- [ ] Update AppDbContext with new DbSets
- [ ] CourseRepository implementation
- [ ] EnrollmentRepository implementation
- [ ] Update IUnitOfWork with new repositories
- [ ] Update UnitOfWork implementation
- [ ] Update ServiceExtensions

### BLL Layer
- [ ] Course DTOs (CourseDto, CreateCourseDto, UpdateCourseDto, AssignDoctorDto)
- [ ] Enrollment DTOs (EnrollmentDto, CreateEnrollmentDto, StudentCoursesDto, CourseStudentsDto)
- [ ] Schedule DTOs (ScheduleDto, CreateScheduleDto, UpdateScheduleDto)
- [ ] ICourseService interface
- [ ] IEnrollmentService interface
- [ ] IScheduleService interface
- [ ] CourseService implementation
- [ ] EnrollmentService implementation
- [ ] ScheduleService implementation
- [ ] CreateCourseValidator
- [ ] UpdateCourseValidator
- [ ] CreateEnrollmentValidator
- [ ] CreateScheduleValidator
- [ ] CourseMappingProfile
- [ ] EnrollmentMappingProfile
- [ ] ScheduleMappingProfile

### API Layer
- [ ] CoursesController
- [ ] EnrollmentsController
- [ ] SchedulesController

### Database
- [ ] Add-Migration AddAcademicTables
- [ ] Update-Database

---

## Next Steps

1. **Complete Phase 1** (Authentication & User Management) first
2. **Review this plan** and make any adjustments
3. **Toggle to Act Mode** to begin implementation
4. **Implement tasks** in order: Domain → DAL → BLL → API → Migration

---

*Generated for Smart Student Management System Phase 2*
