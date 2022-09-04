using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace BlazorPoPupDemo_Server.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpGet("signin-google")]
        public async Task<ActionResult> Google()
        {
            var properties = new AuthenticationProperties{RedirectUri = "/"};

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
