using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Configs.localize;
using API.Extensions;
using API.Middleware;
using API.Workers;
using Application.Features.Auth.Commands.RequestModels;
using Application.Helpers;
using Application.Localize;
using Domain;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Persistence;

namespace API
{
    public class Startup
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        
        public void ConfigureProductionServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SQLServer"))
            );
            ConfigureServices(services);
        }

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SQLServer"))
            );
            ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services )
        {
            services.AddControllers()
                .AddFluentValidation(s =>
                {
                    s.RegisterValidatorsFromAssemblyContaining<LoginCommand>();
                    s.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                });
            services.AddControllers();
            
            services.AddMemoryCache();
            services.AddApplicationServices();
            
            services.AddIdentityCore<AppUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<DataContext>();
            services.AddIdentityServices(Configuration);

            services.AddMediatR(typeof(TestQuery.TestQuery1).Assembly);
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:4200")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });
            
            /*Console.WriteLine($"{_hostEnvironment.WebRootPath}/Resources");*/
            services.AddLocalization();
            services.AddSwaggerDocumentation();
            services.AddHostedService<InvalidateRefreshTokenWorker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataContext dataContext)
        {

            
            app.UseMiddleware<ExceptionMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseSwaggerDocumention();
            }

            app.UseStatusCodePagesWithReExecute("/errors/{0}");    

            
            var localizeData = File.ReadAllText("Configs/localize/localizer.json");
            var localizeObject = JsonSerializer.Deserialize<LocalizeStruture>(localizeData);
            
            var supportedCultures = new List<CultureInfo>();
            foreach (var culture in localizeObject.SupportedLanguages)
            {
                supportedCultures.Add(new CultureInfo(culture));
            }
            
            var requestLocalizationOptions = new RequestLocalizationOptions
            {
                /*
                DefaultRequestCulture = new RequestCulture("en-US"),
                */

                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,

                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            };
            
            app.UseRequestLocalization(requestLocalizationOptions.SetDefaultCulture(localizeObject.DefaultLanguage));


            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}