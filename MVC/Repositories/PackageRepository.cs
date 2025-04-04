using mvc.Models;
using mvc.RepoInterfaces;
using MVC.Models;

namespace mvc.Repositories
{
    public class PackageRepository : GeniricRepository<int, Package>, IPackageRepository, IGeniricRepository<int, Package>
    {
        public PackageRepository(ProjectContext context) : base(context)
        {
        }
    }
    
    
}
