using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DiAnterExpress.Models
{
    public class ShipmentTransaction
    {
        [Key]
        public int id {get; set;}
        [Required]
        public int shipmentId {get; set;}
        [Required]
        public int trasactionId {get; set;}

        [Required]
        public orderType orderType { get; set; } 
    }

     public enum orderType
    {
        Reguler,
        Fast,
    }
}