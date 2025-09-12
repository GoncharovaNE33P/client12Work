using System;
using System.Collections.Generic;

namespace client.Models;

public partial class ToursType
{
    public int IdToursType { get; set; }

    public int IdTour { get; set; }

    public int IdType { get; set; }

    public virtual Tour IdTourNavigation { get; set; } = null!;

    public virtual Type IdTypeNavigation { get; set; } = null!;
}
