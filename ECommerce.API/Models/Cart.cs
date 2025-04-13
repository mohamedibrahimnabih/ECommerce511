using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Models
{
    [PrimaryKey(nameof(ApplicationUserId), nameof(ProductId))]
    public class Cart
    {
        public string ApplicationUserId { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Count { get; set; }
    }
}
