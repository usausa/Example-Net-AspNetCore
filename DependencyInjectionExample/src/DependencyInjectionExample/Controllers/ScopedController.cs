using System;
using DependencyInjectionExample.Services;

namespace DependencyInjectionExample.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class ScopedController : Controller
    {
        private ScopedObject ScopedObject { get; }

        public ScopedController(ScopedObject scopedObject)
        {
            ScopedObject = scopedObject;
        }

        public IActionResult Index([FromServices] ScopedObject scopedObject)
        {
            if (ScopedObject != scopedObject)
            {
                throw new InvalidOperationException("Scoped object unmatch.");
            }

            return View(ScopedObject);
        }
    }
}
