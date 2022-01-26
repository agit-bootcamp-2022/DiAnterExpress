using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;

namespace DiAnterExpress.Models
{
    public class Shipment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string SenderName { get; set; }
        [Required]
        public string SenderContact { get; set; }
        [Required]
        public Point SenderAddress { get; set; }
        [Required]
        public string ReceiverName { get; set; }
        [Required]
        public string ReceiverContact { get; set; }
        [Required]
        public Point ReceiverAddress { get; set; }
        [Required]
        public double TotalWeight { get; set; }
        [Required]
        public double Cost { get; set; }
        [Required]
        public status Status { get; set; }
        [Required]
        public transactionType TransactionType { get; set; }
        [Required]
        public int TransactionId { get; set; }
        [Required]
        public int ShipmentTypeId { get; set; }
        [Required]
        public int BranchId { get; set; }
    }
    public enum status
    {
        OrderReceived,
        InTransit,
        Delivered,
    }

    public enum transactionType
    {
        Internal,
        Tokopedia,
    }
}