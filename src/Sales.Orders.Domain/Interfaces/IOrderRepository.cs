using Sales.Orders.Domain.Entities;
using Sales.Orders.Domain.Enums;

namespace Sales.Orders.Domain.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<Order?> FindByIdAsync(Guid id);
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status);
    void Update(Order entity);
}