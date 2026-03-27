using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Response.Dashboard
{
    public class BestSellerItemDto
    {
        public Guid ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;

        // Dùng để FE phân biệt dòng này là Sản phẩm hay Giỏ quà ("Product" hoặc "GiftBox")
        public string ItemType { get; set; } = string.Empty;

        public int TotalSoldQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
