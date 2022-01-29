using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiAnterExpress.Externals.Dtos
{
    public class TokopodiaReturnData
    {
        public TransactionUpdateReturn UpdateTransaction { get; set; }
    }

    public class TransactionUpdateReturn
    {
        public string message { get; set; }
    }

    public class TransactionUpdateInputDto
    {
        public int transactionId { get; set; }
    }

}