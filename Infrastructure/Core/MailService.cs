using Application.Service;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

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
            var apiKey = _configuration["Email:SendGridKey"]
                ?? throw new InvalidOperationException("Email:SendGridKey is not configured.");

            var fromEmail = _configuration["Email:FromEmail"] ?? "hoangloc1908222@gmail.com";
            var fromName = _configuration["Email:FromName"] ?? "HappyBox";

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", body);

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Body.ReadAsStringAsync();
                throw new Exception($"Failed to send email via SendGrid. Status: {response.StatusCode}. Error: {errorBody}");
            }
        }
    }
}
