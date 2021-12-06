using System;

namespace Application.Interfaces
{
    public interface ILoggedTime
    {
        bool SessionExpired(DateTime loggedAt);

    }
}