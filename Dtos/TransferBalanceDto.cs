using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiAnterExpress.Dtos
{
    public class TransferBalanceDto
    {
        public int CustomerDebitId { get; set; }
        public double Amount { get; set; }
        public int CustomerCreditId { get; set; }
    }
    public class TransactionStatus
    {
        public bool Succeed { get; set; }
        public string Message { get; set; }
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
}