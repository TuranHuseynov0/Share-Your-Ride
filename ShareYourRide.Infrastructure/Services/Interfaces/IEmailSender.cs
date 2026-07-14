using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Interfaces
{
    public interface IEmailSender
    {
        Task SendAsync(string email, string subject, string message);
    }
}
