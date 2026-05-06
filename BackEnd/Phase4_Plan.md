Phase 4: Attendance Management - Implementation Plan
Project: Smart Student Management System (SSIS)
Phase: 4 - Attendance Management
Duration: 2 days
Dependencies: Phase 2 (Courses & Enrollments) must be completed first

📋 Table of Contents
Overview

Prerequisites

Entity Relationship Diagram

Task Breakdown

Authorization Matrix

API Endpoints

Database Schema

Overview
Goal
Implement the complete attendance management module including:

Take Attendance: Doctors can record student attendance per course/date

View Attendance: Students view their attendance, doctors view course attendance

Attendance Reports: Calculate attendance percentages and generate reports

New Entities
Entity	Description
Attendance	Records student attendance for a specific course on a specific date
New Enums
Enum	Values
AttendanceStatus	Present (1), Absent (2), Late (3), Excused (4)
Prerequisites
✅ Phase 2 (Courses, Enrollments) must be functional

✅ Student and Doctor roles must exist

✅ Enrollment data must be available

Entity Relationship Diagram
text
┌─────────────────────────────────────────────────────────────────────────┐
│                         ENTITY RELATIONSHIPS                            │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────┐       ┌─────────────┐       ┌─────────────┐
│    User     │1      │  Attendance │       │   Course    │
│  (Student)  │───────│<───────────>│       │             │
│             │       │             │       │             │
│ PK: Id      │       │ PK: Id      │       │ PK: Id      │
│ FullName    │       │ StudentId   │       │ Name        │
│ Role        │       │ CourseId    │       │ Code        │
└─────────────┘       │ Date        │       └──────┬──────┘
                      │ Status      │              │
                      │ Remarks     │              │
                      └─────────────┘              │
                                                   │
                                            ┌──────┴──────┐
                                            │    User     │
                                            │  (Doctor)   │
                                            │ PK: Id      │
                                            └─────────────┘
