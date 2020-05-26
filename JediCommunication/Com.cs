using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace JediCommunication
{
    public class Com : IDisposable
    {
        private bool _sbepMode = false;

        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;
        private SerialPort _port;

        public Com(string comPort)
        {
            _port = new SerialPort(comPort)
            {
                PortName = comPort,
                ReadTimeout = 2500,
                BaudRate = 9600,
            };
            _port.Open();
            UpdateStatus($"Port {comPort} Open");
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
            if (length < 1 || length > 0xFF) throw new ArgumentException("Length must be 0x01 to 0xFF", nameof(length));
            if (buffer == null || buffer.Length < offset + length) throw new ArgumentException("Buffer overrun", nameof(buffer));

            //There is a third byte, but in regular (non-flash), mode it is always 0.
            var msb = (byte)(location / 0x100);
            var lsb = (byte)(location % 0x100);

            if (!_sbepMode) EnterSbepMode();
             
            SendSbep(new SbepMessage(0x11, (byte)length, 0x00, msb, lsb));
            var response = ReceiveSbep();
            if (response.Data[0] != 0x00 || response.Data[1] != msb || response.Data[2] != lsb)
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
            if (length < 1 || length > 0xFF) throw new ArgumentException("Length must be 0x00 to 0xFF", nameof(length));
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

        private bool Write(int location, int length, byte[] buffer, int offset)
        {
            if (location < 0 || location > 0xFFFF) throw new ArgumentException("Range must be 0x0000 to 0xFFFF", nameof(location));
            if (length < 1 || length > 0xFF) throw new ArgumentException("Length must be 0x00 to 0xFF", nameof(length));
            if (buffer == null || buffer.Length < offset + length) throw new ArgumentException("Buffer overrun", nameof(buffer));

            var payload = new byte[length + 3];
            payload[0] = 0x00; //Most significant memory byte, in regular (non-flash), mode it is always 0.
            payload[1] = (byte)(location / 0x100);
            payload[2] = (byte)(location % 0x100);
            Array.Copy(buffer, offset, payload, 3, length);

            if (!_sbepMode) EnterSbepMode();

            SendSbep(new SbepMessage(0x17, payload));
            var response = ReceiveSbep();
            if (response.Data[0] != 0x00 || response.Data[1] != payload[1] || response.Data[2] != payload[2])
            {
                UpdateStatus("Invalid Write Response. Address requested not returned.");
                return false;
            }

            return true;
        }

        public bool Write(int location, int length, byte[] buffer)
        {
            return Write(location, length, buffer, location);
        }

        public bool Write(int location, params byte[] data)
        {
            var length = data.Length;
            if (length < 1 || length > 0xFF) throw new ArgumentException("Length must be 0x00 to 0xFF", nameof(location));
            return Write(location, length, data, 0);
        }

        public void EnterSbepMode()
        {
            if (_sbepMode) return; //Already in SBEP mode!
            SendSB9600(SB9600Messages.EnterSbep);
            _port.DtrEnable = true;
            _port.RtsEnable = true;
            Thread.Sleep(250);

            var response = new byte[_port.BytesToRead];
            _port.Read(response, 0, response.Length);
            UpdateStatus("Ack: " + String.Join(" ", Array.ConvertAll(response, x => x.ToString("X2"))));
            UpdateStatus("Entered SBEP Mode");
            _sbepMode = true;
        }

        public void ExitSbepMode()
        {
            SendSbep(new SbepMessage(0x10, false, null));
            _port.DtrEnable = false;
            _port.RtsEnable = false;
            Thread.Sleep(1500);
            _port.DiscardInBuffer();
            _port.DiscardOutBuffer();
            UpdateStatus("Exited SBEP Mode");
            _sbepMode = false;
        }

        public void EnterProgrammingMode()
        {
            SendSB9600(SB9600Messages.ProgrammingMode);
            Thread.Sleep(1000);
        }

        public decimal GetFirmwareVersion()
        {           
            if (_sbepMode) ExitSbepMode();

            var message = SendSB9600(SB9600Messages.RequestFirmwareVersion);
            Thread.Sleep(400); //Command sends a few repeat replies. Wait them out.

            return (((message.Data1 >> 4) * 10) + (message.Data1 & 0xF)) + ((((message.Data2 >> 4) * 10) + (message.Data2 & 0xF)) * 0.01M);
        }


        public void Reset()
        {
            SendSB9600(SB9600Messages.Reset);
        }

        public SB9600Message SendSB9600(SB9600Message message)
        {
            int attempts = 0;
            while (true)
            {
                if (_sbepMode) ExitSbepMode();
                var packetSize = message.Bytes.Length;

                _port.DtrEnable = true;
                _port.RtsEnable = true;
                Thread.Sleep(30);
                _port.DiscardInBuffer();
                _port.DiscardOutBuffer();
                UpdateStatus("Sending: " + String.Join(" ", Array.ConvertAll(message.Bytes, x => x.ToString("X2"))));
                _port.Write(message.Bytes, 0, packetSize);
                var echo = ReceiveSB9600();
                if (echo == null || echo.Bytes.Max() == 0x00)
                {
                    if (attempts == 4) throw new Exception("RIB or interface cable does not appear to be connected to the computer. Is the cable connected, and if using a RIB, is it powered on? Is the correct COM port selected?");
                    Thread.Sleep(500);
                    attempts++;
                    continue;
                }

                if (!message.ResponseExpected)
                {
                    _port.DtrEnable = false;
                    _port.RtsEnable = false;
                    return null;
                }

                var response = ReceiveSB9600();
                _port.DtrEnable = false;
                _port.RtsEnable = false;
                Thread.Sleep(30);
                _port.DiscardInBuffer();
                _port.DiscardOutBuffer();

                if (response == null || response.Bytes.Max() == 0x00)
                {
                    if (attempts == 4) throw new Exception("No response from radio. The RIB and/or cable appears to be connected to computer. Is the cable connected to the radio and is the radio powered on?");
                    Thread.Sleep(500);
                    attempts++;
                    continue;
                }
                return response;
            }
        }

        public void SendSbep(SbepMessage message)
        {
            int attempts = 0;
            while (true)
            {
                _port.DiscardInBuffer();
                _port.DiscardOutBuffer();
                var packetSize = message.Bytes.Length;
                UpdateStatus("Sending " + String.Join(" ", Array.ConvertAll(message.Bytes, x => x.ToString("X2"))));

                _port.Write(message.Bytes, 0, packetSize);

                //Every byte sent is echoed back since TX/RX lines are connected.
                //They have to be read back from the serial port before getting to a response packet.

                var sw = Stopwatch.StartNew();
                byte[] echo = new byte[packetSize];
                while (sw.ElapsedMilliseconds < 1000)
                {
                    if (_port.BytesToRead < packetSize)
                    {
                        Thread.Sleep(10);
                        continue;
                    }
                    _port.Read(echo, 0, packetSize);
                    break;
                }
                //A check could be made to see if bytes matched what was sent. But it's kind of pointless
                //as it is not the radio echoing the bytes, it's just the computer hearing itself.
                if (echo.Max() == 0x00)
                {
                    if (attempts == 4) throw new Exception("RIB or interface cable does not appear to be connected to the computer. Did the cable to the computer come loose?");
                    Thread.Sleep(500);
                    attempts++;
                    continue;
                }

                if (!message.ExpectAck) return;

                //Now look for actual response from the radio.
                sw = Stopwatch.StartNew();
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

                if (attempts == 4) throw new Exception("The radio failed to ackowledge command. Try power cycling the radio and running the operation again.");
                Thread.Sleep(500);
                attempts++;
            }
        }

        private readonly byte[] _receiveBuffer = new byte[1024];
        public SB9600Message ReceiveSB9600()
        {
            SB9600Message message = null;
            byte[] buffer = new byte[5];
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 100)
            {
                if (_port.BytesToRead < 5)
                {
                    Thread.Sleep(10);
                    continue;
                }
                _port.Read(buffer, 0, 5);
                message = new SB9600Message(buffer, 5);
                break;
            }

            return message;
        }

        public SbepMessage ReceiveSbep()
        {
            var i = 0;
            SbepMessage message = null;
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 1000 && i < _receiveBuffer.Length)
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

        public void Dispose()
        {
            _port.Close();
            _port.Dispose();
        }
    }
}
