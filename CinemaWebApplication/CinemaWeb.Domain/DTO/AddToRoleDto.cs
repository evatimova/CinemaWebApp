using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaWeb.Domain.DTO
{
    public class AddToRoleDto
    {
        public string Email { get; set; }
        [Display(Name = "Selected Role")]
        public string SelectedRole { get; set; }
        public List<string> Roles { get; set; }
        public AddToRoleDto()
        {
            Roles = new List<string>();
        }
    }
}
