Phase 3: Grade Management - Implementation Plan

Project: Smart Student Management System (SSIS)
Phase: 3 - Grade Management
Duration: 2-3 days
Dependencies: Phase 1 (Auth) + Phase 2 (Courses, Enrollments)

---

📋 Table of Contents

1. Overview
2. Prerequisites
3. Entity Relationship Diagram
4. Task Breakdown
5. Authorization Matrix
6. API Endpoints
7. Database Schema
8. Business Rules & Validations
9. Checklist Summary

---

Overview

Goal

Implement the complete grade management module including:

· Grades: Record, update, and view student grades per course enrollment
· Grade Calculation: Automatic grade letter based on score (if needed)
· Transcript: View student's academic record (GPA, course grades)
· Statistics: Course grade distribution (optional for Phase 3)

New Entities

Entity Description
Grade Stores student's grade for a specific enrollment (course)
GradeAudit (Optional) Logs changes to grades for tracking

Reused Entities from Previous Phases

Entity Usage
User (Student) Grade belongs to a student
Course Grade is associated via Enrollment
Enrollment Link between student and course
GradeLetter Enum (A, B, C, D, F) – already defined in Phase 2

---

Prerequisites

Before starting Phase 3, ensure Phase 1 and Phase 2 are complete:

· ✅ JWT Authentication & Role-based authorization
· ✅ UserService (Student, Doctor, Admin)
· ✅ CourseService and CourseRepository
· ✅ EnrollmentService and EnrollmentRepository
· ✅ UnitOfWork pattern
· ✅ AutoMapper configurations for Courses and Enrollments

---

Entity Relationship Diagram

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

Task Breakdown

3.1 Domain Layer (SSIS.Domain)

3.1.1 Enums

Task File Path Content
T3.1 GradeLetter.cs SSIS.Domain/Enum/ Already exists in Phase 2 (A=1,B=2,C=3,D=4,F=5) – no change needed

3.1.2 Entities

Task File Path Properties
T3.2 Grade.cs SSIS.Domain/Entities/ Id (Guid), EnrollmentId (Guid), Score (decimal, 0-100), GradeLetter (GradeLetter), Remarks (string, max 200), EnteredById (Guid), EnteredAt (DateTime), UpdatedAt (DateTime?)
T3.3 GradeAudit.cs (optional) SSIS.Domain/Entities/ Id, GradeId, OldScore, NewScore, ChangedByUserId, ChangedAt

3.1.3 Update Existing Entities

Task File Updates
T3.4 Enrollment.cs Add navigation: Grade? Grade (one-to-one with Grade)
T3.5 User.cs Add navigation: ICollection<Grade> EnteredGrades (for doctors)

3.1.4 Repository Interfaces

Task File Path Methods
T3.6 IGradeRepository.cs SSIS.Domain/Interfaces/ GetByIdAsync, GetByEnrollmentIdAsync, GetByStudentIdAsync, GetByCourseIdAsync, GetByDoctorIdAsync, UpdateAsync, AddAsync, DeleteAsync, GetWithDetailsAsync

---

3.2 DAL Layer (SSIS.DAL)

3.2.1 Database Context

Task File Updates
T3.7 AppDbContext.cs Add DbSet<Grade>, DbSet<GradeAudit> (if auditing)

3.2.2 Repository Implementation

Task File Path Description
T3.8 GradeRepository.cs SSIS.DAL/Reposatory/ Implement IGradeRepository with eager loading for Enrollment->Student, Course

3.2.3 Unit of Work Updates

Task File Updates
T3.9 IUnitOfWork.cs Add property: IGradeRepository Grades
T3.10 UnitOfWork.cs Implement new property

3.2.4 Service Extensions

Task File Updates
T3.11 ServiceExtensions.cs Register: IGradeRepository, GradeRepository

---

3.3 BLL Layer (SSIS.BLL)

3.3.1 DTOs - Grade

