﻿namespace DependencyInjectionExample.Infrastructure.Resolver
{
    using System;
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

    using Smart.Resolver;
    using Smart.Resolver.Configs;

    public static class SmartResolverHelper
    {
        public static IServiceProvider BuildServiceProvider(ResolverConfig config, IEnumerable<ServiceDescriptor> descriptors)
        {
            foreach (var descriptor in descriptors)
            {
                if (descriptor.ImplementationType != null)
                {
                    config
                        .Bind(descriptor.ServiceType)
                        .To(descriptor.ImplementationType)
                        .ConfigureScope(descriptor.Lifetime);
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    config
                        .Bind(descriptor.ServiceType)
                        .ToMethod(kernel => descriptor.ImplementationFactory(kernel.Get<IServiceProvider>()))
                        .ConfigureScope(descriptor.Lifetime);
                }
                else if (descriptor.ImplementationInstance != null)
                {
                    config
                        .Bind(descriptor.ServiceType)
                        .ToConstant(descriptor.ImplementationInstance)
                        .ConfigureScope(descriptor.Lifetime);
                }
            }

            config.Bind<IServiceProvider>().To<SmartResolverServiceProvider>().InSingletonScope();
            config.Bind<IServiceScopeFactory>().To<SmartResolverServiceScopeFactory>().InSingletonScope();
            config.Bind<IHttpContextAccessor>().To<HttpContextAccessor>().InSingletonScope();
            config.Bind<RequestScopeStorage>().ToSelf().InSingletonScope();

            var resolver = config.ToResolver();
            return resolver.Get<IServiceProvider>();
        }

        private static void ConfigureScope(this IBindingInSyntax syntax, ServiceLifetime lifetime)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    syntax.InSingletonScope();
                    break;
                case ServiceLifetime.Transient:
                    syntax.InTransientScope();
                    break;
                case ServiceLifetime.Scoped:
                    syntax.InRequestScope();
                    break;
            }
        }
    }
}
