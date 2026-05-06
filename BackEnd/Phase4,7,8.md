# Implementation Plan - Phases 4, 7, 8 + Missing Validators

## Project: Smart Student Management System (SSIS)
## Phases: 4 (Attendance), 7 (Reports & Dashboard), 8 (System Administration)
## Excludes: Phase 6 (Notifications - as requested)
## Includes: Missing FluentValidation validators (NFR-06)

---

## Phase 4: Attendance Management

> **Duration**: 2 days  
> **Dependencies**: Phase 1, 2, 3 Complete

---

### Overview

Implement complete attendance management module:
- **Attendance**: Record, update, and view student attendance per course
- **Attendance Percentage**: Calculate attendance percentage per course
- **Attendance Reports**: View attendance reports with filters

### New Enums

| Enum | Values |
|------|--------|
| **AttendanceStatus** | Present (1), Absent (2), Late (3), Excused (4) |

### New Entity

| Entity | Properties |
|--------|------------|
| **Attendance** | Id, StudentId, CourseId, Date, Status, Remarks, CreatedAt, UpdatedAt |

---

### Task Breakdown

#### 4.1 Domain Layer (SSIS.Domain)

| Task | File | Path | Description |
|------|------|------|-------------|
| T4.1 | `AttendanceStatus.cs` | `SSIS.Domain/Enum/` | Present=1, Absent=2, Late=3, Excused=4 |
| T4.2 | `Attendance.cs` | `SSIS.Domain/Entities/` | Id, StudentId, CourseId, Date, Status, Remarks, CreatedAt, UpdatedAt |

#### 4.1.3 Update Existing Entities

| Task | File | Updates |
|------|------|---------|
| T4.3 | `User.cs` | Add: `ICollection<Attendance> Attendances` |
| T4.4 | `Course.cs` | Add: `ICollection<Attendance> Attendances` |

#### 4.1.4 Repository Interfaces

| Task | File | Path | Methods |
|------|------|------|---------|
| T4.5 | `IAttendanceRepository.cs` | `SSIS.Domain/Interfaces/` | GetByIdAsync, GetByStudentIdAsync, GetByCourseIdAsync, GetByDateAsync, GetByStudentAndCourseAsync, AddAsync, UpdateAsync, DeleteAsync |

---

#### 4.2 DAL Layer (SSIS.DAL)

| Task | File | Updates |
|------|------|---------|
| T4.6 | `AppDbContext.cs` | Add: `DbSet<Attendance> Attendances` |
| T4.7 | `AttendanceRepository.cs` | `SSIS.DAL/Repositories/` | Implement `IAttendanceRepository` |
| T4.8 | `IUnitOfWork.cs` | Add: `IAttendanceRepository Attendances` |
| T4.9 | `UnitOfWork.cs` | Implement `attendances` property |
| T4.10 | `ServiceExtensions.cs` (DAL) | Register: `IAttendanceRepository`, `AttendanceRepository` |

---

#### 4.3 BLL Layer (SSIS.BLL)

##### DTOs

| Task | File | Path | Properties |
|------|------|------|------------|
| T4.11 | `AttendanceDto.cs` | `SSIS.BLL/DTOs/Attendance/` | Id, StudentId, StudentName, CourseId, CourseName, Date, Status, Remarks, CreatedAt |
| T4.12 | `TakeAttendanceDto.cs` | `SSIS.BLL/DTOs/Attendance/` | StudentId, CourseId, Date, Status, Remarks |
| T4.13 | `AttendancePercentageDto.cs` | `SSIS.BLL/DTOs/Attendance/` | StudentId, CourseId, TotalClasses, PresentCount, AbsentCount, LateCount, Percentage |

##### Service Interfaces

| Task | File | Path | Methods |
|------|------|------|---------|
| T4.14 | `IAttendanceService.cs` | `SSIS.BLL/Interfaces/` | TakeAttendanceAsync, UpdateAttendanceAsync, GetStudentAttendanceAsync, GetCourseAttendanceAsync, CalculateAttendancePercentageAsync, GetAttendanceReportAsync |

