using Microsoft.AspNetCore.Mvc;

namespace Notification.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Redirect("./swagger");
    }
}
