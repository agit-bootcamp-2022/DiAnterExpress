using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiAnterExpress.Dtos
{
    public class ShipmentInternalInput
    {
        public string Product { get; set; }
        public double TotalWeight { get; set; }
        public string SenderName { get; set; }
        public string SenderContact { get; set; }
        public Location SenderLocation { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverContact { get; set; }
        public Location ReceiverLocation { get; set; }
        public int UangTransUserId { get; set; }
        public int ShipmentTypeId { get; set; }
    }
}