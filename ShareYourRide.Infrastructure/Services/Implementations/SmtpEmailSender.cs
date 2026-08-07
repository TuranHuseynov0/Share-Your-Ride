using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using ShareYourRide.Infrastructure.Services.Interfaces;

namespace ShareYourRide.Infrastructure.Services.Implementations
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(string email, string subject, string message)
        {
            var smtp = _configuration.GetSection("SmtpSettings");

            using var client = new SmtpClient(smtp["Host"], int.Parse(smtp["Port"]!))
            {
                Credentials = new NetworkCredential(smtp["Username"], smtp["Password"]),
                EnableSsl = bool.Parse(smtp["EnableSsl"]!)
            };

            var mail = new MailMessage
            {
                From = new MailAddress(smtp["FromEmail"]!, smtp["FromName"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = false
            };
            mail.To.Add(email);

            await client.SendMailAsync(mail);
        }
    }
}