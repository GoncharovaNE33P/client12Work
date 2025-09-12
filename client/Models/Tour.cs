using System;
using System.Collections.Generic;

namespace client.Models;

public partial class Tour
{
    public int IdTour { get; set; }

    public int TicketCount { get; set; }

    public string NameTour { get; set; } = null!;

    public string? Description { get; set; }

    public int Price { get; set; }

    public int IsActual { get; set; }

    public string? ImagePreview { get; set; }

    public virtual ICollection<ToursType> ToursTypes { get; set; } = new List<ToursType>();
}
