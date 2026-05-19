using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BengosMenu.Data;
namespace BengosMenu.Controllers;
public class MenuController : Controller
{
    private readonly RestaurantContext _context;
    public MenuController(RestaurantContext context) => _context = context;
    public async Task<IActionResult> Index()
    {
        var categories = await _context.Dishes
            .Select(d => d.Category).Distinct().ToListAsync();
        return View(categories);
    }
    public async Task<IActionResult> Category(string category)
    {
        var dishes = await _context.Dishes
            .Include(d => d.DishIngredients).ThenInclude(di => di.Produs)
            .Where(d => d.Category == category).ToListAsync();
        ViewBag.Category = category;
        return View(dishes);
    }
}