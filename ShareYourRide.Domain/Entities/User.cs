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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string FinCode { get; set; }
        public string Email { get; set; }
        public string? ProfileImagePath { get; set; }


        public UserStatus Status { get; set; } = UserStatus.Pending;
        public string? RejectionReason { get; set; }

        public Vehicle? Vehicle { get; set; }
        public Wallet? Wallet { get; set; }
        public ICollection<Trajectory> Trajectories { get; set; } = new List<Trajectory>();
    }
}
