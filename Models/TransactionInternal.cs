using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DiAnterExpress.Models
{
    public class TransactionInternal
    {
        [Key]
        public int Id { get; set; }
        public string Product { get; set; }
        public int PaymmentId { get; set; }

        public Shipment Shipment { get; set; }
    }
}