﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc.Enums;
using mvc.Models;
using mvc.RepoInterfaces;
using mvc.ViewModels;
using MVC.Models;
using System.Linq.Expressions;
using System.Security.Claims;

namespace mvc.Controllers
{
    public class BusinessAdminController : Controller
    {
        private readonly IBussinessRepository DbBusiness;
        private readonly ICategoryReposiotry Dbcategory;
        private readonly IBusinessFeaturesRepoisitory DbBusinessFeatures;
        private readonly ProjectContext _context;

        public BusinessAdminController(IBussinessRepository bussinessRepository, 
                                IBusinessFeaturesRepoisitory businessFeaturesReposiotry, 
                                ICategoryReposiotry categoryReposiotry,
                                ProjectContext context)
        {
            this.DbBusiness = bussinessRepository;
            this.DbBusinessFeatures = businessFeaturesReposiotry;
            this.Dbcategory = categoryReposiotry;
            this._context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // return all business if pageNumber or size == Null 
        // or return some business if we want to divide the view to some pages
        public IActionResult GetAll(int packetId = 0, int pageNumber = 0, int size = 0)
        {
            List<Business> businessList = DbBusiness.GetAll(packetId, pageNumber, size).ToList();
            return View("GetAll", businessList);
        }

        public IActionResult Add()
        {
            BusinessViewModel business = new BusinessViewModel();
            business.categories = Dbcategory.GetAll().ToList();
            business.businessesNameList = DbBusiness.GetAll().Select(b => b.Name).ToList();
            
            try
            {
                // إضافة قائمة الباقات (مع مراعاة الخصائص المتاحة في Package class)
                var packages = _context.Packages.ToList();
                if (packages != null && packages.Any())
                {
                    business.Packages = packages;
                }
                else
                {
                    // إضافة باقة افتراضية إذا لم يتم العثور على أي باقات
                    business.Packages = new List<Package> { 
                        new Package { 
                            Id = 1, 
                            Name = "Regular",
                            // Remove the Price property which doesn't exist
                            MonthlyPrice = 0,
                            YearlyPrice = 0
                            // Description is also removed since it might not exist
                        } 
                    };
                }
            }
            catch (Exception ex)
            {
                // تسجيل الخطأ ولكن لا تدع التطبيق يتوقف
                Console.WriteLine($"Error loading packages: {ex.Message}");
                business.Packages = new List<Package>();
            }
            
            return View("Add", business);
        }

        [HttpPost]
        public async Task<IActionResult> Save(BusinessViewModel busFromReq)
        {
            try
            {
                // التأكد من تسجيل دخول المستخدم
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError("", "يجب تسجيل الدخول لإضافة عمل تجاري");
                    busFromReq.categories = Dbcategory.GetAll().ToList();
                    busFromReq.businessesNameList = DbBusiness.GetAll().Select(b => b.Name).ToList();
                    return View("Add", busFromReq);
                }

                if (ModelState.IsValid)
                {
                    // التحقق من عدم تكرار اسم العمل
                    bool isExist = await DbBusiness.IsBusinessExistAsync(busFromReq.Name);
                    if (isExist)
                    {
                        busFromReq.categories = Dbcategory.GetAll().ToList();
                        busFromReq.businessesNameList = DbBusiness.GetAll().Select(b => b.Name).ToList();
                        ModelState.AddModelError("Name", "This name is already in use");                    
                        return View("Add", busFromReq);
                    }

                    // استخدام معاملة قاعدة بيانات واحدة لضمان تكامل العملية
                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            // تعيين باقة Regular المجانية افتراضيًا
                            int defaultPackageId = 1; // باقة Regular المجانية

                            Console.WriteLine($"Saving new business: {busFromReq.Name}, Category: {busFromReq.CategoryId}, Owner: {userId}");

                            // إنشاء كائن العمل التجاري
                            Business NewBusiness = new Business
                            {
                                Name = busFromReq.Name,
                                Longitude = busFromReq.Longitude,
                                Latitude = busFromReq.Latitude,
                                CategoryId = busFromReq.CategoryId,
                                Description = busFromReq.Description,
                                MainImage = busFromReq.MainImage,
                                Address = busFromReq.Address,
                                OwnerId = userId,
                                SubscriptionEndDate = DateTime.UtcNow.AddMonths(1),
                                IsActive = true,
                                PackageId = defaultPackageId,
                                BusinessType = BusinessType.Regular
                            };

                            // حفظ العمل التجاري
                            await DbBusiness.AddAsync(NewBusiness);
                            int saveResult = await DbBusiness.SaveAsync();
                            if (saveResult <= 0)
                            {
                                throw new Exception("Failed to save business record to database");
                            }
                            
                            Console.WriteLine($"Business saved successfully with result: {saveResult}");

                            // الحصول على معرف العمل المضاف
                            int newBusinessId = DbBusiness.getIdByName(NewBusiness.Name);
                            if (newBusinessId <= 0)
                            {
                                throw new Exception("Retrieved ID is invalid");
                            }
                            Console.WriteLine($"Retrieved business ID: {newBusinessId}");

                            // معالجة ميزات العمل التجاري
                            if (busFromReq.BusinessFeatures != null && busFromReq.BusinessFeatures.Any())
                            {
                                int featuresAdded = 0;
                                Console.WriteLine($"Processing {busFromReq.BusinessFeatures.Count} business features");
                                
                                foreach (var feature in busFromReq.BusinessFeatures)
                                {
                                    if (!string.IsNullOrWhiteSpace(feature.Name))
                                    {
                                        feature.BusinessId = newBusinessId;
                                        await DbBusinessFeatures.AddAsync(feature);
                                        featuresAdded++;
                                    }
                                }

                                if (featuresAdded > 0)
                                {
                                    int featuresSaveResult = await DbBusinessFeatures.SaveAsync();
                                    Console.WriteLine($"Saved {featuresAdded} features with result: {featuresSaveResult}");
                                }
                            }

                            // معالجة ساعات العمل
                            var openingHourRepository = HttpContext.RequestServices.GetService<IOpeningHourRepository>();
                            if (openingHourRepository == null)
                            {
                                // Log the error
                                Console.WriteLine("Error: Could not resolve IOpeningHourRepository");
                                // Handle the missing service appropriately - either throw an exception or continue without it
                            } 
                            else 
                            {
                                // Use the service
                                if (busFromReq.OpeningHours != null && busFromReq.OpeningHours.Any())
                                {
                                    Console.WriteLine($"Processing {busFromReq.OpeningHours.Count} business hours records");
                                    
                                    int hoursAdded = 0;
                                    
                                    foreach (var hour in busFromReq.OpeningHours)
                                    {
                                        hour.BusinessId = newBusinessId;
                                        await openingHourRepository.AddAsync(hour);
                                        hoursAdded++;
                                    }

                                    if (hoursAdded > 0)
                                    {
                                        await openingHourRepository.SaveAsync();
                                        Console.WriteLine($"Saved {hoursAdded} business hours records");
                                    }
                                }
                            }

                            await transaction.CommitAsync();
                            TempData["Success"] = "تم إنشاء العمل التجاري بنجاح!";
                            return RedirectToAction("GetAll");
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            Console.WriteLine($"Transaction rolled back: {ex.Message}");
                            throw; // إعادة رمي الاستثناء للتعامل معه في كتلة catch الخارجية
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // تسجيل الخطأ
                Console.WriteLine($"Error saving business: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                ModelState.AddModelError("", "حدث خطأ أثناء حفظ بيانات العمل التجاري. يرجى المحاولة مرة أخرى.");
            }

            // إعادة تحميل البيانات المطلوبة للنموذج في حالة الخطأ
            busFromReq.categories = Dbcategory.GetAll().ToList();
            busFromReq.businessesNameList = DbBusiness.GetAll().Select(b => b.Name).ToList();
            return View("Add", busFromReq);
        }

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
                BusinessFeatures = DbBusinessFeatures.GetAll(b => b.BusinessId == business.Id).ToList(),
                businessesNameList = DbBusiness.GetAll().Select(b => b.Name).ToList()
            };

            // الحصول على ساعات العمل
            var openingHourRepository = HttpContext.RequestServices.GetService<IOpeningHourRepository>();
            if (openingHourRepository == null)
            {
                // Log the error
                Console.WriteLine("Error: Could not resolve IOpeningHourRepository");
                // Handle the missing service appropriately - either throw an exception or continue without it
            } 
            else 
            {
                // Use the service
                viewModel.OpeningHours = await openingHourRepository.GetByBusinessIdAsync(id);
            }

            return View("Edit", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveEdit(BusinessViewModel busFromReq)
        {
            if (!ModelState.IsValid)
            {
                await ReloadBusinessData(busFromReq);
                return View("Edit", busFromReq);
            }

            // بدء transaction واحد فقط
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // التحقق من تكرار الاسم
                var nameExists = await DbBusiness.GetAll()
                    .AnyAsync(b => b.Id != busFromReq.Id && b.Name == busFromReq.Name);

                if (nameExists)
                {
                    ModelState.AddModelError("Name", "هذا الاسم مستخدم بالفعل");
                    await ReloadBusinessData(busFromReq);
                    return View("Edit", busFromReq);
                }

                // جلب وتحديث البيانات الأساسية
                var businessToUpdate = await DbBusiness.GetByIdAsync(busFromReq.Id);
                if (businessToUpdate == null) return NotFound("العمل التجاري غير موجود");

                // التحقق من الملكية
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (businessToUpdate.OwnerId != currentUserId && !User.IsInRole("Admin"))
                {
                    return Forbid("لا يمكنك تعديل عمل تجاري لا تملكه");
                }

                // تحديث الخصائص
                businessToUpdate.Name = busFromReq.Name;
                businessToUpdate.Longitude = busFromReq.Longitude;
                businessToUpdate.Latitude = busFromReq.Latitude;
                businessToUpdate.CategoryId = busFromReq.CategoryId;
                businessToUpdate.Description = busFromReq.Description;
                businessToUpdate.MainImage = busFromReq.MainImage;
                businessToUpdate.Address = busFromReq.Address;
                businessToUpdate.IsActive = busFromReq.IsActive;

                // تحديث الميزات
                await UpdateBusinessFeatures(busFromReq);

                // تعديل طريقة تحديث ساعات العمل لتجنب transaction جديد
                await UpdateOpeningHoursWithoutTransaction(busFromReq.Id, busFromReq.OpeningHours);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "تم تحديث العمل التجاري بنجاح";
                return RedirectToAction("GetAll");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", $"حدث خطأ: {ex.Message}");
                await ReloadBusinessData(busFromReq);
                return View("Edit", busFromReq);
            }
        }

        private async Task UpdateOpeningHoursWithoutTransaction(int businessId, List<OpeningHour> openingHours)
        {
            if (openingHours == null || !openingHours.Any()) return;

            // حذف الساعات القديمة
            var existingHours = await _context.OpeningHours
                .Where(h => h.BusinessId == businessId)
                .ToListAsync();

            _context.OpeningHours.RemoveRange(existingHours);

            // إضافة الساعات الجديدة
            foreach (var hour in openingHours)
            {
                hour.BusinessId = businessId;
                _context.OpeningHours.Add(hour);
            }
        }

        private async Task ReloadBusinessData(BusinessViewModel model)
        {
            model.categories = await Dbcategory.GetAll().ToListAsync();
            model.BusinessFeatures = await DbBusinessFeatures.GetAll(b => b.BusinessId == model.Id).ToListAsync();
            model.businessesNameList = await DbBusiness.GetAll().Select(b => b.Name).ToListAsync();
        }

        private async Task UpdateBusinessFeatures(BusinessViewModel model)
        {
            if (model.BusinessFeatures == null || !model.BusinessFeatures.Any()) return;

            var existingFeatures = await DbBusinessFeatures.GetAll(f => f.BusinessId == model.Id).ToListAsync();
            foreach (var feature in existingFeatures)
            {
                await DbBusinessFeatures.DeleteAsync(feature.Id);
            }

            foreach (var feature in model.BusinessFeatures.Where(f => !string.IsNullOrWhiteSpace(f.Name)))
            {
                feature.BusinessId = model.Id;
                await DbBusinessFeatures.AddAsync(feature);
            }
        }
        public IActionResult GetBusinessByUserId(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("معرف المستخدم مطلوب");
                }
                
                List<Business> businesses = DbBusiness.GetAll().Where(b => b.OwnerId == id).ToList();
                if (businesses.Count == 0)
                {
                    return Content("لا توجد أعمال تجارية لهذا المستخدم");
                }
                
                return View("myBusiness", businesses);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user businesses: {ex.Message}");
                return StatusCode(500, "حدث خطأ أثناء استرجاع الأعمال التجارية");
            }
        }

