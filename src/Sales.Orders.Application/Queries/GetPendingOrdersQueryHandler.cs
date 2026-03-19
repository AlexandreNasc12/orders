using MediatR;
using Sales.Orders.Application.Common;
using Sales.Orders.Domain.Enums;
using Sales.Orders.Domain.Interfaces;

namespace Sales.Orders.Application.Queries;

public class GetPendingOrdersQueryHandler : IRequestHandler<GetPendingOrdersQuery, Result<IEnumerable<OrderDto>>>
{
    private readonly IOrderRepository _orderRepository;

    public GetPendingOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<IEnumerable<OrderDto>>> Handle(GetPendingOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetByStatusAsync(OrderStatus.Pending);

        var orderDtos = orders.Select(o => new OrderDto(
            o.Id,
            o.Number,
            o.Company.Name,
            o.Customer.Name,
            o.OrderStatus.ToString(),
            o.Total
        ));

        return Result<IEnumerable<OrderDto>>.Success(orderDtos);
    }
}
