using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JediCommunication;

namespace JediFlash
{
    public class Flash
    {
		//Magic string to locate the firmware version. Found that sometimes the word "Copyright" exists twice. Howver with a leading space, it seems
		//unique to be located next to firmware version.               " COPYRIGHT "
		private static readonly byte[] COPYRIGHT_MESSAGE = new byte[] { 0x20, 0x43, 0x6F, 0x70, 0x79, 0x72, 0x69, 0x67, 0x68, 0x74, 0x20 };

		public async Task<bool> ReadFromRadio(string comPort, IProgress<string> progress = null)
		{
			await Task.Run(() =>
			{
				try
				{
					using (var com = new FlashCom(comPort))
                    {
						com.StatusUpdate += (s, e) => progress?.Report(e.Status);
						com.EnterFlashMode();

						var flashContents = new byte[0x80000]; //512MB of flash memory

						for (byte page = 0x40; page < 0x48; page++)
						{
							var offset = (page & 0x0f) * 0x10000;
							for (int i = 0; i < 0x10000; i += 0x80)
							{
								//Reads only support length up to 255! 128 (0x80) used for easy disvisibility
								if (i % 0x4000 == 0)
                                {
                                    progress.Report($"{(float)(offset + i) / (float)(flashContents.Length):P0} Complete");
                                }
								com.Read(page, i, 0x80, flashContents, offset + i);
							}
						}
						progress.Report("Done!");

						//Remove Factory Code
						for (var i = 0x3FFF0; i < 0x40000; i++) flashContents[i] = 0xFF;

						string firmwareVersion = "Unknown Firmware Version";
						int copyrightMessageStart = flashContents.AsSpan().IndexOf(COPYRIGHT_MESSAGE);
						if (copyrightMessageStart > 20)
						{
							for (int i = 1; i < 20; i++)
							{
								if (flashContents[copyrightMessageStart - i] == 0x52)
								{
									firmwareVersion = Encoding.ASCII.GetString(flashContents.AsSpan(copyrightMessageStart - i, 6).ToArray());
									progress.Report($"Firmware identified as {firmwareVersion}");
								}
							}
						}

						string fileName = $"{firmwareVersion}.bin";
						if (File.Exists(fileName))
                        {
							fileName = $"{firmwareVersion}.{DateTime.Now:yyyMMdd-hhmmss}.bin";
						}

						File.WriteAllBytes(fileName, flashContents);
						progress.Report($"File saved to {fileName}");
					}
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
