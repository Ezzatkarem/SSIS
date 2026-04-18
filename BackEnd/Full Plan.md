# ✅ Smart Student Management System - Full Plan (All Phases)

---

## Phase 0: Setup & Infrastructure ✅ (Completed)

| Task | Status |
|------|--------|
| Create Solution & Projects | ✅ |
| Domain Layer (BaseEntity, ISoftDelete, User, UserRole) | ✅ |
| DAL Layer (AppDbContext, ApplicationUser, GenericRepository, UserRepository, UnitOfWork) | ✅ |
| BLL Layer (IJwtService, JwtService) | ✅ |
| API Layer (Swagger, JWT, Program.cs, appsettings.json) | ✅ |
| Build & Swagger Working | ✅ |

---

## Phase 1: Authentication & User Management

### 1.1 DTOs (BLL)
| Task | Description |
|------|-------------|
| T1.1 | Create `LoginRequestDto.cs` (Email, Password) |
| T1.2 | Create `LoginResponseDto.cs` (Token, FullName, Email, Role) |
| T1.3 | Create `RegisterRequestDto.cs` (FullName, Email, Password, Role) |
| T1.4 | Create `UserResponseDto.cs` (Id, FullName, Email, Role) |
| T1.5 | Create `ChangePasswordRequestDto.cs` (CurrentPassword, NewPassword) |

### 1.2 Validators (BLL)
| Task | Description |
|------|-------------|
| T1.6 | Create `LoginRequestValidator.cs` (FluentValidation) |
| T1.7 | Create `RegisterRequestValidator.cs` (FluentValidation) |
| T1.8 | Create `ChangePasswordValidator.cs` (FluentValidation) |

### 1.3 Services (BLL)
| Task | Description |
|------|-------------|
| T1.9 | Create `IUserService.cs` interface |
| T1.10 | Create `UserService.cs` (Login, Register, GetById, Update, Delete) |

### 1.4 Controllers (API)
| Task | Description |
|------|-------------|
| T1.11 | Create `AuthController.cs` (Login, Register endpoints) |
| T1.12 | Create `UsersController.cs` (GetAll, GetById, Update, Delete, ChangePassword) |

### 1.5 Migration
| Task | Description |
|------|-------------|
| T1.13 | Add-Migration InitialCreate |
| T1.14 | Update-Database |
| T1.15 | Seed Admin User (admin@system.com / Admin@123) |

---

## Phase 2: Academic Management

### 2.1 Domain Layer
| Task | Description |
|------|-------------|
| T2.1 | Create `Course.cs` (Id, Name, Code, Credits, Description, DoctorId, Semester, AcademicYear, IsActive) |
| T2.2 | Create `Enrollment.cs` (Id, StudentId, CourseId, EnrollmentDate, IsActive) |
| T2.3 | Create `Schedule.cs` (Id, CourseId, DayOfWeek, StartTime, EndTime, Room) |
| T2.4 | Add DayOfWeekEnum (Sunday-Thursday) |

### 2.2 DAL Layer
| Task | Description |
|------|-------------|
| T2.5 | Add DbSet<Course>, DbSet<Enrollment>, DbSet<Schedule> to AppDbContext |
| T2.6 | Create `ICourseRepository.cs` (interface) |
| T2.7 | Create `CourseRepository.cs` (implementation) |
| T2.8 | Create `IEnrollmentRepository.cs` (interface) |
| T2.9 | Create `EnrollmentRepository.cs` (implementation) |
| T2.10 | Update UnitOfWork with new repositories |

### 2.3 BLL Layer
| Task | Description |
|------|-------------|
| T2.11 | Create `CourseDto.cs`, `CreateCourseDto.cs`, `UpdateCourseDto.cs` |
| T2.12 | Create `EnrollmentDto.cs` |
| T2.13 | Create `ScheduleDto.cs` |
| T2.14 | Create `ICourseService.cs` interface |
| T2.15 | Create `CourseService.cs` (Create, Update, Delete, GetById, GetAll, AssignDoctor) |
| T2.16 | Create `IEnrollmentService.cs` interface |
| T2.17 | Create `EnrollmentService.cs` (Enroll, Unenroll, GetStudentCourses, GetCourseStudents) |
| T2.18 | Create Validators (CourseValidator, EnrollmentValidator) |

### 2.4 API Layer
| Task | Description |
|------|-------------|
| T2.19 | Create `CoursesController.cs` (CRUD endpoints) |
| T2.20 | Create `EnrollmentsController.cs` (Enroll/Unenroll endpoints) |
| T2.21 | Create `SchedulesController.cs` (Schedule management) |

### 2.5 Migration
| Task | Description |
|------|-------------|
| T2.22 | Add-Migration AddAcademicTables |
| T2.23 | Update-Database |

---

## Phase 3: Grade Management

### 3.1 Domain Layer
| Task | Description |
|------|-------------|
| T3.1 | Create `Grade.cs` (Id, StudentId, CourseId, Score, GradeLetter,    , AcademicYear, Remarks) |
| T3.2 | Add GradeLetter Enum (A, B, C, D, F) |

