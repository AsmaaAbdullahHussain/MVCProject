using Microsoft.EntityFrameworkCore;
using mvc.Models;
using mvc.RepoInterfaces;
using MVC.Models;

namespace mvc.Repositories
{
    public class BussinessRepository : GeniricRepository<int, Business>, IBussinessRepository,IGeniricRepository<int, Business>
    {
        public BussinessRepository(ProjectContext context) : base(context)
        {

        }


    }
}
