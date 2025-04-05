using mvc.Models;

namespace mvc.RepoInterfaces
{
    public interface IReviewRepository : IGeniricRepository<int,Review>
    {
        bool IsExist(string email);
    }
}
