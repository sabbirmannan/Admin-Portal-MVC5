using MVC5AdminPortal.Models.DB;
using MVC5AdminPortal.Models.ViewModel;
using System;
using System.Collections.Generic;
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

        public List<LookupAvailableRole> GetAllRoles()
        {
            using (DemoDBEntities db = new DemoDBEntities())
            {
                var roles = db.LOOKUPRoles.Select(o => new LookupAvailableRole
                {
                    LookupRoleId = o.LOOKUPRoleID,
                    RoleName = o.RoleName,
                    RoleDescription = o.RoleDescription
                }).ToList();

                return roles;
            }
        }

        public int GetUserId(string loginName)
        {
            using (DemoDBEntities db = new DemoDBEntities())
            {
                var user = db.SYSUsers.Where(o => o.LoginName.Equals(loginName));
                if (user.Any())
                    return user.FirstOrDefault().SYSUserID;
            }
            return 0;
        }

        public List<UserProfileView> GetAllUserProfiles()
        {
            List<UserProfileView> profiles = new List<UserProfileView>();
            using (DemoDBEntities db = new DemoDBEntities())
            {
                var users = db.SYSUsers.ToList();
                foreach (var u in db.SYSUsers)
                {
                    var upv = new UserProfileView
                    {
                        SYSUserID = u.SYSUserID,
                        LoginName = u.LoginName,
                        Password = u.PasswordEncryptedText
                    };
                    var sup = db.SYSUserProfiles.Find(u.SYSUserID);
                    if (sup != null)
                    {
                        upv.FirstName = sup.FirstName;
                        upv.LastName = sup.LastName;
                        upv.Gender = sup.Gender;
                    }

                    var sur = db.SYSUserRoles.Where(o => o.SYSUserID.Equals(u.SYSUserID));
                    if (sur.Any())
                    {
                        var userRole = sur.FirstOrDefault();
                        upv.LOOKUPRoleID = userRole.LOOKUPRoleID;
                        upv.RoleName = userRole.LOOKUPRole.RoleName;
                        upv.IsRoleActive = userRole.IsActive;
                    }
                    profiles.Add(upv);
                }
            }

            return profiles;
        }

        public UserDataView GetUserDataView(string loginName)
        {
            UserDataView udv = new UserDataView();
            List<UserProfileView> profiles = GetAllUserProfiles();
            List<LookupAvailableRole> roles = GetAllRoles();
            int? userAssignedRoleID = 0, userID = 0;
            string userGender = string.Empty;
            userID = GetUserId(loginName);

            using (DemoDBEntities db = new DemoDBEntities())
            {
                userAssignedRoleID = db.SYSUserRoles.Where(o => o.SYSUserID == userID)?.FirstOrDefault().LOOKUPRoleID;
                userGender = db.SYSUserProfiles.Where(o => o.SYSUserID == userID)?.FirstOrDefault().Gender;
            }

            List<Gender> genders = new List<Gender>();
            genders.Add(new Gender { Text = "Male", Value = "M" });
            genders.Add(new Gender { Text = "Female", Value = "F" });
            udv.UserProfile = profiles;
            udv.UserRoles = new UserRoles
            {
                SelectedRoleID = userAssignedRoleID,
                UserRoleList = roles
            };
            udv.UserGender = new UserGender
            {
                SelectedGender = userGender,
                Gender =
           genders
            };
            return udv;
        }

    }
}

