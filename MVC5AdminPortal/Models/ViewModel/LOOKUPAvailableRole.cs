using System.ComponentModel.DataAnnotations;

namespace MVC5AdminPortal.Models.ViewModel
{
    public class LookupAvailableRole
    {
        [Key]
        public int LookupRoleId { get; set; }

        public string RoleName { get; set; }

        public string RoleDescription { get; set; }
    }
}
