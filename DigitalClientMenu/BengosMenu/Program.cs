using Microsoft.EntityFrameworkCore;
using BengosMenu.Data;
using BengosMenu.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Entity Framework with SQLite
builder.Services.AddDbContext<RestaurantContext>(options =>
    options.UseSqlite("Data Source=restaurant.db"));

var app = builder.Build();

// Seed the database with sample data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RestaurantContext>();
    context.Database.EnsureCreated();
    
    // Check if we already have data
    if (!context.Dishes.Any())
    {
        // Create sample products
        var tomato = new Produs { Name = "Tomato", Unit = "pcs" };
        var cheese = new Produs { Name = "Cheese", Unit = "g" };
        var chicken = new Produs { Name = "Chicken", Unit = "g" };
        var lettuce = new Produs { Name = "Lettuce", Unit = "pcs" };
        var bun = new Produs { Name = "Bun", Unit = "pcs" };
        var beef = new Produs { Name = "Beef", Unit = "g" };
        var onion = new Produs { Name = "Onion", Unit = "pcs" };
        var ketchup = new Produs { Name = "Ketchup", Unit = "ml" };
        var pasta = new Produs { Name = "Pasta", Unit = "g" };
        var cream = new Produs { Name = "Cream", Unit = "ml" };
        var garlic = new Produs { Name = "Garlic", Unit = "cloves" };
        var coffee = new Produs { Name = "Coffee", Unit = "ml" };
        var milk = new Produs { Name = "Milk", Unit = "ml" };
        var sugar = new Produs { Name = "Sugar", Unit = "g" };
        var iceCream = new Produs { Name = "Ice Cream", Unit = "scoops" };
        var chocolate = new Produs { Name = "Chocolate", Unit = "g" };
        
        context.Produses.AddRange(new[] { tomato, cheese, chicken, lettuce, bun, beef, onion, ketchup, pasta, cream, garlic, coffee, milk, sugar, iceCream, chocolate });
        context.SaveChanges();
        
        // Create sample dishes
        var dishes = new[]
        {
            new Dish
            {
                Name = "Classic Burger",
                Description = "Juicy beef patty with fresh vegetables and cheese",
                Price = 12.99m,
                Category = "Main Course",
                ImageUrl = "",
                DishIngredients = new List<DishIngredient>
                {
                    new DishIngredient { Produs = beef, Quantity = 200 },
                    new DishIngredient { Produs = bun, Quantity = 1 },
                    new DishIngredient { Produs = cheese, Quantity = 50 },
                    new DishIngredient { Produs = tomato, Quantity = 2 },
                    new DishIngredient { Produs = lettuce, Quantity = 1 },
                    new DishIngredient { Produs = onion, Quantity = 1 }
                }
            },
            new Dish
            {
                Name = "Chicken Pasta",
                Description = "Creamy pasta with grilled chicken and garlic",
                Price = 14.50m,
                Category = "Main Course",
                ImageUrl = "",
                DishIngredients = new List<DishIngredient>
                {
                    new DishIngredient { Produs = pasta, Quantity = 250 },
                    new DishIngredient { Produs = chicken, Quantity = 150 },
                    new DishIngredient { Produs = cream, Quantity = 100 },
                    new DishIngredient { Produs = garlic, Quantity = 2 }
                }
            },
            new Dish
            {
                Name = "Caesar Salad",
                Description = "Fresh lettuce with chicken, cheese and dressing",
                Price = 9.99m,
                Category = "Appetizers",
                ImageUrl = "",
                DishIngredients = new List<DishIngredient>
                {
                    new DishIngredient { Produs = lettuce, Quantity = 2 },
                    new DishIngredient { Produs = chicken, Quantity = 100 },
                    new DishIngredient { Produs = cheese, Quantity = 30 }
                }
            },
            new Dish
            {
                Name = "Beef Burger",
                Description = "Premium beef burger with special sauce",
                Price = 15.99m,
                Category = "Main Course",
                ImageUrl = "",
                DishIngredients = new List<DishIngredient>
                {
                    new DishIngredient { Produs = beef, Quantity = 250 },
                    new DishIngredient { Produs = bun, Quantity = 1 },
                    new DishIngredient { Produs = cheese, Quantity = 60 },
                    new DishIngredient { Produs = ketchup, Quantity = 20 },
                    new DishIngredient { Produs = onion, Quantity = 1 }
                }
            },
            new Dish
            {
                Name = "Cappuccino",
                Description = "Rich espresso with steamed milk foam",
                Price = 4.50m,
                Category = "Drinks",
                ImageUrl = "",
                DishIngredients = new List<DishIngredient>
                {
                    new DishIngredient { Produs = coffee, Quantity = 150 },
                    new DishIngredient { Produs = milk, Quantity = 100 },
                    new DishIngredient { Produs = sugar, Quantity = 10 }
                }
            },
            new Dish
            {
                Name = "Chocolate Sundae",
                Description = "Vanilla ice cream with chocolate sauce",
                Price = 6.99m,
                Category = "Desserts",
                ImageUrl = "",
                DishIngredients = new List<DishIngredient>
                {
                    new DishIngredient { Produs = iceCream, Quantity = 2 },
                    new DishIngredient { Produs = chocolate, Quantity = 50 }
                }
            },
            new Dish
            {
                Name = "Grilled Chicken Salad",
                Description = "Healthy salad with grilled chicken breast",
                Price = 11.50m,
                Category = "Appetizers",
                ImageUrl = "",
                DishIngredients = new List<DishIngredient>
                {
                    new DishIngredient { Produs = chicken, Quantity = 180 },
                    new DishIngredient { Produs = lettuce, Quantity = 3 },
                    new DishIngredient { Produs = tomato, Quantity = 3 },
                    new DishIngredient { Produs = cheese, Quantity = 40 }
                }
            },
            new Dish
            {
                Name = "Iced Coffee",
                Description = "Cold coffee with milk and ice",
                Price = 5.50m,
                Category = "Drinks",
                ImageUrl = "",
                DishIngredients = new List<DishIngredient>
                {
                    new DishIngredient { Produs = coffee, Quantity = 200 },
                    new DishIngredient { Produs = milk, Quantity = 100 },
                    new DishIngredient { Produs = sugar, Quantity = 15 },
                    new DishIngredient { Produs = iceCream, Quantity = 1 }
                }
            }
        };
        
        context.Dishes.AddRange(dishes);
        context.SaveChanges();
        Console.WriteLine("Database seeded with sample menu data.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
