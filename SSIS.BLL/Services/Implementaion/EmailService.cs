using Microsoft.Extensions.Configuration;
using MimeKit;
using SSIS.PLL.Services.Interfaces;
using System;
using System.Collections.Generic;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Text;

namespace SSIS.PLL.Services.Implementaion
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        #region SendEmailASync
        public async Task SendEmailASync(string toemail, string Subject, string Body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress
                (
                configuration["EmailSettings:FromName"],
                configuration["EmailSettings:FromEmail"]
                ));
            email.To.Add(MailboxAddress.Parse(toemail));
            email.Subject = Subject;
            email.Body = new TextPart("html") { Text = Body };

            using var stmp = new SmtpClient();
            await stmp.ConnectAsync(
                  configuration["EmailSettings:SmtpServer"],
                int.Parse(configuration["EmailSettings:SmtpPort"]),
                SecureSocketOptions.StartTls
                );
            await stmp.AuthenticateAsync(
                   configuration["EmailSettings:SmtpUsername"],
                configuration["EmailSettings:SmtpPassword"]

                );
            await stmp.SendAsync(email);
            await stmp.DisconnectAsync(true);

        } 
        #endregion
    }
}
