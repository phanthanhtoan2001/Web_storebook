using System;
using System.Collections.Generic;

namespace Web_storebook.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string? ShippingMethod { get; set; }

    public decimal? ShippingCost { get; set; }

    public string? PaymentMethod { get; set; }

    public string? OrderStatus { get; set; }

    public string? TrackingNumber { get; set; }

    public DateOnly? DeliveryDate { get; set; }

    public int? CreatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
