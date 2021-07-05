using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaWeb.Domain.DTO
{
    public class CreateRoleDto
    {
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
}
