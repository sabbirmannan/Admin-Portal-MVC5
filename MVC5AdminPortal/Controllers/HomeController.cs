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
        public ActionResult ManageUserPartial(string status = "")
        {
            if (User.Identity.IsAuthenticated)
            {
                string loginName = User.Identity.Name;
                UserManager um = new UserManager();
                UserDataView udv = um.GetUserDataView(loginName);
                string message = string.Empty;
                if (status.Equals("update"))
                    message = "Update Successful";
                else if (status.Equals("delete"))
                    message = "Delete Successful";
                ViewBag.Message = message;
                return PartialView(udv);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult UpdateUserData(int userId, string loginName, string password, string firstName,
            string lastName, string gender, int roleId = 0)
        {
            var upv = new UserProfileView
            {
                SYSUserID = userId,
                LoginName = loginName,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                Gender = gender
            };
            if (roleId > 0)
                upv.LOOKUPRoleID = roleId;
            var um = new UserManager();
            um.UpdateUserAccount(upv);
            return Json(new { success = true });
        }

        [AuthorizeRole("Admin")]
        public ActionResult DeleteUser(int userID)
        {
            var um = new UserManager();
            um.DeleteUser(userID);
            return Json(new { success = true });
        }        [Authorize]
        public ActionResult EditProfile()
        {
            string loginName = User.Identity.Name;
            UserManager um = new UserManager();
            UserProfileView UPV = um.GetUserProfile(um.GetUserId(loginName));
            return View(UPV);
        }        [HttpPost]
        [Authorize]
        public ActionResult EditProfile(UserProfileView profile)
        {
            if (ModelState.IsValid)
            {
                UserManager um = new UserManager();
                um.UpdateUserAccount(profile);
                ViewBag.Status = "Update Sucessful!";
            }
            return View(profile);
        }
    }
}