using Microsoft.AspNetCore.Mvc;
using NotifyAppWithSignalR.Domain;

namespace NotifyAppWithSignalR.Controllers
{
    public class MessageController : BaseController
    {
        private readonly Repository _repository;

        public MessageController(Repository repo)
        {
            _repository = repo;
        }

        public IActionResult History(string id)
        {
            var data = _repository._context.Messages.Where(m => (m.ReciverId == id && m.SenderId == CurrentuserId) || (m.SenderId == id && m.ReciverId == CurrentuserId))
                .OrderBy(m => m.CreatedDate)
                .Reverse()
                .Select(item => new {message=item, isSender=item.SenderId == CurrentuserId})
                .ToList();
            return Ok(data);
        }
    }
}
