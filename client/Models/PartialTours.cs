using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client.Models
{
    public partial class Tour
    {
        public string ActualTour
        {
            get
            {
                if (IsActual == 0)
                {
                    return "Не актуален";
                }
                return "Актуален";
            }
        }

        public string ActualTourColor
        {
            get
            {
                if (IsActual == 0)
                {
                    return "#e31e24";
                }
                if (IsActual == 1)
                {
                    return "#4ee62e";
                }
                return "";
            }
        }
    }
}
