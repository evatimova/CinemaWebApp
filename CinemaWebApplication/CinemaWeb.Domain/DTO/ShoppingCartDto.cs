using CinemaWeb.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaWeb.Domain.DTO
{
    public class ShoppingCartDto
    {
        public List<TicketInShoppingCart> TicketInShoppingCarts { get; set; }
        public int TotalPrice { get; set; }
    }
}
