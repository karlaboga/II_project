using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BengosMenu.Controllers
{
    public class OrdersController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Order management page.";
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(string orderDetails)
        {
            ViewBag.Message = "Order created successfully!";
            return RedirectToAction("Index");
        }
    }
}