Task File Path Properties
T3.12 GradeDto.cs SSIS.BLL/DTOs/Grades/ Id, EnrollmentId, StudentId, StudentName, CourseId, CourseName, CourseCode, Score, GradeLetter, Remarks, EnteredByDoctorName, EnteredAt, UpdatedAt
T3.13 CreateGradeDto.cs SSIS.BLL/DTOs/Grades/ EnrollmentId (or StudentId+CourseId), Score, Remarks
T3.14 UpdateGradeDto.cs SSIS.BLL/DTOs/Grades/ Score, Remarks
T3.15 GradeBulkCreateDto.cs SSIS.BLL/DTOs/Grades/ List of GradeEntryDto (StudentId, Score, Remarks) for a course
T3.16 StudentTranscriptDto.cs SSIS.BLL/DTOs/Grades/ StudentId, StudentName, TotalCredits, GPA, Courses (list of CourseGradeDto)
T3.17 CourseGradeStatisticsDto.cs SSIS.BLL/DTOs/Grades/ CourseId, CourseName, AverageScore, MinScore, MaxScore, GradeDistribution (A/B/C/D/F counts)

3.3.2 Service Interfaces

Task File Path Methods
T3.18 IGradeService.cs SSIS.BLL/Interfaces/ CreateOrUpdateGradeAsync (upsert), UpdateGradeAsync, DeleteGradeAsync, GetGradeByIdAsync, GetGradesByStudentAsync, GetGradesByCourseAsync, GetCourseStatisticsAsync, GetStudentTranscriptAsync, BulkCreateGradesAsync

3.3.3 Service Implementation

Task File Path Description
T3.19 GradeService.cs SSIS.BLL/Services/ Implement all business logic: auto-calc GradeLetter from Score, validate that doctor is assigned to course, check enrollment exists, prevent duplicate grades, handle audit trail

3.3.4 Helper Class for Grade Calculation

Task File Path Method
T3.20 GradeCalculator.cs SSIS.BLL/Helpers/ static GradeLetter GetGradeLetter(decimal score) → A: >=90, B: 80-89, C: 70-79, D: 60-69, F: <60

3.3.5 Validators (FluentValidation)

Task File Path Validation Rules
T3.21 CreateGradeValidator.cs SSIS.BLL/Validators/ EnrollmentId: valid enrollment; Score: 0-100; Remarks: max 200 chars; Check if grade already exists (then should use update)
T3.22 UpdateGradeValidator.cs SSIS.BLL/Validators/ Score: 0-100; Remarks: max 200
T3.23 BulkCreateGradeValidator.cs SSIS.BLL/Validators/ Validate each entry, check doctor authorization for the course

3.3.6 Mapping Profiles (AutoMapper)

Task File Path Mappings
T3.24 GradeMappingProfile.cs SSIS.BLL/Mappings/ Grade ↔ GradeDto; CreateGradeDto → Grade; UpdateGradeDto → Grade

---

3.4 API Layer (SSIS.PL)

3.4.1 Controllers

Task File Path Endpoints
T3.25 GradesController.cs SSIS.PL/Controllers/v1/ See Grades Endpoints below
T3.26 TranscriptController.cs SSIS.PL/Controllers/v1/ For student transcripts and statistics

---

3.5 Database Migration

Task Command Description
T3.27 Add-Migration AddGradeTables Create migration for Grade (and GradeAudit if included)
T3.28 Update-Database Apply migration to database

---

Authorization Matrix

Operation Admin Doctor Student Notes
Grades
Create/Update Grade for a student ✅ ✅ (only for courses they teach) ❌ Doctor can only grade enrolled students in their courses
Delete Grade ✅ ❌ ❌ Admin only (or soft-delete)
View Grade by ID ✅ ✅ (if related to their course) ✅ (own grade) Student can only view own
View all grades of a student ✅ ✅ (if student in their course) ✅ (own) 
View all grades of a course ✅ ✅ (if they teach it) ❌ 
Bulk upload grades (Excel) ✅ ✅ (for their course) ❌ 
View own transcript ✅ ✅ (for students they teach? optional) ✅ Student only own
View course grade statistics ✅ ✅ (if they teach it) ❌ 

---

API Endpoints

Grades Endpoints

Method Endpoint Description Access
GET /api/v1/grades/{id} Get grade by ID Admin, Doctor (own course), Student (own)
GET /api/v1/grades/student/{studentId} Get all grades for a student Admin, Doctor (if student in their course), Student (own)
GET /api/v1/grades/course/{courseId} Get all grades for a course Admin, Doctor (if they teach it)
POST /api/v1/grades Create or update grade (upsert) Admin, Doctor (own course)
PUT /api/v1/grades/{id} Update existing grade Admin, Doctor (own course)
DELETE /api/v1/grades/{id} Delete grade Admin only
POST /api/v1/grades/bulk Bulk create/update grades for a course Admin, Doctor (own course)
GET /api/v1/grades/course/{courseId}/statistics Get grade statistics for a course Admin, Doctor (own course)

