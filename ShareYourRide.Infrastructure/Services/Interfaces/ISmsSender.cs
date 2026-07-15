using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Interfaces
{
    public interface ISmsSender
    {   
        Task SendAsync(string phoneNumber, string message);
    }
}
