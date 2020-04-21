using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JediCommunication
{
    public class Com
    {
        private bool _sbepMode = false;

        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;
        private SerialPort _port;

        public Com(string comPort)
        {
            try
            {
                _port = new SerialPort(comPort)
                {
                    PortName = comPort,
                    ReadTimeout = 2500
                };
                _port.Open();
                UpdateStatus($"Port {comPort} Open");
            }
            catch (Exception ex)
            {
                UpdateStatus(ex.Message);
                return;
            }
        }

        public void Close()
        {
            if (_sbepMode) ExitSbepMode();
            if (_port.IsOpen) _port.Close();
            _port.Dispose();
            _port = null;
        }

        private bool Read(int location, int length, byte[] buffer, int offset)
        {
            if (location < 0 || location > 0xFFFF) throw new ArgumentException("Range must be 0x0000 to 0xFFFF", nameof(location));
            if (length < 1 || length > 0xFF) throw new ArgumentException("Length must be 0x00 to 0xFF", nameof(location));
            if (buffer == null || buffer.Length < offset + length) throw new ArgumentException("Buffer overrun", nameof(location));

            //There is a third byte, but in regular (non-flash), mode it is always 0.
            var msb = (byte)(location / 0x100);
            var lsb = (byte)(location % 0x100);

            if (!_sbepMode) EnterSbepMode();

            SendSbep(new SbepMessage(0x11, (byte)length, 0x00, msb, lsb));
            var response = ReceiveSbep();
            if (response.Data[0] != 0x00 && response.Data[1] != msb && response.Data[2] != lsb)
            {
                UpdateStatus("Invalid Read Response. Address requested not returned.");
                return false;
            }
            if (response.Data.Length - 3 != length)
            {
                UpdateStatus("Invalid Read Response. Length requested not returned.");
                return false;
            }

            response.Data.AsSpan(3).CopyTo(buffer.AsSpan(offset));
            return true;
        }

        public bool Read(int location, int length, byte[] buffer)
        {
            return Read(location, length, buffer, location);
        }

        public byte[] Read(int location, int length)
        {
            if (length < 1 || length > 0xFF) throw new ArgumentException("Length must be 0x00 to 0xFF", nameof(location));
            var buffer = new byte[length];
            var goodRead = Read(location, length, buffer, 0);
            if (goodRead)
            {
                return buffer;
            }
            else
            {
                return null; //Maybe throw error instead.
            }
        }

        public void EnterSbepMode()
        {
            if (_sbepMode) return; //Already in SBEP mode!
            SendSB9600(SB9600Messages.EnterSbep);
            _port.Close();
            Thread.Sleep(250);
            _port.Open();

            _port.DtrEnable = true;
            Thread.Sleep(250);

            var response = new byte[_port.BytesToRead];
            _port.Read(response, 0, response.Length);
            UpdateStatus("Ack: " + String.Join(" ", Array.ConvertAll(response, x => x.ToString("X2"))));
            UpdateStatus("Entered SBEP Mode");
            _sbepMode = true;
        }

        public void ExitSbepMode()
        {
            SendSbep(new SbepMessage(0x10, null));
            Thread.Sleep(25);
            _port.DtrEnable = false;
            Thread.Sleep(100);
            _port.DiscardInBuffer();
            _port.DiscardOutBuffer();
            UpdateStatus("Exited SBEP Mode");
            _port.DtrEnable = true;
            Thread.Sleep(25);
            _port.DtrEnable = false;
            Thread.Sleep(25);
            _sbepMode = false;
        }

        public void EnterProgrammingMode()
        {
            //CSQ mode?
            SendSB9600(SB9600Messages.ProgrammingMode);
            Thread.Sleep(500);
        }

        public void Reset()
        {
            SendSB9600(SB9600Messages.Reset);
        }

        public void SendSB9600(SB9600Message message)
        {
            _port.DiscardInBuffer();
            _port.DiscardOutBuffer();
            var packetSize = message.Bytes.Length;
            _port.DtrEnable = true;
            _port.Write(message.Bytes, 0, packetSize);
            Thread.Sleep(150);
            _port.DtrEnable = false;

            //Whatever sent seems to be echoed back, at least with Jedi.
            //For example a bad checksum is simplied echoed right back without any ack from radio.
            byte[] bytes = new byte[packetSize];
            _port.Read(bytes, 0, packetSize);
            //UpdateStatus("Echoed: " + String.Join(" ", Array.ConvertAll(bytes, x => x.ToString("X2"))));

            if (_port.BytesToRead > 0)
            {
                var response = new byte[_port.BytesToRead];
                _port.Read(response, 0, packetSize);
                UpdateStatus("Received: " + String.Join(" ", Array.ConvertAll(response, x => x.ToString("X2"))));
            }
        }

        public bool SendSbep(SbepMessage message)
        {
            _port.DiscardInBuffer();
            _port.DiscardOutBuffer();
            var packetSize = message.Bytes.Length;
            UpdateStatus("Sending " + String.Join(" ", Array.ConvertAll(message.Bytes, x => x.ToString("X2"))));

            _port.Write(message.Bytes, 0, packetSize);
            Thread.Sleep(30);

            //Whatever sent seems to be echoed back, at least with Jedi.
            //For example a bad checksum is simplied echoed right back without any ack from radio.
            byte[] bytes = new byte[packetSize];
            _port.Read(bytes, 0, packetSize);
            //UpdateStatus("Echoed: " + String.Join(" ", Array.ConvertAll(bytes, x => x.ToString("X2"))));

            int ack = _port.ReadByte();
            UpdateStatus("Ack: " + ack.ToString("X2"));
            return ack == 0x50;
        }

        private readonly byte[] _sbepReceiveBuffer = new byte[1024];
        public SbepMessage ReceiveSbep()
        {
            var i = 0;
            SbepMessage message = null;
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 5000 && i < _sbepReceiveBuffer.Length)
            {
                var avail = _port.BytesToRead;
                if (avail == 0)
                {
                    Thread.Sleep(20);
                    continue; //Don't restart stopwatch.
                }
                _port.Read(_sbepReceiveBuffer, i, avail);
                i += avail;
                message = new SbepMessage(_sbepReceiveBuffer, i);
                if (!message.Incomplete) break;
                sw.Restart(); //Give some more time for bytes
            }

            if (message == null || message.Incomplete || message.Invalid)
            {
                UpdateStatus("Timeout or Checksum Error");
            }
            else
            {
                UpdateStatus("Received: " + String.Join(" ", Array.ConvertAll(message.Bytes.ToArray(), x => x.ToString("X2"))));
            }

            return message;
        }

        protected virtual void OnStatusUpdate(StatusUpdateEventArgs e)
        {
            StatusUpdate?.Invoke(this, e);
        }

        private void UpdateStatus(string status)
        {
            OnStatusUpdate(new StatusUpdateEventArgs() { Status = status });
        }


    }
}