##### Service Implementation

| Task | File | Path | Description |
|------|------|------|-------------|
| T4.15 | `AttendanceService.cs` | `SSIS.BLL/Services/Implementation/` | Business logic for attendance management |

##### Validators

| Task | File | Path | Validation Rules |
|------|------|------|------------------|
| T4.16 | `TakeAttendanceValidator.cs` | `SSIS.BLL/Validators/` | StudentId: valid student; CourseId: valid course; Date: not future; Status: valid enum |

##### Mapping Profiles

| Task | File | Path | Mappings |
|------|------|------|----------|
| T4.17 | `AttendanceMappingProfile.cs` | `SSIS.BLL/Mapper/` | Attendance ↔ AttendanceDto, TakeAttendanceDto → Attendance |

---

#### 4.4 API Layer (SSIS.PL)

| Task | File | Path | Endpoints |
|------|------|------|-----------|
| T4.18 | `AttendanceController.cs` | `SSIS.PL/Controllers/` | See API Endpoints below |

---

#### 4.5 Database Migration

| Task | Command | Description |
|------|---------|-------------|
| T4.19 | `Add-Migration AddAttendanceTable` | Create migration for Attendance |
| T4.20 | `Update-Database` | Apply migration |

---

### Authorization Matrix

| Operation | Admin | Doctor | Student |
|-----------|-------|--------|---------|
| Take Attendance | ✅ | ✅ (own courses) | ❌ |
| Update Attendance | ✅ | ✅ (own courses) | ❌ |
| View Student Attendance | ✅ | ✅ (course students) | ✅ (own) |
| View Course Attendance | ✅ | ✅ (own courses) | ❌ |
| Attendance Percentage | ✅ | ✅ (own courses) | ✅ (own) |

---

### API Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| POST | `/api/v1/attendance` | Take attendance (single/multiple) | Doctor, Admin |
| PUT | `/api/v1/attendance/{id}` | Update attendance | Doctor, Admin |
| GET | `/api/v1/attendance/student/{studentId}` | Get student's attendance | Admin, Doctor, Student (own) |
| GET | `/api/v1/attendance/course/{courseId}` | Get course attendance | Admin, Doctor (own course) |
| GET | `/api/v1/attendance/percentage` | Get attendance percentage | Admin, Doctor, Student (own) |
| DELETE | `/api/v1/attendance/{id}` | Delete attendance record | Admin |

---

### Database Schema

#### Attendance Table

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | UNIQUEIDENTIFIER | PK | Primary key |
| StudentId | UNIQUEIDENTIFIER | FK → Users, NOT NULL | Student |
| CourseId | UNIQUEIDENTIFIER | FK → Courses, NOT NULL | Course |
| Date | DATE | NOT NULL | Attendance date |
| Status | INT | NOT NULL | 1=Present, 2=Absent, 3=Late, 4=Excused |
| Remarks | NVARCHAR(200) | NULL | Optional remarks |
| CreatedAt | DATETIME2 | NOT NULL | Creation timestamp |
| UpdatedAt | DATETIME2 | NULL | Last update |

Unique Constraint: StudentId + CourseId + Date (one record per student per course per day)

---

## Phase 7: Reports & Dashboard

> **Duration**: 2-3 days  
> **Dependencies**: Phase 1, 2, 3, 4 Complete

---

### Overview

Implement reports and dashboard modules:
- **Admin Dashboard**: System statistics and overview
- **Doctor Dashboard**: Course statistics and student performance
- **Student Dashboard**: Personal academic summary
- **Reports**: Grade trends, attendance trends, at-risk students

---

### Task Breakdown

#### 7.1 BLL Layer (SSIS.BLL)

##### Dashboard DTOs

