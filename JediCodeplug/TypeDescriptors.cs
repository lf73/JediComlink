using JediCommon;
using System;
using System.ComponentModel;
using System.Globalization;

namespace JediCodeplug
{
    public class HexByteArrayTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is byte[] a)
            {
                return Common.ConvertToHexString(a);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                return Common.ConvertFromHexString(s);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

    public class HexByteValueTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is byte v)
            {
                return ($"0x{v:X2}");
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                s = s.Replace(" ", "").Replace("0x", "");
                return byte.Parse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

    public class HexIntValueTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is int v)
            {
                return ($"0x{v:X4}");
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                s = s.Replace(" ", "").Replace("0x", "");
                return int.Parse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
