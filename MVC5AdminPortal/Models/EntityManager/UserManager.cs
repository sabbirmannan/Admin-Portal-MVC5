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
            int? userAssignedRoleId = 0, userId = 0;
            string userGender = string.Empty;
            userId = GetUserId(loginName);

            using (DemoDBEntities db = new DemoDBEntities())
            {
                userAssignedRoleId = db.SYSUserRoles.FirstOrDefault(o => o.SYSUserID == userId).LOOKUPRoleID;
                userGender = db.SYSUserProfiles.FirstOrDefault(o => o.SYSUserID == userId).Gender;
            }

            List<Gender> genders = new List<Gender>();
            genders.Add(new Gender { Text = "Male", Value = "M" });
            genders.Add(new Gender { Text = "Female", Value = "F" });
            udv.UserProfile = profiles;
            udv.UserRoles = new UserRoles
            {
                SelectedRoleID = userAssignedRoleId,
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

        public void UpdateUserAccount(UserProfileView user)
        {
            using (var db = new DemoDBEntities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var su = db.SYSUsers.Find(user.SYSUserID);
                        su.LoginName = user.LoginName;
                        su.PasswordEncryptedText = user.Password;
                        su.RowCreatedSYSUserID = user.SYSUserID;
                        su.RowModifiedSYSUserID = user.SYSUserID;
                        su.RowCreatedDateTime = DateTime.Now;
                        su.RowModifiedDateTime = DateTime.Now;
                        db.SaveChanges();                        var userProfile = db.SYSUserProfiles.Where(o => o.SYSUserID == user.SYSUserID);

                        if (userProfile.Any())
                        {
                            var sup = userProfile.FirstOrDefault();
                            sup.SYSUserID = su.SYSUserID;
                            sup.FirstName = user.FirstName;
                            sup.LastName = user.LastName;
                            sup.Gender = user.Gender;
                            sup.RowCreatedSYSUserID = user.SYSUserID;
                            sup.RowModifiedSYSUserID = user.SYSUserID;
                            sup.RowCreatedDateTime = DateTime.Now;
                            sup.RowModifiedDateTime = DateTime.Now;                            db.SaveChanges();                        }

                        if (user.LOOKUPRoleID > 0)
                        {
                            var userRole = db.SYSUserRoles.Where(o => o.SYSUserID == user.SYSUserID);
                            SYSUserRole sur = null;
                            if (userRole.Any())
                            {
                                sur = userRole.FirstOrDefault();
                                sur.LOOKUPRoleID = user.LOOKUPRoleID;
                                sur.SYSUserID = user.SYSUserID;
                                sur.IsActive = true;
                                sur.RowCreatedSYSUserID = user.SYSUserID;
                                sur.RowModifiedSYSUserID = user.SYSUserID;
                                sur.RowCreatedDateTime = DateTime.Now;
                                sur.RowModifiedDateTime = DateTime.Now;
                            }
                            else
                            {
                                sur = new SYSUserRole
                                {
                                    LOOKUPRoleID = user.LOOKUPRoleID,
                                    SYSUserID = user.SYSUserID,
                                    IsActive = true,
                                    RowCreatedSYSUserID = user.SYSUserID,
                                    RowModifiedSYSUserID = user.SYSUserID,
                                    RowCreatedDateTime = DateTime.Now,
                                    RowModifiedDateTime = DateTime.Now
                                };
                                db.SYSUserRoles.Add(sur);
                            }                            db.SaveChanges();
                        }
                        dbContextTransaction.Commit();                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
        }

        public void DeleteUser(int userId)
        {
            using (var db = new DemoDBEntities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var sur = db.SYSUserRoles.Where(o => o.SYSUserID == userId);
                        if (sur.Any())
                        {
                            db.SYSUserRoles.Remove(sur.FirstOrDefault());
                            db.SaveChanges();
                        }

                        var sup = db.SYSUserProfiles.Where(o => o.SYSUserID == userId);
                        if (sup.Any())
                        {
                            db.SYSUserProfiles.Remove(sup.FirstOrDefault());
                            db.SaveChanges();
                        }

                        var su = db.SYSUsers.Where(o => o.SYSUserID == userId);
                        if (su.Any())
                        {
                            db.SYSUsers.Remove(su.FirstOrDefault());
                            db.SaveChanges();
                        }

                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
        }

        public UserProfileView GetUserProfile(int userId)
        {
            var upv = new UserProfileView();
            using (var db = new DemoDBEntities())
            {
                var user = db.SYSUsers.Find(userId);
                if (user != null)
                {
                    upv.SYSUserID = user.SYSUserID;
                    upv.LoginName = user.LoginName;
                    upv.Password = user.PasswordEncryptedText;

                    var sup = db.SYSUserProfiles.Find(userId);
                    if (sup != null)
                    {
                        upv.FirstName = sup.FirstName;
                        upv.LastName = sup.LastName;
                        upv.Gender = sup.Gender;
                    }
                    var sur = db.SYSUserRoles.FirstOrDefault(o => o.SYSUserID == userId);
                    if (sur != null)
                    {
                        upv.LOOKUPRoleID = sur.LOOKUPRoleID;
                        upv.RoleName = sur.LOOKUPRole.RoleName;
                        upv.IsRoleActive = sur.IsActive;
                    }
                }
            }

            return upv;
        }

    }
}

