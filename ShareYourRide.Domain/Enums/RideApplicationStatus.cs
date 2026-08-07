using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Domain.Enums
{
    public enum RideApplicationStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Completed = 3   // Review addımı üçün lazım olacaq (3-cü tapşırıqda istifadə ediləcək)
    }
}