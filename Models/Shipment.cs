using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
namespace DiAnterExpress.Models
{
    public class Shipment
    {
        [Key]
        public int id {get; set;}
        [Required]
        public string senderName {get; set;}
        [Required]
        public string senderContact {get; set;}
        [Required]
        public Point senderAddress {get; set;} 
        [Required]
        public string receiverName {get; set;}
        [Required]
        public string receiverContact {get; set;}
        [Required]
         public Point receiverAddress {get; set;} 
        [Required]
        public double totalWeight {get; set;} 
        [Required]
        public double cost {get; set;} 
        [Required]
        public status status { get; set; }
        [Required]
        public int shipmentTypeId {get; set;}
        [Required]
        public int branchId {get; set;}
    }

    public enum status
    {
        orderReceived,
        InTransit,
        Delivered,
    }
}