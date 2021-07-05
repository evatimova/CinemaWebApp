using CinemaWeb.Domain.DomainModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaWeb.Domain.Identity
{
    public class CinemaWebApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public virtual ShoppingCart UserCart { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
