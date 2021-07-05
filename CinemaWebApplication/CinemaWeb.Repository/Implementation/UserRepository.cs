using CinemaWeb.Domain.Identity;
using CinemaWeb.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CinemaWeb.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<CinemaWebApplicationUser> entities;
        string errorMessage = string.Empty;

        public UserRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<CinemaWebApplicationUser>();
        }
        public IEnumerable<CinemaWebApplicationUser> GetAll()
        {
            return entities.AsEnumerable();
        }

        public CinemaWebApplicationUser Get(string id)
        {
            return entities
                .Include(z => z.UserCart)
                .Include("UserCart.TicketInShoppingCarts")
                .Include("UserCart.TicketInShoppingCarts.Ticket")
                .SingleOrDefault(s => s.Id == id);
        }
        public void Insert(CinemaWebApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Add(entity);
            context.SaveChanges();
        }

        public void Update(CinemaWebApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Update(entity);
            context.SaveChanges();
        }

        public void Delete(CinemaWebApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Remove(entity);
            context.SaveChanges();
        }
    }
}
