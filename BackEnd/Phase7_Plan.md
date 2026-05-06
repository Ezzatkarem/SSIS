# Phase 7: Reports & Analytics — Implementation Plan

**Project:** Smart Student Management System (SSIS)  
**Phase:** 7 - Reports & Analytics  
**Duration:** 2–3 days  
**Dependencies:** Phases 1–6 must be completed

---

## 📋 Table of Contents

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Task Breakdown](#task-breakdown)
- [Authorization Matrix](#authorization-matrix)
- [API Endpoints](#api-endpoints)
- [Checklist Summary](#checklist-summary)
- [Summary Table](#summary-table)

---

## Overview

### Goal

Implement comprehensive reporting and analytics including:

- **Admin Dashboard** — System-wide statistics
- **Doctor Dashboard** — Course performance metrics
- **Student Dashboard** — Personal academic summary
- **Top Students** — Ranking by GPA
- **Course Statistics** — Detailed course analytics

### No New Entities

Reports are read-only queries based on existing data:  
`Users`, `Courses`, `Enrollments`, `Grades`, `Attendances`, `Fees`, `Payments`

---

## Prerequisites

- ✅ Phases 1–6 must be functional
- ✅ GPA data stored in `User.CumulativeGpa` and `Enrollment.SemesterGpa`
- ✅ All data must be populated

---

## Task Breakdown

### 7.1 Domain Layer (`SSIS.Domain`)

#### 7.1.1 Repository Interface

| Task | File | Path | Methods |
|------|------|------|---------|
| T7.1 | `IReportRepository.cs` | `SSIS.Domain/Interfaces/` | `GetTotalStudentsAsync`, `GetTotalDoctorsAsync`, `GetTotalCoursesAsync`, `GetTotalEnrollmentsAsync`, `GetTotalFeesCollectedAsync`, `GetTotalFeesPendingAsync`, `GetOverdueFeesCountAsync`, `GetAverageAttendanceAsync`, `GetTopStudentsAsync`, `GetDoctorCourseStatisticsAsync`, `GetStudentDashboardDataAsync` |

---

### 7.2 DAL Layer (`SSIS.DAL`)

#### 7.2.1 Repository Implementation

| Task | File | Path | Description |
|------|------|------|-------------|
| T7.2 | `ReportRepository.cs` | `SSIS.DAL/Repositories/` | Implement `IReportRepository` |

#### 7.2.2 Unit of Work Updates

| Task | File | Updates |
|------|------|---------|
| T7.3 | `IUnitOfWork.cs` | Add: `IReportRepository Reports` |
| T7.4 | `UnitOfWork.cs` | Implement new repository property |

---

### 7.3 BLL Layer (`SSIS.BLL`)

#### 7.3.1 DTOs — Reports

| Task | File | Path | Properties |
|------|------|------|------------|
| T7.5 | `AdminDashboardDto.cs` | `SSIS.BLL/DTOs/Reports/` | `TotalStudents`, `TotalDoctors`, `TotalCourses`, `TotalEnrollments`, `TotalFeesCollected`, `TotalFeesPending`, `OverdueFeesCount`, `AverageGpa`, `AverageAttendance`, `TopStudents`, `UnreadNotifications` |
| T7.6 | `DoctorDashboardDto.cs` | `SSIS.BLL/DTOs/Reports/` | `Courses`, `TotalStudents`, `AverageGrade`, `AverageAttendance`, `UnreadNotifications` |
| T7.7 | `StudentDashboardDto.cs` | `SSIS.BLL/DTOs/Reports/` | `CurrentGpa`, `EnrolledCoursesCount`, `OverallAttendancePercentage`, `UnreadNotifications`, `UpcomingFees` |
| T7.8 | `TopStudentDto.cs` | `SSIS.BLL/DTOs/Reports/` | `StudentId`, `StudentName`, `Gpa`, `TotalCredits` |
| T7.9 | `CourseStatisticsDto.cs` | `SSIS.BLL/DTOs/Reports/` | `CourseId`, `CourseName`, `CourseCode`, `EnrolledStudentsCount`, `AverageGrade`, `HighestGrade`, `LowestGrade`, `AttendancePercentage` |
| T7.10 | `UpcomingFeeDto.cs` | `SSIS.BLL/DTOs/Reports/` | `FeeId`, `Amount`, `DueDate`, `Semester`, `AcademicYear` |

#### 7.3.2 Service Interface

| Task | File | Path | Methods |
|------|------|------|---------|
| T7.11 | `IReportService.cs` | `SSIS.BLL/Interfaces/` | `GetAdminDashboardAsync`, `GetDoctorDashboardAsync`, `GetStudentDashboardAsync`, `GetTopStudentsAsync`, `GetCourseStatisticsAsync` |

#### 7.3.3 Service Implementation

| Task | File | Path | Description |
|------|------|------|-------------|
| T7.12 | `ReportService.cs` | `SSIS.BLL/Services/` | Business logic for reports |

#### 7.3.4 Mapping Profiles

| Task | File | Path | Mappings |
|------|------|------|----------|
| T7.13 | `ReportMappingProfile.cs` | `SSIS.BLL/Mappings/` | Map raw data to DTOs |

---

### 7.4 API Layer (`SSIS.PL`)

#### 7.4.1 Controller

| Task | File | Path | Endpoints |
|------|------|------|-----------|
| T7.14 | `ReportsController.cs` | `SSIS.PL/Controllers/v1/` | See [API Endpoints](#api-endpoints) |

### 7.5 No Database Migration Required

> No new tables are needed for Phase 7.

---

## Authorization Matrix

| Operation | Admin | Doctor | Student | Notes |
|-----------|:-----:|:------:|:-------:|-------|
| Admin Dashboard | ✅ | ❌ | ❌ | System-wide stats |
| Doctor Dashboard | ✅ | ✅ | ❌ | Doctor sees their courses |
| Student Dashboard | ✅ | ❌ | ✅ | Student sees their data |
| Top Students | ✅ | ❌ | ❌ | |
| Course Statistics | ✅ | ✅ (own) | ❌ | |

---

## API Endpoints

### Reports Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| `GET` | `/api/v1/reports/admin-dashboard` | Admin dashboard stats | Admin |
| `GET` | `/api/v1/reports/doctor-dashboard` | Doctor dashboard stats | Doctor |
| `GET` | `/api/v1/reports/student-dashboard` | Student dashboard stats | Student |
| `GET` | `/api/v1/reports/top-students` | Top students by GPA | Admin |
| `GET` | `/api/v1/reports/course-statistics/{courseId}` | Course statistics | Doctor (own), Admin |

---

## Dashboard Data Sources

### Admin Dashboard

| Field | Data Source |
|-------|-------------|
| `TotalStudents` | Users with `Role=Student` |
| `TotalDoctors` | Users with `Role=Doctor` |
| `TotalCourses` | Courses (`IsDeleted=false`) |
| `TotalEnrollments` | Enrollments |
| `TotalFeesCollected` | Payments (`Status=Completed`) |
| `TotalFeesPending` | Fees (`TotalAmount - PaidAmount`) |
| `OverdueFeesCount` | Fees where `DueDate < Now` and not Paid |
| `AverageAttendance` | Attendances (`Present/Total`) |
| `TopStudents` | `Users.CumulativeGpa` |

### Doctor Dashboard

| Field | Data Source |
|-------|-------------|
| `Courses` | Courses where `DoctorId = current user` |
| `EnrolledStudentsCount` | Enrollments per course |
| `AverageGrade` | Grades per course |
| `AttendancePercentage` | Attendances per course |

### Student Dashboard

| Field | Data Source |
|-------|-------------|
| `CurrentGpa` | `User.CumulativeGpa` |
| `EnrolledCoursesCount` | Enrollments where `IsActive=true` |
| `OverallAttendancePercentage` | Attendances (`Present/Total`) |
| `UnreadNotifications` | Notifications where `IsRead=false` |
| `UpcomingFees` | Fees where Not Paid and `DueDate > Now` |

---

## Checklist Summary

### Domain Layer
- [ ] `IReportRepository` interface

### DAL Layer
- [ ] `ReportRepository` implementation
- [ ] Update `IUnitOfWork`
- [ ] Update `UnitOfWork`

### BLL Layer
- [ ] Report DTOs (`AdminDashboardDto`, `DoctorDashboardDto`, `StudentDashboardDto`, `TopStudentDto`, `CourseStatisticsDto`, `UpcomingFeeDto`)
- [ ] `IReportService` interface
- [ ] `ReportService` implementation
- [ ] `ReportMappingProfile`

### API Layer
- [ ] `ReportsController`

### Database
- [ ] No migration needed (uses existing tables)

---

## Summary Table

| Phase | Duration | Tasks | New Tables | New Enums |
|-------|----------|-------|------------|-----------|
| Phase 4 | 2 days | 22 | `Attendance` | `AttendanceStatus` |
| Phase 5 | 2–3 days | 37 | `Fee`, `Payment` | `FeeStatus`, `PaymentStatus` |
| Phase 6 | 1–2 days | 19 | `Notification` | `NotificationType` |
| Phase 7 | 2–3 days | 14 | None (reports only) | None |