using Application.Service;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Core
{

    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
          
            var smtpServer = _configuration["Email:SmtpHost"] ?? throw new InvalidOperationException("Email:SmtpHost is not configured.");
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUsername = _configuration["Email:SmtpUsername"] ?? throw new InvalidOperationException("Email:SmtpUsername is not configured.");
            var smtpPassword = _configuration["Email:SmtpPassword"] ?? throw new InvalidOperationException("Email:SmtpPassword is not configured.");
            
          
            var fromEmail = _configuration["Email:FromEmail"] ?? smtpUsername;
            var fromName = _configuration["Email:FromName"] ?? "HappyBox";

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
             
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
               
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}

