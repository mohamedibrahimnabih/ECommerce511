namespace ECommerce.API.DTOs.Response
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Note { get; set; }
        public bool Status { get; set; }
    }
}
