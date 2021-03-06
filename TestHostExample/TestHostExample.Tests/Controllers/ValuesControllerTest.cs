﻿namespace TestHostExample.Controllers
{
    using System.Net.Http;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;

    using Xunit;

    public class ValuesControllerTest
    {
        [Fact]
        public async void GetReturnValueArray()
        {
            var config = new ConfigurationBuilder().Build();

            var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseStartup<Startup>();

            var server = new TestServer(host);

            using (var client = server.CreateClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/api/values/");
                var response = await client.SendAsync(request);

                var content = await response.Content.ReadAsStringAsync();

                Assert.Equal(content, "[\"value1\",\"value2\"]");
            }
        }
    }
}
