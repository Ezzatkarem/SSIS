using AutoMapper;
using SSIS.BLL.DTOs.Attendances;
using SSIS.BLL.Responce;
using SSIS.BLL.Services.Interfaces;
using SSIS.Domain.Entities;
using SSIS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.Services.Implementaion
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendaceRepo attendaceRepo ;
        private readonly IUserRepo userRepo ;
        private readonly INotificationService notificationService  ;
        private readonly IMapper mapper;
        private readonly ICourseRepository courseRepository;
        private readonly IEnrollmentRepository enrollmentRepository;
        private readonly IUnitOfWork unitOfWork ;

        public AttendanceService(IAttendaceRepo attendaceRepo, IUserRepo userRepo, INotficationRepo notficationRepo, IMapper mapper, ICourseRepository courseRepository, IEnrollmentRepository enrollmentRepository, IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            this.attendaceRepo = attendaceRepo;
            this.userRepo = userRepo;
            this.mapper = mapper;
            this.courseRepository = courseRepository;
            this.enrollmentRepository = enrollmentRepository;
            this.unitOfWork = unitOfWork;
            this.notificationService = notificationService;
        }

        public async Task<Responce<int>> GetConsecutiveAbsentAttendanceAsync(Guid CourseId, Guid StudentId)
        {
            var ConsecutiveAbsent=await attendaceRepo.GetConsecutiveAbsencesAsync(CourseId, StudentId);
            return new Responce<int> (ConsecutiveAbsent,true,null!);
        }

        public async Task<Responce<AttendanceDto>> GetcourseAttendanceAsync(Guid Courseid)
        {
            var attendance=await attendaceRepo.GetByCourseIdAsync(Courseid);
            if(!attendance.Any())
            {
                return new Responce<AttendanceDto>(null!, false, "Not attendance in this course");
            }
            var res = mapper.Map<AttendanceDto>(attendance);
            return new Responce<AttendanceDto>(res,true,null!);

        }

        public async Task<Responce<List<AttendancePercentageDto>>> GetStudentAttendancePersentageAsync(Guid studentid)
        {
            var persentage = await attendaceRepo.GetAttendancePercentageByCourseAsync(studentid);
            if(!persentage.Any())
            {
                return new Responce<List<AttendancePercentageDto>>(null, false, "No attendance Records found");
            }
            var res = new List<AttendancePercentageDto>();
            foreach(var courseid in persentage.Keys)
            {
                var course=await courseRepository.GetByIdAsync(courseid);
                res.Add(new AttendancePercentageDto
                {
                    courseId = courseid,
                    courseName=course!.Name,
                    persentage=persentage[courseid]
                });
            }
            return new Responce<List<AttendancePercentageDto>>(res, true, " attendance Persentage retrieved");
        }

        public async Task<Responce<double>> GetOverAllCourseAttendancePersentageAsync(Guid courseid)
        {
            var persentage=await attendaceRepo.GetOveerAllAttendancePercentageForCourseAsync(courseid);
            return new Responce<double>(persentage, true, null!);
        }

        public async Task<Responce<double>> GetOverAllStudentAttendancePersentageAsync(Guid StudentId)
        {
            var persentage = await attendaceRepo.GetOverAllAttendancePercentageForStudentAsync(StudentId);
            return new Responce<double>(persentage, true, null!);
        }

        public async Task<Responce<AttendanceDto>> GetStudentAttendanceAsync(Guid studentid)
        {
            var attendance = await attendaceRepo.GetByStudentIdAsync(studentid);
            if (!attendance.Any())
            {
                return new Responce<AttendanceDto>(null!, false, "Not attendance in this course");
            }
            var res = mapper.Map<AttendanceDto>(attendance);
            return new Responce<AttendanceDto>(res, true, null!);
        }

        public async Task<Responce<List<StudentAttendancePercentageDto>>> GetCourseAttendancePersentageAsync(Guid courseid)
        {
            var enroll = await enrollmentRepository.GetByCourseAsync(courseid);
            if (!enroll.Any())
            {
                return new Responce<List<StudentAttendancePercentageDto>>(null, false, "No student enroll in this course");
            }
            var persentage = await attendaceRepo.GetAttendancePercentageByCourseAsync(courseid);
            if (!persentage.Any())
            {
                return new Responce<List<StudentAttendancePercentageDto>>(null, false, "No attendance Records found");
            }
            var res = new List<StudentAttendancePercentageDto>();
            foreach (var enrollment in enroll)
            {
                var student=await userRepo.GetByIdAsync(enrollment.StudentId);

                var per = persentage.ContainsKey(enrollment.StudentId) ? persentage[enrollment.StudentId] : 0;
                res.Add(new StudentAttendancePercentageDto
                {
                    StudentId = enrollment.StudentId,
                    StudentName = student!.FullName,
                    persentage = Math.Round(per, 2)
                });
            }
            return new Responce<List<StudentAttendancePercentageDto>>(res, true, " attendance Persentage retrieved");
        }

        public async Task<Responce<object>> TakeAttendanceAsync(TakeAttendanceDto dto, Guid doctorid)
        {
            var course = await courseRepository.GetByIdAsync(dto.CourseId);
            if (course!.DoctorId != doctorid) 
            {
                return new Responce<object>(null!, false, "You Are not authorized for this Course.");
            }
            var enrollmentstudent=await enrollmentRepository.GetByCourseAsync(dto.CourseId);
           var enrollmentstudentIds= enrollmentstudent.Select(x => x.StudentId);


            var validateAttendances=dto.attendances.Where(a=>enrollmentstudentIds.Contains(a.studentId)).ToList();
            var existingAttendance=await attendaceRepo.GetByCourseIdAsync(dto.CourseId);
            var todelete=existingAttendance.Where(a=>a.Date.Date==dto.Date.Date).ToList();
            foreach (var att in todelete)
                await attendaceRepo.DeleteAsync(att);

            foreach (var studentattendance in validateAttendances)
            {
                var attendance = new Attendance
                {
                    Id = Guid.NewGuid(),
                    StudentId=studentattendance.studentId,
                    courseId=dto.CourseId,
                    Date=dto.Date.ToUniversalTime(),
                    AttendanceState=studentattendance.AttendanceState,
                    CreatedAt=DateTime.UtcNow

                };
                await attendaceRepo.AddAsync(attendance);
            }
            await unitOfWork.SaveChangesAsync();
            foreach (var attendance in validateAttendances)
            {
               await notificationService.NotifyAttendanceRecordedAsync(attendance.studentId,  course.Name,attendance.AttendanceState.ToString());
            }
            return new Responce<object>(null!, true, null!);



        }
    }
}
