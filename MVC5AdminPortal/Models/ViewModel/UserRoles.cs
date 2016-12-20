using System.Collections.Generic;

namespace MVC5AdminPortal.Models.ViewModel
{
    public class UserRoles
    {
        public int? SelectedRoleID { get; set; }
        public IEnumerable<LookupAvailableRole> UserRoleList { get; set; }
    }
}
