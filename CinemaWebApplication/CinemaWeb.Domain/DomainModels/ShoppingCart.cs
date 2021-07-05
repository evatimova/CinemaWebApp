using CinemaWeb.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaWeb.Domain.DomainModels
{
    public class ShoppingCart : BaseEntity
    {
        public string OwnerId { get; set; }
        public CinemaWebApplicationUser Owner { get; set; }

        public virtual ICollection<TicketInShoppingCart> TicketInShoppingCarts{ get; set; }

    }
}
