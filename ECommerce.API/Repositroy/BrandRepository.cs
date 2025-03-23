using ECommerce.API.Data;
using ECommerce.API.Models;
using ECommerce.API.Repositories.IRepositories;
using ECommerce.API.Repositroy;

namespace ECommerce.API.Repositories
{
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        private readonly ApplicationDbContext applicationDb;

        public BrandRepository(ApplicationDbContext applicationDb) : base(applicationDb)
        {
            this.applicationDb = applicationDb;
        }
    }
}
