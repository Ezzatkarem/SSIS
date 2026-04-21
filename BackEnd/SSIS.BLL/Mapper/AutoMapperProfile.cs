using AutoMapper;
using SSIS.BLL.DTOs.Fee;
using SSIS.BLL.DTOs.Grades;
using SSIS.BLL.DTOs.Login;
using SSIS.BLL.DTOs.Payment;
using SSIS.Domain.Entities;
using SSIS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // // mapper For Grade
            CreateMap<Grade, GradeDTO>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.FullName))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name))
                .ForMember(dest => dest.CourseCode, opt => opt.MapFrom(src => src.Course.Code))
                .ForMember(dest => dest.Credits, opt => opt.MapFrom(src => src.Course.Credits))
                .ForMember(dest => dest.academicYear, opt => opt.MapFrom(src => src.Course.AcademicYear))
                .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => src.Course.Semester))
                .ForMember(dest => dest.Remarks, opt => opt.MapFrom(src => src.Remarks))
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score));
            CreateMap<GradeDTO, Grade>()
                .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => src.Semester))
                .ForMember(dest => dest.Remarks, opt => opt.MapFrom(src => src.Remarks))
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.GradeLetter, opt => opt.MapFrom(src => src.GradeLetter))
                .ForMember(dest => dest.Student, opt => opt.Ignore())
                .ForMember(dest => dest.Course, opt => opt.Ignore())
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score));



            // Mapping for fee
            CreateMap<CreateFeeDto, Fee>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Student, opt => opt.Ignore())
            .ForMember(dest => dest.Payments, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.academicYear, opt => opt.MapFrom(src => src.AcademicYear))
            .ForMember(dest => dest.semester, opt => opt.MapFrom(src => src.Semester))
                        .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))



             .ForMember(dest => dest.feeStatus, opt => opt.MapFrom(src => FeeStaus.Unpaid))
             .ForMember(dest => dest.PaidAmount, opt => opt.MapFrom(src => 0m));



            CreateMap<FeeSettingsDto, Fee>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Student, opt => opt.Ignore())
            .ForMember(dest => dest.StudentId, opt => opt.Ignore())
            .ForMember(dest => dest.Payments, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())

             .ForMember(dest => dest.feeStatus, opt => opt.MapFrom(src => FeeStaus.Unpaid))
             .ForMember(dest => dest.PaidAmount, opt => opt.MapFrom(src => 0m));



            CreateMap<Fee, FeeResponceDto>()

             .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.FullName))
             .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payments))
             .ForMember(dest => dest.ReminingAmount, opt => opt.Ignore());



            CreateMap<InitiatePaymentDto, Payment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.StudentId, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentDate, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => "Paymob"))
                .ForMember(dest => dest.TransactionId, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => PaymentStatus.Pending))
                .ForMember(dest => dest.ReceipeUrl, opt => opt.Ignore())
                .ForMember(dest => dest.PaymobOrderId, opt => opt.Ignore())
                .ForMember(dest => dest.Fee, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Student, opt => opt.Ignore());



            CreateMap<ManualPaymentDto, Payment>()
                  .ForMember(dest => dest.Id, opt => opt.Ignore())
                  .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => PaymentStatus.Completed))
                  .ForMember(dest => dest.PaymobOrderId, opt => opt.Ignore())
                  .ForMember(dest => dest.ReceipeUrl, opt => opt.Ignore())
                  .ForMember(dest => dest.TransactionId, opt => opt.Ignore())
                  .ForMember(dest => dest.Fee, opt => opt.Ignore())
                  .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                  .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                  .ForMember(dest => dest.Student, opt => opt.Ignore());

            CreateMap<Payment, PaymentResponceDto>();






        }

    }
}
