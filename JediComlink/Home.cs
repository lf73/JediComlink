using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace JediComlink
{
    public partial class Home : Form
    {
        private static string ComPort = "COM1";
        public Home()
        {
            InitializeComponent();
        }

        private void Test()
        {
            var codeplug = File.ReadAllBytes("image.hex");
            var changeLog = new StringBuilder();
            serialPort.BaudRate = 9600;
            serialPort.PortName = "COM4";
            serialPort.ReadTimeout = 25;
            serialPort.Open();
            byte[] buffer  = new byte[50];
            int offset = 0;

            while (!backgroundWorker.CancellationPending)
            {
                try
                {
                    buffer[offset] = (byte)serialPort.ReadByte();
                    serialPort.Write(buffer, offset, 1);
                    offset++;
                }
                catch (TimeoutException) //Awful way to do this. Change to a timing.
                {
                    if (offset > 0)
                    {
                        //serialPort.Write(buffer, 0, offset);

                        /*
                         * 
                         * 1A 12 1A 06 DB 
                            03 04 02 F6 
                            02 46 B7 
                         */


                        var token = buffer.AsSpan(0, offset - 1);

                        if (token.SequenceEqual(new byte[] { 0x01, 0x02, 0x00, 0x40 }))
                        {
                            backgroundWorker.ReportProgress(0, "ENTER TEST MODE");
                        }
                        else if (token.SequenceEqual(new byte[] { 0x00, 0x01, 0x01, 0x08 }))
                        {
                            backgroundWorker.ReportProgress(0, "Memory Access");
                        }
                        else if (buffer[0] == 0x08 && buffer[1] == 0x00 && buffer[2] == 0x00 && buffer[3] == 0xC0 && buffer[4] == 0xAB)
                        {
                            backgroundWorker.ReportProgress(0, "Request Firmware Version");
                            //  0x08 0x73  8.73   (0x40 unknown)
                            serialPort.Write(new byte[] { 0x08, 0x08, 0x73, 0x40, 0x44 }, 0, 5);
                        }
                        else if (buffer[0] == 0x00 && buffer[1] == 0x12 && buffer[2] == 0x01 && buffer[3] == 0x06 && buffer[4] == 0x02)
                        {
                            backgroundWorker.ReportProgress(0, "ENTER SBEP MODE");
                            serialPort.Write(new byte[] { 0x50 }, 0, 1);
                        }
                        else if (buffer[0] == 0xF1 && buffer[1] == 0x19 && buffer[2] == 0xF5)
                        {
                            backgroundWorker.ReportProgress(0, "Req RadioKey??");
                            serialPort.Write(new byte[] { 0x50 }, 0, 1);
                            serialPort.Write(new byte[] { 0xF2 }, 0, 1);
                            serialPort.Write(new byte[] { 0x87, 0x01, 0x85 }, 0, 3);
                        }
                        else if (buffer.AsSpan(0, 2).SequenceEqual(new byte[] { 0xF5, 0x11 }))
                        {
                            var len = buffer[2];
                            var location = buffer[4] * 256 + buffer[5];
                            var bytes = codeplug.AsSpan(location, len);

                            backgroundWorker.ReportProgress(0, $"Read Memory  {len} bytes at 0x{location.ToString("x2")}");
                            serialPort.Write(new byte[] { 0x50 }, 0, 1);

                            if (len + 4 < 16)
                            {
                                byte packetLength = (byte)(len + 4 + 0xf0);
                                SendSbep2((new byte[] { packetLength, 0x80, buffer[3], buffer[4], buffer[5] }).Concat(bytes.ToArray()).ToArray());
                            }
                            else
                            {
                                byte packetLength = (byte)(len + 4);
                                SendSbep2((new byte[] { 0xFF, 0x80, 0x00, packetLength, buffer[3], buffer[4], buffer[5] }).Concat(bytes.ToArray()).ToArray());
                            }

                            //serialPort.Write(new byte[] { 0xF8 }, 0, 1);
                            //serialPort.Write(new byte[] { 0x80, 0x00, 0x00, 0x00, 0x3B, 0x01, 0x02, 0x00, 0x49 }, 0, 9);
                        }
                        else if (buffer.AsSpan(0, 2).SequenceEqual(new byte[] { 0xFF, 0x17 }))
                        {
                            var len = buffer[3] - 4;
                            var location = buffer[5] * 256 + buffer[6];

                            backgroundWorker.ReportProgress(0, $"Write Memory  {len} bytes at 0x{location.ToString("x2")}");
                            serialPort.Write(new byte[] { 0x50 }, 0, 1);

                            for(int i=location, j = 7; i < location + len; i++, j++)
                            {
                                if (codeplug[i] != buffer[j])
                                {
                                    changeLog.AppendLine($" --> {i:X2}  {codeplug[i]:X2}   {buffer[j]:X2}");
                                }
                                codeplug[i] = buffer[j];
                            }

                            SendSbep2(new byte[] { 0xF4, 0x84, buffer[4], buffer[5], buffer[6] });

                        }
                        //else if (buffer.AsSpan(0, 7).SequenceEqual(new byte[] { 0xF5, 0x11, 0x02, 0x00, 0x02, 0x26, 0xCF }))
                        //{
                        //    backgroundWorker.ReportProgress(0, "Read Memory 2 bytes at 0x0226");
                        //    serialPort.Write(new byte[] { 0x50 }, 0, 1);
                        //    serialPort.Write(new byte[] { 0xF6 }, 0, 1);
                        //    serialPort.Write(new byte[] { 0x80, 0x00, 0x02, 0x26, 0x08, 0x0C, 0x4D }, 0, 7);
                        //}
                        //else if (buffer.AsSpan(0, 7).SequenceEqual(new byte[] { 0xF5, 0x11, 0x20, 0x00, 0x00, 0x00, 0xD9 }))
                        //{
                        //    backgroundWorker.ReportProgress(0, "Read Memory");
                        //    serialPort.Write(new byte[] { 0x50 }, 0, 1);
                        //    serialPort.Write(new byte[] { 0xFF }, 0, 1);
                        //    serialPort.Write(new byte[] { 0x80, 0x00, 0x24 }, 0, 3);
                        //    serialPort.Write(new byte[] { 0x80, 0x00, 0x02, 0x26, 0x08, 0x0C, 0x4D }, 0, 7);
                        //}
                        else if (token.SequenceEqual(new byte[] { 0xF1, 0x10 }))
                        {
                            backgroundWorker.ReportProgress(0, "RESET");
                            serialPort.Write(new byte[] { 0x50 }, 0, 1);
                        }
                        else if (token.SequenceEqual(new byte[] { 0x1A, 0x12, 0x1A, 0x06 }))
                        {
                            backgroundWorker.ReportProgress(0, "Unknown -- Called from Read Flash 0x1A, 0x12, 0x1A, 0x06");
                        }
                        else if (token.SequenceEqual(new byte[] { 0x03, 0x04, 0x02 }))
                        {
                            backgroundWorker.ReportProgress(0, "Unknown -- Called from Read Flash 0x03, 0x04, 0x02");
                            serialPort.Write(new byte[] { 0x50 }, 0, 1);
                        }
                        else
                        {
                            serialPort.Write(new byte[] { 0x50 }, 0, 1);
                            string output = "";
                            for (int i = 0; i < offset; i++) output += buffer[i].ToString("X2") + " ";
                            backgroundWorker.ReportProgress(0, output);
                        }

                        offset = 0;
                        for (int i = 0; i < offset; i++) buffer[i] = 0;
                    }
                }
            }
            serialPort.Close();
            System.IO.File.WriteAllBytes("image.hex", codeplug);
            backgroundWorker.ReportProgress(0, changeLog.ToString());
        }

        private void GetBlock01()
        {
            backgroundWorker.ReportProgress(0, "\r\n\r\nReading Block 01");
            var sw = new System.Diagnostics.Stopwatch();
            sw.Restart();

            try
            {
                serialPort.PortName = ComPort;
                serialPort.ReadTimeout = 500;
                serialPort.Open();
            }
            catch (Exception ex)
            {
                backgroundWorker.ReportProgress(0, ex.Message);
                return;
            }

            try
            {
                EnterSBEP();
                Thread.Sleep(500);

                SendSbep(new byte[] { 0xF5, 0x11, 0x3B, 0x00, 0x00, 0x02 });
                var response = ReceiveSbep();
                var bytes = new Span<byte>(response, 3, 0x3B);

                int checkSumCalc = -0X55 + 0x3B + 0x01; //Seed + Block Size + Block ID

                for (int i = 0; i < bytes.Length -1; i++)
                {
                    checkSumCalc += bytes[i];
                }
                checkSumCalc &= 0xff;

                var ExternalRadioBlock = bytes.Slice(0x00, 2).ToArray();
                var Serial = Encoding.ASCII.GetString(bytes.Slice(0x02, 10).ToArray());
                var Model = Encoding.ASCII.GetString(bytes.Slice(0x0C, 12).ToArray());
                int CheckSum = bytes[0x3A];

                backgroundWorker.ReportProgress(0, $"Serial {Serial} Model {Model} Checksum {CheckSum:X2} Calc {checkSumCalc:X2}");

                return;
                //Change something
                //var newBlcok = bytes.ToArray();

                //Good Block
                var newBlcok = new byte[] {
                   0x02, 0x00, 0x34, 0x36, 0x36, 0x41, 0x41, 0x51,
                   0x35, 0x34, 0x38, 0x33, 0x48, 0x30, 0x31, 0x55,
                   0x43, 0x44, 0x36, 0x50, 0x57, 0x31, 0x42, 0x4E,
                   0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x02, 0x00,
                   0x00, 0x01, 0x00, 0x00, 0x00, 0x3D, 0x01, 0x99,
                   0x00, 0x00, 0x00, 0x00, 0x01, 0xDC, 0x01, 0x00,
                   0x53, 0xC2, 0xA1, 0xAF, 0xCB, 0xC6, 0xDB, 0x22,
                   0x09, 0xBA, 0xD0 };

                ////Bad Block
                //newBlcok = new byte[] {
                //   0x02, 0x00, 0x34, 0x36, 0x36, 0x41, 0x41, 0x51,
                //   0x36, 0x34, 0x38, 0x33, 0x48, 0x30, 0x31, 0x55,
                //   0x43, 0x44, 0x36, 0x50, 0x57, 0x31, 0x42, 0x4E,
                //   0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x02, 0x00,
                //   0x00, 0x01, 0x00, 0x00, 0x00, 0x3D, 0x01, 0x99,
                //   0x00, 0x00, 0x00, 0x00, 0x01, 0xDC, 0x01, 0x00,
                //   0x53, 0xC2, 0xA1, 0xAF, 0xCB, 0xC6, 0xDB, 0x22,
                //   0x09, 0xBA, 0xD0 };


            //newBlcok = File.ReadAllBytes("Block01.HEX");

            //newBlcok[11] = 0x34;  //Was 33

            int newCheckSumCalc = -0X55 + 0x3B + 0x01; //Seed + Block Size + Block ID
                for (int i = 0; i < newBlcok.Length - 1; i++)
                {
                    newCheckSumCalc += newBlcok[i];
                }
                newCheckSumCalc &= 0xff;
                newBlcok[0x3A] = (byte)newCheckSumCalc;

                byte[] command = new byte[] { 0xFF, 0x17, 0x00, 0x3F, 0x00, 0x00, 0x02 };
                command = command.Concat(newBlcok).ToArray();
                backgroundWorker.ReportProgress(0, FormatHex(command));
                SendSbep(command);

                ExitSBEP();
                Reset();

                //File.WriteAllBytes("Block01.HEX", newBlcok);
            }
            catch (Exception ex)
            {
                backgroundWorker.ReportProgress(0, ex.Message);
                return;
            }
            finally
            {
                serialPort.Close();
                sw.Stop();
                backgroundWorker.ReportProgress(0, $"Finished {sw.ElapsedMilliseconds:N0}");
            }
        }

        private void SendSbep2(byte[] data)
        {
            var msg = new byte[data.Length + 1];
            var crc = 0;
            for (int i = 0; i < data.Length; i++)
            {
                crc = (crc + data[i]) & 0xff;
                msg[i] = data[i];
            }
            crc ^= 0xFF;
            msg[msg.Length - 1] = (byte)crc;

            //backgroundWorker.ReportProgress(0, "WRITE: " + String.Join(" ", Array.ConvertAll(msg, x => x.ToString("X2"))));

            serialPort.Write(msg, 0, msg.Length);
        }


        static byte x = 0;
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
//            Test();
  //          return;
            //if (DateTime.Now.Day == 29)
            //{
            //    //GetBlock01();
            //    Test();
            //    return;
            //}

            var sw = new System.Diagnostics.Stopwatch();
            sw.Restart();

            try
            {
                serialPort.PortName = ComPort;
                serialPort.ReadTimeout = 2500;
                serialPort.Open();
            }
            catch (Exception ex)
            {
                backgroundWorker.ReportProgress(0, ex.Message);
                return;
            }

            //EnterProgrammingMode();

            //SendSB9600(0x08, 0x00, 0x00, 0xC0);  //Check firmware version
            //T 08 00 00 C0 AB
            //R 08 00 00 C0 AB 08 08 73 40 44  //Version 8.73
            //                 08 08 73 40 44
            //                 08 08 73 40 44
            //                 08 08 73 40 44
            //                 08 08 73 40 44


            string fileName = "MTS2000-" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".hex";
            backgroundWorker.ReportProgress(0, $"Output File: {fileName}");

            try
            {
                EnterSBEP();
                Thread.Sleep(500);

                //Write Memory
                //var bytes = File.ReadAllBytes("MTS2000-2019-10-23_17-24-39.hex");
                //for (int msb = 0x00; msb < 0x80; msb++)
                //{
                //    for (int lsb = 0; lsb < 0xFF; lsb += 0x20)
                //    {
                //        if (msb == 0x7F && lsb == 0xE0) break;

                //        var instr = new byte[] { 0xFF, 0x17, 0x00, 0x24, 0x00, (byte)msb, (byte)lsb };
                //        var data = bytes.AsSpan((msb * 256) + lsb, 0x20).ToArray();
                //        var packet = instr.Concat(data).ToArray();

                //        SendSbep(packet);
                //        var response = ReceiveSbep();
                //    }
                //}

                //for (int i = 0x00; i <= 0xFF; i++)
                //{
                //    SendSbep(new byte[] { 0xF2, 0x20, (byte)i});
                //    var response = ReceiveSbep();
                //    //48 30 31 55 43 44 36 50 57 31 42 4E
                //    //SendSbep(new byte[] { 0xF5, 0x11, 0x3B, 0x00, 0x00, 0x02 });
                //    //response = ReceiveSbep();
                //}




                //Read Memory
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {

                    for (int msb = 0x00; msb < 0x100; msb++)
                    {
                        for (int lsb = 0; lsb < 0xFF; lsb += 0x20)
                        {
                            //MTS 2000 Codeplug from 0x00 to 0x7FDF
                            //Then repeats first 0x200 bytes starting at 0x7FE0
                            if (msb == 0x7F && lsb == 0xE0) break;
                            SendSbep(new byte[] { 0xF5, 0x11, 0x20, 0x00, (byte)msb, (byte)lsb });
                            var response = ReceiveSbep();
                            fs.Write(response, 3, 32);
                        }
                    }
                }

                backgroundWorker.ReportProgress(0, $"\r\n\r\nOutput File: {fileName}\r\n\r\n");

                ExitSBEP();
                Reset();
            }
            catch (Exception ex)
            {
                backgroundWorker.ReportProgress(0, ex.Message);
                return;
            }
            finally
            {
                serialPort.Close();
                sw.Stop();
                backgroundWorker.ReportProgress(0, $"Finished {sw.ElapsedMilliseconds:N0}");
            }

            // SendSbep(new byte[] { 0xF1, 0x19 });
            // var xxx = ReceiveSbep();

            //Test Write
            //SendSbep(new byte[] { 0xF5, 0x17, 0x00, 0x00, 0x04, 0x35 });
            //var xxx = ReceiveSbep();

            //Fix ASK
            //SendSbep(new byte[] { 0xF5, 0x17, 0x00, 0x02, 0x82, 0x00 });
            //var xxx = ReceiveSbep();
            //SendSbep(new byte[] { 0xF5, 0x17, 0x00, 0x02, 0x89, 0x94 });
            //var xxdd = ReceiveSbep();





        }

        private void EnterProgrammingMode()
        {
            //CSQ mode?  
            SendSB9600(0x01, 0x02, 0x00, 0x40);
            Thread.Sleep(500);
        }

        private void EnterSBEP()
        {
            backgroundWorker.ReportProgress(0, "Entering SBEP Mode");
            SendSB9600(0x00, 0x12, 0x01, 0x06);
            serialPort.Close();
            Thread.Sleep(250);
            serialPort.Open();

            serialPort.DtrEnable = true;
            Thread.Sleep(250);

            var response = new byte[serialPort.BytesToRead];
            serialPort.Read(response, 0, response.Length);
            backgroundWorker.ReportProgress(0, "Ack: " + String.Join(" ", Array.ConvertAll(response, x => x.ToString("X2"))));

            backgroundWorker.ReportProgress(0, "Entered SBEP Mode");
        }

        private void ExitSBEP()
        {
            serialPort.Write(new byte[] {0xF1, 0x10, 0xFE }, 0, 3);
            Thread.Sleep(25);
            serialPort.DtrEnable = false;
            Thread.Sleep(25);
            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
            backgroundWorker.ReportProgress(0, "Exited SBEP Mode");
            serialPort.DtrEnable = true;
            Thread.Sleep(25);
            serialPort.DtrEnable = false;
            Thread.Sleep(25);
        }

        private void Reset()
        {
            SendSB9600(0x00, 0x00, 0x01, 0x08);
        }


        private void SendSB9600(byte address, byte subaddress, byte value, byte operation)
        {
            int crc = 0;
            crc = SB9600CRCTable[crc ^ address];
            crc = SB9600CRCTable[crc ^ subaddress];
            crc = SB9600CRCTable[crc ^ value];
            crc = SB9600CRCTable[crc ^ operation];

            var msg = new byte[] {address, subaddress, value, operation, (byte)crc};
            backgroundWorker.ReportProgress(0, "Sending " + String.Join(" ", Array.ConvertAll(msg, x => x.ToString("X2"))));

            serialPort.DtrEnable = true;
            serialPort.Write(msg, 0, 5);
            Thread.Sleep(150);
            serialPort.DtrEnable = false;

            //Whatever sent seems to be echoed back, at least with Jedi.
            //For example a bad checksum is simplied echoed right back without any ack from radio.
            byte[] bytes = new byte[5];
            serialPort.Read(bytes, 0, 5);
            //backgroundWorker.ReportProgress(0, "Echoed: " + String.Join(" ", Array.ConvertAll(bytes, x => x.ToString("X2"))));

            if (serialPort.BytesToRead > 0)
            {
                var response = new byte[serialPort.BytesToRead];
                serialPort.Read(response, 0, response.Length);
                backgroundWorker.ReportProgress(0, "Received: " + String.Join(" ", Array.ConvertAll(response, x => x.ToString("X2"))));
            }
        }

        private void SendSbep(byte[] data)
        {
            backgroundWorker.ReportProgress(0, "");
            var msg = new byte[data.Length + 1];
            var crc = 0;
            for (int i = 0; i < data.Length; i++)
            {
                crc = (crc + data[i]) & 0xff;
                msg[i] = data[i];
            }
            crc ^= 0xFF;
            msg[msg.Length - 1] = (byte)crc;

            backgroundWorker.ReportProgress(0, "Sending " + String.Join(" ", Array.ConvertAll(msg, x => x.ToString("X2"))));

            serialPort.Write(msg, 0, msg.Length);
            Thread.Sleep(30);

            //Whatever sent seems to be echoed back, at least with Jedi.
            //For example a bad checksum is simplied echoed right back without any ack from radio.
            byte[] bytes = new byte[msg.Length];
            serialPort.Read(bytes, 0, msg.Length);
            //backgroundWorker.ReportProgress(0, "Echoed: " + String.Join(" ", Array.ConvertAll(bytes, x => x.ToString("X2"))));

            int ack = serialPort.ReadByte();
            backgroundWorker.ReportProgress(0, "Ack: " + ack.ToString("X2"));
        }

        private byte[] ReceiveSbep()
        {
            var msg = new List<byte>();

            var hdr = serialPort.ReadByte();
            msg.Add((byte)hdr);

            var op = (hdr >> 4) & 0xf;
            var datalen = hdr & 0xf;

            if (op == 0xF)
            {
                op = serialPort.ReadByte();
                msg.Add((byte)op);
            }

            if (datalen == 0xF)
            {
                if (op != 0xF) msg.Add((byte)serialPort.ReadByte());
                datalen = serialPort.ReadByte();
                msg.Add((byte)datalen);
            }

            var data = new List<byte>();
            for (int i=0; i < datalen; i++)
            {
                byte x = (byte)serialPort.ReadByte();
                msg.Add(x);
                if (i+1 < datalen) data.Add(x);
            }

            var crc = 0;
            foreach (var b in msg)
            {
                crc = (crc + b) & 0xFF;
            }
            crc ^= 0xFF;

            backgroundWorker.ReportProgress(0, "msg  " + String.Join(" ", Array.ConvertAll(msg.ToArray(), x => x.ToString("X2"))));


            if (crc != 0)
            {
                throw new Exception("Failed CRC Check");
            }

            backgroundWorker.ReportProgress(0, "Received " + String.Join(" ", Array.ConvertAll(data.ToArray(), x => x.ToString("X2"))));

            return data.ToArray();
        }



        private void ReadButton_Click(object sender, EventArgs e)
        {
            if(backgroundWorker.IsBusy)
            {
                backgroundWorker.CancelAsync();
                ReadButton.BackColor = System.Drawing.Color.LightGray;
                return;
            }

            ReadButton.BackColor = System.Drawing.Color.Green;
            Status.Text = "";
            backgroundWorker.RunWorkerAsync();
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            Status.AppendText(e.UserState as string);
            Status.AppendText("\r\n");
        }



        private static readonly byte[] SB9600CRCTable = {
              0x00, 0x99, 0xad, 0x34, 0xc5, 0x5c, 0x68, 0xf1, 0x15, 0x8c, 0xb8, 0x21,
              0xd0, 0x49, 0x7d, 0xe4, 0x2a, 0xb3, 0x87, 0x1e, 0xef, 0x76, 0x42, 0xdb,
              0x3f, 0xa6, 0x92, 0x0b, 0xfa, 0x63, 0x57, 0xce, 0x54, 0xcd, 0xf9, 0x60,
              0x91, 0x08, 0x3c, 0xa5, 0x41, 0xd8, 0xec, 0x75, 0x84, 0x1d, 0x29, 0xb0,
              0x7e, 0xe7, 0xd3, 0x4a, 0xbb, 0x22, 0x16, 0x8f, 0x6b, 0xf2, 0xc6, 0x5f,
              0xae, 0x37, 0x03, 0x9a, 0xa8, 0x31, 0x05, 0x9c, 0x6d, 0xf4, 0xc0, 0x59,
              0xbd, 0x24, 0x10, 0x89, 0x78, 0xe1, 0xd5, 0x4c, 0x82, 0x1b, 0x2f, 0xb6,
              0x47, 0xde, 0xea, 0x73, 0x97, 0x0e, 0x3a, 0xa3, 0x52, 0xcb, 0xff, 0x66,
              0xfc, 0x65, 0x51, 0xc8, 0x39, 0xa0, 0x94, 0x0d, 0xe9, 0x70, 0x44, 0xdd,
              0x2c, 0xb5, 0x81, 0x18, 0xd6, 0x4f, 0x7b, 0xe2, 0x13, 0x8a, 0xbe, 0x27,
              0xc3, 0x5a, 0x6e, 0xf7, 0x06, 0x9f, 0xab, 0x32, 0xcf, 0x56, 0x62, 0xfb,
              0x0a, 0x93, 0xa7, 0x3e, 0xda, 0x43, 0x77, 0xee, 0x1f, 0x86, 0xb2, 0x2b,
              0xe5, 0x7c, 0x48, 0xd1, 0x20, 0xb9, 0x8d, 0x14, 0xf0, 0x69, 0x5d, 0xc4,
              0x35, 0xac, 0x98, 0x01, 0x9b, 0x02, 0x36, 0xaf, 0x5e, 0xc7, 0xf3, 0x6a,
              0x8e, 0x17, 0x23, 0xba, 0x4b, 0xd2, 0xe6, 0x7f, 0xb1, 0x28, 0x1c, 0x85,
              0x74, 0xed, 0xd9, 0x40, 0xa4, 0x3d, 0x09, 0x90, 0x61, 0xf8, 0xcc, 0x55,
              0x67, 0xfe, 0xca, 0x53, 0xa2, 0x3b, 0x0f, 0x96, 0x72, 0xeb, 0xdf, 0x46,
              0xb7, 0x2e, 0x1a, 0x83, 0x4d, 0xd4, 0xe0, 0x79, 0x88, 0x11, 0x25, 0xbc,
              0x58, 0xc1, 0xf5, 0x6c, 0x9d, 0x04, 0x30, 0xa9, 0x33, 0xaa, 0x9e, 0x07,
              0xf6, 0x6f, 0x5b, 0xc2, 0x26, 0xbf, 0x8b, 0x12, 0xe3, 0x7a, 0x4e, 0xd7,
              0x19, 0x80, 0xb4, 0x2d, 0xdc, 0x45, 0x71, 0xe8, 0x0c, 0x95, 0xa1, 0x38,
              0xc9, 0x50, 0x64, 0xfd };

        private void ParseButton_Click(object sender, EventArgs e)
        {
 
            var codeplug = new Codeplug(@"c:\JediDumps\466AWA2867H01UCD6PW1BN-Codeplug.bin");
            var x = codeplug.Serialize();
            File.WriteAllText(@"c:\JediDumps\TestParse.txt", codeplug.GetText());
                //int i = 0;

                //while (i < 0x1500)
                //{
                //    int checkSumCalc = 0;
                //    bool extendedChecksumFlag = false;

                //    int size = bytes[i++];

                //    if (size == 0 || size == 1) //Likely need some kind of bit check
                //    {
                //        checkSumCalc = -0X5555 + size + bytes[i];
                //        size = (size) * 256 + bytes[i++];
                //        extendedChecksumFlag = true;
                //    }
                //    else
                //    {
                //        checkSumCalc = -0X55 + size;
                //    }

                //    if (size == 0) {
                //        fs.WriteLine("pad 0");
                //        continue;
                //    }

                //    int startOfBlock = i;

                //    int blockType = bytes[i++];
                //    if (blockType == 0x61) size = 0x221;  //s/b block 5b
                //    if (blockType == 0x3c) size = 0x7b;

                //    int endOfBlock = i + size -1;

                //    var block = new byte[size-1]; //Size includes block type
                //    checkSumCalc += blockType;

                //    for (int j=0; i < endOfBlock; i++, j++)
                //    {
                //        block[j] = bytes[i];
                //        checkSumCalc += bytes[i];
                //    }


                //    int checkSum = 0;
                //    if (extendedChecksumFlag)
                //    {
                //        checkSum = bytes[i++] * 256 + bytes[i++];
                //        checkSumCalc &= 0xffff;
                //        //Todo
                //    }
                //    else
                //    {
                //        checkSumCalc &= 0xff;
                //        checkSum = bytes[i++];
                //    }

                //    fs.WriteLine($"Block {blockType:X2}");
                //    fs.WriteLine($"# {size - 1} bytes starting at 0x{startOfBlock:X4} with checksum of 0x{checkSum:X2}");
                //    fs.WriteLine($"# \"{FormatHexPreview(block)}\"");
                //    if (checkSum != checkSumCalc) fs.WriteLine($"#Failed Checksum! Expected {checkSum:X2} Calculated {checkSumCalc:X2}");

                //    fs.WriteLine(FormatHex(block));
                //    fs.WriteLine("");
                //}

    

        }

        private string FormatHex(byte[] data)
        {
            var sb = new StringBuilder();
            var l = 1;
            foreach (var b in data)
            {
                sb.Append(b.ToString("X2"));
                if (l < data.Length) sb.Append(l++ % 16 == 0 ? "\r\n" : " ");
            }
            return sb.ToString();
        }

        private string FormatHexPreview(byte[] data)
        {
            var sb = new StringBuilder();
            foreach (var b in data)
            {
                if (b < 32 || b > 127)
                {
                    sb.Append(".");
                }
                else
                {
                    sb.Append((char)b);
                }
            }
            return sb.ToString();
        }

        private void WriteButton_Click(object sender, EventArgs e)
        {
           

        }

        private void ComPortComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            ComPort = this.ComPortComboBox.Text;
        }
    }
}
