using MediatR;
using Sales.Orders.Application.Common;
using Sales.Orders.Domain.Entities;

namespace Sales.Orders.Application.Queries;

public record GetPendingOrdersQuery() : IRequest<Result<IEnumerable<OrderDto>>>;

public record OrderDto(Guid Id, int Number, string CompanyName, string CustomerName, string Status, decimal Total);
