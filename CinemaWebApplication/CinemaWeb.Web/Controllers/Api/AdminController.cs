using CinemaWeb.Domain.DomainModels;
using CinemaWeb.Domain.DTO;
using CinemaWeb.Domain.Identity;
using CinemaWeb.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaWeb.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<CinemaWebApplicationUser> _userManager;

        public AdminController(IOrderService orderService, UserManager<CinemaWebApplicationUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        [HttpGet("[action]")]
        public List<Order> GetAllActiveOrders()
        {
            return this._orderService.GetAllOrders();
        }

        [HttpPost("[action]")]
        public Order GetDetailsForOrder(BaseEntity model)
        {
            return this._orderService.GetOrderDetails(model);
        }

        [HttpPost("[action]")]
        public bool ImportAllUsers(List<UserRegisterDto> model)
        {
            bool status = true;

            foreach (var item in model)
            {
                var userCheck = _userManager.FindByEmailAsync(item.Email).Result;
                if (userCheck == null)
                {
                    var user = new CinemaWebApplicationUser
                    {
                        UserName = item.Email,
                        NormalizedUserName = item.Email,
                        Email = item.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        UserCart = new ShoppingCart()
                    };
                    var result = _userManager.CreateAsync(user, item.Password).Result;
                    var roleAdded = _userManager.AddToRoleAsync(user, item.Role).Result;
                    status = status && result.Succeeded;
                }
                else
                {
                    continue;
                }
            }
            return status;
        }
    }
}
