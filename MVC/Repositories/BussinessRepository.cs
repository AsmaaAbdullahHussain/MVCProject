﻿using Microsoft.EntityFrameworkCore;
using mvc.Enums;
using mvc.Models;
using mvc.RepoInterfaces;
using MVC.Models;

namespace mvc.Repositories
{
    public class BussinessRepository : GeniricRepository<int, Business>, IBussinessRepository,IGeniricRepository<int, Business>
    {
        private ProjectContext _context;
        public BussinessRepository(ProjectContext context) : base(context)
        {
            this._context = context;
        }

        public int getIdByName(string name)
        {
            Business b = _context.Businesses.FirstOrDefault(b => b.Name == name);
            return b.Id;
        }
        public IQueryable<Business> GetAll(int PacketrId, int size = 0, int pageNumber = 1)
        {
            if (pageNumber > 0 && size > 0)
            {
                return dbSet.Where(b => b.PackageId == PacketrId).Skip((pageNumber - 1) * size).Take(size);
            }
            return dbSet;
        }
        public async Task<bool> IsBusinessExistAsync(string name)
        {
            return await _context.Businesses.AnyAsync(b => b.Name == name);
        }

    }
}
