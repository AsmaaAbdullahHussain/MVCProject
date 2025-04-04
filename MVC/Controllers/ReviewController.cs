using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc.Models;
using mvc.RepoInterfaces;
using mvc.ViewModels.ReviewVM;
using System.Threading.Tasks;

namespace mvc.Controllers
{
    public class ReviewController : Controller
    {
        IReviewRepository _reviewRepository;
        public ReviewController(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }
        public async Task<IActionResult> Index()
        {
            List<Review> reviews = await _reviewRepository.GetAll().ToListAsync();
            return View(reviews);
        }
        public IActionResult Add()
        {
            return View();
        }
        public async Task<IActionResult> SaveAdd(AddReviewVM reviewVM)
        {
            if (ModelState.IsValid)
            {
                Review review = new Review
                {
                    Email = reviewVM.Email,
                    BusinessId = reviewVM.BusinessId,
                    Rating = reviewVM.Rating,
                    Comment = reviewVM.Comment
                };
                await _reviewRepository.AddAsync(review);/////////////////////
                await _reviewRepository.SaveAsync();
                RedirectToAction("Index");
            }

            return View("Add", reviewVM);

        }
        public async Task<IActionResult> Edit(int id)
        {
            Review review = await _reviewRepository.GetByIdAsync(id);
            EditReviewVM reviewVM = new EditReviewVM
            {
                Email = review.Email,
                Rating = review.Rating,
                Comment = review.Comment
            };
            if (review == null)
            {
                return NotFound();
            }
            return View(reviewVM);
        }
        public IActionResult SaveEdit(int id,EditReviewVM reviewVM)
        {
            if (ModelState.IsValid)
            {
                Review review = new Review
                {
                    Id = id,
                    Email = reviewVM.Email,
                    Rating = reviewVM.Rating,
                    Comment = reviewVM.Comment
                };
                _reviewRepository.Update(review);
                _reviewRepository.SaveAsync();
                return RedirectToAction("Index");
            }
            return View("Edit", reviewVM);
        }
        public async Task<ActionResult> Delete(int id)
        {
            await _reviewRepository.DeleteAsync(id);
            await _reviewRepository.SaveAsync();
            return RedirectToAction("Index");
        }

    }
}
