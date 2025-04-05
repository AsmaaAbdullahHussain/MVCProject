using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using mvc.Models;
using mvc.RepoInterfaces;
using mvc.ViewModels.ReviewVM;
using MVC.Hubs;
using System.Threading.Tasks;

namespace mvc.Controllers
{
    public class ReviewController : Controller
    {
        IReviewRepository _reviewRepository;
        IBussinessRepository _bussinessRepository;
        private readonly IHubContext<ReviewHub> hubcontext;

        public ReviewController(IReviewRepository reviewRepository, IBussinessRepository bussinessRepository, IHubContext<ReviewHub> hubcontext)
        {
            _reviewRepository = reviewRepository;
            _bussinessRepository = bussinessRepository;
            this.hubcontext = hubcontext;
        }
        public async Task<IActionResult> GetAllByBussniss(int bussnissId)
        {
            List<Review> reviews = await _reviewRepository.GetAll(r=>r.BusinessId==bussnissId).ToListAsync();
            return View(reviews);
        }
        public IActionResult Add()
        {
            return View();
        }
        public async Task<IActionResult> SaveAdd(AddReviewVM reviewVM)
        {
            Business business = await _bussinessRepository.GetByIdAsync(reviewVM.BusinessId);
            if (business == null)
            {
                ModelState.AddModelError("BusinessId", "Business not found");
            }
            if(_reviewRepository.IsExist(reviewVM.Email))
            {
                ModelState.AddModelError("Email", "Email already add an review you ");
            }
            if (ModelState.IsValid)
            {
                Review review = new Review
                {
                    Email = reviewVM.Email,
                    BusinessId = reviewVM.BusinessId,
                    Rating = reviewVM.Rating,
                    Comment = reviewVM.Comment
                };
                await _reviewRepository.AddAsync(review);
                await _reviewRepository.SaveAsync();
                await hubcontext.Clients.All.SendAsync("NewReviewArrived", reviewVM);

                return RedirectToAction("GetAllByBussniss", new { bussnissId = review.BusinessId });
            }

            return View("Add", reviewVM);

        }
        public async Task<IActionResult> Edit(int id)/////////////////
        {
            Review review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            EditReviewVM reviewVM = new EditReviewVM
            {
                Id = review.Id,
                Email = review.Email,
                Rating = review.Rating,
                Comment = review.Comment
            };
            
            return View(reviewVM);
        }
        public async Task<IActionResult> SaveEditAsync(int id,EditReviewVM reviewVM)
        {
           
            if (ModelState.IsValid)
            {
                Review review = await _reviewRepository.GetByIdAsync(id);
                review.Email = reviewVM.Email;
                review.Rating = reviewVM.Rating;
                review.Comment = reviewVM.Comment;
                _reviewRepository.SaveAsync();

                return RedirectToAction("GetAllByBussniss", new { bussnissId = review.BusinessId });
            }
            return View("Edit", reviewVM);
        }
        public async Task<ActionResult> Delete(int id)/////
        {
            Review review = await _reviewRepository.GetByIdAsync(id);
            await _reviewRepository.DeleteAsync(id);
            await _reviewRepository.SaveAsync();
            return RedirectToAction("GetAllByBussniss", new { bussnissId = review.BusinessId });
        }

    }
}
