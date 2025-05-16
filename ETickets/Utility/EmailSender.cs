using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace ETickets.Utility
{
    public class EmailSender : IEmailSender
    {
        Task IEmailSender.SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("mernasaad131@gmail.com", "tayk gseb ydkn whbd")
            };

            return client.SendMailAsync(
         new MailMessage(from: "mernasaad131@gmail.com",
                         to: email,
                         subject,
                         htmlMessage
                         )
         {
             IsBodyHtml = true
         });
        }
    }
}
