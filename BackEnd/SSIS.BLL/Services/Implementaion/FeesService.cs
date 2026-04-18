using AutoMapper;
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
            if (fee != null)
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


        public Task<Responce<FeeResponceDto>> UpdateFeeAsync(Guid feeId, UpdateFee dto)
        {
            throw new NotImplementedException();
        }
        public Task AutoGenerateFeesAsync(FeeSettingsDto dto)
        {
            throw new NotImplementedException();
        }

        #region GetFeeByIdAsync
        public async Task<Responce<FeeResponceDto>> GetFeeByIdAsync(Guid feeId)
        {
            var fee = feeRepo.GetByIdAsync(feeId);
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
