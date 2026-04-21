using AutoMapper;
using Microsoft.VisualBasic;
using SSIS.BLL.DTOs.Fee;
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
    public class FeesService : IFeeService
    {
        private readonly IFeeRepo feeRepo;
        private readonly IUserRepo userRepo;
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

       

        public FeesService(IFeeRepo feeRepo, IUserRepo userRepo, IMapper mapper,IUnitOfWork unitOfWork)
        {
            this.feeRepo = feeRepo;
            this.userRepo = userRepo;
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }


        #region CreateFeeForStudentAsync
        public async Task<Responce<FeeResponceDto>> CreateFeeForStudentAsync(CreateFeeDto dto)
        {
            var user = await userRepo.GetByIdAsync(dto.StudentId);
            if (user == null || user.Role != UserRole.Student)
            {
                return new Responce<FeeResponceDto>(null, false, "Student Not Found");
            }
            var existing = await feeRepo.GetByStudentIdAsync(dto.StudentId);
            if (existing.Any(p => p.semester == dto.Semester && p.academicYear == dto.AcademicYear))
            {
                return new Responce<FeeResponceDto>(null, false, " Fee Alrady exists for this students  for this semester and Year");
            }
            var fee = mapper.Map<Fee>(dto);
            fee.StudentId = dto.StudentId;
            fee.CreatedAt = DateTime.Now;
            fee.PaidAmount = 0;
            fee.feeStatus = FeeStaus.Unpaid;
            await feeRepo.AddAsync(fee);
            await unitOfWork.SaveChangesAsync();
            var res = mapper.Map<FeeResponceDto>(fee);
            return new Responce<FeeResponceDto>(res, true, "Create Fee Has Successfully");


        }
        #endregion


        #region DeleteFeeAsync
        public async Task<Responce<bool>> DeleteFeeAsync(Guid feeId)
        {
            var fee = await feeRepo.GetByIdAsync(feeId);
            if (fee == null)
            {
                return new Responce<bool>(false, false, "Fee Not Found");

            }
            if (fee.PaidAmount > 0)
            {
                return new Responce<bool>(false, false, "Cannot fee with existing Payments");

            }
            await feeRepo.DeleteAsync(fee);
            await unitOfWork.SaveChangesAsync();
            return new Responce<bool>(true, true, "Delete Fee has seccessfully");


        }
        #endregion

        #region GetAllFeesAsync
        public async Task<Responce<IReadOnlyList<FeeResponceDto>>> GetAllFeesAsync()
        {
            var fees = await feeRepo.GetAllAsync();
            var res = mapper.Map<IReadOnlyList<FeeResponceDto>>(fees);
            return new Responce<IReadOnlyList<FeeResponceDto>>(res, true, null!);

        }
        #endregion

        #region GetFeesByStudentAsync
        public async Task<Responce<IReadOnlyList<FeeResponceDto>>> GetFeesByStudentAsync(Guid studentId)
        {
            var user = await userRepo.GetByIdAsync(studentId);
            if (user == null || user.Role != UserRole.Student)
            {
                return new Responce<IReadOnlyList<FeeResponceDto>>(null!, false, "Student Not Found");
            }
            var fees = await feeRepo.GetByStudentIdAsync(studentId);
            if (!fees.Any())
            {
                return new Responce<IReadOnlyList<FeeResponceDto>>(null!, false, "Student Not Have Fees Yet ");

            }
            var res = mapper.Map<IReadOnlyList<FeeResponceDto>>(fees);
            return new Responce<IReadOnlyList<FeeResponceDto>>(res, true, "Delete Fee has seccesfuly");

        }
        #endregion

        #region GetMyFeesAsync
        public async Task<Responce<IReadOnlyList<FeeResponceDto>>> GetMyFeesAsync(Guid studentId)
        {
            var user = await userRepo.GetByIdAsync(studentId);
            if (user == null || user.Role != UserRole.Student)
            {
                return new Responce<IReadOnlyList<FeeResponceDto>>(null!, false, "Student Not Found");
            }
            var fees = await feeRepo.GetByStudentIdAsync(studentId);
            if (!fees.Any())
            {
                return new Responce<IReadOnlyList<FeeResponceDto>>(null!, false, "Student Not Have Fees Yet ");

            }
            var res = mapper.Map<IReadOnlyList<FeeResponceDto>>(fees);
            return new Responce<IReadOnlyList<FeeResponceDto>>(res, true, null!);

        }
        #endregion


        #region UpdateFeeAsync
        public async Task<Responce<FeeResponceDto>> UpdateFeeAsync(Guid feeId, UpdateFee dto)
        {
            var fees = await feeRepo.GetByIdAsync(feeId);
            if (fees == null)
            {
                return new Responce<FeeResponceDto>(null, false, "Fee Not found");
            }
            fees.TotalAmount = dto.TotalAmount;
            fees.DueDate = dto.DueDate;
            fees.UpdatedAt=DateTime.Now;
            var res = mapper.Map<FeeResponceDto>(fees);
            await feeRepo.UpdateAsync(fees);
            await unitOfWork.SaveChangesAsync();
            return new Responce<FeeResponceDto>(res, false, "Fee Not found");


        }
        #endregion
        #region AutoGenerateFeesAsync
        public async Task<Responce<bool>> AutoGenerateFeesAsync(FeeSettingsDto dto)
        {
            var students = await userRepo.GetAllAsync();
            students = students.Where(p => p.Role == UserRole.Student).ToList();
            foreach (var student in students)
            {
                var existingfees = await feeRepo.GetByStudentIdAsync(student.Id);
                if (existingfees.Any(f => f.semester == dto.Semester && f.academicYear == dto.AcademicYear))
                    continue;

                var createFeeDto = new CreateFeeDto
                {
                    StudentId = student.Id,
                    Semester = dto.Semester,
                    AcademicYear = dto.AcademicYear,
                    TotalAmount = dto.AmountPerStudent,  
                    DueDate = dto.DueDate
                }; await CreateFeeForStudentAsync(createFeeDto);

            }
                return new Responce<bool>(true, true, "auto generate fees has succesfully");

        } 
        #endregion

        #region GetFeeByIdAsync
        public async Task<Responce<FeeResponceDto>> GetFeeByIdAsync(Guid feeId)
        {
            var fee =await feeRepo.GetByIdAsync(feeId);
            if (fee == null)
            {
                return new Responce<FeeResponceDto>(null, false, "fee Not Found");
            }
            var res = mapper.Map<FeeResponceDto>(fee);
            return new Responce<FeeResponceDto>(res, true, null);

        } 
        #endregion
    }
}
