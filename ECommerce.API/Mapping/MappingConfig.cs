using ECommerce.API.DTOs.Request;
using ECommerce.API.DTOs.Response;
using ECommerce.API.Models;
using Mapster;

namespace ECommerce.API.Mapping
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Category, CategoryResponse>()
                .Map(des => des.Note, src => src.Description);
        }
    }
}
