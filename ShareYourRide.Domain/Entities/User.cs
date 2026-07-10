using ShareYourRide.Domain.Common;
using ShareYourRide.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Domain.Entities
{
    public class User : BaseEntity
    {
        public Guid ApplicationUserId { get; set; }   // Identity ApplicationUser-ə FK (Guid)

        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public DateTime BirthDate { get; set; }
        public string FinCode { get; set; } = default!;
        public string? ProfileImagePath { get; set; }

        public UserStatus Status { get; set; } = UserStatus.Pending;
        public string? RejectReason { get; set; }

        public Vehicle? Vehicle { get; set; }
        public Wallet? Wallet { get; set; }
        public ICollection<Trajectory> Trajectories { get; set; } = new List<Trajectory>();
    }
}
