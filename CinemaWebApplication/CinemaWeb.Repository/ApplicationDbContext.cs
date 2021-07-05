using CinemaWeb.Domain.DomainModels;
using CinemaWeb.Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace CinemaWeb.Repository
{
    public class ApplicationDbContext : IdentityDbContext<CinemaWebApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<TicketInOrder> TicketInOrders { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<TicketInShoppingCart> TicketInShoppingCarts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Ticket>().Property(z => z.Id).ValueGeneratedOnAdd();

            builder.Entity<ShoppingCart>().Property(z => z.Id).ValueGeneratedOnAdd();

            //builder.Entity<TicketInShoppingCart>().HasKey(z => new { z.TicketId, z.ShoppingCartId });

            builder.Entity<TicketInShoppingCart>().HasOne(z => z.Ticket)
                .WithMany(z => z.TicketInShoppingCarts).HasForeignKey(z => z.ShoppingCartId);

            builder.Entity<TicketInShoppingCart>().HasOne(z => z.ShoppingCart)
               .WithMany(z => z.TicketInShoppingCarts).HasForeignKey(z => z.TicketId);

            builder.Entity<ShoppingCart>().HasOne<CinemaWebApplicationUser>(z => z.Owner)
                .WithOne(z => z.UserCart)
                .HasForeignKey<ShoppingCart>(z => z.OwnerId);

            //builder.Entity<TicketInOrder>()
            //   .HasKey(z => new { z.TicketId, z.OrderId });

            builder.Entity<TicketInOrder>()
                .HasOne(z => z.SelectedTicket)
                .WithMany(z => z.TicketsInOrder)
                .HasForeignKey(z => z.OrderId);

            builder.Entity<TicketInOrder>()
                .HasOne(z => z.UserOrder)
                .WithMany(z => z.TicketsInOrder)
                .HasForeignKey(z => z.TicketId);
        }
    }
}
