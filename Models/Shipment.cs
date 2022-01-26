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
        public string SenderName { get; set; }
        public string SenderContact { get; set; }
        [Required]
        public Point SenderAddress { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverContact { get; set; }
        [Required]
        public Point ReceiverAddress { get; set; }
        [Required]
        public double TotalWeight { get; set; }
        [Required]
        public double Cost { get; set; }
        [Required]
        public Status Status { get; set; }
        [Required]
        public TransactionType TransactionType { get; set; }
        [Required]
        public int TransactionId { get; set; }
        public int ShipmentTypeId { get; set; }
        public int BranchId { get; set; }

        public TransactionInternal TransactionInternal { get; set; }
        public ShipmentType ShipmentType { get; set; }
        public Branch Branch { get; set; }
    }

    public enum Status
    {
        OrderReceived,
        SendingToDestBranch,
        ArrivedAtDestBranch,
        Delivered
    }

    public enum TransactionType
    {
        Internal,
        Tokopodia
    }
}