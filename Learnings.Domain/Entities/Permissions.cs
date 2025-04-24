using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Domain.Entities
{
    public class Permissions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PermissionId { get; set; }

        [Required]
        [MaxLength(255)]
        public string PermissionName { get; set; }
        [Required]
        [MaxLength(255)]
        public string PermissionDescription { get; set; }
    }
}
