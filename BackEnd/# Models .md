# Models Structure with Attributes (بدون كود)

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
| **FeeStatus** | Pending (1), Partial (2), Paid (3), Overdue (4) |
| **DayOfWeekEnum** | Sunday (1), Monday (2), Tuesday (3), Wednesday (4), Thursday (5) |

---

## Entities/

| Entity | Properties | Relationships |
|--------|------------|---------------|
| **User** | Id, FullName, Email, PasswordHash, Role, PhoneNumber, ProfilePicture, IsActive, IsDeleted, DeletedAt, DeletedBy, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy | → Enrollments, → TaughtCourses, → Grades, → Attendances, → Fees, → Notifications |
| **Course** | Id, Name, Code, Credits, Description, DoctorId, Semester, AcademicYear, IsActive, CreatedAt, UpdatedAt | → Doctor (User), → Enrollments, → Grades, → Attendances, → Schedules |
| **Enrollment** | Id, StudentId, CourseId, EnrollmentDate, IsActive, CreatedAt | → Student (User), → Course |
| **Grade** | Id, StudentId, CourseId, Score, GradeLetter, Semester, AcademicYear, Remarks, CreatedAt, UpdatedAt | → Student (User), → Course |
| **Attendance** | Id, StudentId, CourseId, Date, Status, Remarks, CreatedAt | → Student (User), → Course |
| **Schedule** | Id, CourseId, DayOfWeek, StartTime, EndTime, Room, CreatedAt | → Course |
| **Fee** | Id, StudentId, Semester, AcademicYear, TotalAmount, PaidAmount, DueDate, Status, CreatedAt, UpdatedAt | → Student (User), → Payments |
| **Payment** | Id, FeeId, Amount, PaymentDate, PaymentMethod, TransactionId, ReceiptNumber, CreatedAt | → Fee |
| **Notification** | Id, UserId, Title, Message, IsRead, ReadAt, Type, CreatedAt | → User |
| **AuditLog** | Id, UserId, Action, Entity, EntityId, OldValues, NewValues, Timestamp, IpAddress, UserAgent | → User (optional) |

---

## Constraints Summary

| Entity | Unique Constraints | Required Fields |
|--------|-------------------|-----------------|
| **User** | Email | FullName, Email, PasswordHash, Role |
| **Course** | Code | Name, Code, Credits |
| **Enrollment** | StudentId + CourseId | StudentId, CourseId |
| **Grade** | StudentId + CourseId + Semester | StudentId, CourseId, Score |
| **Attendance** | - | StudentId, CourseId, Date, Status |
| **Schedule** | - | CourseId, DayOfWeek, StartTime, EndTime |
| **Fee** | - | StudentId, Semester, TotalAmount, DueDate |
| **Payment** | - | FeeId, Amount, PaymentDate |
| **Notification** | - | UserId, Title, Message |