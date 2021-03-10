using DevExpressGrid.Extensions;
using DevExpressGrid.Extensions.Models;
using DevExpressGrid.Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevExpressGrid.Presentation.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetData([FromQuery] FilterRequest model)
        {
            var context = new ExampleDbContext();
            context.Database.EnsureCreated();
            var result = context.Customers.AsQueryable().GetData(model);
            return Json(result);
        }
    }
}
