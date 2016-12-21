using System;

namespace MVC5AdminPortal.Models.ViewModel
{
    public class UserMessage
    {
        public int MessageID { get; set; }

        public int SYSUserID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MessageText { get; set; }

        public DateTime? LogDate { get; set; }
    }
}