namespace AutofacExample.Services
{
    using System;
    using System.ComponentModel;

    using AutofacExample.ComponentModel;

    public class ProductionService : IService
    {
        private readonly byte[] token;

        public string Name => $"Production({BitConverter.ToString(token)})";

        public ProductionService([TypeConverter(typeof(HexStringConverter))] byte[] token)
        {
            this.token = token;
        }
    }
}
