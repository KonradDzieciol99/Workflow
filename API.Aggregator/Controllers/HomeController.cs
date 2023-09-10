using Microsoft.AspNetCore.Mvc;

namespace API.Aggregator.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Redirect("./swagger");
    }
}
