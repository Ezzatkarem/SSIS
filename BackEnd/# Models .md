# ✅ Models Structure with Attributes (محدث بعد كل التعديلات)

---

## Common/

| Class | Properties |
|-------|------------|
| **BaseEntity** | Id (PK), CreatedAt, UpdatedAt, CreatedBy, UpdatedBy |
| **ISoftDelete** | IsDeleted, DeletedAt, DeletedBy (Interface) |

---

## Enums/

| Enum | Values |
|------|--------|
| **UserRole** | Admin (1), Doctor (2), Student (3) |
| **GradeLetter** | A (1), B (2), C (3), D (4), F (5) |
| **AttendanceStatus** | Present (1), Absent (2), Late (3), Excused (4) |
| **FeeStatus** | Unpaid (1), Partial (2), Paid (3), Overdue (4) |
| **PaymentStatus** | Pending (1), Completed (2), Failed (3), Refunded (4) |
| **DayOfWeekEnum** | Sunday (1), Monday (2), Tuesday (3), Wednesday (4), Thursday (5) |
| **NotificationType** | FeeCreated(1), FeeReminder(2), PaymentSuccess(3), PaymentFailed(4), FeeOverdue(5), UserRegistered(10), ProfileUpdated(11), UserDeleted(12), WelcomeMessage(13), CourseCreated(20), CourseUpdated(21), CourseDeleted(22), EnrollmentCreated(30), EnrollmentRemoved(31), GradeEntered(40), GradeUpdated(41), AttendanceRecorded(50), AdminBroadcast(100), DoctorBroadcast(101) |

---

## Entities/

| Entity | Properties | Relationships |
|--------|------------|---------------|
| **User** | Id, FullName, Email, PasswordHash, Role, PhoneNumber, ProfilePicture, IsActive, IsDeleted, DeletedAt, DeletedBy, CumulativeGpa, TotalCompletedCredits, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy | → Enrollments, → TaughtCourses, → Grades, → Attendances, → Fees, → Notifications, → Payments |
| **Course** | Id, Name, Code, Credits, Description, DoctorId, Semester, AcademicYear, IsActive, IsDeleted, CreatedAt, UpdatedAt | → Doctor (User), → Enrollments, → Grades, → Attendances, → Schedules, → Prerequisites, → RequiredFor |
| **CoursePrerequisite** | Id, CourseId, PrerequisiteCourseId, CreatedAt | → Course (main), → PrerequisiteCourse |
| **Enrollment** | Id, StudentId, CourseId, EnrollmentDate, IsActive, Semester, AcademicYear, SemesterGpa, TotalCredits, CreatedAt | → Student (User), → Course |
| **Grade** | Id, StudentId, CourseId, Score, GradeLetter, GradePoints, Credits, Semester, AcademicYear, Remarks, CreatedAt, UpdatedAt | → Student (User), → Course |
| **Attendance** | Id, StudentId, CourseId, Date, Status, Remarks, CreatedAt | → Student (User), → Course |
| **Schedule** | Id, CourseId, DayOfWeek, StartTime, EndTime, Room, CreatedAt | → Course |
| **Fee** | Id, StudentId, Semester, AcademicYear, TotalAmount, PaidAmount, DueDate, Status, CreatedAt, UpdatedAt | → Student (User), → Payments |
| **Payment** | Id, FeeId, StudentId, Amount, PaymentDate, PaymentMethod, TransactionId, Status, PaymobOrderId, ReceiptUrl, CreatedAt | → Fee, → Student (User) |
| **Notification** | Id, UserId, Title, Message, Type, IsRead, ReadAt, CreatedAt | → User |
| **AuditLog** | Id, UserId, Action, Entity, EntityId, OldValues, NewValues, Timestamp, IpAddress, UserAgent, CreatedAt | → User (optional) |

---

## Constraints Summary

| Entity | Unique Constraints | Required Fields |
|--------|-------------------|-----------------|
| **User** | Email | FullName, Email, PasswordHash, Role |
| **Course** | Code | Name, Code, Credits |
| **CoursePrerequisite** | CourseId + PrerequisiteCourseId | CourseId, PrerequisiteCourseId |
| **Enrollment** | StudentId + CourseId + Semester + AcademicYear | StudentId, CourseId, Semester, AcademicYear |
| **Grade** | StudentId + CourseId + Semester + AcademicYear | StudentId, CourseId, Score, Semester, AcademicYear |
| **Attendance** | StudentId + CourseId + Date | StudentId, CourseId, Date, Status |
| **Schedule** | - | CourseId, DayOfWeek, StartTime, EndTime |
| **Fee** | - | StudentId, Semester, AcademicYear, TotalAmount, DueDate |
| **Payment** | - | FeeId, StudentId, Amount, PaymentDate, PaymentMethod, Status |
| **Notification** | - | UserId, Title, Message, Type |
| **AuditLog** | - | Action, Entity, EntityId, Timestamp |

---

## Summary of Changes from Previous Version

| Addition | Description |
|----------|-------------|
| **User** | + `CumulativeGpa`, `TotalCompletedCredits` |
| **User Relationships** | + `→ Payments` |
| **Course** | + `→ Prerequisites`, `→ RequiredFor` |
| **CoursePrerequisite** | New entity for many-to-many self relationship |
| **Enrollment** | + `Semester`, `AcademicYear`, `SemesterGpa`, `TotalCredits` |
| **Grade** | + `GradePoints`, `Credits` |
| **FeeStatus** | Changed from `Pending` to `Unpaid` |
| **Payment** | + `StudentId`, `PaymobOrderId`, `ReceiptUrl` |
| **PaymentStatus** | New enum |
| **NotificationType** | New enum with all types |
| **AuditLog** | New entity for system auditing |

---

**This document reflects the current state of the SSIS data model after all phases (1-7).** 😊