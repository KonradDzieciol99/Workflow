using Microsoft.AspNetCore.Mvc;

namespace Tasks.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Redirect("/swagger");
    }
}
