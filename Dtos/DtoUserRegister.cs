using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiAnterExpress.Dtos
{
    public class DtoUserInputBranch
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class DtoUserRegister
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public role Role { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class DtoUserOutput
    {

    }

    public class DtoUserCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class DtoUserIdInput
    {
        public string UserId { get; set; }
    }

    public enum role
    {
        branch
    }
}