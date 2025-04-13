namespace ECommerce.API.Repositroy.IRepository
{
    public interface IOrderItemRepository : IRepository<OrderItem>
    {
        public void CreateRange(IEnumerable<OrderItem> orderItems);
    }
}
