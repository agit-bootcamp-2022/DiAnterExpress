using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiAnterExpress.Dtos
{
    public class BranchCreateDto
    {
        public string Name {get; set;}
        public string Address {get; set;}
        public string City {get; set;}
        public int Phone {get; set;}
    }
}