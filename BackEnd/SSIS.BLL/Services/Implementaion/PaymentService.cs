using AutoMapper;
using Microsoft.Extensions.Configuration;
using Paymob.Net.Models;
using SSIS.BLL.DTOs.Payment;
using SSIS.BLL.Responce;
using SSIS.BLL.Services.Interfaces;
using SSIS.Domain.Entities;
using SSIS.Domain.Enum;
using SSIS.Domain.Interfaces;
using System.Net.Http.Json;
using static System.Net.Mime.MediaTypeNames;

namespace SSIS.BLL.Services.Implementaion
{
    public class PaymentService : IPaymentService
    {
            private readonly HttpClient _httpClient;
            private readonly IConfiguration _config;
            private readonly IpaymentRepo _paymentRepo;
            private readonly IFeeRepo _feeRepo;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public PaymentService(HttpClient httpClient, IConfiguration config, IpaymentRepo paymentRepo, IFeeRepo feeRepo, IUnitOfWork unitOfWork, IMapper mapper)
            {
                _httpClient = httpClient;
                _config = config;
                _paymentRepo = paymentRepo;
                _feeRepo = feeRepo;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            private static string? _cachedToken;
            private static DateTime _tokenExpiry = DateTime.MinValue;

            private async Task<string> GetAuthTokenAsync()
            {
                if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry)
                    return _cachedToken;

                var request = new { api_key = _config["Paymob:ApiKey"] };
                var response = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/auth/tokens", request);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

                _cachedToken = result?.token;
                _tokenExpiry = DateTime.UtcNow.AddMinutes(50);

                return _cachedToken!;
            }

            private async Task<string> CreateOrderAsync(string token, long amountCents)
            {
                var request = new
                {
                    auth_token = token,
                    delivery_needed = false,
                    amount_cents = amountCents,
                    currency = "EGP"
                };
                var response = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/ecommerce/orders", request);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<OrderResponse>();
                return result?.id.ToString();
            }

            private async Task<string> GetPaymentKeyAsync(string token, string orderId, long amountCents, string integrationId, object billingData)
            {
                var request = new
                {
                    auth_token = token,
                    amount_cents = amountCents,
                    expiration = 3600,
                    order_id = orderId,
                    billing_data = billingData,
                    currency = "EGP",
                    integration_id = integrationId
                };

                var response = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/acceptance/payment_keys", request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    // 🔴 سجل الخطأ من Paymob
                    throw new Exception($"Paymob error: {response.StatusCode} - {responseBody}");
                }

