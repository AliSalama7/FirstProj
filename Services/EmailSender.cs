using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace MoviesApp.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var frommail = "salamaali9090@outlook.com";
            var frompassword = "01025775547Ali";

            var message = new MailMessage();
            message.From = new MailAddress(frommail);
            message.Subject = subject;
            message.To.Add(email);
            message.Body = $"<html><body>{htmlMessage}</body></html>";
            message.IsBodyHtml = true;

            var smtpclient = new SmtpClient("smtp-mail.outlook.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(frommail , frompassword),
                EnableSsl = true
            };

            smtpclient.Send(message);
        }
    }
}
