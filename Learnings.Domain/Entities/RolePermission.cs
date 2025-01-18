using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Domain.Entities
{
    public class RolePermissions
    {
        [ForeignKey("Role")]
        public string RoleId { get; set; }
        public IdentityRole Roles { get; set; }  

        [ForeignKey("Permissions")]
        public int PermissionId { get; set; }
        public Permissions Permission { get; set; }  
    }

}
