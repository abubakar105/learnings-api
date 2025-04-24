using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Dtos.RolesDto
{
    public class PermissionsDto
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public string PermissionDescription { get; set; }
    }
}
