
using BlazingPizza.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BlazingPizza.Controllers;
[Route("orders")]
[ApiController]
public class OrdersController : Controller
{
    private readonly PizzaStoreContext _db;
    public OrdersController(PizzaStoreContext db)
    {
        _db = db;
    }
    [HttpPost]
    public async Task<ActionResult<int>> PlaceOrder(Order order)
    {
        if (order.Pizzas == null || !order.Pizzas.Any())
        {
            return BadRequest("An order must contain at least one pizza.");
        }
        order.CreatedTime = DateTime.Now;
        order.UserId = "local-user";
        order.DeliveryAddress ??= new Address();
        foreach (var pizza in order.Pizzas)
        {
            var special = await _db.Specials.FindAsync(pizza.SpecialId);
            if (special == null)
            {
                return BadRequest($"Pizza special {pizza.SpecialId} was not found.");
            }
            pizza.Special = special;
            pizza.Toppings ??= new List<PizzaTopping>();
            foreach (var topping in pizza.Toppings)
            {
                var savedTopping = await _db.Toppings.FindAsync(topping.ToppingId);
                if (savedTopping == null)
                {
                    return BadRequest($"Topping {topping.ToppingId} was not found.");
                }
                topping.Topping = savedTopping;
            }
        }
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return Ok(order.OrderId);
    }
    [HttpGet]
    public async Task<ActionResult<List<OrderWithStatus>>> GetOrders()
    {
        var orders = await _db.Orders
            .Include(o => o.Pizzas)
                .ThenInclude(p => p.Special)
            .Include(o => o.Pizzas)
                .ThenInclude(p => p.Toppings)
                    .ThenInclude(t => t.Topping)
            .Include(o => o.DeliveryAddress)
            .ToListAsync();
        return orders
            .OrderByDescending(o => o.CreatedTime)
            .Select(OrderWithStatus.FromOrder)
            .ToList();
    }
    [HttpGet("{orderId:int}")]
    public async Task<ActionResult<OrderWithStatus>> GetOrderWithStatus(int orderId)
    {
        var order = await _db.Orders
            .Where(o => o.OrderId == orderId)
            .Include(o => o.Pizzas)
                .ThenInclude(p => p.Special)
            .Include(o => o.Pizzas)
                .ThenInclude(p => p.Toppings)
                    .ThenInclude(t => t.Topping)
            .Include(o => o.DeliveryAddress)
            .SingleOrDefaultAsync();
        if (order == null)
        {
            return NotFound();
        }
        return OrderWithStatus.FromOrder(order);
    }
}
