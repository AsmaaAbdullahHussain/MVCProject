using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using mvc.Enums;
using mvc.Models;
using mvc.RepoInterfaces;
using PayPalCheckoutSdk.Orders;
using System.Security.Claims;

//using PayPal.Api;
using System.Threading.Tasks;

namespace mvc.Controllers
{
    public class PaymentController : Controller
    {
        IPaymentRepository _paymentRepository;
        IBussinessRepository _bussinessRepository;

        public PaymentController(IPaymentRepository paymentRepository,IConfiguration configuration,IBussinessRepository bussinessRepository)
        {
            _paymentRepository = paymentRepository;
            _bussinessRepository = bussinessRepository;

        }


        [Authorize]
        public async Task<IActionResult> CreateOrder(int packageId,SubscriptionType subscription,int BussnissId,decimal amount, string currency = "USD")
        {
            try
            {
                //seve new record in checkout table with status pending
                Checkout checkout = new Checkout
                {
                    Amount = amount,
                    PaymentMethod = Enums.PaymentMethod.PayPal,
                    PaymentStatus = PaymentStatus.Pending,
                    BusinessId = BussnissId,
                    PackageId = packageId,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    SubscriptionType = subscription
                };
                await _paymentRepository.AddAsync(checkout);
                await _paymentRepository.SaveAsync();
                // Create the order using the payment repository
                string baseUrl = $"{Request.Scheme}://{Request.Host}";
                var approvalUrl = await _paymentRepository.CreateOrderAsync(checkout , currency , baseUrl);
                return Redirect(approvalUrl);
            }
            catch (Exception ex)
            {
                // Log error
                return RedirectToAction("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Success(int id,SubscriptionType subscription,int businessId ,int packageid,string token)
        {
           // try
            //{

                Order? response = await _paymentRepository.CapturePaymentAsync(token);

                if (response.Status == "COMPLETED")
                {
                    //update payment status and transactionId in checkout table =>i need checout id
                    Checkout checkout = await _paymentRepository.GetByIdAsync(id);
                    checkout.PaymentStatus = PaymentStatus.Completed;
                    checkout.TransactionId = response.Id;

                    //update bussness table => subscription end date and package id => need bussiness id
                    Business business =await _bussinessRepository.GetByIdAsync(businessId);
                    if (subscription == SubscriptionType.Monthly)
                    {
                        business.SubscriptionEndDate = DateTime.UtcNow.AddMonths(1);
                    }
                    else
                    {
                        business.SubscriptionEndDate = DateTime.UtcNow.AddYears(1);
                    }
                    business.PackageId = packageid;
                    await _paymentRepository.SaveAsync();
                    //view show success message
                    return View("Success");
                }

                return RedirectToAction("Error");
            //}
            //catch (Exception ex)
            //{
            //    // Log error
            //    return RedirectToAction("Error");
            //}
        }

        [HttpGet]
        public IActionResult Cancel()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }

        
        public async Task<IActionResult> GetAllChecouts()
        {
            var checkouts =await _paymentRepository.GetAll().ToListAsync();
            return View(checkouts);
        }

    }
}
