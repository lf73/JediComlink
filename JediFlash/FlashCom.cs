using JediCommunication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JediFlash
{
    internal class FlashCom : IDisposable
	{  
		//Decrypt Boot Strap and Boot Loader
		private static byte[] _bootStrap = BootCode.BootStrap.Select(x => x ^= 0x55).ToArray();
		private static byte[] _bootLoader = BootCode.BootLoader.Select(x => x ^= 0x55).ToArray();

		public event EventHandler<StatusUpdateEventArgs> StatusUpdate;
        private SerialPort _port;

        public FlashCom(string comPort)
        {
            _port = new SerialPort(comPort)
            {
                PortName = comPort,
                ReadTimeout = 10000,
                BaudRate = 3600,
            };
            _port.Open();
            UpdateStatus($"Port {comPort} Open");
        }

		public void EnterFlashMode()
        {
			UpdateStatus("Power on radio. If already on, power cycle radio.");
			while (!Setup())
			{
				UpdateStatus(Environment.NewLine + "Try again by power cycling radio to on position. If mismatch error, try the following:\r\n  1. Remove programming cable from radio.\r\n  2. Power cycle radio to on position.\r\n  3. Wait for self test.\r\n  4. Re-attach cable to radio.\r\n  5. Power cycle radio.\r\n\r\n");
			}
		}

		private bool Setup()
		{
			_port.BaudRate = 3600;
			UpdateStatus("Waiting for radio to enter bootstrap mode.");
			while (_port.ReadByte() != 0x00) ;
			UpdateStatus("Received break control signal, starting bootstrap code upload.");
			_port.DiscardInBuffer();
			_port.BaseStream.WriteByte(0xff);
			_port.Write(_bootStrap, 0, 0x0400);
			UpdateStatus("Bootstrap upload complete. Verifying...");
			//Verify upload against characters echoed back. Attempted to do this between writes, but any very small delay caused boot strap loader to believe it is done.
			for (int y = 0; y < _bootStrap.Length; y++)
			{
				var x = _port.ReadByte();
				if (x != _bootStrap[y])
				{
					UpdateStatus($"Mismatch {y:x4}: Sent {_bootStrap[y]:x2}  Received: {x:x2}");
					_port.DiscardInBuffer();
					Thread.Sleep(1000);
					_port.DiscardInBuffer();
					return false;
				}
			}
			UpdateStatus("Upload Verified.");

			UpdateStatus("Changing Baud Rate to 115,200.");
			_port.BaudRate = 115200;

			UpdateStatus("Waiting for ackowledgement (0x50)");
			while (_port.ReadByte() != 0x50) ;

			UpdateStatus("Uploading Boot Loader");

			for (int i = 0x00; i < _bootLoader.Length; i += 0x100)
			{
				var bytesToWrite = Math.Min(0x100, _bootLoader.Length - i);
				Write(0x80, i + 0x2000, bytesToWrite, _bootLoader, i);
			}

			SendSbep(new SbepMessage(0x19, new byte[] { }));
			ReceiveSbep(); //0x19 signals that upload is complete and sets led and message. Return is always FF FF FF
			UpdateStatus("Boot Loader Upload Complete");
			UpdateStatus("Green LED should be lit and front display radio should show Jedi-Comlink");
			return true;
		}

		public void SendSbep(SbepMessage message)
		{
			int attempts = 0;
			while (true)
			{
				_port.DiscardInBuffer();
				_port.DiscardOutBuffer();
				var packetSize = message.Bytes.Length;

				_port.Write(message.Bytes, 0, packetSize);

				if (!message.ExpectAck) return;

				//Wait for response
				var sw = Stopwatch.StartNew();
				int ack = 0;
				while (sw.ElapsedMilliseconds < 1000)
				{
					if (_port.BytesToRead < 1)
					{
						Thread.Sleep(10);
						continue;
					}
					ack = _port.ReadByte(); break;
				}

				if (ack == 0x50) return;

				if (attempts == 4) throw new Exception("The radio failed to acknowledge command. Try power cycling the radio and running the operation again.");
				Thread.Sleep(500);
				attempts++;
			}
		}

		private readonly byte[] _receiveBuffer = new byte[1024];
		public SbepMessage ReceiveSbep()
		{
			var i = 0;
			SbepMessage message = null;
			var sw = Stopwatch.StartNew();
			while (sw.ElapsedMilliseconds < 2000 && i < _receiveBuffer.Length)
			{
				var avail = _port.BytesToRead;
				if (avail == 0)
				{
					Thread.Sleep(20);
					continue; //Don't restart stopwatch.
				}
				_port.Read(_receiveBuffer, i, avail);
				i += avail;
				message = new SbepMessage(_receiveBuffer, i);
				if (!message.Incomplete) break;
				sw.Restart(); //Give some more time for bytes
			}
			return message;
		}

		public bool Read(byte page, int location, int length, byte[] buffer, int offset)
		{
			if (location < 0 || location > 0xFFFF) throw new ArgumentException("Range must be 0x0000 to 0xFFFF", nameof(location));
			if (length < 1 || length > 0xFF) throw new ArgumentException("Length must be 0x01 to 0xFF", nameof(length));
			if (buffer == null || buffer.Length < offset + length) throw new ArgumentException("Buffer overrun", nameof(buffer));

			var msb = (byte)(location / 0x100);
			var lsb = (byte)(location % 0x100);

			SendSbep(new SbepMessage(0x11, (byte)length, page, msb, lsb));
			var response = ReceiveSbep();
			if (response.Data[0] != 0x00 && response.Data[1] != msb && response.Data[2] != lsb)
			{
				throw new Exception("Error reading to the requested address.");
			}
			if (response.Data.Length - 3 != length)
			{
				throw new Exception("Error reading to the requested length.");
			}

			response.Data.AsSpan(3).CopyTo(buffer.AsSpan(offset));
			return true;
		}

		public bool Read(byte page, int location, int length, byte[] buffer)
		{
			return Read(page, location, length, buffer, location);
		}

		public byte[] Read(byte page, int location, int length)
		{
			if (length < 1 || length > 0xFF) throw new ArgumentException("Length must be 0x00 to 0xFF", nameof(length));
			var buffer = new byte[length];
			var goodRead = Read(page, location, length, buffer, 0);
			if (goodRead)
			{
				return buffer;
			}
			else
			{
				throw new Exception("Error reading the requested address.");
			}
		}

		public void Write(byte page, int location, int length, byte[] buffer, int offset)
		{
			if (location < 0 || location > 0xFFFF) throw new ArgumentException("Range must be 0x0000 to 0xFFFF", nameof(location));
			if (length < 1) throw new ArgumentException("Length must be greater than 0x00", nameof(length));
			if (buffer == null || buffer.Length < offset + length) throw new ArgumentException("Buffer overrun", nameof(buffer));

			var payload = new byte[length + 3];
			payload[0] = page;
			payload[1] = (byte)(location / 0x100);
			payload[2] = (byte)(location % 0x100);
			Array.Copy(buffer, offset, payload, 3, length);

			SendSbep(new SbepMessage(0x17, payload));
			var response = ReceiveSbep();
			if (response.Data[0] != payload[0] || response.Data[1] != payload[1] || response.Data[2] != payload[2])
			{
				throw new Exception("Error writting to the requested address.");
			}
		}

		public void Write(byte page, int location, int length, byte[] buffer)
		{
			Write(page, location, length, buffer, location);
		}

		public void Write(byte page, int location, params byte[] data)
		{
			var length = data.Length;
			Write(page, location, length, data, 0);
		}


		protected virtual void OnStatusUpdate(StatusUpdateEventArgs e)
        {
            StatusUpdate?.Invoke(this, e);
        }

        private void UpdateStatus(string status)
        {
            OnStatusUpdate(new StatusUpdateEventArgs() { Status = status });
        }

        public void Dispose()
        {
            _port.Close();
            _port.Dispose();
        }
    }
}
