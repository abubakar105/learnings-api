using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Dtos.RolesDto
{
    public class AssignPermissionsToRoleDTO
    {
        [Required]
        public List<int> PermissionIds { get; set; } = new List<int >();

        [Required]
        public string RoleId { get; set; }
    }
}
