using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using JediCodeplug;
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
            ComPort = this.NormalComPortComboBox.Text;
        }


        private void Home_Load(object sender, EventArgs e)
        {
            _codeplug = new Codeplug(@"MTS2000-2020-04-26_16-04-01.hex");
            UpdateCodeplug();
            Status.Text = _codeplug.GetTextDump();
            NormalComPortComboBox.DataSource = SerialPort.GetPortNames();
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
            propertyGrid1.SelectedObject = _codeplug.ExternalCodeplug;
            CodeplugView.Nodes[0].Nodes[0].Collapse();
        }

        private Codeplug _codeplug;

        private void CodeplugView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            propertyGrid1.SelectedObject = e.Node.Tag;
        }

        bool isBusy = false;
        private async void ReadButton_Click(object sender, EventArgs e)
        {
            if (isBusy) return;
            try
            {              
                isBusy = true;
                Status.Clear();
                _codeplug = null;

                var codeplug = await Codeplug.ReadFromRadio((String)NormalComPortComboBox.SelectedValue, new Progress<string>(s => Status.AppendText(s + Environment.NewLine)));

                if (codeplug != null)
                {
                    _codeplug = codeplug;
                    UpdateCodeplug();
                    Status.Text = _codeplug.GetTextDump();
                }
            }
            finally
            {
                isBusy = false;
            }
        }

        private async void WriteButton_Click(object sender, EventArgs e)
        {
            if (isBusy || _codeplug == null) return;
            try
            {
                isBusy = true;
                Status.Clear();
                var codeplug = await _codeplug.WriteToRadio((String)NormalComPortComboBox.SelectedValue, new Progress<string>(s => Status.AppendText(s + Environment.NewLine)));
                Status.AppendText("Done!");
            }
            finally
            {
                isBusy = false;
            }
  
            ////Round Trip Test
            //var sb = new StringBuilder();
            //var x = _codeplug.Serialize();
            //sb.AppendLine($"Original Size: {_codeplug.OriginalBytes.Length:X4} New Size: {x.Length:X4}");

            //for (var i = 0; i < Math.Min(x.Length, _codeplug.OriginalBytes.Length); i++)
            //{
            //    if (_codeplug.OriginalBytes[i] != x[i])
            //    {
            //        sb.AppendLine($"Mismatch {i:X4} Was {_codeplug.OriginalBytes[i]:X2} now {x[i]:X2}");
            //    }
            //}

            //Status.Text = sb.ToString();
        }

        Emulator _emulator = null;
        private void EmulatorButton_Click(object sender, EventArgs e)
        {
            if (_emulator == null)
            {
                isBusy = true;
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
                propertyGrid1.SelectedObject = _codeplug.InternalCodeplug.Block10;
                isBusy = false;
            }
        }

        private void _emulator_StatusUpdate(object sender, EmulatorStatusUpdateEventArgs e)
        {
            EmulatorStatus.BeginInvoke(new Action(() => EmulatorStatus.AppendText(e.Status + Environment.NewLine)));
        }

        private void button2_Click(object sender, EventArgs e)
        {

            //Fix ASK
            //SendSbep(new byte[] { 0xF5, 0x17, 0x00, 0x02, 0x82, 0x00 });
            //var xxx = ReceiveSbep();
            //SendSbep(new byte[] { 0xF5, 0x17, 0x00, 0x02, 0x89, 0x94 });
            //var xxdd = ReceiveSbep();



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
            //_codeplug.ExternalCodeplug.DynamicModeSelect = null;
            _codeplug.ExternalCodeplug.DynamicRadio = null; 


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

        private void NormalComPortComboBox_DropDown(object sender, EventArgs e)
        {
            NormalComPortComboBox.DataSource = SerialPort.GetPortNames();
        }
    }
}
