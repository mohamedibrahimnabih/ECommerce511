
using ECommerce.API.Data;
using ECommerce.API.Models;
using ECommerce.API.Repositories.IRepositories;
using ECommerce.API.Repositroy;

namespace ECommerce.API.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
