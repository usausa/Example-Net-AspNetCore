namespace AutofacExample.Services
{
    using System;
    using System.ComponentModel;

    using AutofacExample.ComponentModel;

    public class ProductionService : IService
    {
        private readonly byte[] key;

        public string Name => $"Production({BitConverter.ToString(key)})";

        public ProductionService([TypeConverter(typeof(HexStringConverter))] byte[] key)
        {
            this.key = key;
        }
    }
}
