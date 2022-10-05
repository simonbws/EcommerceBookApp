
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBookApp.Utility
{
    //this class if fake implementation of email sender in order to not
    //receive an error message
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //var emailToSend = new MimeMessage();
            //emailToSend.From.Add(MailboxAddress.Parse("dotnettrainingTest@gmail.com"));
            //emailToSend.From.Add(MailboxAddress.Parse(email));
            //emailToSend.Subject = subject;
            //emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            ////sending email using smtl client
            //using (var emailClient = new SmtpClient())
            //{
            //    emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            //    emailClient.Authenticate("dotnettraining23@gmail.com", "dotnetTraining@1");
            //    emailClient.Send(emailToSend);
            //    emailClient.Disconnect(true);
            
            return Task.CompletedTask;



        }
    }
}
