using System;
using System.Collections.Generic;
using System.Text;

namespace DA.Models
{
    public class User : BaseModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string HashPassword { get; set; }
    }
}