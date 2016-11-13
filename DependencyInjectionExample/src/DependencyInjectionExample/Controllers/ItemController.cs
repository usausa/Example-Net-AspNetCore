namespace DependencyInjectionExample.Controllers
{
    using System.Collections.Generic;

    using DependencyInjectionExample.Models;
    using DependencyInjectionExample.Services;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    public class ItemController : Controller
    {
        private MasterService MasterService { get; }

        public ItemController(MasterService masterService)
        {
            MasterService = masterService;
        }

        [HttpGet]
        public IEnumerable<ItemEntity> Get()
        {
            return MasterService.QueryItemList();
        }
    }
}
