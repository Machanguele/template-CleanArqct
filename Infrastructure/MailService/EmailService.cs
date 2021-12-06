using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.MailService
{
    public class EmailService: IEmailSenderService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public async Task SendEmailAsync(List<string> emails, string subject, string message)
        {
            await Execute(_configuration["SendGrid:ApiKey"], subject, message, emails);
        } 

        public Task Execute(string apiKey, string subject, string message, List<string> emails)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage
            {
                From = new EmailAddress(_configuration["SendGrid:SenderEmail"], _configuration["SendGrid:SenderName"]),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };

            foreach (var email in emails)
            {
                msg.AddTo(new EmailAddress(email));
            }

            Task response =  client.SendEmailAsync(msg);
            Console.WriteLine("Response");
            Console.WriteLine(response.IsCompletedSuccessfully);
            return response;
        }

        
        public string GetEmailBodyForUserCredentials(string fullName ,string email, string password)
        {
            return $@"
                <div style='font-size:15px;font-family: Roboto,Helvetica,Arial,sans-serif;padding:2rem;'>
                    <h1>Registration successFull</h2>
                    <p style='color:#000'>
                       Dear user, {fullName} you have been have access to Complaints and Grievance Management system.
                    </p>
                        
                     <h2>Access Credentials</h2>  
                      <p><b> Email: </b> {email} and <b>Password: </> {password}</p>
                    </br>
               
                    <footer>Thanks, APM Team.</footer>
                </div>";
        }
    }
}