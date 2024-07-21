using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotifyAppWithSignalR.Domain
{
    public class Message
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string Body { get; set; }

        public string ReciverId { get; set; }

        public string SenderId { get; set; }

        public bool IsSeen { get; set; }

        public IdentityUser Reciver { get; set; }

        public IdentityUser Sender { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