                var result = await response.Content.ReadFromJsonAsync<PaymentKeyResponse>();
                return result?.token;
            }
            #region InitiatePaymentAsync
            public async Task<Responce<PaymentResponceDto>> InitiatePaymentAsync(InitiatePaymentDto dto, Guid StudentId)
            {
                var fee = await _feeRepo.GetByIdWithStudentAsync(dto.feeId);
                if (fee == null)
                {
                    return new Responce<PaymentResponceDto>(null!, false, "Fee not found");

                }
                if (fee.StudentId != StudentId)
                {
                    return new Responce<PaymentResponceDto>(null!, false, "You are not authorized to pay this fee");

                }

                var AmountDue = fee.TotalAmount - fee.PaidAmount;
                if (dto.Amount <= 0 || dto.Amount > AmountDue)
                {
                    return new Responce<PaymentResponceDto>(null!, false, $"Amount must be between 1 and {AmountDue}");

                }
                var AmountnCents = (long)(dto.Amount * 100);
                var token = await GetAuthTokenAsync();
                if (string.IsNullOrEmpty(token))
                    return new Responce<PaymentResponceDto>(null!, false, "Failed to get auth token");

                var orderId = await CreateOrderAsync(token, AmountnCents);
                if (string.IsNullOrEmpty(orderId))
                    return new Responce<PaymentResponceDto>(null!, false, "Failed to create order");
                var billingData = new
                {
                    email = fee.Student.Email,
                    first_name = fee.Student.FullName.Split(' ')[0],
                    last_name = fee.Student.FullName.Split(' ').Length > 1 ? fee.Student.FullName.Split(' ')[1] : "Unknown",
                    phone_number = fee.Student.PhoneNumber ?? "01000000000",
                    country = "EG",
                    street = "Unknown",
                    building = "1",
                    floor = "1",
                    apartment = "1",
                    city = "Cairo"
                };
                var paymentToken = await GetPaymentKeyAsync(token, orderId, AmountnCents, _config["Paymob:IntegrationId"], billingData);
                if (string.IsNullOrEmpty(paymentToken))
                    return new Responce<PaymentResponceDto>(null!, false, "Failed to get payment key");
                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    FeeId = fee.Id,
                    StudentId = StudentId,
                    Amount = dto.Amount,
                    PaymentMethod = "Paymob",
                    PaymentStatus = PaymentStatus.Pending,
                    PaymobOrderId = orderId,
                    CreatedAt = DateTime.UtcNow
                };
                await _paymentRepo.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                var paymentUrl = $"https://accept.paymob.com/api/acceptance/iframes/{_config["Paymob:IframeId"]}?payment_token={paymentToken}"; var responseDto = new PaymentResponceDto
                {
                    PaymentUrl = paymentUrl,
                    Id = payment.Id,
                    Amount = dto.Amount,
                    feeId = dto.feeId,
                    PaymentDate = DateTime.UtcNow

                };
                return new Responce<PaymentResponceDto>(responseDto, true, "Payment initiated");
            } 
            #endregion
            public async Task<Responce<PaymentResponceDto>> GetPaymentbyIdAsync(Guid PaymentId)
            {
                var payment = await _paymentRepo.GetByIdAsync(PaymentId);
                if (payment == null)
                {
                    return new Responce<PaymentResponceDto>(null!, false, "Payment not found");
                }
                return new Responce<PaymentResponceDto>(_mapper.Map<PaymentResponceDto>(payment), true, "Success");
            }

            public async Task<Responce<bool>> HandelPaymobCallbackAsync(PaymobCallBackDto dto)
            {
                // سجل البيانات عشان التتبع
                Console.WriteLine($"Paymob Callback received. Type: {dto?.Type}, Success: {dto?.Obj?.Success}, OrderId: {dto?.Obj?.Order?.Id}, TransactionId: {dto?.Obj?.Id}");

                if (dto?.Obj == null || dto.Obj.Success != true)
                {
                    return new Responce<bool>(false, false, $"Transaction not successful or data missing. Success: {dto?.Obj?.Success}");
                }

                // جيب الـ payment باستخدام order id
                var orderId = dto.Obj.Order.Id.ToString();
                var payment = await _paymentRepo.GetByPaymobOrderIdAsync(orderId);
                
                if (payment == null)
                {
                    Console.WriteLine($"Payment record not found for OrderId: {orderId}");
                    return new Responce<bool>(false, false, "Payment not found in our records");
                }

                if (payment.PaymentStatus == PaymentStatus.Completed)
                {
                    return new Responce<bool>(true, true, "Payment already processed");
                }

                // ✅ هنا بنحط الـ transactionId وتحديث الحالة
                payment.PaymentStatus = PaymentStatus.Completed;
                payment.TransactionId = dto.Obj.Id.ToString();
                payment.PaymentDate = DateTime.UtcNow;

                await _paymentRepo.UpdateAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                // تحديث الفاتورة
                var fee = await _feeRepo.GetByIdAsync(payment.FeeId);
                if (fee != null)
                {
                    fee.PaidAmount += payment.Amount;
                    if (fee.PaidAmount >= fee.TotalAmount)
                        fee.feeStatus = FeeStaus.Paid;
                    else if (fee.PaidAmount > 0)
                        fee.feeStatus = FeeStaus.Partial;
                    
                    fee.UpdatedAt = DateTime.UtcNow;
                    await _feeRepo.UpdateAsync(fee);
                    await _unitOfWork.SaveChangesAsync();
                }

                return new Responce<bool>(true, true, "Callback processed successfully");
            }


            public async Task<Responce<PaymentResponceDto>> RecordManualPaymentAsync(ManualPaymentDto dto)
            {
                var fee = await _feeRepo.GetByIdAsync(dto.feeId);
                if (fee == null)
                    throw new Exception("Fee not found");

                var amountDue = fee.TotalAmount - fee.PaidAmount;
                if (dto.Amount <= 0 || dto.Amount > amountDue)
                    throw new Exception($"Amount must be between 1 and {amountDue}");

                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    FeeId = dto.feeId,
                    StudentId = dto.StudentId,
                    Amount = dto.Amount,
                    PaymentMethod = dto.PaymentMethod,
                    PaymentStatus = PaymentStatus.Completed,
                    TransactionId = dto.RefrenceNumber,
                    PaymentDate = dto.PaymentDate,
                    CreatedAt = DateTime.UtcNow
                };
                await _paymentRepo.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                fee.PaidAmount += dto.Amount;
                if (fee.PaidAmount >= fee.TotalAmount)
                    fee.feeStatus = FeeStaus.Paid;
                else if (fee.PaidAmount > 0)
                    fee.feeStatus = FeeStaus.Partial;
                fee.UpdatedAt = DateTime.UtcNow;
                await _feeRepo.UpdateAsync(fee);
                await _unitOfWork.SaveChangesAsync();

                return new Responce<PaymentResponceDto>(_mapper.Map<PaymentResponceDto>(payment), true, "Success");
            }
            public async Task<Responce<IReadOnlyList<PaymentResponceDto>>> GetPaymentsByStudentAsync(Guid studentId)
            {
                var payments = await _paymentRepo.GetByStudentIdAsync(studentId);
                var dtos = _mapper.Map<IReadOnlyList<PaymentResponceDto>>(payments);
                return new Responce<IReadOnlyList<PaymentResponceDto>>(dtos, true, "Success");
            }
        }

    public class AuthResponse
    {
        public string token { get; set; }
    }

    public class OrderResponse
    {
        public int id { get; set; }
    }

    public class PaymentKeyResponse
    {
        public string token { get; set; }
    }
}

