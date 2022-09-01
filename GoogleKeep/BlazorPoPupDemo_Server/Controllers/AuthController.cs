using Google.Apis.Auth.OAuth2;
using Google.Apis.Books.v1;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
namespace BlazorPoPupDemo_Server.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        static string[] Scopes = { DriveService.Scope.DriveReadonly };
        static string ApplicationName = "Adizon";

        [HttpGet("signin-google")]
        public async Task<ActionResult> Google()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/"
            };
            

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
       


        [HttpGet("logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext
                   .SignOutAsync(
                   CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }
    }
}