### 3.2 DAL Layer
| Task | Description |
|------|-------------|
| T3.3 | Add DbSet<Grade> to AppDbContext |
| T3.4 | Create `IGradeRepository.cs` interface |
| T3.5 | Create `GradeRepository.cs` (implementation) |
| T3.6 | Update UnitOfWork |

### 3.3 BLL Layer
| Task | Description |
|------|-------------|
| T3.7 | Create `GradeDto.cs`, `EnterGradeDto.cs`, `UpdateGradeDto.cs` |
| T3.8 | Create `GpaResponseDto.cs` |
| T3.9 | Create `IGradeService.cs` interface |
| T3.10 | Create `GradeService.cs` (EnterGrade, UpdateGrade, GetStudentGrades, GetCourseGrades, CalculateGPA) |
| T3.11 | Create `GpaCalculator.cs` helper class |
| T3.12 | Create Validators (GradeEntryValidator) |

### 3.4 API Layer
| Task | Description |
|------|-------------|
| T3.13 | Create `GradesController.cs` (Enter, Update, GetStudentGrades, GetCourseGrades, GetGPA) |

### 3.5 Migration
| Task | Description |
|------|-------------|
| T3.14 | Add-Migration AddGradesTable |
| T3.15 | Update-Database |

---

## Phase 4: Attendance Management

### 4.1 Domain Layer
| Task | Description |
|------|-------------|
| T4.1 | Create `Attendance.cs` (Id, StudentId, CourseId, Date, Status, Remarks) |
| T4.2 | Add AttendanceStatus Enum (Present, Absent, Late, Excused) |

### 4.2 DAL Layer
| Task | Description |
|------|-------------|
| T4.3 | Add DbSet<Attendance> to AppDbContext |
| T4.4 | Create `IAttendanceRepository.cs` interface |
| T4.5 | Create `AttendanceRepository.cs` (implementation) |

### 4.3 BLL Layer
| Task | Description |
|------|-------------|
| T4.6 | Create `AttendanceDto.cs`, `TakeAttendanceDto.cs` |
| T4.7 | Create `AttendancePercentageDto.cs` |
| T4.8 | Create `IAttendanceService.cs` interface |
| T4.9 | Create `AttendanceService.cs` (TakeAttendance, GetStudentAttendance, GetCourseAttendance, CalculatePercentage) |
| T4.10 | Create Validators (AttendanceValidator) |

### 4.4 API Layer
| Task | Description |
|------|-------------|
| T4.11 | Create `AttendanceController.cs` (TakeAttendance, GetStudentAttendance, GetCourseAttendance) |

### 4.5 Migration
| Task | Description |
|------|-------------|
| T4.12 | Add-Migration AddAttendanceTable |
| T4.13 | Update-Database |

---

## Phase 5: Fee Management

### 5.1 Domain Layer
| Task | Description |
|------|-------------|
| T5.1 | Create `Fee.cs` (Id, StudentId, Semester, AcademicYear, TotalAmount, PaidAmount, DueDate, Status) |
| T5.2 | Create `Payment.cs` (Id, FeeId, Amount, PaymentDate, PaymentMethod, TransactionId, ReceiptNumber) |
| T5.3 | Add FeeStatus Enum (Pending, Partial, Paid, Overdue) |

### 5.2 DAL Layer
| Task | Description |
|------|-------------|
| T5.4 | Add DbSet<Fee>, DbSet<Payment> to AppDbContext |
| T5.5 | Create `IFeeRepository.cs` interface |
| T5.6 | Create `FeeRepository.cs` (implementation) |
| T5.7 | Create `IPaymentRepository.cs` interface |
| T5.8 | Create `PaymentRepository.cs` (implementation) |

### 5.3 BLL Layer
| Task | Description |
|------|-------------|
| T5.9 | Create `FeeDto.cs`, `SetupFeeDto.cs`, `PaymentDto.cs`, `RecordPaymentDto.cs` |
| T5.10 | Create `IFeeService.cs` interface |
| T5.11 | Create `FeeService.cs` (SetupFees, RecordPayment, GetStudentFees, GetPaymentStatus) |
| T5.12 | Create Validators (FeeSetupValidator, PaymentValidator) |

### 5.4 API Layer
| Task | Description |
|------|-------------|
| T5.13 | Create `FeesController.cs` (SetupFees, RecordPayment, GetStudentFees) |

### 5.5 Migration
| Task | Description |
|------|-------------|
| T5.14 | Add-Migration AddFeesAndPayments |
| T5.15 | Update-Database |

---

## Phase 6: Notification Management

### 6.1 Domain Layer
| Task | Description |
|------|-------------|
| T6.1 | Create `Notification.cs` (Id, UserId, Title, Message, IsRead, ReadAt, Type, CreatedAt) |

### 6.2 DAL Layer
| Task | Description |
|------|-------------|
| T6.2 | Add DbSet<Notification> to AppDbContext |
| T6.3 | Create `INotificationRepository.cs` interface |
| T6.4 | Create `NotificationRepository.cs` (implementation) |

