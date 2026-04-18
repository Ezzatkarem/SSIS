using Microsoft.EntityFrameworkCore;
using SSIS.BLL.DTOs.Grades;
using SSIS.BLL.Responce;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.Services.Interfaces
{
    public interface IGradeService
    {
        Task<Responce<GradeDTO>> EnterGradeAsync( GradeDTO gradeDTO,Guid doctorId);
        Task<Responce<List<GradeDTO>>> GetStudentGradesAsync(Guid StudintId,int semester,int academicYear);
        Task<Responce<UpdateGradeDTO>> UpdateGradesAsync(Guid DoctorId, UpdateGradeDTO gradeDTO,Guid GradeId);
        Task<Responce<GpaResponce>> CalculateGpaAsync ( Guid studintId,int? semester=null,int? academicYear=null);
        Task<Responce<IReadOnlyList<GradeDTO>>> GetGradesByCourseAsync(Guid courseId);
        Task<Responce<bool>> deleteGradeAsync(Guid gradeId,Guid DoctorId);

    }
}
