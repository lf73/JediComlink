using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JediCommunication;

namespace JediCodeplug
{
    public class Codeplug
    {
        public Block01 InternalCodeplug { get; protected set; } = new Block01();

        public Block30 ExternalCodeplug { get; protected set; } = new Block30();

        public decimal FirmwareVersion { get; set; }

        [TypeConverter(typeof(HexByteArrayTypeConverter))]
        public byte[] FactoryCode { get; set; }

        [TypeConverter(typeof(HexByteArrayTypeConverter))]
        public byte[] CalculatedAuthCode { get; set; }

        public string AuthCodeStatus { get; set; }

        /// <summary>
        /// Original source bytes used to unpack codeplug.
        /// </summary>
        public byte[] OriginalBytes { get; private set; }

        public Codeplug(string path)
        {           
            var contents = File.ReadAllBytes(path);
            OriginalBytes = contents;
            InternalCodeplug.Deserialize(contents, 0);
            ExternalCodeplug.Deserialize(contents, InternalCodeplug.ExternalCodeplugVector);
        }

        public Codeplug(byte[] codeplugBytes)
        {
            OriginalBytes = codeplugBytes;
            InternalCodeplug.Deserialize(codeplugBytes, 0);
            ExternalCodeplug.Deserialize(codeplugBytes, InternalCodeplug.ExternalCodeplugVector);
        }

        public string GetTextDump()
        {
            return InternalCodeplug?.GetTextDump() ?? "" + ExternalCodeplug?.GetTextDump() ?? "";
        }

        public byte[] Serialize()
        {
            byte[] bytes = new byte[0x8200]; //Total Size of Internal and External EEPROMS.
            InternalCodeplug.Serialize(bytes, 0);
            ExternalCodeplug.Serialize(bytes, InternalCodeplug.ExternalCodeplugVector);
            return bytes.Take(InternalCodeplug.ExternalCodeplugVector + ExternalCodeplug.ExternalCodeplugSize).ToArray();
        }

        public static async Task<Codeplug> ReadFromRadio(string comPort, IProgress<string> progress = null)
        {
            Codeplug codeplug = null;
            await Task.Run(() =>
            {
                try
                {
                    using var com = new Com(comPort);
                    com.StatusUpdate += (s, e) => progress?.Report(e.Status);
                    com.EnterProgrammingMode();
                    var firmwareVersion = com.GetFirmwareVersion();
                    var extStartBytes = com.Read(0x0002, 0x02);
                    var extStart = extStartBytes[0] * 0x100 + extStartBytes[1];
                    var lengthBytes = com.Read(extStart + 0x26, 0x02);

                    //Calculate total bytes to read from 0x0000 to end of External Codeplug
                    var length = extStart + (lengthBytes[0] * 0x100 + lengthBytes[1]);
                    var codeplugBytes = new byte[length];

                    for (int i = 0; i < length; i += 0x20)
                    {
                        var bytesToRead = Math.Min(0x20, length - i);
                        com.Read(i, bytesToRead, codeplugBytes);
                    }
                    string fileName = "MTS2000-" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".hex";
                    File.WriteAllBytes(fileName, codeplugBytes);
                    codeplug = new Codeplug(codeplugBytes);
                    codeplug.FirmwareVersion = firmwareVersion;
                    codeplug.FactoryCode = com.Read(0x81F0, 0x10);
                    AuthCode.Calculate(codeplug);
                    com.ExitSbepMode();
                }
                catch (Exception e)
                {
                    if (progress != null)
                    {
                        progress.Report($"\r\n\r\nOperation Failed!\r\n\r\n{e.Message}");
                    }
                    else
                    {
                        throw;
                    }
                }
            }).ConfigureAwait(false);
            return codeplug;
        }

        public void RecalculateAuthCode()
        {
            if (FirmwareVersion == 0.00m) return;
            AuthCode.Calculate(this);
            AuthCodeStatus = "Recalculated";
            CalculatedAuthCode.CopyTo(InternalCodeplug.AuthCode, 0);
        }

        public async Task<bool> WriteToRadio(string comPort, IProgress<string> progress = null)
        {
            var codeplugBytes = Serialize();            
            await Task.Run(() =>
            {
                try
                {
                    using var com = new Com(comPort);
                    com.StatusUpdate += (s, e) => progress?.Report(e.Status);
                    com.EnterProgrammingMode();
                    for (int i = 0x00; i < codeplugBytes.Length; i += 0x20)
                    {
                        var bytesToWrite = Math.Min(0x20, codeplugBytes.Length - i);
                        com.Write(i, bytesToWrite, codeplugBytes);
                    }
                    com.ExitSbepMode();
                    com.Reset();
                }
                catch (Exception e)
                {
                    if (progress != null)
                    {
                        progress.Report($"\r\n\r\nOperation Failed!\r\n\r\n{e.Message}");
                    }
                    else
                    {
                        throw;
                    }
                }
            }).ConfigureAwait(false);
            return true;
        }
    }
}