### 6.3 BLL Layer
| Task | Description |
|------|-------------|
| T6.5 | Create `NotificationDto.cs`, `SendNotificationDto.cs` |
| T6.6 | Create `INotificationService.cs` interface |
| T6.7 | Create `NotificationService.cs` (Send, GetUserNotifications, MarkAsRead, GetUnreadCount) |

### 6.4 API Layer
| Task | Description |
|------|-------------|
| T6.8 | Create `NotificationsController.cs` (Send, Get, MarkAsRead, UnreadCount) |

### 6.5 Migration
| Task | Description |
|------|-------------|
| T6.9 | Add-Migration AddNotificationsTable |
| T6.10 | Update-Database |

---

## Phase 7: Reports & Analytics (Smart Features)

### 7.1 BLL Layer
| Task | Description |
|------|-------------|
| T7.1 | Create `AdminDashboardDto.cs` (TotalStudents, TotalDoctors, TotalCourses, AverageAttendance, AverageGrade, TopStudents) |
| T7.2 | Create `DoctorDashboardDto.cs` (EnrolledStudents, AttendancePercentage, AverageGrades) |
| T7.3 | Create `StudentDashboardDto.cs` (GPA, CoursesCount, AttendancePercentage, UnreadNotifications) |
| T7.4 | Create `PerformanceAnalyticsDto.cs` (GradeTrends, AttendanceTrends) |
| T7.5 | Create `IReportService.cs` interface |
| T7.6 | Create `ReportService.cs` (GetAdminDashboard, GetDoctorDashboard, GetStudentDashboard, GetStudentAnalytics, GetAtRiskStudents) |

### 7.2 API Layer
| Task | Description |
|------|-------------|
| T7.7 | Create `ReportsController.cs` (AdminDashboard, DoctorDashboard, StudentDashboard, StudentAnalytics, AtRiskStudents) |
| T7.8 | Create `ExportController.cs` (Export to PDF/Excel) |

### 7.3 Smart Alerts (Background Service)
| Task | Description |
|------|-------------|
| T7.9 | Create `SmartAlertService.cs` (Check Low Grades, Check Excessive Absences, Check Overdue Fees) |
| T7.10 | Add Background Service for automated alerts |

---

## Phase 8: System Administration

### 8.1 DAL Layer
| Task | Description |
|------|-------------|
| T8.1 | Create `AuditLog.cs` (Id, UserId, Action, Entity, EntityId, OldValues, NewValues, Timestamp, IpAddress) |
| T8.2 | Add DbSet<AuditLog> to AppDbContext |
| T8.3 | Create `IAuditLogRepository.cs` interface |
| T8.4 | Create `AuditLogRepository.cs` (implementation) |

### 8.2 BLL Layer
| Task | Description |
|------|-------------|
| T8.5 | Create `AuditLogDto.cs` |
| T8.6 | Create `SystemSettingsDto.cs` |
| T8.7 | Create `IAuditLogService.cs` interface |
| T8.8 | Create `AuditLogService.cs` (GetAuditLogs) |
| T8.9 | Create `ISystemService.cs` interface |
| T8.10 | Create `SystemService.cs` (GetSettings, UpdateSettings, ClearCache) |

### 8.3 API Layer
| Task | Description |
|------|-------------|
| T8.11 | Create `AdminController.cs` (GetAuditLogs, GetSettings, UpdateSettings, ClearCache) |

### 8.4 Migration
| Task | Description |
|------|-------------|
| T8.12 | Add-Migration AddAuditLogs |
| T8.13 | Update-Database |

---

## Phase 9: Testing & Deployment

### 9.1 Testing
| Task | Description |
|------|-------------|
| T9.1 | Create Unit Tests for Services (xUnit, Moq) |
| T9.2 | Test UserService (Register, Login, GetById) |
| T9.3 | Test GradeService (CalculateGPA) |
| T9.4 | Test AttendanceService (CalculatePercentage) |
| T9.5 | Integration Tests for API endpoints |

### 9.2 Deployment
| Task | Description |
|------|-------------|
| T9.6 | Configure Production appsettings.json |
| T9.7 | Run migrations on production database |
| T9.8 | Publish to IIS / Azure |
| T9.9 | Configure CORS for frontend |
| T9.10 | Final testing and documentation |

---

## Timeline Summary

| Phase | Duration | Tasks |
|-------|----------|-------|
| Phase 0 | Done | 25 |
| Phase 1 | 3-4 days | 15 |
| Phase 2 | 3-4 days | 23 |
| Phase 3 | 2-3 days | 15 |
| Phase 4 | 2 days | 13 |
| Phase 5 | 2-3 days | 15 |
| Phase 6 | 1-2 days | 10 |
| Phase 7 | 2-3 days | 10 |
| Phase 8 | 1-2 days | 13 |
| Phase 9 | 2-3 days | 10 |
| **Total** | **~20-25 days** | **~149 tasks** |

---

## نبدأ Phase 1 (Auth Controller) دلوقتي؟ 😊