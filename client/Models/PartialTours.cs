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
    }
}
