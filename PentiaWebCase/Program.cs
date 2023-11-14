var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "salesperson_index",
    pattern: "SalesPerson/Index/{id?}",
    defaults: new { controller = "SalesPerson", action = "Index" });

app.MapControllerRoute(
    name: "salesperson_details",
    pattern: "SalesPerson/Details/{id?}",
    defaults: new { controller = "SalesPerson", action = "Details" });

app.MapControllerRoute(
    name: "orderhistory",
    pattern: "SalesPerson/OrderHistory/{id?}",
    defaults: new { controller = "SalesPerson", action = "OrderHistory" });

app.Run();
