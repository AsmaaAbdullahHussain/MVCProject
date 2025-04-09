using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc.Enums;
using mvc.Models;
using mvc.RepoInterfaces;
using mvc.ViewModels;
using System.Linq.Expressions;
using System.Security.Claims;

namespace mvc.Controllers
{
    public class BusinessController : Controller
    {
        private readonly IBussinessRepository DbBusiness;
        private readonly ICategoryReposiotry Dbcategory;
        private readonly IBusinessFeaturesRepoisitory DbBusinessFeatures;
        public BusinessController(IBussinessRepository bussinessRepository, IBusinessFeaturesRepoisitory DbBusinessFeatures, ICategoryReposiotry categoryReposiotry)
        {
            this.DbBusiness = bussinessRepository;
            this.DbBusinessFeatures = DbBusinessFeatures;
            this.Dbcategory = categoryReposiotry;
        }
        public IActionResult Index()
        {
            return View();
        }

        // return all business if pageNumber or size == Null 
        // or return some business if we want to divid the view to some pages
        public IActionResult GetAll(int packetId,int pageNumber, int size)
        {
            List<Business> businessList = DbBusiness.GetAll(packetId, pageNumber, size).ToList();

            return View("GetAll", businessList);
        }
        public IActionResult Add()
        {
            BusinessViewModel business = new BusinessViewModel();
            business.categories = Dbcategory.GetAll().ToList();

            business.businessesNameList = DbBusiness.GetAll().Select(b=>b.Name).ToList();

            return View("Add", business);
        }
        [HttpPost]
        public  async Task<IActionResult> Save(BusinessViewModel busFromReq)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool isExist = await DbBusiness.IsBusinessExistAsync(busFromReq.Name);

                    if (isExist)
                    {
                        busFromReq.categories = Dbcategory.GetAll().ToList();
                        busFromReq.businessesNameList = DbBusiness.GetAll().Select(b => b.Name).ToList();
                        ModelState.AddModelError("Name", "this Name is used before");
                        return View("Add", busFromReq);
                    }

                    Business NewBusiness = new Business
                    {
                        Name = busFromReq.Name,
                        Longitude = busFromReq.Longitude,
                        Latitude = busFromReq.Latitude,
                        CategoryId = busFromReq.CategoryId,
                        Description = busFromReq.Description,
                        MainImage = busFromReq.MainImage,
                        Address = busFromReq.Address,
                        OwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        SubscriptionEndDate = DateTime.UtcNow.AddMonths(1),
                        IsActive = false
                    };

                    await DbBusiness.AddAsync(NewBusiness);
                    await DbBusiness.SaveAsync();

                    int NewBusinessid = DbBusiness.getIdByName(NewBusiness.Name);

                    if(busFromReq.BusinessFeatures != null)
                    {
                        foreach (BusinessFeatures feature in busFromReq.BusinessFeatures)
                        {
                            feature.BusinessId = NewBusinessid;
                            await DbBusinessFeatures.AddAsync(feature);
                        }
                    }


                    return RedirectToAction("GetAll");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Something went wrong during save.");
                }
            }
            busFromReq.categories = Dbcategory.GetAll().ToList();
            busFromReq.businessesNameList = DbBusiness.GetAll().Select(b => b.Name).ToList();
            return View("Add", busFromReq);
        }


        //public async Task<IActionResult> Search(string searchTerm, int? pageNumber, int? size)
        //{
        //    if (string.IsNullOrEmpty(searchTerm))
        //    {
        //        return RedirectToAction("GetAll"); 
        //    }

        //    var results = DbBusiness.Search(b =>
        //        b.Name.Contains(searchTerm) || b.Description.Contains(searchTerm)
        //        , pageNumber, size).ToList();

        //    return View("SearchResults", results); 
        //}

        public async Task<IActionResult> Delete(int id)
        {
            var business = await DbBusiness.GetByIdAsync(id);

            if (business == null)
            {
                return NotFound(); 
            }

            await DbBusiness.DeleteAsync(id);
            await DbBusiness.SaveAsync(); 

            return RedirectToAction("GetAll"); 
        }
        public async Task<IActionResult> Edit(int id)
        {

            var business = await DbBusiness.GetByIdAsync(id);

            if (business == null)
            {
                return NotFound();
            }

            BusinessViewModel viewModel = new BusinessViewModel
            {
                Id = business.Id,
                Name = business.Name,
                CategoryId = business.CategoryId,
                Description = business.Description,
                MainImage = business.MainImage,
                Latitude = business.Latitude,
                Longitude = business.Longitude,
                Address = business.Address,
                IsActive = business.IsActive,
                SubscriptionEndDate = business.SubscriptionEndDate,
                categories = Dbcategory.GetAll().ToList(),
                BusinessFeatures = DbBusinessFeatures.GetAll()?.Where(b => b.BusinessId== business.Id).ToList(),
                businessesNameList = DbBusiness.GetAll().Select(b => b.Name).ToList()
            };
            return View("Edit",viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> SaveEdit(BusinessViewModel busFromReq)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    bool isExist = await DbBusiness.IsBusinessExistAsync(busFromReq.Name);

                    if (isExist)
                    {
                        ModelState.AddModelError("Name", "this Name is used before");
                        return View("Add", busFromReq);
                    }

                    Business oldBusiness = await DbBusiness.GetByIdAsync(busFromReq.Id);

                    oldBusiness.Id = busFromReq.Id;
                    oldBusiness.Name = busFromReq.Name;
                    oldBusiness.Longitude = busFromReq.Longitude;
                    oldBusiness.Latitude = busFromReq.Latitude;
                    oldBusiness.CategoryId = busFromReq.CategoryId;
                    oldBusiness.Description = busFromReq.Description;
                    oldBusiness.MainImage = busFromReq.MainImage;
                    oldBusiness.Address = busFromReq.Address;

                    oldBusiness.OwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    oldBusiness.SubscriptionEndDate = DateTime.UtcNow.AddMonths(1);

                   

                    DbBusiness.Update(oldBusiness);
                    await DbBusiness.SaveAsync();
                    //return just new features
                    if(busFromReq.BusinessFeatures != null)
                    {
                        foreach (BusinessFeatures feature in busFromReq.BusinessFeatures)
                        {
                            feature.BusinessId = busFromReq.Id;
                            await DbBusinessFeatures.AddAsync(feature);
                        }
                    }

                    return RedirectToAction("GetAll");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "some thing is ronge during save");
                }
            }
            busFromReq.categories = Dbcategory.GetAll().ToList();
            busFromReq.BusinessFeatures = DbBusinessFeatures.GetAll()?.Where(b => b.BusinessId == busFromReq.Id).ToList();
            busFromReq.businessesNameList = DbBusiness.GetAll().Select(b => b.Name).ToList();

            return View("Edit", busFromReq);
        }

        public IActionResult getBusinessByUserId(string id)
        {
            List<Business> businesses = DbBusiness.GetAll().Where(b=>b.OwnerId==id).ToList();
            if (businesses.Count == 0)
            {
                return Content("no businesses");
            }
            return View("myBusiness",businesses);
        }

        public async Task<IActionResult> getBusinssById(int id)
        {
            Business business = await DbBusiness.GetByIdAsync(id);
            if (business == null)
            {
                return Content("no businesses");
            }
            return View(business);
        }
        
        [HttpGet]
        public IActionResult GetUserBusinessIds()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new int[0]);
            }
            
            var businessIds = DbBusiness.GetAll()
                .Where(b => b.OwnerId == userId)
                .Select(b => b.Id)
                .ToList();
            
            return Json(businessIds);
        }
 

    }
}
