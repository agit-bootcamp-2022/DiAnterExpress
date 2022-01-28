using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiAnterExpress.Dtos
{
    public class DtoShipmentCreateTokopodia
    {
        public int transactionId { get; set; }
        public string senderName { get; set; }
        public string senderContact { get; set; }
        public double senderLat { get; set; }
        public double senderLong { get; set; }
        public string receiverName { get; set; }
        public string receiverContact { get; set; }
        public double receiverLat { get; set; }
        public double receiverLong { get; set; }
        public double totalWeight { get; set; }
        public int shipmentTypeId { get; set; }
    }

    public class DtoShipmentCreateReturn
    {
        public int shipmentId { get; set; }
        public string statusOrder { get; set; }
    }
}