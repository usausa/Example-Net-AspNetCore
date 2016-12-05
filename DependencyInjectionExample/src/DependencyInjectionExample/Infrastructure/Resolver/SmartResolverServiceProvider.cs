﻿namespace DependencyInjectionExample.Infrastructure.Resolver
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;

    using Smart.Resolver;

    public class SmartResolverServiceProvider : IServiceProvider, ISupportRequiredService
    {
        private static readonly Type EnumerableType = typeof(IEnumerable<>);

        private readonly IResolver resolver;

        /// <summary>
        ///
        /// </summary>
        /// <param name="resolver"></param>
        public SmartResolverServiceProvider(IResolver resolver)
        {
            this.resolver = resolver;
        }

        public object GetService(Type serviceType)
        {
            return GetServiceInternal(serviceType, false);
        }

        public object GetRequiredService(Type serviceType)
        {
            return GetServiceInternal(serviceType, true);
        }

        private object GetServiceInternal(Type serviceType, bool required)
        {
            if (serviceType.GetTypeInfo().IsGenericType && serviceType.GetGenericTypeDefinition() == EnumerableType)
            {
                return ResolverHelper.ConvertArray(
                    serviceType.GenericTypeArguments[0],
                    resolver.ResolveAll(serviceType.GenericTypeArguments[0], null));
            }

            if (required)
            {
                return resolver.Get(serviceType);
            }

            bool result;
            return resolver.TryGet(serviceType, out result);
        }
    }
}
