using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Dtos
{
    public class AddUserAddressDto
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; }

        [Required, MaxLength(200)]
        public string Street { get; set; }

        [MaxLength(100)]
        public string Apartment { get; set; }

        [Required, MaxLength(100)]
        public string City { get; set; }

        [Required, MaxLength(100)]
        public string State { get; set; }

        [Required, MaxLength(20)]
        public string PostalCode { get; set; }

        [Required, MaxLength(100)]
        public string Country { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public bool IsDefault { get; set; } = false;
    }
}
