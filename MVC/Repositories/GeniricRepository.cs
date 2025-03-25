using mvc.RepoInterfaces;
using MVC.Models;

namespace mvc.Repositories
{
    public class GeniricRepository<Id,T>: IGeniricRepository<Id, T> where T : class
    {
        protected ProjectContext _context;
        public GeniricRepository(ProjectContext context)
        {
            _context = context;
        }

    }
}
