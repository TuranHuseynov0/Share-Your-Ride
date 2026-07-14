using ShareYourRide.Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Implementations
{
    public class ConsoleEmailSender : IEmailSender
    {
        public Task SendAsync(string email, string subject, string message)
        {
            Console.WriteLine($"[EMAIL -> {email}] {subject}: {message}");
            return Task.CompletedTask;
        }
    }
}
