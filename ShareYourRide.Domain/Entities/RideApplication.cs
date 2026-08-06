using ShareYourRide.Domain.Common;
using ShareYourRide.Domain.Enums;
using System;

namespace ShareYourRide.Domain.Entities
{
    public class RideApplication : BaseEntity
    {
        public Guid DriverTrajectoryId { get; set; }
        public Trajectory DriverTrajectory { get; set; } = default!;

        // YENİ — sərnişinin hansı öz trayektoriyası ilə müraciət etdiyi (gün/dayanacaq üst-üstə düşməsini yoxlamaq üçün)
        public Guid PassengerTrajectoryId { get; set; }
        public Trajectory PassengerTrajectory { get; set; } = default!;

        public Guid PassengerUserId { get; set; }
        public User PassengerUser { get; set; } = default!;

        public RideApplicationStatus Status { get; set; } = RideApplicationStatus.Pending;

        // YENİ — Apply anında hesablanıb saxlanılır ki, sonradan (statuslar dəyişsə belə) sabit qalsın
        public int CommonStopsCount { get; set; }
        public decimal Price { get; set; }
    }
}