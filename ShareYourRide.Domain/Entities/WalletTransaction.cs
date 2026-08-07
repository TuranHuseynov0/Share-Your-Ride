using ShareYourRide.Domain.Common;
using ShareYourRide.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Domain.Entities
{
    public class WalletTransaction : BaseEntity
    {
        public Guid WalletId { get; set; }
        public Wallet Wallet { get; set; } = default!;

        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public Guid? RelatedRideApplicationId { get; set; }
    }
}
