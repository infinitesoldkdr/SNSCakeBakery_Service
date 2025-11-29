using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SNSCakeBakery_Service.Models
{
    public class Address
    {
        public int AddressId { get; set; }

        public string UserId { get; set; }  // string per your rule

        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }
    }
}
