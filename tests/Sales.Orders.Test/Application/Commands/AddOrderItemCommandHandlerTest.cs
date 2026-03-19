using NSubstitute;
using Sales.Orders.Application.Commands;
using Sales.Orders.Domain.Entities;
using Sales.Orders.Domain.Enums;
using Sales.Orders.Domain.Interfaces;
using Sales.Orders.Domain.ValueObjects;

namespace Sales.Orders.Test.Application.Commands;

public class AddOrderItemCommandHandlerTest
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AddOrderItemCommandHandler _handler;

    public AddOrderItemCommandHandlerTest()
    {
        _orderRepository = Substitute.For<IOrderRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new AddOrderItemCommandHandler(_orderRepository, _unitOfWork);
    }

    [Fact(DisplayName = "Should return failure when order is not found")]
    public async Task T1()
    {
        var command = new AddOrderItemCommand(Guid.NewGuid(), Guid.NewGuid(), "Product", 1, 10, 0);
        _orderRepository.FindByIdAsync(command.OrderId).Returns((Order?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Order not found", result.Error);
    }

    [Fact(DisplayName = "Should return failure when order is not pending")]
    public async Task T2()
    {
        var command = new AddOrderItemCommand(Guid.NewGuid(), Guid.NewGuid(), "Product", 1, 10, 0);
        var order = new Order(Customer.Create(Guid.NewGuid(), "Cust").Value!, Company.Create(Guid.NewGuid(), "Comp").Value!, []);
        
        // Force order to not be pending using reflection
        typeof(Order).GetProperty(nameof(Order.OrderStatus))?.SetValue(order, OrderStatus.Completed);
        
        _orderRepository.FindByIdAsync(command.OrderId).Returns(order);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Cannot add item: Order is not pending.", result.Error);
    }

    [Fact(DisplayName = "Should return success, update and commit when order is valid")]
    public async Task T3()
    {
        var command = new AddOrderItemCommand(Guid.NewGuid(), Guid.NewGuid(), "Product", 2, 50, 5);
        var order = new Order(Customer.Create(Guid.NewGuid(), "Cust").Value!, Company.Create(Guid.NewGuid(), "Comp").Value!, []);
        
        _orderRepository.FindByIdAsync(command.OrderId).Returns(order);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        
        // Validate that AddItem actually worked implicitly checking the value
        Assert.Single(order.Items);
        Assert.Equal(95m, order.Total); // 2 * 50 - 5

        _orderRepository.Received(1).Update(order);
        await _unitOfWork.Received(1).CommitAsync();
    }
}
