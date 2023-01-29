﻿using System.ComponentModel.DataAnnotations;

namespace ETicaret.WebUI.Models
{
    public class UserDetailsModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        public IEnumerable<string> SelectedRoles { get; set; }
    }
}
