using CinemaWeb.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaWeb.Domain.DomainModels
{
    public class Order : BaseEntity
    {
        public CinemaWebApplicationUser User { get; set; }
        public string UserId { get; set; }
        public virtual ICollection<TicketInOrder> TicketsInOrder { get; set; }
    }
}
