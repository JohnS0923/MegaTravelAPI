using MegaTravelAPI.Data;

namespace MegaTravelAPI.Models
{
    public class TripPaymentResponseModel
    {
        public virtual TripPayment TripPayment { get; set; } = null!;
        public int StatusCode { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; } 
        public int userId { get; set; }
    }
}