Task Breakdown
4.1 Domain Layer (SSIS.Domain)
4.1.1 Enums
Task	File	Path	Content
T4.1	AttendanceStatus.cs	SSIS.Domain/Enums/	Present=1, Absent=2, Late=3, Excused=4
4.1.2 Entities
Task	File	Path	Properties
T4.2	Attendance.cs	SSIS.Domain/Entities/	Id, StudentId, CourseId, Date, Status, Remarks, CreatedAt
T4.3	Update User.cs	SSIS.Domain/Entities/	Add navigation: ICollection<Attendance> Attendances
T4.4	Update Course.cs	SSIS.Domain/Entities/	Add navigation: ICollection<Attendance> Attendances
4.1.3 Repository Interfaces
Task	File	Path	Methods
T4.5	IAttendanceRepository.cs	SSIS.Domain/Interfaces/	GetByStudentIdAsync, GetByCourseIdAsync, GetByStudentCourseAndDateAsync, GetAttendancePercentageByCourseAsync, GetStudentAttendancePercentageByCourseAsync, GetOverallAttendancePercentageForStudentAsync, GetOverallAttendancePercentageForCourseAsync, GetConsecutiveAbsencesAsync
4.2 DAL Layer (SSIS.DAL)
4.2.1 Database Context
Task	File	Updates
T4.6	AppDbContext.cs	Add DbSet<Attendance>
4.2.2 Repository Implementation
Task	File	Path	Description
T4.7	AttendanceRepository.cs	SSIS.DAL/Repositories/	Implement IAttendanceRepository
4.2.3 Unit of Work Updates
Task	File	Updates
T4.8	IUnitOfWork.cs	Add: IAttendanceRepository Attendances
T4.9	UnitOfWork.cs	Implement new repository property
4.2.4 Service Extensions
Task	File	Updates
T4.10	ServiceExtensions.cs	Register: IAttendanceRepository, IAttendanceService
4.3 BLL Layer (SSIS.BLL)
4.3.1 DTOs - Attendance
Task	File	Path	Properties
T4.11	AttendanceDto.cs	SSIS.BLL/DTOs/Attendance/	Id, StudentId, StudentName, CourseId, CourseName, Date, Status, Remarks, CreatedAt
T4.12	TakeAttendanceDto.cs	SSIS.BLL/DTOs/Attendance/	CourseId, Date, List<StudentAttendanceDto>
T4.13	StudentAttendanceDto.cs	SSIS.BLL/DTOs/Attendance/	StudentId, Status, Remarks
T4.14	AttendancePercentageDto.cs	SSIS.BLL/DTOs/Attendance/	CourseId, CourseName, Percentage
T4.15	StudentAttendancePercentageDto.cs	SSIS.BLL/DTOs/Attendance/	StudentId, StudentName, Percentage
4.3.2 Service Interfaces
Task	File	Path	Methods
T4.16	IAttendanceService.cs	SSIS.BLL/Interfaces/	TakeAttendanceAsync, GetStudentAttendanceAsync, GetCourseAttendanceAsync, GetStudentAttendancePercentageAsync, GetCourseAttendancePercentageAsync, GetOverallAttendancePercentageAsync
4.3.3 Service Implementation
Task	File	Path	Description
T4.17	AttendanceService.cs	SSIS.BLL/Services/	Business logic for attendance
4.3.4 Validators
Task	File	Path	Validation Rules
T4.18	TakeAttendanceValidator.cs	SSIS.BLL/Validators/	CourseId required, Date not future, Students list not empty
4.3.5 Mapping Profile
Task	File	Path	Mappings
T4.19	AttendanceMappingProfile.cs	SSIS.BLL/Mappings/	Attendance <-> AttendanceDto
4.4 API Layer (SSIS.PL)
4.4.1 Controller
Task	File	Path	Endpoints
T4.20	AttendanceController.cs	SSIS.PL/Controllers/v1/	See Attendance Endpoints
4.5 Database Migration
Task	Command	Description
T4.21	Add-Migration AddAttendanceTables	Create migration for Attendance
T4.22	Update-Database	Apply migration
Authorization Matrix
Operation	Admin	Doctor	Student	Notes
Take Attendance	✅	✅ (own courses)	❌	Doctor can only take attendance for their courses
View Student Attendance	✅	✅ (their students)	✅ (own)	Student sees only their own
View Course Attendance	✅	✅ (own courses)	❌	
View Attendance Percentage	✅	✅ (own courses)	✅ (own)	
API Endpoints
Attendance Endpoints
Method	Endpoint	Description	Access
POST	/api/v1/attendance/take	Take attendance for a course	Doctor, Admin
GET	/api/v1/attendance/student/{studentId}	Get student's attendance records	Student (own), Doctor, Admin
GET	/api/v1/attendance/course/{courseId}	Get course's attendance records	Doctor (own course), Admin
GET	/api/v1/attendance/student/{studentId}/percentage	Get attendance percentage by course	Student (own), Doctor, Admin
GET	/api/v1/attendance/course/{courseId}/student-percentages	Get each student's percentage	Doctor (own course), Admin
GET	/api/v1/attendance/student/{studentId}/overall	Get overall attendance percentage	Student (own), Doctor, Admin
Database Schema
Attendance Table
Column	Type	Constraints	Description
Id	UNIQUEIDENTIFIER	PK	Primary key
StudentId	UNIQUEIDENTIFIER	FK -> Users, NOT NULL	Student reference
CourseId	UNIQUEIDENTIFIER	FK -> Courses, NOT NULL	Course reference
Date	DATETIME2	NOT NULL	Attendance date
Status	INT	NOT NULL	1=Present, 2=Absent, 3=Late, 4=Excused
Remarks	NVARCHAR(500)	NULL	Optional comments
CreatedAt	DATETIME2	NOT NULL	Creation timestamp
Unique	StudentId + CourseId + Date		One record per student per course per day
Checklist Summary
Domain Layer
AttendanceStatus enum

Attendance entity

Update User entity with Attendances navigation

Update Course entity with Attendances navigation

IAttendanceRepository interface

DAL Layer
Update AppDbContext with Attendances DbSet

AttendanceRepository implementation

Update IUnitOfWork with Attendances

Update UnitOfWork implementation

Update ServiceExtensions

BLL Layer
Attendance DTOs (AttendanceDto, TakeAttendanceDto, StudentAttendanceDto, etc.)

IAttendanceService interface

AttendanceService implementation

TakeAttendanceValidator

AttendanceMappingProfile

API Layer
AttendanceController

Database
Add-Migration AddAttendanceTables

Update-Database