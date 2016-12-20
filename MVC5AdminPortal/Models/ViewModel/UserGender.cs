using System.Collections.Generic;

namespace MVC5AdminPortal.Models.ViewModel
{
    public class UserGender
    {
        public string SelectedGender { get; set; }
        public IEnumerable<Gender> Gender { get; set; }    }
}
