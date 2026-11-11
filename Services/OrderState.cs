
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
namespace BlazingPizza.Services;
public class OrderState
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;
    public OrderState(HttpClient httpClient, NavigationManager navigationManager)
    {
        _httpClient = httpClient;
        _navigationManager = navigationManager;
    }
    public bool ShowingConfigureDialog { get; private set; }
    public Pizza ConfiguringPizza { get; private set; }
    public Order Order { get; private set; } = new Order();
    public void ShowConfigurePizzaDialog(PizzaSpecial special)
    {
        ConfiguringPizza = new Pizza()
        {
            Special = special,
            SpecialId = special.Id,
            Size = Pizza.DefaultSize,
            Toppings = new List<PizzaTopping>(),
        };
        ShowingConfigureDialog = true;
    }
    public void CancelConfigurePizzaDialog()
    {
        ConfiguringPizza = null;
        ShowingConfigureDialog = false;
    }
    public void ConfirmConfigurePizzaDialog()
    {
        Order.Pizzas.Add(ConfiguringPizza);
        ConfiguringPizza = null;
        ShowingConfigureDialog = false;
    }
    public void RemoveConfiguredPizza(Pizza pizza)
    {
        Order.Pizzas.Remove(pizza);
    }
    public async Task<int> PlaceOrder()
    {
        var orderUrl = _navigationManager.ToAbsoluteUri("orders");
        var response = await _httpClient.PostAsJsonAsync(orderUrl, Order);
        response.EnsureSuccessStatusCode();
        var orderId = await response.Content.ReadFromJsonAsync<int>();
        Order = new Order();
        return orderId;
    }
}
