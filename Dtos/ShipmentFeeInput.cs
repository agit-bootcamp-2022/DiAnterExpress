using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;

namespace DiAnterExpress.Dtos
{
    public class ShipmentFeeInput
    {
        public Location SenderAddress { get; set; }
        public Location ReceiverAddress { get; set; }
        public double Weight { get; set; }
        public int ShipmentTypeId { get; set; }
    }

    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}