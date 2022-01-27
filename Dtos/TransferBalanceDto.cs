using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiAnterExpress.Dtos
{
    public class TransferBalanceDto
    {
        public int customerDebitId { get; set; }
        public double Amount { get; set; }
        public int customerCreditId { get; set; }
    }

    public class GetTransferBalanceDto
    {
        public TransferBalanceDto transferBalance { get; set; }
        public TransactionStatus transactionStatus { get; set; }
    }

    public class TransactionStatus
    {
        public bool Succeed { get; set; }
        public string Message { get; set; }
    }
}