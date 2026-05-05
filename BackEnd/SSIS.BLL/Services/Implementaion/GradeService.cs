using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SSIS.BLL.DTOs.Grades;
using SSIS.BLL.Responce;
using SSIS.BLL.Services.Interfaces;
using SSIS.Domain.Entities;
using SSIS.Domain.Enum;
using SSIS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.Services.Implementaion
{
    public class GradeService : IGradeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGradeRepository _gradeRepository;
        private readonly IMapper mapper;
        private readonly ICourseRepository _courseRepository;
        private readonly IUserRepo userRepo;
        private readonly IEnrollmentRepository _enrollmentRepository;

        
        public GradeService(IUnitOfWork unitOfWork, IGradeRepository gradeRepository, IMapper mapper, ICourseRepository courseRepository, IUserRepo userRepo,IEnrollmentRepository enrollmentRepository)
        {
            _unitOfWork = unitOfWork;
            _gradeRepository = gradeRepository;
            this.mapper = mapper;
            _courseRepository = courseRepository;
            _enrollmentRepository = enrollmentRepository;
            this.userRepo = userRepo;
        }
        #region EnterGradeAsync
        public async Task<Responce<GradeDTO>> EnterGradeAsync(GradeDTO gradeDTO, Guid doctorId)
        {
            var course = await _courseRepository.GetByIdAsync(gradeDTO.CourseId);
            if (course.DoctorId != doctorId)
            {
                return new Responce<GradeDTO>(null, false, "You are not authorized to enter grades for this course");
            }
            var enrollment = await _enrollmentRepository.ExistsAsync(gradeDTO.StudentId, gradeDTO.CourseId);
            if (!enrollment)
            {
                return new Responce<GradeDTO>(null, false, "Student is not enrolled in this course");
            }
            var grade = mapper.Map<Grade>(gradeDTO);
            grade.Id = Guid.NewGuid();
            grade.GradeLetter = GetGradeLatter(gradeDTO.Score);
            await _unitOfWork.Grades.AddAsync(grade);
            await _unitOfWork.SaveChangesAsync();
            return new Responce<GradeDTO>(mapper.Map<GradeDTO>(grade), true, "Grade entered successfully");
        }
        #region GetGradeLatter
        public GradeLetter GetGradeLatter(decimal score)
        {
            if (score >= 90)
                return GradeLetter.A;
            else if (score >= 80)
                return GradeLetter.B;
            else if (score >= 70)
                return GradeLetter.C;
            else if (score >= 60)
                return GradeLetter.D;
            else
                return GradeLetter.F;
        }
        #endregion
        #endregion

        #region UpdateGradesAsync
        public async Task<Responce<UpdateGradeDTO>> UpdateGradesAsync(Guid DoctorId, UpdateGradeDTO gradeDTO, Guid GradeId)
        {
            try
            {
                var grade = await _gradeRepository.GetByIdAsync(GradeId);
                if (grade == null)
                {
                    return new Responce<UpdateGradeDTO>(null, false, "Grade not found");
                }
                var course = await _courseRepository.GetByIdAsync(grade.CourseId);
                if (course.DoctorId != DoctorId)
                {
                    return new Responce<UpdateGradeDTO>(null, false, "You are not authorized to update grades for this course");
                }
                grade.Score = gradeDTO.Score;
                grade.Remarks = gradeDTO.Remarks;
                grade.GradeLetter = GetGradeLatter(gradeDTO.Score);
                grade.UpdatedAt = DateTime.UtcNow;
                _gradeRepository.UpdateAsync(grade);
                await _unitOfWork.SaveChangesAsync();
                return new Responce<UpdateGradeDTO>(gradeDTO, true, "Grade updated successfully");



            }

            catch (Exception ex)
            {
                return new Responce<UpdateGradeDTO>(null, false, $"An error occurred while updating the grade: {ex.Message}");

            }
        }
        #endregion
   
        #region GetGradesByCourseAsync
        public async Task<Responce<IReadOnlyList<GradeDTO>>> GetGradesByCourseAsync(Guid courseId)
        {
            try
            {
                var grades = await _gradeRepository.GetByCourseIdAsync(courseId);
                return new Responce<IReadOnlyList<GradeDTO>>(mapper.Map<IReadOnlyList<GradeDTO>>(grades), true, "Grades retrieved successfully");
            }
            catch (Exception ex)
            {
                return new Responce<IReadOnlyList<GradeDTO>>(null, false, $"An error occurred while retrieving grades: {ex.Message}");
            }

        } 
        #endregion

        #region deleteGradeAsync
        public async Task<Responce<bool>> deleteGradeAsync(Guid gradeId, Guid DoctorId)
        {
            var grade = await _gradeRepository.GetByIdAsync(gradeId);
            if (grade == null)
            {
                return new Responce<bool>(false, false, "Grade not found");
            }
            var course = await _courseRepository.GetByIdAsync(grade.CourseId);
            if (course.DoctorId != DoctorId)
            {
                return new Responce<bool>(false, false, "You are not authorized to delete grades for this course");
            }
            _gradeRepository.DeleteAsync(grade);
            await _unitOfWork.SaveChangesAsync();
            return new Responce<bool>(true, true, "Grade deleted successfully");
        }
        #endregion


        #region GetStudentGradesAsync
        public async Task<Responce<List<GradeDTO>>> GetStudentGradesAsync(Guid StudintId,int semester ,int academicYear)
        {
            var student = await userRepo.GetByIdAsync(StudintId);
            if (student == null)
            {
                return new Responce<List<GradeDTO>>(null, false, "Student not found");
            }
            var grades = await _gradeRepository.GetByStudentIdAsync(StudintId, semester, academicYear);
            if (grades == null)
            {
                return new Responce<List<GradeDTO>>(null, false, "No grades found for this student");
            }
            return new Responce<List<GradeDTO>>(mapper.Map<List<GradeDTO>>(grades), true, "Grades retrieved successfully");

        } 
        #endregion

        #region CalculateGpaAsync
        public async Task<Responce<GpaResponce>> CalculateGpaAsync(Guid studintId,int? semester=null,int? academicYear=null)
        {
            var student = await userRepo.GetByIdAsync(studintId);
            if (student == null)
            {
                return new Responce<GpaResponce>(null, false, "Student not found");
            }
            var grades = await _gradeRepository.GetByStudentIdAsync(studintId, semester ?? 0, academicYear ?? 0);
            if (grades == null || !grades.Any())
            {
                return new Responce<GpaResponce>(null, false, "No grades found for this student");
            }
            var sortedGrades = grades.OrderBy(g => g.AcademicYear).ThenBy(g => g.Semester).ToList();
            var targetsemester = semester ?? sortedGrades.Last().Semester;
            var targetAcademicYear = academicYear ?? sortedGrades.Last().AcademicYear;
            var totalCredits = 0;
            var totalPoints = 0m;
            
            decimal semesterPoints = 0m;
            int semesterCredits = 0;
            foreach (var item in grades)
            {
                int gradepoints;
                if(item.GradeLetter == GradeLetter.A)
                {
                    gradepoints = 4;
                }
                else if (item.GradeLetter == GradeLetter.B)
                {
                    gradepoints = 3;
                }
                else if (item.GradeLetter == GradeLetter.C)
                {
                    gradepoints = 2;
                }
                else if (item.GradeLetter == GradeLetter.D)
                {
                    gradepoints = 1;
                }
                else
                {
                    gradepoints = 0;
                }
                item.Course.Credits = (semester==1?1: item.Course.Credits);
                totalPoints += gradepoints * (item.Course.Credits);
                totalCredits += item.Course.Credits;
                if (item.Semester == targetsemester && item.AcademicYear == targetAcademicYear) 
                {
                    semesterPoints += gradepoints * item.Course.Credits;
                    semesterCredits += item.Course.Credits;
                }

               
            }
            var smasterGpa = (semesterCredits>0? semesterPoints / semesterCredits:0);
            var cumulativeGpa =(totalCredits>0? totalPoints / totalCredits:0);

            return new Responce<GpaResponce>(new GpaResponce
            {
                SemesterGpa = Math.Round(smasterGpa, 2),
                CumulativeGpa = Math.Round(cumulativeGpa, 2),
                TotalCredits = totalCredits,
                Semester = targetsemester,
                AcademicYear = targetAcademicYear
            }, true, "GPA calculated successfully");

        }


        #endregion

        public async Task<Responce<GradeDTO>> GetgradeBYIdAsync (Guid gradeid)
        {
            var grade= await _unitOfWork.Grades.GetByIdAsync(gradeid);
            var res=mapper.Map<GradeDTO>(grade);
            return new Responce<GradeDTO>(res, true, null!); 
        }

    }
}
