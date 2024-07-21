using Microsoft.AspNetCore.Identity;

namespace NotifyAppWithSignalR.Models
{
    public class MainModel
    {
        public List<IdentityUser> Users { get; set; } = new List<IdentityUser>();

        public IdentityUser CurrentUser { get; set; }
    }
}
