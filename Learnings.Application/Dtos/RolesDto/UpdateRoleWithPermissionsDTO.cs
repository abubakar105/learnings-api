using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Dtos.RolesDto
{
    public class UpdateRoleWithPermissionsDTO
    {
        public string RoleId { get; set;}
        public string RoleName { get; set;}
        public string RoleDescription { get; set;}
        public List<int> PermissionsList { get; set;}
    }
}
