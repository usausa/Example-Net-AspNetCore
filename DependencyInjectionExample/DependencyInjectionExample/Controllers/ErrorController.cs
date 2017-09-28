namespace DependencyInjectionExample.Controllers
{
    using System.Diagnostics;

    using DependencyInjectionExample.Models;

    using Microsoft.AspNetCore.Mvc;

    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
