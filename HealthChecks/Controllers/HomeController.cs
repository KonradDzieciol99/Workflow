// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860
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
