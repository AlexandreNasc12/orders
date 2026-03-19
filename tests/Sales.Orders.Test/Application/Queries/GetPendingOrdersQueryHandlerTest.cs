using NSubstitute;
using Sales.Orders.Application.Queries;
using Sales.Orders.Domain.Entities;
using Sales.Orders.Domain.Enums;
using Sales.Orders.Domain.Interfaces;
using Sales.Orders.Domain.ValueObjects;

namespace Sales.Orders.Test.Application.Queries;

public class GetPendingOrdersQueryHandlerTest
{
    private readonly IOrderRepository _orderRepository;
    private readonly GetPendingOrdersQueryHandler _handler;

    public GetPendingOrdersQueryHandlerTest()
    {
        _orderRepository = Substitute.For<IOrderRepository>();
        _handler = new GetPendingOrdersQueryHandler(_orderRepository);
    }

    [Fact(DisplayName = "Should map returned orders to exact Dto matching order properties")]
    public async Task T1()
    {
        var order = new Order(Customer.Create(Guid.NewGuid(), "Cust").Value!, Company.Create(Guid.NewGuid(), "Comp").Value!, []);
        // Setup mock to return the order
        _orderRepository.GetByStatusAsync(OrderStatus.Pending).Returns(new List<Order> { order });

        var query = new GetPendingOrdersQuery();
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var dtos = result.Value!.ToList();
        
        Assert.Single(dtos);
        Assert.Equal(order.Id, dtos.First().Id);
        Assert.Equal(order.Number, dtos.First().Number);
        Assert.Equal("Comp", dtos.First().CompanyName);
        Assert.Equal("Cust", dtos.First().CustomerName);
        Assert.Equal(OrderStatus.Pending.ToString(), dtos.First().Status);
        Assert.Equal(order.Total, dtos.First().Total);
    }
}
