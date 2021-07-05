using CinemaWeb.Domain.DomainModels;
using CinemaWeb.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaWeb.Repository.Implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<Order> entities;
        string errorMessage = string.Empty;

        public OrderRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<Order>();
        }
        public List<Order> GetAllOrders()
        {
            return entities
                .Include(z => z.TicketsInOrder)
                .Include(z => z.User)
                .Include("TicketsInOrder.SelectedTicket")
                .ToListAsync().Result;
        }

        public Order GetOrderDetails(BaseEntity model)
        {
            return entities
                .Include(z => z.TicketsInOrder)
                .Include(z => z.User)
                .Include("TicketsInOrder.SelectedTicket")
                .SingleOrDefaultAsync(z => z.Id == model.Id).Result;
        }
    }
}
