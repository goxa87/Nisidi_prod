using EventB.Services.Logger;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
//using System.Net.Mail;
using System.Threading.Tasks;

namespace EventB.Services.SenderServices
{
    public class MailSender
    {

        private readonly ILogger _logger;
        public MailSender( ILogger logger)
        {
            _logger = logger;
        }

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

            emailMessage.From.Add(new MailboxAddress("support@stable-nisidi.ru"));
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

                    //await client.ConnectAsync("wpl42.hosting.reg.ru", 587, false);
                    //await client.AuthenticateAsync(new NetworkCredential("support@stable-nisidi.ru", "utjhubqrhen456*+"));
                    await client.ConnectAsync("127.0.0.1", 25, false);


                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
            }
            catch(Exception ex)
            {
                await _logger.LogStringToFile($"Новый Отправка произошла ошибка {ex.Message}\n{ex.StackTrace}");
            }            
        }
    }
}
