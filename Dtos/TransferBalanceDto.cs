using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiAnterExpress.Dtos
{
    public class TransferBalanceDto
    {
        public int customerDebitId { get; set; }
        public double debitAmount { get; set; }
        public int customerCreditId { get; set; }
        public double creditAmount { get; set; }
    }

    public class GetTransferBalanceDto
    {
        public TransferBalanceDto transferBalance { get; set; }
    }
}