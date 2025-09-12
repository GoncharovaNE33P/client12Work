using System;
using System.Collections.Generic;

namespace client.Models;

public partial class Country
{
    public string CountryCode { get; set; } = null!;

    public string NameCountry { get; set; } = null!;

    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
}
