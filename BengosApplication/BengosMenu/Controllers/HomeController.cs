using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BengosMenu.Data;
using Bengos.Models;
namespace BengosMenu.Controllers;
public class HomeController : Controller
{
    private readonly RestaurantContext _context;
    public HomeController(RestaurantContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        var dishes = await _context.Dishes
            .Include(d => d.DishIngredients)
            .ThenInclude(di => di.Produs)
            .ToListAsync();
        var grouped = dishes
            .GroupBy(d => d.Category ?? "Other")
            .ToDictionary(g => g.Key, g => g.ToList());
        return View(grouped);
    }
    public IActionResult Privacy()
    {
        return View();
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}