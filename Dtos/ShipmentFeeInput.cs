using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;

namespace DiAnterExpress.Dtos
{
    public class ShipmentFeeInput
    {
        public double LatSenderAddress { get; set; }
        public double LongSenderAddress { get; set; }
        public double LatReceiverAddress { get; set; }
        public double LongReceiverAddress { get; set; }
        public double Weight { get; set; }
        public int ShipmentTypeId { get; set; }
    }
    
}