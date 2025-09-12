using System;
using System.Collections.Generic;

namespace client.Models;

public partial class Type
{
    public int IdType { get; set; }

    public string NameType { get; set; } = null!;

    public virtual ICollection<ToursType> ToursTypes { get; set; } = new List<ToursType>();
}
