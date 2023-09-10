using Microsoft.AspNetCore.Mvc;

namespace Chat.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Redirect("./swagger");
    }
}
