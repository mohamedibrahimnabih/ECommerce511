using ECommerce.API.Data;
using ECommerce.API.Models;
using ECommerce.API.Repositories.IRepositories;
using ECommerce.API.Repositroy;

namespace ECommerce.API.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext dbContext;

        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

    }
}
