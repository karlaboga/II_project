//using BengosMenu.Data;
using BengostMenu.Data;
using QRCoder;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;
namespace BengosMenu.Controllers
{
    public class MenuController : Controller
    {
        private RestaurantContext db = new RestaurantContext();
        // Homepage: Show categories
        public ActionResult Index()
        {
            var categories = db.Dishes
                .Select(d => d.Category)
                .Distinct()
                .ToList();
            return View(categories);
        }
        // Show dishes by category
        public ActionResult Category(string category)
        {
            var dishes = db.Dishes
                .Include(d => d.DishIngredients.Select(di => di.Produs))
                .Where(d => d.Category == category)
                .ToList();
            ViewBag.Category = category;
            return View(dishes);
        }
        // Generate QR Code Image (Points to Windows host IP)
        public ActionResult GenerateQRCode()
        {
            // REPLACE WITH YOUR WINDOWS HOST'S WIFI IP (get via ipconfig)
            // Default IIS Express ports: http://localhost:8080 or https://localhost:44391
            string url = "http://localhost:8080";
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                using (Bitmap bitmap = qrCode.GetGraphic(20))
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    return File(ms.ToArray(), "image/png");
                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}