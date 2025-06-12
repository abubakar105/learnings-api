using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Dtos
{
    public class UserAddressDto
    {
        public Guid AddressId { get; set; }
        public string FullName { get; set; }
        public string Street { get; set; }
        public string Apartment { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
