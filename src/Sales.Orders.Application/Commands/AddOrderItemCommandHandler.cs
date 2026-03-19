using MediatR;
using Sales.Orders.Application.Common;
using Sales.Orders.Domain.Entities;
using Sales.Orders.Domain.Interfaces;
using Sales.Orders.Domain.ValueObjects;

namespace Sales.Orders.Application.Commands;

public class AddOrderItemCommandHandler : IRequestHandler<AddOrderItemCommand, Result<Guid>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddOrderItemCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.FindByIdAsync(request.OrderId);
        if (order == null)
        {
            return Result<Guid>.Failure("Order not found");
        }

        if (order.OrderStatus != Sales.Orders.Domain.Enums.OrderStatus.Pending)
        {
            return Result<Guid>.Failure("Cannot add item: Order is not pending.");
        }

        var product = new Product(request.ProductId, request.ProductName);
        var orderItem = new OrderItem(product, request.Quantity, request.UnitPrice, request.Discount);

        order.AddItem(orderItem);

        _orderRepository.Update(order);
        await _unitOfWork.CommitAsync();

        return Result<Guid>.Success(orderItem.Id);
    }
}
