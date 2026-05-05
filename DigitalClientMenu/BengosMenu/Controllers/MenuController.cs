//using BengosMenu.Data;
using BengostMenu.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
namespace BengosMenu.Controllers
{
    public class MenuController : Controller
    {
        private RestaurantContext db = new RestaurantContext();
        public ActionResult Index()
        {
            var dishes = db.Dishes
                .Include(d => d.DishIngredients.Select(di => di.Produs))
                .ToList();
            return View(dishes);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}