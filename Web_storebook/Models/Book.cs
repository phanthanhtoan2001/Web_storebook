using System;
using System.Collections.Generic;

namespace Web_storebook.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string? BookCode { get; set; }

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public string Isbn { get; set; } = null!;

    public decimal Price { get; set; }

    public decimal? DiscountPrice { get; set; }

    public string? Description { get; set; }

    public DateOnly PublicationDate { get; set; }

    public int StockQuantity { get; set; }

    public int? PublisherId { get; set; }

    public int? CategoryId { get; set; }

    public string? Language { get; set; }

    public int? Pages { get; set; }

    public decimal? Weight { get; set; }

    public string? Dimension { get; set; }

    public string? CoverType { get; set; }

    public string? ImageBook { get; set; }

    public int? CreatedBy { get; set; }

    public virtual Category? Category { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Publisher? Publisher { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
