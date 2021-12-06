using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Dtos;
using Application.Features.Auth.Commands.RequestModels;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Features.Auth.Commands.Handlers
{
     public class LoginHandler : IRequestHandler<LoginCommand, LoginDto>
        {
            private readonly SignInManager<AppUser> _signInManager;
            private readonly UserManager<AppUser> _userManager;
            private readonly ILocalizerHelper<LoginHandler> _localizerHelper;
            private readonly IJwtGenerator _jwtGenerator;
            private readonly DataContext _context;
            private readonly IEmailSenderService _emailSenderService;

            public LoginHandler(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager,
                ILocalizerHelper<LoginHandler> localizerHelper, IJwtGenerator jwtGenerator, DataContext context,
                IEmailSenderService emailSenderService)
            {
                _signInManager = signInManager;
                _userManager = userManager;
                _localizerHelper = localizerHelper;
                _jwtGenerator = jwtGenerator;
                _context = context;
                _emailSenderService = emailSenderService;
            }
            
            public  async Task<LoginDto> Handle(LoginCommand request, CancellationToken cancellationToken)
            {
                
                var user = await _userManager.Users
                    .Where(x => x.Email == request.Email)
                    .FirstOrDefaultAsync();
                
                
                if (user == null || user.Archived)
                { 
                    throw  new WebException(_localizerHelper.GetString("Fail to Login"),
                        (WebExceptionStatus) HttpStatusCode.BadRequest);
                }
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                
                if (result.Succeeded)
                {
                    user.LoggedIn = true;
                    await _context.SaveChangesAsync(cancellationToken);
                    var roles = await _userManager.GetRolesAsync(user);
                 
                         var authInfo = await _jwtGenerator.GenerateToken(user, false);
                         
                         //put your email, Just for test
                         string email = "developer.machanguele@gmail.com";
                         var emailList = new List<string>();
                         emailList.Add(email);

                        
                         await _emailSenderService.SendEmailAsync(emailList, "Test Email",
                             "Testing email sender");
                         
                         return  new LoginDto
                         {
                             Email = user.Email,
                             Token =  authInfo.Token,
                             RefreshToken = authInfo.RefreshToken.Token,
                             Username = user.UserName,
                             FullName = user.FullName,
                             Roles =  roles
                         };
                }
                throw  new WebException(_localizerHelper.GetString("Login Failed"),
                    (WebExceptionStatus) HttpStatusCode.BadRequest);
            }
        }
}