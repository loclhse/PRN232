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
            // Đọc config từ Email section trong appsettings.json
            var smtpServer = _configuration["Email:SmtpHost"] ?? throw new InvalidOperationException("Email:SmtpHost is not configured.");
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUsername = _configuration["Email:SmtpUsername"] ?? throw new InvalidOperationException("Email:SmtpUsername is not configured.");
            var smtpPassword = _configuration["Email:SmtpPassword"] ?? throw new InvalidOperationException("Email:SmtpPassword is not configured.");
            
            // Email để hiển thị cho người nhận
            var fromEmail = _configuration["Email:FromEmail"] ?? smtpUsername;
            var fromName = _configuration["Email:FromName"] ?? "HappyBox";

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                // Dùng SmtpUsername và SmtpPassword để xác thực
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                // Hiển thị FromEmail và FromName cho người nhận
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