| Task | File | Path | Properties |
|------|------|------|------------|
| T7.1 | `AdminDashboardDto.cs` | `SSIS.BLL/DTOs/Dashboard/` | TotalStudents, TotalDoctors, TotalCourses, ActiveEnrollments, AverageAttendance, AverageGrade, RecentGrades, TopStudents |
| T7.2 | `DoctorDashboardDto.cs` | `SSIS.BLL/DTOs/Dashboard/` | CourseCount, StudentCount, AverageAttendance, AverageGrade, GradeDistribution, RecentSubmissions |
| T7.3 | `StudentDashboardDto.cs` | `SSIS.BLL/DTOs/Dashboard/` | GPA, CoursesCount, AttendancePercentage, UnreadNotifications, UpcomingSchedules, RecentGrades |
| T7.4 | `PerformanceAnalyticsDto.cs` | `SSIS.BLL/DTOs/Dashboard/` | GradeTrends, AttendanceTrends, RiskLevel |

##### Report DTOs

| Task | File | Path | Properties |
|------|------|------|------------|
| T7.5 | `CourseGradeStatisticsDto.cs` | `SSIS.BLL/DTOs/Reports/` | CourseId, CourseName, AverageScore, MinScore, MaxScore, GradeDistribution (A/B/C/D/F counts) |
| T7.6 | `AtRiskStudentDto.cs` | `SSIS.BLL/DTOs/Reports/` | StudentId, StudentName, CourseName, GPA, AttendancePercentage, RiskLevel |

##### Service Interfaces

| Task | File | Path | Methods |
|------|------|------|---------|
| T7.7 | `IReportService.cs` | `SSIS.BLL/Interfaces/` | GetAdminDashboardAsync, GetDoctorDashboardAsync, GetStudentDashboardAsync, GetCourseGradeStatisticsAsync, GetAtRiskStudentsAsync, GetStudentAnalyticsAsync |

##### Service Implementation

| Task | File | Path | Description |
|------|------|------|-------------|
| T7.8 | `ReportService.cs` | `SSIS.BLL/Services/Implementation/` | Implement all dashboard and report logic |

---

#### 7.2 API Layer (SSIS.PL)

| Task | File | Path | Endpoints |
|------|------|------|-----------|
| T7.9 | `DashboardController.cs` | `SSIS.PL/Controllers/` | See API Endpoints below |
| T7.10 | `ReportsController.cs` | `SSIS.PL/Controllers/` | Advanced reports (optional) |

---

### Authorization Matrix

| Operation | Admin | Doctor | Student |
|-----------|-------|--------|---------|
| Admin Dashboard | ✅ | ❌ | ❌ |
| Doctor Dashboard | ❌ | ✅ (own data) | ❌ |
| Student Dashboard | ❌ | ❌ | ✅ (own data) |
| Grade Statistics | ✅ | ✅ (own courses) | ❌ |
| At-Risk Students | ✅ | ✅ (own students) | ❌ |

---

### API Endpoints

#### Dashboard Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | `/api/v1/dashboard/admin` | Get admin dashboard data | Admin |
| GET | `/api/v1/dashboard/doctor` | Get doctor dashboard data | Doctor |
| GET | `/api/v1/dashboard/student` | Get student dashboard data | Student (own) |
| GET | `/api/v1/dashboard/student/{studentId}` | Get specific student dashboard | Admin, Doctor |

#### Report Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | `/api/v1/reports/course/{courseId}/statistics` | Course grade statistics | Admin, Doctor |
| GET | `/api/v1/reports/at-risk` | Get at-risk students | Admin, Doctor |
| GET | `/api/v1/reports/student/{studentId}/analytics` | Student performance analytics | Admin, Doctor (own students) |

---

## Phase 8: System Administration (Audit Logs)

> **Duration**: 1-2 days  
> **Dependencies**: Phase 1 Complete

---

### Overview

Implement system administration features:
- **Audit Logs**: Track all system changes
- **System Settings**: Manage system configuration (optional for now)

---

### Task Breakdown

#### 8.1 Domain Layer (SSIS.Domain)

| Task | File | Path | Properties |
|------|------|------|------------|
| T8.1 | `AuditLog.cs` | `SSIS.Domain/Entities/` | Id, UserId, Action, Entity, EntityId, OldValues, NewValues, Timestamp, IpAddress, UserAgent |

