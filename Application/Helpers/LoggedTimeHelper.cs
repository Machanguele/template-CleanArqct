using System;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Application.Helpers
{
    public class LoggedTimeHelper : ILoggedTime
    {
        private readonly IConfiguration _configuration;

        public LoggedTimeHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public bool SessionExpired(DateTime loggedAt)
        {
            var newDate = new DateTime();

            var difYears = (newDate.Year - loggedAt.Year) *365*24*60;
            var difMonths = (newDate.Month - loggedAt.Month) *30*24*60;
            var difDays= (newDate.Day - loggedAt.Day) *24*60;
            var difMinutes = newDate.Minute - loggedAt.Minute;

            var tokenMinutes = Convert.ToDouble(_configuration["Token:Duration"]);

            return (difYears + difMonths + difDays + difMinutes) > tokenMinutes;
        }
    }
}