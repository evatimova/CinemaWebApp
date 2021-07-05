using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace CinemaWeb.Domain.DTO
{
    public class RemoveFromRoleDto
    {
        public string Email { get; set; }
        [Display(Name = "Selected Role")]
        public string SelectedRole { get; set; }
        public List<string> Roles { get; set; }
        public RemoveFromRoleDto()
        {
            Roles = new List<string>();
        }
    }
}
