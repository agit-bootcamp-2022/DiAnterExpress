using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Models;

namespace DiAnterExpress.Externals.Dtos
{
    public class TransferBalanceDto
    {
        public int CustomerDebitId { get; set; }
        public double Amount { get; set; }
        public int CustomerCreditId { get; set; }
    }

    public class TransferBalanceOutput
    {
        public bool Succeed { get; set; }
        public int ReceiverWalletMutationId { get; set; }
        public string Message { get; set; }
    }

    public class TransactionStatus
    {
        public bool Succeed { get; set; }
        public string Message { get; set; }
    }

    public class ReturnData
    {
        public string message { get; set; }
        public TransferBalanceOutput TransferBalance { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
        public UserToken LoginUser { get; set; }
        public List<ProfileOutput> ProfileByCustomerIdAsync { get; set; }
        public TransactionStatus UpdateStatusTransaction { get; set; }
    }

    public class UserToken
    {
        public string Token { get; set; }
        public string Expired { get; set; }
        public string Message { get; set; }
    }

    public class LoginUserInput
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ProfileOutput
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class ResponseProfileOutput
    {
        public ProfileOutput profilOutput { get; set; }
    }

    public class UpdateStatusTransactionInput
    {
        public int transactionId { get; set; }
        public string transactionStatus { get; set; }
    }
}