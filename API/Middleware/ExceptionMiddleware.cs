using System.Net;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Application.Errors;
using System.Text.Json;
using Application.Helpers;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger,
        IHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            
            catch (WebException ex) 
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int) ex.Status;
                
                var response = _env.IsDevelopment()
                    ? new ApiException((HttpStatusCode) ex.Status, ex.Message, ex.StackTrace)
                    : new ApiException((HttpStatusCode) ex.Status, ex.Message);
                
                FileLoggerHelper.SaveLog(_configuration, (int) ex.Status, ex.Message, ex.StackTrace);
                
                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
            
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

                var response = _env.IsDevelopment()
                    ? new ApiException((HttpStatusCode) (int) HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString())
                    : new ApiException((HttpStatusCode) (int) HttpStatusCode.InternalServerError, ex.Message);
                
                FileLoggerHelper.SaveLog(_configuration, (int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace);
                
                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}