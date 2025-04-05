using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using mvc.Migrations;
using mvc.Models;
using mvc.RepoInterfaces;
using System.Threading.Tasks;
using mvc.ViewModels.PackageVM;

namespace mvc.Controllers
{
    public class PackageController : Controller
    {
        IPackageRepository _packageRepository;
        public PackageController(IPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }
        public async Task<IActionResult> GetAll()
        {
            List<Package> packages = await _packageRepository.GetAll().ToListAsync();
            return View(packages);

        }
        public async Task<IActionResult> GetById(int id)
        {
            Package package =await _packageRepository.GetByIdAsync(id);
            return View(package);
        }
        public IActionResult Add()
        {
            return View();
        }
        public async Task<IActionResult> SaveAdd(AddPackageVM newPackage)
        {
            Package package = new Package
            {
                Name = newPackage.Name,
                MonthlyPrice = newPackage.MonthlyPrice,
                YearlyPrice = newPackage.YearlyPrice,
            };
            await _packageRepository.AddAsync(package);
            await _packageRepository.SaveAsync();
            return RedirectToAction("GetAll");
        }
        public async Task<IActionResult> Delete(int id)
        {
            await _packageRepository.DeleteAsync(id);
            await _packageRepository.SaveAsync();
            return RedirectToAction("GetAll");
        }
        public async Task<IActionResult> Update(int id)
        {
            Package package = await _packageRepository.GetByIdAsync(id);
           
            return View(package);
        }
        public async Task<IActionResult> SaveUpdate(int id,UpdatePackageVM updatedPackage)
        {
            Package package = new Package
            {
                Id = id,
                Name = updatedPackage.Name,
                MonthlyPrice = updatedPackage.MonthlyPrice,
                YearlyPrice = updatedPackage.YearlyPrice,
            };
            _packageRepository.Update(package);
            await _packageRepository.SaveAsync();
            return RedirectToAction("GetAll");
        }

    }
}
