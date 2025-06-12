using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Domain.Entities
{
    public class Users : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicUrl { get; set; }
        public ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();
    }
}
