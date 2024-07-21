using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NotifyAppWithSignalR.Domain;
using NotifyAppWithSignalR.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace NotifyAppWithSignalR.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHubContext<SignalRHub> _hubContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Repository _repository;

        public HomeController(ILogger<HomeController> logger, IHubContext<SignalRHub> hubContext, UserManager<IdentityUser> userManager, Repository repository)
        {
            _logger = logger;
            _hubContext = hubContext;
            _userManager = userManager;
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var model = new MainModel();
            var x1 = _repository._context.Messages
                .Where(m => m.SenderId == CurrentuserId)
                .Select(message => message.Reciver)
                .Distinct();

            var x2 = _repository._context.Messages
                .Where(m => m.ReciverId == CurrentuserId)
                .Select(message => message.Sender)
                .Distinct();

            model.Users = x1.Union(x2).ToList();
            model.CurrentUser = await _userManager.FindByIdAsync(CurrentuserId);

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public JsonResult Users([FromQuery]string term)
        {
            return Json(_repository._context.Users.Where(user => user.UserName.Contains(term)).Select(u => new {display=u.UserName, val=u.Id}).ToList());
        }
    }
}
