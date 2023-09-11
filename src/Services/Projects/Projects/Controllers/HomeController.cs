using Microsoft.AspNetCore.Mvc;

namespace Projects.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Redirect("./swagger");
    }
}