        public async Task<IActionResult> GetBusinessById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("معرف العمل التجاري غير صحيح");
                }
                
                Business business = await DbBusiness.GetByIdAsync(id);
                if (business == null)
                {
                    return NotFound("لم يتم العثور على العمل التجاري");
                }
                
                return View(business);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching business: {ex.Message}");
                return StatusCode(500, "حدث خطأ أثناء استرجاع العمل التجاري");
            }
        }

        [HttpGet]
        public IActionResult GetUserBusinessIds()
        {
            try
            {
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "المستخدم غير مسجل الدخول", data = new int[0] });
                }
                
                var businessIds = DbBusiness.GetAll()
                    .Where(b => b.OwnerId == userId)
                    .Select(b => b.Id)
                    .ToList();
                
                return Json(new { success = true, data = businessIds });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user business IDs: {ex.Message}");
                return Json(new { success = false, message = "حدث خطأ أثناء استرجاع معرفات الأعمال التجارية", data = new int[0] });
            }
        }

        [HttpGet]
        public IActionResult Search(string searchTerm, int pageNumber = 0, int size = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return RedirectToAction("GetAll");
                }
                
                var businesses = DbBusiness.GetAll(
                    b => b.Name.Contains(searchTerm) || b.Description.Contains(searchTerm),
                    pageNumber, 
                    size
                ).ToList();
                
                ViewBag.SearchTerm = searchTerm;
                
                return View("SearchResults", businesses);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching businesses: {ex.Message}");
                TempData["Error"] = "حدث خطأ أثناء البحث عن الأعمال التجارية";
                return RedirectToAction("GetAll");
            }
        }
    }
}
