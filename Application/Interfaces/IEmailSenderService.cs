using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(List<string> emails, string subject, string message);
        string GetEmailBodyForUserCredentials(string fullName, string email, string password);
    }
}