using System;
using System.Collections.Generic;

namespace Web_storebook.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? ShippingAddress { get; set; }

    public string? BillingAddress { get; set; }

    public string? MembershipLevel { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
