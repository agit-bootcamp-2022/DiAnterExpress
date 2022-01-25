using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DiAnterExpress.Models
{
    public class Branch
    {
        [Key]
        public int id {get; set;}
        [Required]
        public string name {get; set;}
        [Required]
        public string address {get; set;}
    }
}