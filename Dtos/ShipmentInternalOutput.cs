using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiAnterExpress.Dtos
{
    public class ShipmentInternalOutput
    {
        public int ShipmentId { get; set; }
        public Enum StatusOrder { get; set; }
    }
}