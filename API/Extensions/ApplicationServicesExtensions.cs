using System.Linq;
using Application.Errors;
using Application.Helpers;
using Application.Interfaces;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.MailService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(ILocalizerHelper<>), typeof(LocalizerHelper<>));
            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddScoped<ISpecificUser, SpecificUser>();
            services.AddScoped<IJwtGenerator, JwtGenerator>();
            services.AddScoped<IEmailSenderService, EmailService>();
            services.AddScoped<ILoggedTime, LoggedTimeHelper>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage).ToArray();

                    var errorResponse = new ApiValidationErrorResponse
                    {
                        Errors = errors
                    };
                    
                    return new BadRequestObjectResult(errorResponse);
                };
            });
            return services;
        }
        
    }
}