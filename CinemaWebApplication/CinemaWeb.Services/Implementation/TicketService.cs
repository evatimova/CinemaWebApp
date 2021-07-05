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
    public class TicketService : ITicketService
    {
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<TicketInShoppingCart> _ticketInShoppingCartRepository;

        public TicketService(IRepository<Ticket> ticketRepository, IUserRepository userRepository, IRepository<TicketInShoppingCart> ticketInShoppingCartRepository)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _ticketInShoppingCartRepository = ticketInShoppingCartRepository;
        }


        public bool AddToShoppingCart(AddToShoppingCardDto item, string userID)
        {
            var user = this._userRepository.Get(userID);

            var userShoppingCard = user.UserCart;

            if (item.TicketId != null && userShoppingCard != null)
            {
                var tiket = this.GetDetailsForProduct(item.TicketId);

                if (tiket != null)
                {
                    TicketInShoppingCart itemToAdd = new TicketInShoppingCart
                    {
                        Ticket = tiket,
                        TicketId = tiket.Id,
                        ShoppingCart = userShoppingCard,
                        ShoppingCartId = userShoppingCard.Id,
                        Quantity = item.Quantity
                    };

                    this._ticketInShoppingCartRepository.Insert(itemToAdd);
                    return true;
                }
                return false;
            }
            return false;
        }

        public void CreateNewProduct(Ticket p)
        {
            this._ticketRepository.Insert(p);
        }

        public void DeleteProduct(Guid id)
        {
            var ticket = this.GetDetailsForProduct(id);
            this._ticketRepository.Delete(ticket);
        }

        public List<Ticket> GetAllProducts()
        {
            return this._ticketRepository.GetAll().ToList();
        }

        public List<Ticket> GetAllTicketsByGenre(string genre)
        {
            return GetAllProducts().Where(z => z.FilmGenre.Equals(genre)).ToList();
        }

        public List<Ticket> FilterTicketsByDate()
        {
            return GetAllProducts().Where(z =>
                DateTime.Compare(z.FilmTime, DateTime.Now) > 0).ToList();
        }

        public Ticket GetDetailsForProduct(Guid? id)
        {
            return this._ticketRepository.Get(id);
        }

        public AddToShoppingCardDto GetShoppingCartInfo(Guid? id)
        {
            var tiket = this.GetDetailsForProduct(id);
            AddToShoppingCardDto model = new AddToShoppingCardDto
            {
                SelectedTicket = tiket,
                TicketId = tiket.Id,
                Quantity = 1
            };
            return model;
        }

        public void UpdeteExistingProduct(Ticket p)
        {
            this._ticketRepository.Update(p);
        }
    }
}