#### 8.1.3 Update Existing Entities

| Task | File | Updates |
|------|------|---------|
| T8.2 | `User.cs` | Add: `ICollection<AuditLog> AuditLogs` |

#### 8.1.4 Repository Interfaces

| Task | File | Path | Methods |
|------|------|------|---------|
| T8.3 | `IAuditLogRepository.cs` | `SSIS.Domain/Interfaces/` | GetByIdAsync, GetByUserIdAsync, GetByEntityAsync, GetByDateRangeAsync, AddAsync, GetAllAsync |

---

#### 8.2 DAL Layer (SSIS.DAL)

| Task | File | Updates |
|------|------|---------|
| T8.4 | `AppDbContext.cs` | Add: `DbSet<AuditLog> AuditLogs` |
| T8.5 | `AuditLogRepository.cs` | `SSIS.DAL/Repositories/` | Implement `IAuditLogRepository` |
| T8.6 | `IUnitOfWork.cs` | Add: `IAuditLogRepository AuditLogs` |
| T8.7 | `UnitOfWork.cs` | Implement `AuditLogs` property |
| T8.8 | `ServiceExtensions.cs` (DAL) | Register: `IAuditLogRepository`, `AuditLogRepository` |

---

#### 8.3 BLL Layer (SSIS.BLL)

##### DTOs

| Task | File | Path | Properties |
|------|------|------|------------|
| T8.9 | `AuditLogDto.cs` | `SSIS.BLL/DTOs/Admin/` | Id, UserId, UserName, Action, Entity, EntityId, OldValues, NewValues, Timestamp, IpAddress |

##### Service Interfaces

| Task | File | Path | Methods |
|------|------|------|---------|
| T8.10 | `IAuditLogService.cs` | `SSIS.BLL/Interfaces/` | GetAuditLogsAsync, LogActionAsync, GetUserActivityAsync |

##### Service Implementation

| Task | File | Path | Description |
|------|------|------|-------------|
| T8.11 | `AuditLogService.cs` | `SSIS.BLL/Services/Implementation/` | Business logic for audit logs |

---

#### 8.4 API Layer (SSIS.PL)

| Task | File | Path | Endpoints |
|------|------|------|-----------|
| T8.12 | `AdminController.cs` | `SSIS.PL/Controllers/` | See API Endpoints below |

---

#### 8.5 Database Migration

| Task | Command | Description |
|------|---------|-------------|
| T8.13 | `Add-Migration AddAuditLogs` | Create migration for AuditLog |
| T8.14 | `Update-Database` | Apply migration |

---

### Authorization Matrix

| Operation | Admin | Doctor | Student |
|-----------|-------|--------|---------|
| View Audit Logs | ✅ | ❌ | ❌ |
| Export Audit Logs | ✅ | ❌ | ❌ |

---

### API Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | `/api/v1/admin/audit-logs` | Get audit logs (paginated, filterable) | Admin |
| GET | `/api/v1/admin/audit-logs/{id}` | Get audit log by ID | Admin |
| GET | `/api/v1/admin/audit-logs/user/{userId}` | Get user activity | Admin |

---

### Database Schema

#### AuditLog Table

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | UNIQUEIDENTIFIER | PK | Primary key |
| UserId | UNIQUEIDENTIFIER | FK → Users, NULL | User who made the change |
| Action | NVARCHAR(100) | NOT NULL | Create, Update, Delete, etc. |
| Entity | NVARCHAR(100) | NOT NULL | Entity name (Course, Grade, etc.) |
| EntityId | UNIQUEIDENTIFIER | NULL | ID of the affected entity |
| OldValues | NVARCHAR(MAX) | NULL | JSON of old values |
| NewValues | NVARCHAR(MAX) | NULL | JSON of new values |
| Timestamp | DATETIME2 | NOT NULL | When the action occurred |
| IpAddress | NVARCHAR(50) | NULL | User's IP address |
| UserAgent | NVARCHAR(500) | NULL | User's browser/device info |

---

## Missing Validators (NFR-06)

> Implement FluentValidation for all DTOs

