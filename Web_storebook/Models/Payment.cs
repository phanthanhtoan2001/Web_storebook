﻿using System;
using System.Collections.Generic;

namespace Web_storebook.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? OrderId { get; set; }

    public DateTime PaymentDate { get; set; }

    public string? PaymentMethod { get; set; }

    public decimal PaymentAmount { get; set; }

    public string? PaymentStatus { get; set; }

    public virtual Order? Order { get; set; }
}