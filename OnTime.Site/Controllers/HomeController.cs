using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;

namespace OnTime.Site.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}