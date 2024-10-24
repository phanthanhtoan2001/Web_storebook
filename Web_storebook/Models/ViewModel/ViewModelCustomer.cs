using System.ComponentModel.DataAnnotations;

namespace Web_storebook.Models.ViewModel
{
    public class ViewModelCustomer
    {
       

            [Required]
            [StringLength(255)]
            public string FirstName { get; set; }

            [Required]
            [StringLength(255)]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Phone]
            [StringLength(15)]
            public string Phone { get; set; }

            [StringLength(500)]
            public string ShippingAddress { get; set; }

            [StringLength(500)]
            public string BillingAddress { get; set; }

            [StringLength(50)]
            public string MembershipLevel { get; set; }

            [DataType(DataType.Date)]
            public DateOnly? DateOfBirth { get; set; }

            public DateTime RegistrationDate { get; set; } = DateTime.Now;
        
    }
}
