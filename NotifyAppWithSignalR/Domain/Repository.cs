using NotifyAppWithSignalR.Data;

namespace NotifyAppWithSignalR.Domain
{
    public class Repository
    {
        public ApplicationDbContext _context;
        public Repository(ApplicationDbContext context) 
        {
            _context = context;
        }
    }
}
