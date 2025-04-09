using Microsoft.AspNetCore.Identity;

namespace ECommerce.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Address { get; set; }
    }
}
