using System;
using System.Collections.Generic;

namespace MegaTravelAPI.Data
{
    public partial class Agent
    {
        public Agent()
        {
            Trips = new HashSet<Trip>();
        }

        public int AgentId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string OfficeLocation { get; set; } = null!;
        public string Phone { get; set; } = null!;

        public virtual Login LoginInfo { get; set; } = null!;
        public virtual ICollection<Trip> Trips { get; set; }
    }
}
