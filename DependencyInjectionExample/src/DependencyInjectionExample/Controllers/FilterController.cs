namespace DependencyInjectionExample.Controllers
{
    using DependencyInjectionExample.Infrastructure.Mvc;
    using DependencyInjectionExample.Services;

    using Microsoft.AspNetCore.Mvc;

    [MetricsFilter]
    public class FilterController : Controller
    {
        public IActionResult Index([FromServices] MetricsManager metricsManager)
        {
            return View(metricsManager);
        }
    }
}
