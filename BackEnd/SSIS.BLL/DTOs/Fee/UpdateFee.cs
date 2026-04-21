

namespace SSIS.BLL.DTOs.Fee
{
    public class UpdateFee
    {
        public decimal TotalAmount { get; set; }
        public DateTime DueDate { get; set; }
        public string? Reason { get; set; }

    }
}
