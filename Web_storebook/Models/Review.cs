using System;
using System.Collections.Generic;

namespace Web_storebook.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int? CustomerId { get; set; }

    public int? BookId { get; set; }

    public int Rating { get; set; }

    public string? ReviewText { get; set; }

    public DateTime ReviewDate { get; set; }

    public bool? VerifiedPurchase { get; set; }

    public virtual Book? Book { get; set; }

    public virtual Customer? Customer { get; set; }
}
