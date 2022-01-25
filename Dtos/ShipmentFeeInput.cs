using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;

namespace DiAnterExpress.Dtos
{
    public class ShipmentFeeInput
    {
        public Point SenderAddress { get; set; }
        public Point ReceiverAddress { get; set; }
        public double Weight { get; set; }
        public int ShipmentTypeId { get; set; }
    }
}