using ShareYourRide.Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Implementations
{
    public class ConsoleSmsSender : ISmsSender
    {
        public Task SendAsync(string phoneNumber, string message)
        {
            Console.WriteLine($"[SMS -> {phoneNumber}]: {message}");
            return Task.CompletedTask;
        }
    }
}
