using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence;


namespace API.Workers
{
    public class InvalidateRefreshTokenWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;

        public InvalidateRefreshTokenWorker(IServiceScopeFactory serviceScopeFactory
            , IConfiguration configuration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<DataContext>();
                /*var loggedTime = services.GetRequiredService<ILoggedTime>();
                var usermanager = services.GetRequiredService<UserManager<AppUser>>();*/
                
                while (!stoppingToken.IsCancellationRequested)
                {
                    var refreshTokens = await context.RefreshTokens
                        .Include(x => x.User)
                        .Where(x => !x.Invalidated && x.ExpiresAt < DateTime.Now)
                        .ToListAsync();

                    var timer = Convert.ToDouble(_configuration["LogoutWorker"]);

                    foreach (var refreshToken in refreshTokens)
                    {
                        try
                        {
                            Console.WriteLine("Do something");
                            Console.WriteLine();
                            refreshToken.Invalidated = true;
                            refreshToken.User.LoggedIn = false;
                            await context.SaveChangesAsync(cancellationToken: stoppingToken);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }
                    await Task.Delay((int) timer, stoppingToken);
                }
            }

        }
    }
}