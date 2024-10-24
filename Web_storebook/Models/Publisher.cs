using System;
using System.Collections.Generic;

namespace Web_storebook.Models;

public partial class Publisher
{
    public int PublisherId { get; set; }

    public string PublisherName { get; set; } = null!;

    public string? ContactEmail { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Website { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
