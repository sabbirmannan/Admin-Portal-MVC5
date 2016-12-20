using MVC5AdminPortal.Models.EntityManager;
using MVC5AdminPortal.Models.ViewModel;
using System.Web.Mvc;
using System.Web.Security;

namespace MVC5AdminPortal.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(UserSignUpView usv)
        {
            if (ModelState.IsValid)
            {
                UserManager um = new UserManager();
                if (!um.IsLoginNameExist(usv.LoginName))
                {
                    um.AddUserAccount(usv);
                    FormsAuthentication.SetAuthCookie(usv.FirstName, false);
                    return RedirectToAction("Welcome", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Login Name already exist");
                }
            }
            return View();
        }

        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(UserLoginView ulv, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                UserManager UM = new UserManager();
                string password = UM.GetUserPassword(ulv.LoginName);

                if (string.IsNullOrEmpty(password))
                {
                    ModelState.AddModelError("", "The User Login or Password Provided is Incorrect");
                }
                else
                {
                    if (ulv.Password.Equals(password))
                    {
                        FormsAuthentication.SetAuthCookie(ulv.LoginName, false);
                        return RedirectToAction("Welcome", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "The Password Provided is Incorrect");
                    }
                }
            }
            return View(ulv);
        }

        [Authorize]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}