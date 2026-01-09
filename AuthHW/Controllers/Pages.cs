using Microsoft.AspNetCore.Mvc;

namespace AuthHW.Controllers
{
    [Route("{*url}")]
    public class SpaController : Controller
    {
        public IActionResult Index()
        {
            return PhysicalFile(
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"),
                "text/html"
            );
        }
    }
}