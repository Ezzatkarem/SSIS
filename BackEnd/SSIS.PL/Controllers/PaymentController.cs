using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SSIS.BLL.DTOs.Payment;
using SSIS.BLL.Services.Implementaion;
using SSIS.BLL.Services.Interfaces;

namespace SSIS.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService paymentService;
        private readonly IUserService userService;

        public PaymentController(IPaymentService paymentService, IUserService userService)
        {
            this.paymentService = paymentService;
            this.userService = userService;
        }
        [HttpPost("initiate")]
        public async Task<IActionResult> initaitePayment([FromBody] InitiatePaymentDto dto)
        {
            try
            {
                var studentid = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
                var res=await paymentService.InitiatePaymentAsync(dto,studentid);
                if(!res.IsSuccess)
                {
                    return BadRequest(new {message=res.Message});
                }
                return Ok(res.Data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });

            }
        }
        [HttpPost("manual")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> RecordManualPayment([FromBody] ManualPaymentDto dto)
        {
            try
            {
                var res = await paymentService.RecordManualPaymentAsync(dto);
                if (!res.IsSuccess)
                {
                    return BadRequest(new { message = res.Message });
                }
                return Ok(res.Data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });

            }
        }
       

        [HttpPost("callback")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymobCallback([FromBody] PaymobCallBackDto dto)
        {
            // سجل البيانات عشان تتأكد
            Console.WriteLine($"Callback endpoint hit. OrderId: {dto?.Obj?.Order?.Id}, Success: {dto?.Obj?.Success}");

            var res = await paymentService.HandelPaymobCallbackAsync(dto);
            if (!res.IsSuccess)
                return BadRequest(new { message = res.Message });
            return Ok(res.Data);
        }
        [HttpGet("{pymentid}")]
        public async Task<IActionResult> Getpymentbyid(Guid paymentid)
        {
            var res = await paymentService.GetPaymentbyIdAsync(paymentid);
            if (!res.IsSuccess)
                return BadRequest(new { message = res.Message });
            return Ok(res.Data);
        }
        [HttpGet("student/{studentid}")]
        public async Task<IActionResult> Getpymentbystudent(Guid studentid)
        {
            var res = await paymentService.GetPaymentsByStudentAsync(studentid);
            if (!res.IsSuccess)
                return BadRequest(new { message = res.Message });
            return Ok(res.Data);
        }

        [HttpGet("my-payments")]
        public async Task<IActionResult> GetMyPayments()
        {
            var studentid =  Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var result = await paymentService.GetPaymentsByStudentAsync(studentid);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });
            return Ok(result.Data);
        }

    }
}
