using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace talker.WebUI.EmailService
{
    public class EmailSender : IEmailSender
    {
        private static string EmailHtmlHeader = "<html><head></head><body style=\"background-color:#e8e8e8;padding: 6rem 3rem;margin:5rem 0 0 0;color:#464646!important;font-family:-apple-system,BlinkMacSystemFont,&quot;Segoe UI&quot;,Roboto,&quot;Helvetica Neue&quot;,Arial,&quot;Noto Sans&quot;,sans-serif,&quot;Apple Color Emoji&quot;,&quot;Segoe UI Emoji&quot;,&quot;Segoe UI Symbol&quot;,&quot;Noto Color Emoji&quot;;line-height:1.5;\"><header><nav style=\"display:flex;margin-bottom:5px;\"><h1 style=\"margin: 0 auto;text-align:center;background-color:#e8e8e8;\"><a href=\"https://talker.ninja\" style=\"color:#1f1f1f!important;text-decoration:none;font-size:2.2rem;font-weight:500;\">Talker</a></h1></nav></header><div style=\"max-width:70%;margin: 0 auto;background-color:#e8e8e8;\"><main role=\"main\">";
        private static string EmailHtmlFooter = "</main></div></body></html>";
        public EmailSender() { }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            string fromMail = "noreply@talker.ninja";
            string fromPassword = "Admin123$";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = subject;
            message.To.Add(new MailAddress(email));
            message.Body = EmailHtmlHeader + htmlMessage + EmailHtmlFooter;
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.titan.email")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };
            smtpClient.Send(message);
        }

    }
}
