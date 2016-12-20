using MVC5AdminPortal.Models.DB;
using MVC5AdminPortal.Models.ViewModel;
using System;
using System.Linq;

namespace MVC5AdminPortal.Models.EntityManager
{
    public class UserManager
    {
        public void AddUserAccount(UserSignUpView user)
        {
            using (DemoDBEntities db = new DemoDBEntities())
            {
                SYSUser su = new SYSUser();
                su.LoginName = user.LoginName;
                su.PasswordEncryptedText = user.Password;
                su.RowCreatedSYSUserID = user.SYSUserID > 0 ? user.SYSUserID : 1;
                su.RowModifiedSYSUserID = user.SYSUserID > 0 ? user.SYSUserID : 1;
                su.RowCreatedDateTime = DateTime.Now;
                su.RowModifiedDateTime = DateTime.Now;

                db.SYSUsers.Add(su);
                db.SaveChanges();

                SYSUserProfile sup = new SYSUserProfile();
                sup.SYSUserID = su.SYSUserID;
                sup.FirstName = user.FirstName;
                sup.LastName = user.LastName;
                sup.Gender = user.Gender;
                sup.RowCreatedSYSUserID = user.SYSUserID > 0 ? user.SYSUserID : 1;
                sup.RowModifiedSYSUserID = user.SYSUserID > 0 ? user.SYSUserID : 1;
                sup.RowCreatedDateTime = DateTime.Now;
                sup.RowModifiedDateTime = DateTime.Now;

                db.SYSUserProfiles.Add(sup);
                db.SaveChanges();

                if (user.LOOKUPRoleID > 0)
                {
                    SYSUserRole sur = new SYSUserRole();
                    sur.LOOKUPRoleID = user.LOOKUPRoleID;
                    sur.SYSUserID = user.SYSUserID;
                    sur.IsActive = true;
                    sur.RowCreatedSYSUserID = user.SYSUserID > 0 ? user.SYSUserID : 1;
                    sur.RowModifiedSYSUserID = user.SYSUserID > 0 ? user.SYSUserID : 1;
                    sur.RowCreatedDateTime = DateTime.Now;
                    sur.RowModifiedDateTime = DateTime.Now;
                    db.SYSUserRoles.Add(sur);
                    db.SaveChanges();
                }

            }
        }

        public bool IsLoginNameExist(string loginName)
        {
            using (DemoDBEntities db = new DemoDBEntities())
            {
                return db.SYSUsers.Any(o => o.LoginName.Equals(loginName));
            }
        }

        public string GetUserPassword(string loginName)
        {
            using (DemoDBEntities db = new DemoDBEntities())
            {
                var user = db.SYSUsers.Where(o => o.LoginName.ToLower().Equals(loginName));
                if (user.Any())
                    return user.FirstOrDefault().PasswordEncryptedText;
                else
                    return string.Empty;

            }
        }

        public bool IsUserInRole(string loginName, string roleName)
        {
            using (DemoDBEntities db = new DemoDBEntities())
            {
                SYSUser su = db.SYSUsers.FirstOrDefault(o => o.LoginName.ToLower().Equals(loginName));
                if (su != null)
                {
                    var roles = from q in db.SYSUserRoles
                                join r in db.LOOKUPRoles on q.LOOKUPRoleID equals r.LOOKUPRoleID
                                where r.RoleName.Equals(roleName) && q.SYSUserID.Equals(su.SYSUserID)
                                select r.RoleName;
                    if (roles != null)
                    {
                        return roles.Any();
                    }
                }
                return false;
            }
        }
    }
}