Transcript Endpoints

Method Endpoint Description Access
GET /api/v1/transcript/me Get logged-in student's transcript Student
GET /api/v1/transcript/student/{studentId} Get transcript for any student Admin, Doctor
GET /api/v1/transcript/course/{courseId}/export Export course grades as CSV/Excel Admin, Doctor (own course)

---

Database Schema

Grades Table

Column Type Constraints Description
Id UNIQUEIDENTIFIER PK Primary key
EnrollmentId UNIQUEIDENTIFIER FK -> Enrollments, NOT NULL, UNIQUE Each enrollment has at most one grade
Score DECIMAL(5,2) NOT NULL, CHECK (0-100) Numerical score
GradeLetter INT NOT NULL Foreign key to GradeLetter enum (1-5)
Remarks NVARCHAR(200) NULL Optional comments
EnteredById UNIQUEIDENTIFIER FK -> Users, NOT NULL Doctor or Admin who entered
EnteredAt DATETIME2 NOT NULL When first entered
UpdatedAt DATETIME2 NULL Last update timestamp

Unique Constraint: EnrollmentId unique (one grade per enrollment)

GradeAudit Table (optional)

Column Type Constraints Description
Id UNIQUEIDENTIFIER PK Primary key
GradeId UNIQUEIDENTIFIER FK -> Grades, NOT NULL Referenced grade
OldScore DECIMAL(5,2) NULL Previous score
NewScore DECIMAL(5,2) NOT NULL New score
ChangedByUserId UNIQUEIDENTIFIER FK -> Users, NOT NULL Who made the change
ChangedAt DATETIME2 NOT NULL Timestamp of change

---

Business Rules & Validations

Grade Entry Rules

1. Enrollment Must Exist: Grade can only be added for an active enrollment.
2. Unique Grade per Enrollment: A student can have only one grade per course.
3. Doctor Authorization: Only the doctor assigned to the course (or Admin) can enter/update grades for that course.
4. Score Range: Score must be between 0 and 100 inclusive.
5. Auto Grade Letter: GradeLetter is automatically derived from Score using the mapping:
   · A: 90-100
   · B: 80-89
   · C: 70-79
   · D: 60-69
   · F: 0-59
6. Remarks Optional: Max 200 characters.
7. Update Tracking: If auditing is enabled, any change to Score should log old/new values.
8. No Grade After Finalization: (Optional rule) Grades cannot be changed after a deadline (e.g., end of semester) – can be configured.

Transcript Calculation

· GPA: Sum of (Grade points × Credits) / Total Credits.
    Grade points: A=4.0, B=3.0, C=2.0, D=1.0, F=0.0.
· Only include courses with grades (not including withdrawals or incomplete if defined).

---

Checklist Summary

Domain Layer

· Grade entity
· GradeAudit entity (optional)
· Update Enrollment entity with Grade navigation
· Update User entity with EnteredGrades
· IGradeRepository interface

DAL Layer

· AppDbContext updated with DbSet<Grade>
· GradeRepository implementation
· IUnitOfWork updated
· UnitOfWork updated
· ServiceExtensions updated

BLL Layer

· GradeDto, CreateGradeDto, UpdateGradeDto, BulkCreateGradeDto
· StudentTranscriptDto, CourseGradeStatisticsDto
· IGradeService interface
· GradeService implementation
· GradeCalculator helper
· CreateGradeValidator, UpdateGradeValidator, BulkCreateGradeValidator
· GradeMappingProfile

API Layer

· GradesController (all endpoints)
· TranscriptController

Database

· Add-Migration AddGradeTables
· Update-Database

---

Next Steps

1. Complete Phase 2 (Academic Management) first – ensures Enrollments exist.
2. Review this Phase 3 plan with your team.
3. Toggle to Act Mode to begin implementation.
4. Implement in order: Domain → DAL → BLL → API → Migration.

---

Generated for Smart Student Management System Phase 3 – Grade Management