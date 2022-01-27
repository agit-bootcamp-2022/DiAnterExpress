using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiAnterExpress.Dtos
{
    public class ShipmentInternalInsertDto
    {
        public string Product { get; set; }
        public double TotalWeight { get; set; }
        public string SenderName { get; set; }
        public string SenderContact { get; set; }
        public Location SenderLocation { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverContact { get; set; }
        public Location ReceiverLocation { get; set; }
        public UserCredential UserCredential { get; set; }
        public int ShipmentTypeId { get; set; }
    }

    public class UserCredential
    {
        public string UangTransUsername { get; set; }
        public string UangTransPassword { get; set; }
    }

    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}