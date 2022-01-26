using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiAnterExpress.Dtos
{
    public class DtoShipmentOutput
    {
        public int Id { get; set; }
        public string SenderName { get; set; }
        public string SenderContact { get; set; }
        public location SenderAddress { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverContact { get; set; }
        public location ReceiverAddress { get; set; }
        public int ShipmentId { get; set; }
        public string ShipmentType { get; set; }
        public string Product { get; set; }
        public double TotalWeight { get; set; }
        public double Cost { get; set; }
        public string Status { get; set; }

    }

    public class location
    {
        public double Lat { get; set; }
        public double Long { get; set; }
    }
}