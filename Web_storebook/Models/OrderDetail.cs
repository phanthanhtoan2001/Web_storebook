﻿using System;
using System.Collections.Generic;

namespace Web_storebook.Models;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int? OrderId { get; set; }

    public int? BookId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? Discount { get; set; }

    public virtual Book? Book { get; set; }

    public virtual Order? Order { get; set; }
}