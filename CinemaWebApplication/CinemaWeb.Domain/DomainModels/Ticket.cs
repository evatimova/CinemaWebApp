using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaWeb.Domain.DomainModels
{
    public class Ticket : BaseEntity
    {
        [Required]
        public string FilmName { get; set; }
        [Required]
        public string FilmImage { get; set; }
        [Required]
        public string FilmDescription { get; set; }
        [Required]
        public int FilmPrice { get; set; }
        [Required]
        public string FilmGenre { get; set; }
        [Required]
        public DateTime FilmTime { get; set; } 
        public float Rating { get; set; }
        public virtual ICollection<TicketInShoppingCart> TicketInShoppingCarts { get; set; }
        public virtual ICollection<TicketInOrder> TicketsInOrder { get; set; }

    }
}
