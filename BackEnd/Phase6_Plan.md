# Phase 6: Notifications Management — Implementation Plan

**Project:** Smart Student Management System (SSIS)  
**Phase:** 6 - Notifications Management  
**Duration:** 1–2 days  
**Dependencies:** Phase 1 (Users) must be completed first

---

## 📋 Table of Contents

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Entity Relationship Diagram](#entity-relationship-diagram)
- [Task Breakdown](#task-breakdown)
- [Authorization Matrix](#authorization-matrix)
- [API Endpoints](#api-endpoints)
- [Database Schema](#database-schema)
- [Checklist Summary](#checklist-summary)

---

## Overview

### Goal

Implement complete notification system including:

- **System Notifications** — Auto-generated notifications for events
- **Admin Broadcast** — Admin can send notifications to specific groups
- **Doctor Broadcast** — Doctor can send notifications to their students
- **User Management** — View, mark as read, delete notifications

### New Entities

| Entity | Description |
|--------|-------------|
| **Notification** | User notifications |

### New Enums

| Enum | Values |
|------|--------|
| **NotificationType** | `FeeCreated(1)`, `FeeReminder(2)`, `PaymentSuccess(3)`, `PaymentFailed(4)`, `FeeOverdue(5)`, `UserRegistered(10)`, `ProfileUpdated(11)`, `UserDeleted(12)`, `WelcomeMessage(13)`, `CourseCreated(20)`, `CourseUpdated(21)`, `CourseDeleted(22)`, `EnrollmentCreated(30)`, `EnrollmentRemoved(31)`, `GradeEntered(40)`, `GradeUpdated(41)`, `AttendanceRecorded(50)`, `AdminBroadcast(100)`, `DoctorBroadcast(101)` |

---

## Prerequisites

- ✅ Phase 1 (Users, Authentication) must be functional
- ✅ User roles (Admin, Doctor, Student) must exist

---

## Entity Relationship Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         ENTITY RELATIONSHIPS                            │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────┐       ┌─────────────────┐
│    User     │1      │   Notification  │
│             │───────│                 │
│ PK: Id      │       │ PK: Id          │
│ FullName    │       │ UserId (FK)     │
│ Email       │       │ Title           │
└─────────────┘       │ Message         │
                      │ Type            │
                      │ IsRead          │
                      │ ReadAt          │
                      │ CreatedAt       │
                      └─────────────────┘
```

---

## Task Breakdown

### 6.1 Domain Layer (`SSIS.Domain`)

#### 6.1.1 Enums

| Task | File | Path | Content |
|------|------|------|---------|
| T6.1 | `NotificationType.cs` | `SSIS.Domain/Enums/` | FeeCreated=1, FeeReminder=2, PaymentSuccess=3, PaymentFailed=4, FeeOverdue=5, UserRegistered=10, ProfileUpdated=11, UserDeleted=12, WelcomeMessage=13, CourseCreated=20, CourseUpdated=21, CourseDeleted=22, EnrollmentCreated=30, EnrollmentRemoved=31, GradeEntered=40, GradeUpdated=41, AttendanceRecorded=50, AdminBroadcast=100, DoctorBroadcast=101 |

#### 6.1.2 Entities

| Task | File | Path | Properties |
|------|------|------|------------|
| T6.2 | `Notification.cs` | `SSIS.Domain/Entities/` | `Id`, `UserId`, `Title`, `Message`, `Type`, `IsRead`, `ReadAt`, `CreatedAt` |
| T6.3 | Update `User.cs` | `SSIS.Domain/Entities/` | Add navigation: `ICollection<Notification> Notifications` |

#### 6.1.3 Repository Interfaces

| Task | File | Path | Methods |
|------|------|------|---------|
| T6.4 | `INotificationRepository.cs` | `SSIS.Domain/Interfaces/` | `GetByUserIdAsync`, `GetUnreadByUserIdAsync`, `GetUnreadCountAsync`, `MarkAsReadAsync`, `MarkAllAsReadAsync` |

---

### 6.2 DAL Layer (`SSIS.DAL`)

#### 6.2.1 Database Context

| Task | File | Updates |
|------|------|---------|
| T6.5 | `AppDbContext.cs` | Add `DbSet<Notification>` |

#### 6.2.2 Repository Implementation

| Task | File | Path | Description |
|------|------|------|-------------|
| T6.6 | `NotificationRepository.cs` | `SSIS.DAL/Repositories/` | Implement `INotificationRepository` |

#### 6.2.3 Unit of Work Updates

| Task | File | Updates |
|------|------|---------|
| T6.7 | `IUnitOfWork.cs` | Add: `INotificationRepository Notifications` |
| T6.8 | `UnitOfWork.cs` | Implement new repository property |

---

### 6.3 BLL Layer (`SSIS.BLL`)

#### 6.3.1 DTOs — Notification

| Task | File | Path | Properties |
|------|------|------|------------|
| T6.9 | `NotificationDto.cs` | `SSIS.BLL/DTOs/Notifications/` | `Id`, `Title`, `Message`, `Type`, `IsRead`, `CreatedAt`, `ReadAt` |
| T6.10 | `SendNotificationDto.cs` | `SSIS.BLL/DTOs/Notifications/` | `UserId`, `Title`, `Message`, `Type` |
| T6.11 | `AdminBroadcastDto.cs` | `SSIS.BLL/DTOs/Notifications/` | `Title`, `Message`, `AllStudents`, `AllDoctors`, `AllUsers`, `CourseId`, `StudentId`, `DoctorId`, `AcademicLevel` |
| T6.12 | `DoctorBroadcastDto.cs` | `SSIS.BLL/DTOs/Notifications/` | `Title`, `Message`, `MyStudents`, `CourseId`, `StudentId`, `AcademicLevel` |

#### 6.3.2 Service Interfaces

| Task | File | Path | Methods |
|------|------|------|---------|
| T6.13 | `INotificationService.cs` | `SSIS.BLL/Interfaces/` | `SendNotificationAsync`, `GetUserNotificationsAsync`, `GetUnreadNotificationsAsync`, `GetUnreadCountAsync`, `MarkAsReadAsync`, `MarkAllAsReadAsync`, `SendAdminBroadcastAsync`, `SendDoctorBroadcastAsync`, `NotifyWelcomeAsync`, `NotifyFeeCreatedAsync`, `NotifyPaymentSuccessAsync`, `NotifyPaymentFailedAsync`, `NotifyGradeEnteredAsync`, `NotifyGradeUpdatedAsync`, `NotifyEnrollmentCreatedAsync`, `NotifyEnrollmentRemovedAsync`, `NotifyCourseCreatedAsync`, `NotifyCourseUpdatedAsync`, `NotifyCourseDeletedAsync`, `NotifyAttendanceRecordedAsync`, `NotifyProfileUpdatedAsync`, `NotifyUserRegisteredAsync`, `NotifyUserDeletedAsync`, `NotifyAdminOverdueFeesAsync` |

#### 6.3.3 Service Implementation

| Task | File | Path | Description |
|------|------|------|-------------|
| T6.14 | `NotificationService.cs` | `SSIS.BLL/Services/` | Business logic for notifications |

#### 6.3.4 Mapping Profile

| Task | File | Path | Mappings |
|------|------|------|----------|
| T6.15 | `NotificationMappingProfile.cs` | `SSIS.BLL/Mappings/` | `Notification ↔ NotificationDto` |

---

### 6.4 API Layer (`SSIS.PL`)

#### 6.4.1 Controllers

| Task | File | Path | Endpoints |
|------|------|------|-----------|
| T6.16 | `NotificationsController.cs` | `SSIS.PL/Controllers/v1/` | See [Notification Endpoints](#notification-endpoints) |
| T6.17 | `AdminController.cs` | `SSIS.PL/Controllers/v1/` | Add broadcast endpoint |

---

### 6.5 Database Migration

| Task | Command | Description |
|------|---------|-------------|
| T6.18 | `Add-Migration AddNotificationsTable` | Create migration for Notification |
| T6.19 | `Update-Database` | Apply migration |

---

## Authorization Matrix

| Operation | Admin | Doctor | Student | Notes |
|-----------|:-----:|:------:|:-------:|-------|
| View My Notifications | ✅ | ✅ | ✅ | All users |
| Mark as Read | ✅ | ✅ | ✅ | Own notifications |
| Delete Notification | ✅ | ✅ | ✅ | Own notifications |
| Admin Broadcast | ✅ | ❌ | ❌ | To any group |
| Doctor Broadcast | ❌ | ✅ | ❌ | To their students only |
| System Notifications | Auto | Auto | Auto | Generated by system |

---

## API Endpoints

### Notification Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| `GET` | `/api/v1/notifications` | Get my notifications | All authenticated |
| `GET` | `/api/v1/notifications/unread` | Get unread notifications | All authenticated |
| `GET` | `/api/v1/notifications/unread-count` | Get unread count | All authenticated |
| `PUT` | `/api/v1/notifications/{id}/read` | Mark as read | All authenticated (own) |
| `PUT` | `/api/v1/notifications/mark-all-read` | Mark all as read | All authenticated |

### Admin Broadcast Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| `POST` | `/api/v1/admin/broadcast` | Send broadcast notification | Admin only |

### Doctor Broadcast Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| `POST` | `/api/v1/notifications/broadcast-by-doctor` | Send to students | Doctor only |

---

## Database Schema

### Notifications Table

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `Id` | UNIQUEIDENTIFIER | PK | Primary key |
| `UserId` | UNIQUEIDENTIFIER | FK -> Users, NOT NULL | Recipient |
| `Title` | NVARCHAR(200) | NOT NULL | Notification title |
| `Message` | NVARCHAR(2000) | NOT NULL | Notification content |
| `Type` | INT | NOT NULL | `NotificationType` enum |
| `IsRead` | BIT | NOT NULL, DEFAULT 0 | Read status |
| `ReadAt` | DATETIME2 | NULL | When read |
| `CreatedAt` | DATETIME2 | NOT NULL | Creation timestamp |

### System Notification Triggers

| Event | Notification Type | Recipient |
|-------|-------------------|-----------|
| User Registration | `WelcomeMessage` | New user |
| User Registration | `UserRegistered` | All Admins |
| Profile Update | `ProfileUpdated` | User |
| User Deleted | `UserDeleted` | All Admins |
| Course Created | `CourseCreated` | All Admins |
| Course Updated | `CourseUpdated` | All Admins |
| Course Deleted | `CourseDeleted` | All Admins |
| Enrollment Created | `EnrollmentCreated` | Student |
| Enrollment Removed | `EnrollmentRemoved` | Student |
| Grade Entered | `GradeEntered` | Student |
| Grade Updated | `GradeUpdated` | Student |
| Attendance Recorded | `AttendanceRecorded` | Student |
| Fee Created | `FeeCreated` | Student |
| Payment Success | `PaymentSuccess` | Student |
| Payment Failed | `PaymentFailed` | Student |
| Fee Overdue | `FeeOverdue` | Admins |

---

## Checklist Summary

### Domain Layer
- [ ] `NotificationType` enum
- [ ] `Notification` entity
- [ ] Update `User` with `Notifications` navigation
- [ ] `INotificationRepository` interface

### DAL Layer
- [ ] Update `AppDbContext` with `Notifications` DbSet
- [ ] `NotificationRepository` implementation
- [ ] Update `IUnitOfWork`
- [ ] Update `UnitOfWork`

### BLL Layer
- [ ] DTOs: `NotificationDto`, `SendNotificationDto`, `AdminBroadcastDto`, `DoctorBroadcastDto`
- [ ] `INotificationService` interface
- [ ] `NotificationService` implementation
- [ ] `NotificationMappingProfile`

### API Layer
- [ ] `NotificationsController`
- [ ] `AdminController` (broadcast endpoint)

### Database
- [ ] `Add-Migration AddNotificationsTable`
- [ ] `Update-Database`