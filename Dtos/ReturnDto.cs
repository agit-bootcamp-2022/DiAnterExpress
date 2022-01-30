using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiAnterExpress.Dtos
{
    public class ReturnSuccessDto<T>
    {
        public string status { get; set; } = ReturnStatus.success.ToString();
        public T data { get; set; }
    }

    public class ReturnErrorDto
    {
        public string status { get; set; } = ReturnStatus.error.ToString();
        public string message { get; set; }
    }


    public enum ReturnStatus
    {
        success,
        error,
    }
}