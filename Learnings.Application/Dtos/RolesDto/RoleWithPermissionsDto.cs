using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Dtos.RolesDto
{
    public class RoleWithPermissionsDto
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }  
        public List<PermissionsDto> Permissions { get; set; }
    }
}
