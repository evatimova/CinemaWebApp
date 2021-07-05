using CinemaWeb.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaWeb.Repository.Interface
{
    public interface IUserRepository
    {
        IEnumerable<CinemaWebApplicationUser> GetAll();
        CinemaWebApplicationUser Get(string id);
        void Insert(CinemaWebApplicationUser entity);
        void Update(CinemaWebApplicationUser entity);
        void Delete(CinemaWebApplicationUser entity);
    }
}
