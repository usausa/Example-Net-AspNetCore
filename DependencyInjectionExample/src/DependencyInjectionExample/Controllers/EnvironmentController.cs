namespace DependencyInjectionExample.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    ///
    /// </summary>
    public class EnvironmentController : Controller
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
