using System;
using System.Collections.Generic;

namespace client.Models;

public partial class Hotel
{
    public int IdHotel { get; set; }

    public string NameHotel { get; set; } = null!;

    public int CountOfStars { get; set; }

    public string CountryCode { get; set; } = null!;

    public string? Description { get; set; }

    public virtual Country CountryCodeNavigation { get; set; } = null!;
}
