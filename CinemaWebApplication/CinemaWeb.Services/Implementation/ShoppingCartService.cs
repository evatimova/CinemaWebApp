using CinemaWeb.Domain.DomainModels;
using CinemaWeb.Domain.DTO;
using CinemaWeb.Repository.Interface;
using CinemaWeb.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CinemaWeb.Services.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IRepository<TicketInOrder> _ticketInOrderRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IUserRepository _userRepository;

        public ShoppingCartService(IRepository<ShoppingCart> shoppingCartRepository, IRepository<TicketInOrder> ticketInOrderRepository, IRepository<Order> orderRepository, IUserRepository userRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _ticketInOrderRepository = ticketInOrderRepository;
        }

        public bool deleteFromShoppingCart(string userId, Guid id)
        {
            if (!string.IsNullOrEmpty(userId) && id != null)
            {

                var loggedInUser = this._userRepository.Get(userId);

                var userShoppingCart = loggedInUser.UserCart;

                var itemToDelete = userShoppingCart.TicketInShoppingCarts.Where(z => z.TicketId.Equals(id)).FirstOrDefault();

                userShoppingCart.TicketInShoppingCarts.Remove(itemToDelete);

                this._shoppingCartRepository.Update(userShoppingCart);

                return true;
            }
            return false;
        }

        public ShoppingCartDto getShoppingCartInfo(string userId)
        {
            var loggedInUser = this._userRepository.Get(userId);

            var userTicketCart = loggedInUser.UserCart;

            var AllTickets = userTicketCart.TicketInShoppingCarts.ToList();

            var allTticketPrice = AllTickets.Select(z => new
            {
                TicketPrice = z.Ticket.FilmPrice,
                Quantity = z.Quantity
            }).ToList();


            var totalPrice = 0;

            foreach (var item in allTticketPrice)
            {
                totalPrice += item.TicketPrice * item.Quantity;
            }

            ShoppingCartDto shoppingCartDto = new ShoppingCartDto
            {
                TicketInShoppingCarts = AllTickets,
                TotalPrice = totalPrice
            };
            return shoppingCartDto; 
        }

        public bool orderNow(string userId)
        {
            if (!string.IsNullOrEmpty(userId)) 
            {
                var loggedInUser = this._userRepository.Get(userId);

                var userShoppingCart = loggedInUser.UserCart;

                Order orderItem = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    User = loggedInUser,
                };

                this._orderRepository.Insert(orderItem);

            List<TicketInOrder> ticketsInOrder = new List<TicketInOrder>();

           var result = userShoppingCart.TicketInShoppingCarts
                .Select(z => new TicketInOrder
                {
                    OrderId = orderItem.Id,
                    TicketId = z.Ticket.Id,
                    SelectedTicket = z.Ticket,
                    UserOrder = orderItem,
                    Quantity = z.Quantity
                }).ToList();


                ticketsInOrder.AddRange(result);

            foreach (var item in ticketsInOrder)
            {
                    this._ticketInOrderRepository.Insert(item);
            }

            loggedInUser.UserCart.TicketInShoppingCarts.Clear();

                this._userRepository.Update(loggedInUser);

                return true;
            }
            return false;
        }
    }
}
