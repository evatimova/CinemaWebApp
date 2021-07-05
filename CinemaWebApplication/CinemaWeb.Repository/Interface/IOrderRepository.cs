using CinemaWeb.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaWeb.Repository.Interface
{
    public interface IOrderRepository
    {
        List<Order> GetAllOrders();
        Order GetOrderDetails(BaseEntity model);
    }
}
