using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Response.MomoPayment
{
    public class MomoQueryGatewayResultDto
    {
        public bool IsSuccessStatusCode { get; set; }
        public int HttpStatusCode { get; set; }
        public string RawResponse { get; set; } = string.Empty;

        public int ResultCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public long? TransId { get; set; }
    }
}
