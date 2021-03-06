﻿namespace AutofacExample.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    public class HexStringConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var s = value as string;
            if (s != null)
            {
                return Decode(s, culture);
            }

            return base.ConvertFrom(context, culture, value);
        }

        private static byte[] Decode(string code, IFormatProvider culture)
        {
            var bytes = new byte[code.Length / 2];

            for (var index = 0; index < bytes.Length; index++)
            {
                bytes[index] = byte.Parse(code.Substring(index * 2, 2), NumberStyles.HexNumber, culture);
            }

            return bytes;
        }
    }
}
