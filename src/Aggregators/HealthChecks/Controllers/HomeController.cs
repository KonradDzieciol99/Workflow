using Microsoft.AspNetCore.Mvc;

namespace HealthChecks.Controllers;

public class HomeController : Controller
{
    // GET: localhost:n/<tutaj>
    public IActionResult Index()
    {
        return Redirect("/hc-ui");
    }
}
