using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using mvc.Models.Authorize;
using mvc.ViewModels;
using System.Security.Claims;

namespace mvc.Controllers
{
    public class AccountController : Controller
    {
        UserManager<ApplicationUser> _userManger; //to deal with db
        SignInManager<ApplicationUser> _sinInManger;//to make cookie
        public AccountController(UserManager<ApplicationUser>userManger,SignInManager<ApplicationUser>sinInManger)
        {
            _userManger = userManger;
            _sinInManger=sinInManger;
        }

        public IActionResult Login()
        {

            return View("Login");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(LoginViewModel userDataReq)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser userfromDb = await _userManger.FindByNameAsync(userDataReq.Name);
                if (userfromDb != null)
                {
                    bool isfound=
                        await _userManger.CheckPasswordAsync(userfromDb, userDataReq.Password);
                    if (isfound)
                    {



                        await _sinInManger.SignInAsync(userfromDb, userDataReq.RememberMe);
                        return RedirectToAction("Index", "HomePage");
                    }
                }
                ModelState.AddModelError("Password", "Incorrect password Or User ❌");

            }
            return View("Login", userDataReq);
        
        }

        public IActionResult Register()
        {
            return View("Register");
        }
        #region Register_Onclick
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel? UserFromReq)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser userApp=new ApplicationUser();
                userApp.Address=UserFromReq.Address;
                userApp.UserName = UserFromReq.Name;
                userApp.Email = UserFromReq.Email;

              IdentityResult result= await _userManger.CreateAsync(userApp,UserFromReq.Password);
                if (result.Succeeded)
                {


                 
                        // Add role
                        await _userManger.AddToRoleAsync(userApp, UserFromReq.Role);//AddToRoleAsync ليس  case sestive

                    await _sinInManger.SignInAsync(userApp, isPersistent: false);

                        return RedirectToAction("Index", "HomePage");
                    



                }
                foreach (var error in result.Errors)
                {
                   
                        ModelState.AddModelError("", error.Description); // general error
                }
            }

            return View("Register", UserFromReq);
        }

        #endregion

        #region LogOut
        public async Task<IActionResult> Logout()
        {
           await _sinInManger.SignOutAsync();
            return RedirectToAction("Login");
        }
        #endregion
    }
}
