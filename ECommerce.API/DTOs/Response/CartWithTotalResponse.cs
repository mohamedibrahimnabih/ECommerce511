namespace ECommerce.API.DTOs.Response
{
    public class CartWithTotalResponse
    {
        public IEnumerable<CartResponse>? Carts { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
