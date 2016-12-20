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
    }
}