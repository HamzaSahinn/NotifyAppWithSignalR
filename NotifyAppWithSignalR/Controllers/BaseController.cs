using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace NotifyAppWithSignalR.Controllers
{
    public class BaseController : Controller
    {
        public UserManager<IdentityUser> UserManager { get; set; }

        public string CurrentuserId { get
            {
                return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier.ToString())?.Value;
            } }
    }
}