---

### Validators to Create

| Task | File | Path | Validates | Validation Rules |
|------|------|------|-----------|-------------------|
| T9.1 | `CreateCourseValidator.cs` | `SSIS.BLL/Validators/` | CreateCourseDto | Name: 3-100 chars; Code: required, format; Credits: 1-10; Semester: required; AcademicYear: required |
| T9.2 | `UpdateCourseValidator.cs` | `SSIS.BLL/Validators/` | UpdateCourseDto | Name: 3-100 chars; Credits: 1-10 |
| T9.3 | `CreateEnrollmentValidator.cs` | `SSIS.BLL/Validators/` | CreateEnrollmentDto | StudentId: valid; CourseId: valid; Not already enrolled |
| T9.4 | `CreateScheduleValidator.cs` | `SSIS.BLL/Validators/` | CreateScheduleDto | CourseId: valid; DayOfWeek: 1-5; StartTime < EndTime; Room: required |
| T9.5 | `UpdateScheduleValidator.cs` | `SSIS.BLL/Validators/` | UpdateScheduleDto | DayOfWeek: 1-5; StartTime < EndTime; Room: required |
| T9.6 | `EnterGradeValidator.cs` | `SSIS.BLL/Validators/` | GradeDTO | StudentId: valid; CourseId: valid; Score: 0-100; Remarks: max 200 |
| T9.7 | `UpdateGradeValidator.cs` | `SSIS.BLL/Validators/` | UpdateGradeDTO | Score: 0-100; Remarks: max 200 |
| T9.8 | `CreateFeeValidator.cs` | `SSIS.BLL/Validators/` | CreateFeeDto | StudentId: valid student; Semester: required; AcademicYear: required; TotalAmount: >0; DueDate: future |
| T9.9 | `RecordPaymentValidator.cs` | `SSIS.BLL/Validators/` | ManualPaymentDto | FeeId: valid; Amount: >0; PaymentDate: not future |
| T9.10 | `ChangePasswordValidator.cs` | `SSIS.BLL/Validators/` | ChangePasswordRequestDto | CurrentPassword: required; NewPassword: min 6 chars; ConfirmPassword: match |
| T9.11 | `UpdateUserValidator.cs` | `SSIS.BLL/Validators/` | UpdateUserRequestDto | FullName: 3-100 chars; Email: valid format; PhoneNumber: valid format (optional) |

---

## Implementation Order

Execute in this order to maintain dependencies:

### Step 1: Phase 4 - Attendance Management
1. Create `AttendanceStatus.cs` enum
2. Create `Attendance.cs` entity
3. Update `User.cs` and `Course.cs` with navigation properties
4. Create `IAttendanceRepository.cs` interface
5. Update `AppDbContext.cs` with DbSet
6. Create `AttendanceRepository.cs`
7. Update `IUnitOfWork.cs` and `UnitOfWork.cs`
8. Update `ServiceExtensions.cs` (DAL)
9. Create DTOs (`AttendanceDto.cs`, `TakeAttendanceDto.cs`, `AttendancePercentageDto.cs`)
10. Create `IAttendanceService.cs` interface
11. Create `AttendanceService.cs` implementation
12. Create `TakeAttendanceValidator.cs`
13. Update `AutoMapperProfile.cs` with mappings
14. Update `ServiceExtentions.cs` (BLL)
15. Create `AttendanceController.cs`
16. Run migration: `Add-Migration AddAttendanceTable`
17. Update database

### Step 2: Phase 7 - Reports & Dashboard
1. Create Dashboard DTOs (`AdminDashboardDto.cs`, `DoctorDashboardDto.cs`, `StudentDashboardDto.cs`)
2. Create Report DTOs (`CourseGradeStatisticsDto.cs`, `AtRiskStudentDto.cs`, `PerformanceAnalyticsDto.cs`)
3. Create `IReportService.cs` interface
4. Create `ReportService.cs` implementation
5. Update `ServiceExtentions.cs` (BLL) with registration
6. Create `DashboardController.cs`
7. Create `ReportsController.cs` (optional)

