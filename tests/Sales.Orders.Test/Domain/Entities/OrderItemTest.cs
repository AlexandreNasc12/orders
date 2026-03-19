using Sales.Orders.Domain.Entities;
using Sales.Orders.Domain.ValueObjects;

namespace Sales.Orders.Test.Domain.Entities;

public class OrderItemTest
{
    private readonly Product _product = new(Guid.NewGuid(), "Product Test");

    [Fact(DisplayName = "Initialize sets properties and calculates total correctly")]
    public void T1()
    {
        var item = new OrderItem(_product, 2.0, 50m, 5m);
        
        Assert.Equal(2.0, item.Quantity);
        Assert.Equal(50m, item.UnitPrice);
        Assert.Equal(5m, item.Discount);
        Assert.Equal(95m, item.Total); // (50 * 2) - 5
    }
}
