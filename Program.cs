
using BlazingPizza;
using BlazingPizza.Data;
using BlazingPizza.Services;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();
builder.Services.AddScoped<OrderState>();
builder.Services.AddHttpClient();
builder.Services.AddSqlite<PizzaStoreContext>("Data Source=pizza.db");
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.MapBlazorHub();
app.MapControllers();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.MapFallbackToPage("/_Host");
// Initialize the database and update the existing schema.
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PizzaStoreContext>();
    if (db.Database.EnsureCreated())
    {
        SeedData.Initialize(db);
    }
    var connection = db.Database.GetDbConnection();
    await connection.OpenAsync();
    try
    {
        var columns = new HashSet<string>();
        await using (var command = connection.CreateCommand())
        {
            command.CommandText = "PRAGMA table_info('Addresses');";
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                columns.Add(reader.GetString(1));
            }
        }
        if (!columns.Contains("Email"))
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "ALTER TABLE Addresses ADD COLUMN Email TEXT NULL;";
            await command.ExecuteNonQueryAsync();
        }
        if (!columns.Contains("Phone"))
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "ALTER TABLE Addresses ADD COLUMN Phone TEXT NULL;";
            await command.ExecuteNonQueryAsync();
        }
    }
    finally
    {
        await connection.CloseAsync();
    }
}
app.Run();
