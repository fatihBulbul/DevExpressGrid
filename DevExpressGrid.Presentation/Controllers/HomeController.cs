using DevExpressGrid.Extensions;
using DevExpressGrid.Extensions.Common;
using DevExpressGrid.Extensions.Models;
using DevExpressGrid.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DevExpressGrid.Presentation.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetData([FromForm] FilterRequest model)
        {
            var context = new ExampleDbContext();
            context.Database.EnsureCreated();
            var result = context.Customers.AsQueryable().Load(model);
            return Json(result);
        }
    }
}
