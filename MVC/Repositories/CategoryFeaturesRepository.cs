﻿using mvc.Models;
using mvc.RepoInterfaces;
using MVC.Models;
using System.Linq.Expressions;

namespace mvc.Repositories
{
    public class CategoryFeaturesRepository : GeniricRepository<int, CategoryFeatures>, IcategoryFeaturesRepository, IGeniricRepository<int, CategoryFeatures>
    {
        public CategoryFeaturesRepository(ProjectContext context) : base(context)
        {
        }

      
    }
}
