using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class HexByteArrayTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is byte[] a)
            {
                return String.Join(" ", Array.ConvertAll(a, x => x.ToString("X2")));
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
                s = s.Replace(" ", "").Replace("-", "");
                if (s.Length % 2 != 0)
                {
                    throw new Exception("Hex string must be an even number of digits");
                }

                byte[] data = new byte[s.Length / 2];
                for (int index = 0; index < data.Length; index++)
                {
                    string byteValue = s.Substring(index * 2, 2);
                    data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                }

                return data;
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
