using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Response.MomoPayment
{
    public class MomoPaymentStatusResponse
    {
        public Guid OrderId { get; set; }
        public string MomoOrderId { get; set; } = string.Empty;
        public string RequestId { get; set; } = string.Empty;

        public decimal Amount { get; set; }
        public long? TransId { get; set; }

        public int ResultCode { get; set; }
        public string Message { get; set; } = string.Empty;

        public string LocalPaymentStatus { get; set; } = string.Empty;
    }
}
