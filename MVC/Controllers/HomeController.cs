using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc.Models;
using mvc.RepoInterfaces;

namespace mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IBussinessRepository _businessRepository;
    private readonly ICategoryReposiotry _categoryRepository;

    public HomeController(
        ILogger<HomeController> logger,
        IBussinessRepository businessRepository,
        ICategoryReposiotry categoryRepository)
    {
        _logger = logger;
        _businessRepository = businessRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        try
        {
            // Number of businesses per page
            int pageSize = 6;
            
            // Get premium/sponsored businesses (those with non-free packages)
            var sponsoredBusinesses = await _businessRepository.GetAll(b => 
                b.IsActive && 
                b.PackageId > 1// Assuming packageId 1 is the free package
                  


            )
            .Include(b => b.Category)
            .Include(b => b.BusinessFeatures)
            .Include(b => b.Reviews)
            .OrderByDescending(b => b.PackageId) // Higher package first
            .ThenByDescending(b => b.SubscriptionEndDate) // Latest subscription first
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            
            // Get categories for dropdown
            ViewBag.Categories = await _categoryRepository.GetAll().ToListAsync();
            
            // Pagination info
            var totalBusinesses = await _businessRepository.GetAll(b => 
                b.IsActive && 
                b.PackageId > 1 
              //  b.SubscriptionEndDate > DateTime.Now
            ).CountAsync();
            
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalBusinesses / pageSize);
            
            return View(sponsoredBusinesses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading home page");
            return View(new List<Business>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> SearchSuggestions(string query)
    {
        if (string.IsNullOrEmpty(query) || query.Length < 2)
            return Json(new List<string>());
            
        try
        {
            // Search in business names and descriptions
            var businessSuggestions = await _businessRepository.GetAll(b => 
                b.IsActive && 
                (b.Name.Contains(query) || b.Description.Contains(query))
            )
            .Select(b => new { text = b.Name })
            .Take(5)
            .ToListAsync();
            
            // Search in business features
            var featureSuggestions = await _businessRepository.GetAll(b => 
                b.IsActive && 
                b.BusinessFeatures.Any(f => f.Name.Contains(query))
            )
            .SelectMany(b => b.BusinessFeatures.Where(f => f.Name.Contains(query)))
            .Select(f => new { text = f.Name })
            .Distinct()
            .Take(5)
            .ToListAsync();
            
            var combinedSuggestions = businessSuggestions.Union(featureSuggestions).Take(10).ToList();
            
            return Json(combinedSuggestions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting search suggestions");
            return Json(new List<string>());
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
