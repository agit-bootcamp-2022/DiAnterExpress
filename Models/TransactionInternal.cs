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
        public int id {get; set;}
        [Required]
        public string product {get; set;}
        [Required]
        public int paymentId {get; set;}
    }
}