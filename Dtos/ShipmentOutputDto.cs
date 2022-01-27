using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiAnterExpress.Dtos
{
    public class ShipmentOutputDto
    {
        public int Id { get; set; }
        public string SenderName { get; set; }
        public string SenderContact { get; set; }
        public location SenderAddress { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverContact { get; set; }
        public location ReceiverAddress { get; set; }
        public double TotalWeight { get; set; }
        public double Cost { get; set; }
        public string Status { get; set; }
        public string TransactionType { get; set; }
        public int TransactionId { get; set; }
        public int ShipmentTypeId { get; set; }
        public int BranchSrcId { get; set; }
        public int BranchDstId { get; set; }

    }

    public class location
    {
        public double Lat { get; set; }
        public double Long { get; set; }
    }
}