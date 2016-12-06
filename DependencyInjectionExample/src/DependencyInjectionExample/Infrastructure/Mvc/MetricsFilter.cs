﻿namespace DependencyInjectionExample.Infrastructure.Mvc
{
    using DependencyInjectionExample.Services;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class MetricsFilterAttribute : TypeFilterAttribute
    {
        public MetricsFilterAttribute()
            : base(typeof(MetricsActionFilter))
        {
        }

        private class MetricsActionFilter : IActionFilter
        {
            private readonly MetricsManager metricsManager;

            public MetricsActionFilter(MetricsManager metricsManager)
            {
                this.metricsManager = metricsManager;
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                metricsManager.Increment();
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
            }
        }
    }
}