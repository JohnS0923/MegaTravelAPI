using System;
using System.Collections.Generic;

namespace MegaTravelAPI.Data
{
    public partial class Trip
    {
        public int TripId { get; set; }
        public int UserId { get; set; }
        public int AgentId { get; set; }
        public string TripName { get; set; } = null!;
        public string Location { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumAdults { get; set; }
        public int NumChildren { get; set; }

        public virtual TripPayment TripPayment { get; set; } = null!;

        public virtual Agent Agent { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
