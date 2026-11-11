
using System.ComponentModel.DataAnnotations;
namespace BlazingPizza
{
    public class Address
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter your full name.")]
        [StringLength(100)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(254)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter your phone number.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(30)]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Please enter your street address.")]
        [StringLength(150)]
        public string Line1 { get; set; }
        [StringLength(150)]
        public string Line2 { get; set; }
        [Required(ErrorMessage = "Please enter your city.")]
        [StringLength(100)]
        public string City { get; set; }
        [Required(ErrorMessage = "Please enter your state.")]
        [StringLength(100)]
        public string Region { get; set; }
        [Required(ErrorMessage = "Please enter your postal code.")]
        [StringLength(20)]
        public string PostalCode { get; set; }
    }
}
