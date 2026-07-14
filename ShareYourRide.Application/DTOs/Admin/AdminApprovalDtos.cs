using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Application.DTOs.Admin
{
    public class ApproveUserDto
    {
        [Required] public Guid UserId { get; set; }
    }

    public class RejectUserDto
    {
        [Required] public Guid UserId { get; set; }
        [Required] public string Reason { get; set; } = default!;   // sabit siyahıdan seçilir (Service qatında validate olunacaq)
    }

    public class PendingUserListItemDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = default!;
        public string FinCode { get; set; } = default!;   // admin panelində tam görünür
        public bool HasVehicle { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
