
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace NotifyAppWithSignalR.Domain
{
    [Authorize]
    public class SignalRHub:Hub
    {
        UserManager<IdentityUser> _userManager;
        Repository _repository;
        public SignalRHub(UserManager<IdentityUser> userManager, Repository repo)
        {
            _userManager = userManager;
            _repository = repo;
        }

        public async Task SendPrivate(string userId, string message)
        {
            var userTask = _userManager.FindByIdAsync(userId);
            userTask.Wait();

            if (userTask.IsFaulted == false && userTask.Result != null) 
            {
                _repository._context.Messages.Add(new Message() 
                {
                    Body=message,
                    IsSeen = false,
                    ReciverId = userId,
                    SenderId = Context.UserIdentifier,
                    CreatedDate = DateTime.UtcNow,
                });

                _repository._context.SaveChanges();
                await Clients.User(userId).SendAsync("MessageRecived", Context.UserIdentifier, userTask.Result.UserName, message, DateTime.Now);
            }
        }
    }
}
