using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace JediCommon
{
    public static class Common
    {
        private static readonly Dictionary<string, Model> _models;
        private static readonly Dictionary<decimal, Firmware> _firmwares;
        static Common()
        {
            var defs = JsonSerializer.Deserialize<JediDefinition>(File.ReadAllText("JediDefinitions.json"));

            _models = defs.Models.ToDictionary(k => k.ModelNumber, e => e);
            _firmwares = defs.Firmwares.ToDictionary(k => k.Version, e => e);
        }

        public static List<Model> Models { get => _models.Values.ToList(); }
        public static List<Firmware> Firmwares { get => _firmwares.Values.ToList(); }

        public static Model GetModel(string modelNumber)
        {
            if (_models.ContainsKey(modelNumber))
            {
                return _models[modelNumber];
            }
            else
            {
                return new Model() { ModelNumber = modelNumber, ModelName = "Unknown", Band = "Unknown", Description = "Unknown" };
            }
        }

        public static Firmware GetFirmware(decimal version)
        {
            if (_firmwares.ContainsKey(version))
            {
                return _firmwares[version];
            }
            else
            {
                return new Firmware() { Version = version, Comments = "Unknown Version" };
            }
        }

        public static string ConvertToHexString(byte[] values, string separator = " ")
        {
            return String.Join(separator, Array.ConvertAll(values, x => x.ToString("X2")));
        }

        public static byte[] ConvertFromHexString(string value)
        {
            value = value.Replace(" ", "").Replace("-", "");
            if (value.Length % 2 != 0)
            {
                throw new Exception("Hex string must be an even number of digits");
            }

            byte[] data = new byte[value.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = value.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;
        }

    }
}
