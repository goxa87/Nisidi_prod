using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services.SenderServices
{
    public class MailSender
    {
        /// <summary>
        /// Это нужно от ложить и попробовать после того как сайт окажется на сервере. нужен ссл сертификат.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="target"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendEmailAsync(string email, string target, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("mail@nisidi.ru"));
            emailMessage.To.Add(new MailboxAddress(email));
            emailMessage.Subject = target;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };
            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("127.0.0.1", 25, false);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
            }
            catch(Exception ex)
            {
                
            }            
        }
    }
}
