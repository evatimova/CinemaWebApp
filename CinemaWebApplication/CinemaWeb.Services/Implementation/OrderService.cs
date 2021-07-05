using CinemaWeb.Domain.DomainModels;
using CinemaWeb.Repository.Interface;
using CinemaWeb.Services.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaWeb.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public List<Order> GetAllOrders()
        {
            return this._orderRepository.GetAllOrders();
        }

        public Order GetOrderDetails(BaseEntity model)
        {
            return this._orderRepository.GetOrderDetails(model);
        }
    }
}
