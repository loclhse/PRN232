using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Request.MomoPayment
{
    public class MomoCreateGatewayRequestDto
    {
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string TransactionReference { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? OrderInfo { get; set; }
    }
}
