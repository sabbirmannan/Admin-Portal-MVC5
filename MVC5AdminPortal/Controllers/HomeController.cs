using MVC5AdminPortal.Models.EntityManager;
using MVC5AdminPortal.Models.ViewModel;
using MVC5AdminPortal.Security;
using System.Web.Mvc;

namespace MVC5AdminPortal.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Welcome()
        {
            return View();
        }

        [AuthorizeRole("Admin")]
        public ActionResult AdminOnly()
        {
            return View();
        }

        public ActionResult UnAuthorized()
        {
            return View();
        }

        [AuthorizeRole("Admin")]
        public ActionResult ManageUserPartial()
        {
            if (User.Identity.IsAuthenticated)
            {
                string loginName = User.Identity.Name;
                UserManager um = new UserManager();
                UserDataView udv = um.GetUserDataView(loginName);
                return PartialView(udv);
            }

            return View();
        }
    }
}