### Step 3: Phase 8 - Audit Logs
1. Create `AuditLog.cs` entity
2. Update `User.cs` with navigation property
3. Create `IAuditLogRepository.cs` interface
4. Update `AppDbContext.cs` with DbSet
5. Create `AuditLogRepository.cs`
6. Update `IUnitOfWork.cs` and `UnitOfWork.cs`
7. Update `ServiceExtensions.cs` (DAL)
8. Create `AuditLogDto.cs`
9. Create `IAuditLogService.cs` interface
10. Create `AuditLogService.cs` implementation
11. Update `ServiceExtentions.cs` (BLL)
12. Create `AdminController.cs`
13. Run migration: `Add-Migration AddAuditLogs`
14. Update database

### Step 4: Missing Validators
1. Create all validators listed in "Missing Validators" section
2. Register validators in `ServiceExtentions.cs` (BLL) or `Program.cs`

---

## Checklist Summary

### Phase 4: Attendance
- [ ] `AttendanceStatus.cs` enum
- [ ] `Attendance.cs` entity
- [ ] Update `User.cs` (navigation)
- [ ] Update `Course.cs` (navigation)
- [ ] `IAttendanceRepository.cs`
- [ ] Update `AppDbContext.cs`
- [ ] `AttendanceRepository.cs`
- [ ] Update `IUnitOfWork.cs`
- [ ] Update `UnitOfWork.cs`
- [ ] Update `ServiceExtensions.cs` (DAL)
- [ ] `AttendanceDto.cs`, `TakeAttendanceDto.cs`, `AttendancePercentageDto.cs`
- [ ] `IAttendanceService.cs`
- [ ] `AttendanceService.cs`
- [ ] `TakeAttendanceValidator.cs`
- [ ] Update `AutoMapperProfile.cs`
- [ ] Update `ServiceExtentions.cs` (BLL)
- [ ] `AttendanceController.cs`
- [ ] Migration: `Add-Migration AddAttendanceTable`
- [ ] `Update-Database`

### Phase 7: Reports & Dashboard
- [ ] `AdminDashboardDto.cs`
- [ ] `DoctorDashboardDto.cs`
- [ ] `StudentDashboardDto.cs`
- [ ] `PerformanceAnalyticsDto.cs`
- [ ] `CourseGradeStatisticsDto.cs`
- [ ] `AtRiskStudentDto.cs`
- [ ] `IReportService.cs`
- [ ] `ReportService.cs`
- [ ] Update `ServiceExtentions.cs` (BLL)
- [ ] `DashboardController.cs`
- [ ] `ReportsController.cs` (optional)

### Phase 8: Audit Logs
- [ ] `AuditLog.cs` entity
- [ ] Update `User.cs` (navigation)
- [ ] `IAuditLogRepository.cs`
- [ ] Update `AppDbContext.cs`
- [ ] `AuditLogRepository.cs`
- [ ] Update `IUnitOfWork.cs`
- [ ] Update `UnitOfWork.cs`
- [ ] Update `ServiceExtensions.cs` (DAL)
- [ ] `AuditLogDto.cs`
- [ ] `IAuditLogService.cs`
- [ ] `AuditLogService.cs`
- [ ] Update `ServiceExtentions.cs` (BLL)
- [ ] `AdminController.cs`
- [ ] Migration: `Add-Migration AddAuditLogs`
- [ ] `Update-Database`

### Missing Validators
- [ ] `CreateCourseValidator.cs`
- [ ] `UpdateCourseValidator.cs`
- [ ] `CreateEnrollmentValidator.cs`
- [ ] `CreateScheduleValidator.cs`
- [ ] `UpdateScheduleValidator.cs`
- [ ] `EnterGradeValidator.cs`
- [ ] `UpdateGradeValidator.cs`
- [ ] `CreateFeeValidator.cs`
- [ ] `RecordPaymentValidator.cs`
- [ ] `ChangePasswordValidator.cs`
- [ ] `UpdateUserValidator.cs`

---

## Files to Create/Modify Summary

### New Files to Create

