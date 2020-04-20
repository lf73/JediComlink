using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JediCommunication
{
	public class SbepMessage
	{
		public byte OpCode { get; private set; }
		public int Length { get; private set; }
		public byte[] Data { get; private set; }
		public byte Checksum { get; private set; }
		public byte[] Bytes { get; private set; }
		public bool Incomplete { get; private set; }
		public bool Invalid { get; private set; }

		public override string ToString()
		{
			return $"Op: {OpCode:X2} Data: {String.Join(" ", Array.ConvertAll(Data, x => x.ToString("X2")))} ";
		}

		public SbepMessage(byte opCode, params byte[] data)
		{
			OpCode = opCode;
			Data = data;

			if (data != null)
			{
				Length = data.Length + 1;
			}

			var bytes = new List<Byte>();
			bytes.Add((byte)((Math.Min(0x0f, (int)opCode) * 16) + Math.Min(0x0f, Length)));
			if (opCode > 0x0e) bytes.Add(opCode);
			if (Length > 0x0e)
			{
				bytes.Add((byte)(Length / 256));
				bytes.Add((byte)(Length % 256));
			}
			if (data != null)
			{
				bytes.AddRange(data);
				Checksum = (byte)(0xff - (bytes.Sum(x => x) & 0xFF));
				bytes.Add(Checksum);
			}

			Bytes = bytes.ToArray();
		}

		public SbepMessage(byte[] buffer, int bufferLength)
		{
			try
			{
				var msn = buffer[0] >> 4;
				var lsn = buffer[0] & 0xf;
				var dataStart = 0;

				if (msn < 0xf && lsn < 0xf)
				{
					OpCode = (byte)msn;
					Length = lsn;
					dataStart = 1;
				}
				else if (msn == 0xf && lsn < 0xf)
				{
					OpCode = buffer[1];
					Length = lsn;
					dataStart = 2;

				}
				else if (msn < 0xf && lsn == 0xf)
				{
					OpCode = (byte)msn;
					Length = buffer[1];
					dataStart = 3;

				}
				else
				{
					OpCode = buffer[1];
					Length = (buffer[2] * 256) + buffer[3];
					dataStart = 4;
				}

				if (Length > 0)
				{
					if (bufferLength < dataStart + Length)
					{
						Incomplete = true;
						return;
					}

					if (bufferLength > dataStart + Length)
					{
						Invalid = true;
						return;
					}

					if ((buffer.Take(bufferLength).Sum(x => x) & 0xFF) != 0xFF)
					{
						Invalid = true;
					}
				}

				Data = new byte[Length - 1];
				Buffer.BlockCopy(buffer, dataStart, Data, 0, Data.Length);
				Checksum = buffer[bufferLength];

				Bytes = new byte[bufferLength];
				Buffer.BlockCopy(buffer, 0, Bytes, 0, bufferLength);
			}
			catch (Exception)
			{
				Invalid = true;
			}
		}
	}
}
