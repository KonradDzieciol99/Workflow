﻿using Microsoft.AspNetCore.Mvc;

namespace Photos.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Redirect("./swagger");
    }
}
