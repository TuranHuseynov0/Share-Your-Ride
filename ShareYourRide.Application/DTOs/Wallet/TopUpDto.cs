using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Application.DTOs.Wallet
{
    public class TopUpDto
    {
        [Required, Range(5, double.MaxValue, ErrorMessage = "Minimum ödəniş 5 AZN olmalıdır!")]
        public decimal Amount { get; set; }
    }
}
