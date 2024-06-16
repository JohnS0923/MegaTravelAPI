using System.ComponentModel.DataAnnotations;

namespace MegaTravelAPI.Data
{
    public partial class TripPayment
    {
        [Key]
        public int PaymentId { get; set; }
        public int TripId { get; set; }
        public bool PaymentStatus { get; set; }


    }
}
