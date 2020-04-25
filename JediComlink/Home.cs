using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using JediCodeplug;
using JediCommunication;
using JediEmulator;

namespace JediComlink
{
    public partial class Home : Form
    {
        private static string ComPort = "COM1";
        public Home()
        {
            InitializeComponent();
        }

        private void ComPortComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            ComPort = this.ComPortComboBox.Text;
        }


        private void Home_Load(object sender, EventArgs e)
        {
            _codeplug = new Codeplug(@"MTS2000-2020-04-24_17-43-27.hex");
            UpdateCodeplug();
            Status.Text = _codeplug.GetTextDump();

        }

        private void UpdateCodeplug()
        {
            //Declare local funtion for recursion and list extraction
            static void PopulateNode(TreeNode node, Object obj)
            {
                if (obj is Block block)
                {
                    node = node.Nodes.Add(block.Description);
                    node.Tag = block;
                }
                foreach (var property in obj.GetType().GetProperties())
                {
                    if (property.GetValue(obj) is Block val)
                        PopulateNode(node, val);
                    else if (property.GetValue(obj) is IEnumerable<Block> blocks)
                        foreach (var blockItem in blocks)
                            PopulateNode(node, blockItem);
                }
            }

            CodeplugView.BeginUpdate();
            CodeplugView.Nodes.Clear();
            var root = CodeplugView.Nodes.Add("Codeplug");
            root.Tag = _codeplug;
            PopulateNode(root, _codeplug);
            CodeplugView.ExpandAll();
            CodeplugView.EndUpdate();
            Status.Text = _codeplug.InternalCodeplug.ToString() + "\n\n" + _codeplug.ExternalCodeplug.ToString();
            propertyGrid1.SelectedObject = _codeplug;
        }

        private Codeplug _codeplug;

        private void CodeplugView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            propertyGrid1.SelectedObject = e.Node.Tag;
        }

        Com _com = null;
        private async void ReadButton_Click(object sender, EventArgs e)
        {
            if (_com != null) return;
            Status.Clear();

            _com = new Com("COM1");
            _com.StatusUpdate += _com_StatusUpdate;

            var codeplug = await Codeplug.ReadFromRadio(_com);
            _com.Reset();
            _com.StatusUpdate -= _com_StatusUpdate;
            _com.Close();
            _com = null;

            if (codeplug != null)
            {
                _codeplug = codeplug;
                UpdateCodeplug();
                Status.Text = _codeplug.GetTextDump();
            }
        }


        private void _com_StatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            Status.BeginInvoke(new Action(() => Status.AppendText(e.Status + Environment.NewLine)));
        }

        Emulator _emulator = null;
        private void EmulatorButton_Click(object sender, EventArgs e)
        {
            if (_emulator == null)
            {
                _emulator = new Emulator(_codeplug.Serialize());
                _emulator.StatusUpdate += _emulator_StatusUpdate;
                _emulator.Start();

                File.WriteAllBytes("Start-Emulator.hex", _emulator.CodePlugBytes);

                EmulatorButton.Text = "Stop";
            }
            else
            {
                _emulator.Stop();
                File.WriteAllBytes("Stop-Emulator.hex", _emulator.CodePlugBytes);
                _codeplug = new Codeplug(_emulator.CodePlugBytes);
                _emulator = null;
                EmulatorButton.Text = "Start";
                UpdateCodeplug();
                propertyGrid1.SelectedObject = _codeplug.ExternalCodeplug;
            }

        }

        private void _emulator_StatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            EmulatorStatus.BeginInvoke(new Action(() => EmulatorStatus.AppendText(e.Status + Environment.NewLine)));
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void WriteButton_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            var x = _codeplug.Serialize();
            sb.AppendLine($"Original Size: {_codeplug.OriginalBytes.Length:X4} New Size: {x.Length:X4}");

            for (var i = 0; i < Math.Min(x.Length, _codeplug.OriginalBytes.Length); i++)
            {
                if (_codeplug.OriginalBytes[i] != x[i])
                {
                    sb.AppendLine ($"Mismatch {i:X4} Was {_codeplug.OriginalBytes[i]:X2} now {x[i]:X2}");
                }
            }

            Status.Text = sb.ToString();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            _codeplug.ExternalCodeplug.Block55 = null;//Block56or62List.Clear();
            _codeplug.ExternalCodeplug.Block3D = null;// .Block3E = null;
            _codeplug.ExternalCodeplug.Block73 = null;
            _codeplug.ExternalCodeplug.Block3B = null;
            _codeplug.ExternalCodeplug.Block34 = null;
            _codeplug.ExternalCodeplug.Block35 = null;
            _codeplug.ExternalCodeplug.Block3C = null;
            _codeplug.ExternalCodeplug.Block39 = null;
            _codeplug.ExternalCodeplug.Block51 = null;
            _codeplug.ExternalCodeplug.Block36 = null;
            _codeplug.ExternalCodeplug.Block54 = null;


            //_codeplug.ExternalCodeplug.Block3D.BlockA0.BlockA1 =
            //    new BlockA1()
            //    {
            //        BlockA2List = new List<BlockA2>()
            //        {
            //            new BlockA2()
            //            {
            //                Contents = new byte[] {0x0A, 0x28, 0x03}
            //                //Contents = new byte[] {0x00, 0x00, 0x00}
            //            }
            //        }
            //    };

            //_codeplug.ExternalCodeplug.Block3D.BlockA0.BlockA3 =
            //    new BlockA3()
            //    {
            //        Contents = new byte[] { 0x01, 0x27, 0x10 }
            //        //Contents = new byte[] { 0x00, 0x00, 0x00 }
            //    };


            string fileName = "MTS2000-WhoreTest-" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".hex";

            var bytes = _codeplug.Serialize();
            File.WriteAllBytes(fileName, bytes);
            _codeplug = new Codeplug(_codeplug.Serialize());

            UpdateCodeplug();


        }
    }
}
