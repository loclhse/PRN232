namespace Application.Service
{
    public interface IMailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