**Domain Layer:**
- `SSIS.Domain/Enum/AttendanceStatus.cs`
- `SSIS.Domain/Entities/Attendance.cs`
- `SSIS.Domain/Entities/AuditLog.cs`
- `SSIS.Domain/Interfaces/IAttendanceRepository.cs`
- `SSIS.Domain/Interfaces/IAuditLogRepository.cs`

**DAL Layer:**
- `SSIS.DAL/Repositories/AttendanceRepository.cs`
- `SSIS.DAL/Repositories/AuditLogRepository.cs`

**BLL Layer:**
- `SSIS.BLL/DTOs/Attendance/AttendanceDto.cs`
- `SSIS.BLL/DTOs/Attendance/TakeAttendanceDto.cs`
- `SSIS.BLL/DTOs/Attendance/AttendancePercentageDto.cs`
- `SSIS.BLL/DTOs/Dashboard/AdminDashboardDto.cs`
- `SSIS.BLL/DTOs/Dashboard/DoctorDashboardDto.cs`
- `SSIS.BLL/DTOs/Dashboard/StudentDashboardDto.cs`
- `SSIS.BLL/DTOs/Dashboard/PerformanceAnalyticsDto.cs`
- `SSIS.BLL/DTOs/Reports/CourseGradeStatisticsDto.cs`
- `SSIS.BLL/DTOs/Reports/AtRiskStudentDto.cs`
- `SSIS.BLL/DTOs/Admin/AuditLogDto.cs`
- `SSIS.BLL/Interfaces/IAttendanceService.cs`
- `SSIS.BLL/Interfaces/IReportService.cs`
- `SSIS.BLL/Interfaces/IAuditLogService.cs`
- `SSIS.BLL/Services/Implementation/AttendanceService.cs`
- `SSIS.BLL/Services/Implementation/ReportService.cs`
- `SSIS.BLL/Services/Implementation/AuditLogService.cs`
- `SSIS.BLL/Validators/CreateCourseValidator.cs`
- `SSIS.BLL/Validators/UpdateCourseValidator.cs`
- `SSIS.BLL/Validators/CreateEnrollmentValidator.cs`
- `SSIS.BLL/Validators/CreateScheduleValidator.cs`
- `SSIS.BLL/Validators/UpdateScheduleValidator.cs`
- `SSIS.BLL/Validators/EnterGradeValidator.cs`
- `SSIS.BLL/Validators/UpdateGradeValidator.cs`
- `SSIS.BLL/Validators/CreateFeeValidator.cs`
- `SSIS.BLL/Validators/RecordPaymentValidator.cs`
- `SSIS.BLL/Validators/ChangePasswordValidator.cs`
- `SSIS.BLL/Validators/UpdateUserValidator.cs`
- `SSIS.BLL/Mapper/AttendanceMappingProfile.cs` (or update existing)

**API Layer:**
- `SSIS.PL/Controllers/AttendanceController.cs`
- `SSIS.PL/Controllers/DashboardController.cs`
- `SSIS.PL/Controllers/ReportsController.cs`
- `SSIS.PL/Controllers/AdminController.cs`

### Files to Modify

**Domain Layer:**
- `SSIS.Domain/Entities/User.cs` - Add navigation properties
- `SSIS.Domain/Entities/Course.cs` - Add navigation property

**DAL Layer:**
- `SSIS.DAL/Data/AppDbContext.cs` - Add DbSets
- `SSIS.DAL/UnitOfWorks/IUnitOfWork.cs` - Add repository properties
- `SSIS.DAL/UnitOfWorks/UnitOfWork.cs` - Implement properties
- `SSIS.DAL/Extensions/ServiceExtensions.cs` - Register repositories

**BLL Layer:**
- `SSIS.BLL/Extentions/ServiceExtentions.cs` - Register services
- `SSIS.BLL/Mapper/AutoMapperProfile.cs` - Add mappings

### Migrations
- `Add-Migration AddAttendanceTable`
- `Add-Migration AddAuditLogs`
- `Update-Database` (run after each migration)
