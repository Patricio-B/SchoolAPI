using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Entities.Models
{
    public class Student : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
