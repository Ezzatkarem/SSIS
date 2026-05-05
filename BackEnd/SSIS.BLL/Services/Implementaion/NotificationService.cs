using AutoMapper;
using Paymob.Net.Models;
using SSIS.BLL.DTOs.Notification;
using SSIS.BLL.Responce;
using SSIS.BLL.Services.Interfaces;
using SSIS.DAL.Migrations;
using SSIS.DAL.Repositories;
using SSIS.Domain.Entities;
using SSIS.Domain.Enum;
using SSIS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SSIS.BLL.Services.Implementaion
{
    public class NotificationService : INotificationService
    {
        private readonly IMapper mapper;
        private readonly INotficationRepo notficationRepo ;
        private readonly IUserRepo userRepo;
        private readonly IUnitOfWork unitOfWork;
        private readonly ICourseRepository courseRepository;
        private readonly IEnrollmentRepository enrollmentRepository ;


        public NotificationService(IMapper mapper, INotficationRepo notficationRepo, IUserRepo userRepo, IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.notficationRepo = notficationRepo;
            this.userRepo = userRepo;
            this.unitOfWork = unitOfWork;
        }

        #region SendNotificationAsync
        public async Task SendNotificationAsync(SendNotificationDto dto)
        {
            var notification = new Notification()
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                Message = dto.Message,
                Title = dto.Title,
                NotificationType = dto.NotificationType,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            await notficationRepo.AddAsync(notification);
            await unitOfWork.SaveChangesAsync();

        }
        #endregion
        #region GetUnReadNotificationAsync
        public async Task<Responce<IReadOnlyList<NotificationDto>>> GetUnReadNotificationAsync(Guid UserId)
        {
            try
            {

                var notification = await notficationRepo.GetUnreadNotificationsByUserIdAsync(UserId);
                if (notification == null)
                {
                    return new Responce<IReadOnlyList<NotificationDto>>(null!, false, "No Have Notification");

                }
                var res = mapper.Map<IReadOnlyList<NotificationDto>>(notification);
                return new Responce<IReadOnlyList<NotificationDto>>(res, true, null!);
            }
            catch (Exception ex)
            {
                return new Responce<IReadOnlyList<NotificationDto>>(null!, false, "No Have Notification");

            }
        }
        #endregion
        #region GetUserNotificationAsync
        public async Task<Responce<IReadOnlyList<NotificationDto>>> GetUserNotificationAsync(Guid Userid)
        {
            var notification = await notficationRepo.GetNotificationsByUserIdAsync(Userid);
            var res = mapper.Map<IReadOnlyList<NotificationDto>>(notification);
            return new Responce<IReadOnlyList<NotificationDto>>(res, true, null!);

        }
        #endregion

        #region MarkAllAsReadAsync
        public async Task MarkAllAsReadAsync(Guid UserId)
        {


            await notficationRepo.markAllAsreadAsync(UserId);
            await unitOfWork.SaveChangesAsync();

        }
        #endregion

        #region MarkAsReadAsync
        public async Task MarkAsReadAsync(Guid NotificationId, Guid UserId)
        {
            var note = await notficationRepo.GetByIdAsync(NotificationId);
            if (note.UserId == UserId)
            {

                await notficationRepo.markAsReadAsync(NotificationId);
                await unitOfWork.SaveChangesAsync();
            }
        }
        #endregion
        #region GetUnReadCountAsync
        public async Task<int> GetUnReadCountAsync(Guid UserId)
        {

            return await notficationRepo.GetUnreadNotificationsCountByUserIdAsync(UserId);


        }

        #endregion




        #region NotifyFeeCreatedAsync
        public async Task NotifyFeeCreatedAsync(Guid studentId, decimal Amount, int semester, int Year)
        {
            await SendNotificationAsync(new SendNotificationDto
            {
                UserId = studentId,
                Title = "New Fee Invoice",
                Message = $"A New fee invoice Has been created for semester {semester}, Year {Year} ",
                NotificationType = NotificationType.FeeCreated
            });
        }
        #endregion

        #region NotifyPaymentFaildAsync
        public async Task NotifyPaymentFaildAsync(Guid studentId, decimal Amount, int semester, string ErrorMessage)
        {
            await SendNotificationAsync(new SendNotificationDto
            {
                UserId = studentId,
                Title = "Payment Faild",
                Message = $"Payment of Amount {Amount} Field ,Reason : {ErrorMessage}.",
                NotificationType = NotificationType.PaymentFaild
            });
        }
        #endregion

        #region NotifyPaymentSeccessAsync
        public async Task NotifyPaymentSeccessAsync(Guid studentId, decimal Amount, string transctionId)
        {
            await SendNotificationAsync(new SendNotificationDto
            {
                UserId = studentId,
                Title = "Payment Successful",
                Message = $"Payment of Amount {Amount} EGP has been successfully processed,trnsaction Id:{transctionId}.",
                NotificationType = NotificationType.PaymentSeccess
            });
        }
        #endregion

        #region NotifyFeeReminderAsync
        public async Task NotifyFeeReminderAsync(Guid studentId, decimal Amount, DateTime DueDate)
        {
            await SendNotificationAsync(new SendNotificationDto
            {
                UserId = studentId,
                Title = "Fee Payment  Reminder",
                Message = $"Reminder: Fee of Amount {Amount} EGP is due  by {DueDate:yyyyy-MM-dd}.",
                NotificationType = NotificationType.FeeReminder
            });
        }
        #endregion

        #region NotifyAdminOverduefeesAsync
        public async Task NotifyAdminOverduefeesAsync(int Overduecount)
        {
            var admins = await userRepo.GetByRoleAsync(UserRole.Admin);
            foreach (var admin in admins)
            {

                await SendNotificationAsync(new SendNotificationDto
                {
                    UserId = admin.Id,
                    Title = "Over Fee Alerts",
                    Message = $"there are   {Overduecount} overdue fees that require your attention.",
                    NotificationType = NotificationType.FeeOverDue
                });
            }
        }
        #endregion








        #region NotifyAttendanceRecordedAsync
        public async Task NotifyAttendanceRecordedAsync(Guid studentId, string courseName, string status)
        {
            await SendNotificationAsync(new SendNotificationDto
            {
                UserId = studentId,
                Title = "Attendance Recorded",
                Message = $"Your attendance for {courseName} has been recorded as {status}.",
                NotificationType = NotificationType.AttendanceRecorded
            });
        }
        #endregion



        #region NotifyCourseCreatedAsync
        public async Task NotifyCourseCreatedAsync(string courseName, string courseCode)
        {
            var admins = await userRepo.GetByRoleAsync(UserRole.Admin);
            foreach (var admin in admins)
            {
                await SendNotificationAsync(new SendNotificationDto
                {
                    UserId = admin.Id,
                    Title = "New Course Added",
                    Message = $"A new course has been added: {courseName} ({courseCode})",
                    NotificationType = NotificationType.courseCreated
                });
            }
        }
        #endregion

        #region NotifyCourseUpdatedAsync
        public async Task NotifyCourseUpdatedAsync(string courseName, string courseCode)
        {
            var admins = await userRepo.GetByRoleAsync(UserRole.Admin);
            foreach (var admin in admins)
            {
                await SendNotificationAsync(new SendNotificationDto
                {
                    UserId = admin.Id,
                    Title = "Course Updated",
                    Message = $"Course details have been updated: {courseName} ({courseCode})",
                    NotificationType = NotificationType.CourseUpdated
                });
            }
        }
        #endregion

        #region NotifyCourseDeletedAsync
        public async Task NotifyCourseDeletedAsync(string courseName, string courseCode)
        {
            var admins = await userRepo.GetByRoleAsync(UserRole.Admin);
            foreach (var admin in admins)
            {
                await SendNotificationAsync(new SendNotificationDto
                {
                    UserId = admin.Id,
                    Title = "Course Deleted",
                    Message = $"Course has been deleted: {courseName} ({courseCode})",
                    NotificationType = NotificationType.CourseDeletred
                });
            }
        }
        #endregion


        #region NotifyEnrollmentCreatedAsync
        public async Task NotifyEnrollmentCreatedAsync(Guid studentId, string courseName)
        {
            await SendNotificationAsync(new SendNotificationDto
            {
                UserId = studentId,
                Title = "Enrollment Successful",
                Message = $"You have been successfully enrolled in {courseName}.",
                NotificationType = NotificationType.EnrollmentCreated
            });
        }
        #endregion

        #region NotifyEnrollmentRemovedAsync
        public async Task NotifyEnrollmentRemovedAsync(Guid studentId, string courseName)
        {
            await SendNotificationAsync(new SendNotificationDto
            {
                UserId = studentId,
                Title = "Enrollment Cancelled",
                Message = $"Your enrollment in {courseName} has been cancelled.",
                NotificationType = NotificationType.EnrollmentRemoved
            });
        }
        #endregion





        #region NotifyGradeEnteredAsync
        public async Task NotifyGradeEnteredAsync(Guid studentId, string courseName, decimal score)
        {
            await SendNotificationAsync(new SendNotificationDto
            {
                UserId = studentId,
                Title = "New Grade Posted",
                Message = $"A new grade of {score} has been posted for {courseName}.",
                NotificationType = NotificationType.GradeEnterd
            });
        }
        #endregion

        #region NotifyGradeUpdatedAsync
        public async Task NotifyGradeUpdatedAsync(Guid studentId, string courseName, decimal oldScore, decimal newScore)
        {
            await SendNotificationAsync(new SendNotificationDto
            {
                UserId = studentId,
                Title = "Grade Updated",
                Message = $"Your grade for {courseName} has been updated from {oldScore} to {newScore}.",
                NotificationType = NotificationType.GradeUpdate
            });
        } 
        #endregion



        #region NotifyWellcomeAsync
        public async Task NotifyWellcomeAsync(Guid UserId, string username, string userrole)
        {
            var message = $"Welcome to SSIS, {username}! 🎉\n" +
                          $"Your account has been successfully created as {userrole}.\n" +
                          $"You can now log in and start using the system.";
            await SendNotificationAsync(new SendNotificationDto
            {
                UserId = UserId,
                Title = "Welcome to SSIS! 🎉 ",
                Message = message,
                NotificationType = NotificationType.WelcomeMessage
            });
        } 
        #endregion
        #region NotifyUserRegisterAsync
        public async Task NotifyUserRegisterAsync(Guid AdminId, string username, string userrole)
        {
            await SendNotificationAsync(new SendNotificationDto
            {
                UserId = AdminId,
                Title = "New User Registered",
                Message = $"A new user has registered: {username} (Role: {userrole})",
                NotificationType = NotificationType.UserRegisterd
            });
        } 
        #endregion
        #region NotifyProfileUpdateAsync
        public async Task NotifyProfileUpdateAsync(Guid Userid, string UpdateField)
        {
            await SendNotificationAsync(new SendNotificationDto
            {
                UserId = Userid,
                Title = "Profile Updated",
                Message = $"Your {UpdateField} has been updated successfully.",
                NotificationType = NotificationType.ProfileUpdate
            });
        } 
        #endregion

        #region NotifyUserDeleteAsync
        public async Task NotifyUserDeleteAsync(Guid AdminId, string username)
        {
            await SendNotificationAsync(new SendNotificationDto
            {
                UserId = AdminId,
                Title = "User Deleted",
                Message = $"User {username} has been deleted from the system.",
                NotificationType = NotificationType.UserDelete
            });
        }

        #endregion
        public async Task<int> sendAdminBroadcastAsync(AdminBroadcastDto dto)
        {
            var userIds = new List<Guid>();
            if (dto.AllUsers)
            {
                var alluser = await userRepo.GetAllAsync();
                var userid = alluser.Select(p => p.Id).ToList();
            }
            else if (dto.AllDoctors)
            {
                var alluser = await userRepo.GetByRoleAsync(UserRole.Doctor);
                var userid = alluser.Select(p => p.Id).ToList();
            }
            else if (dto.AllStudents)
            {
                var alluser = await userRepo.GetByRoleAsync(UserRole.Student);
                var userid = alluser.Select(p => p.Id).ToList();
            }
            else if (dto.CourceId.HasValue)
            {
                var enrollments = await enrollmentRepository.GetByCourseAsync(dto.CourceId.Value);
                userIds = enrollments.Select(e => e.StudentId).Distinct().ToList();
            }
            else if (dto.StudentId.HasValue)
            {
                userIds.Add(dto.StudentId.Value);
            }
            else if (dto.AcademicLevel.HasValue)
            {
                var alluser = await userRepo.GetByRoleAsync(UserRole.Student);
                var filter = alluser.Where(p => p.Level == dto.AcademicLevel);
                var userid = alluser.Select(p => p.Id).ToList();
            }
            foreach (var userId in userIds)
            {
                await SendNotificationAsync(new SendNotificationDto
                {
                    UserId = userId,
                    Title = dto.Title,
                    Message = dto.Message,
                    NotificationType = NotificationType.AdmonBroadcast
                });
            }
            return userIds.Count;


        }
        public async Task<int> SendNotificationsByDoctor(DoctorBroadcastDto dto, Guid doctorId)
        {
            var userIds = new List<Guid>();

            if (dto.MyStudents)  
            {
                var courses = await courseRepository.GetByDoctorAsync(doctorId);
                var courseIds = courses.Select(c => c.Id).ToList();

                var enrollments = await enrollmentRepository.GetByCourseIdsAsync(courseIds);
                userIds = enrollments.Select(e => e.StudentId).Distinct().ToList();
            }
            else if (dto.CourseId.HasValue)  
            {
                var enrollments = await enrollmentRepository.GetByCourseAsync(dto.CourseId.Value);
                userIds = enrollments.Select(e => e.StudentId).Distinct().ToList();
            }
            else if (dto.StudentId.HasValue) 
            {
                userIds.Add(dto.StudentId.Value);
            }
            else if (dto.AcademicLevel.HasValue)  
            {
                var students = await userRepo.GetByRoleAsync(UserRole.Student);
                var filtered = students.Where(s => s.Level == dto.AcademicLevel.Value);
                userIds = filtered.Select(s => s.Id).ToList();
            }
            else
            {
                return 0;
            }

            foreach (var userId in userIds)
            {
                await SendNotificationAsync(new SendNotificationDto
                {
                    UserId = userId,
                    Title = dto.Title,
                    Message = dto.Message,
                    NotificationType = NotificationType.DoctorBroadcast
                });
            }

            return userIds.Count;
        }


    }
}
