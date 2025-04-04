using mvc.Models;
using mvc.RepoInterfaces;
using MVC.Models;

namespace mvc.Repositories
{
    public class ReviewRepository: GeniricRepository<int, Review>, IReviewRepository,IGeniricRepository<int, Review>
    {
        public ReviewRepository(ProjectContext context) : base(context)
        {
        }

    }
}
