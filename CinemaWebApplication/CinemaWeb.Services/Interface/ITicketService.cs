using CinemaWeb.Domain.DomainModels;
using CinemaWeb.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaWeb.Services.Interface
{
    public interface ITicketService
    {
        List<Ticket> GetAllProducts();
        List<Ticket> GetAllTicketsByGenre(string genre);
        List<Ticket> FilterTicketsByDate();
        Ticket GetDetailsForProduct(Guid? id);
        void CreateNewProduct(Ticket p);
        void UpdeteExistingProduct(Ticket p);
        AddToShoppingCardDto GetShoppingCartInfo(Guid? id);
        void DeleteProduct(Guid id);
        bool AddToShoppingCart(AddToShoppingCardDto item, string userID);
    }
}
