using System;
using System.Collections.Generic;
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
using JediFlash;

namespace JediComlink
{
    public partial class Home : Form
    {
        private Codeplug _codeplug;
        private bool _isBusy = false;
        private Emulator _emulator = null;

        public Home()
        {
            InitializeComponent();
            var portNames = SerialPort.GetPortNames();
            NormalComPortComboBox.DataSource = portNames;
            FlashComPortComboBox.DataSource = portNames;
            EmulatorComPortComboBox.DataSource = portNames;
        }

        #region Normal Read / Write
        private async void NormalReadButton_Click(object sender, EventArgs e)
        {
            if (_isBusy) return;
            try
            {
                _isBusy = true;
                NormalStatus.Clear();
                _codeplug = null;

                var codeplug = await Codeplug.ReadFromRadio((String)NormalComPortComboBox.SelectedValue, new Progress<string>(s => NormalStatus.AppendText(s + Environment.NewLine)));

                if (codeplug != null)
                {
                    _codeplug = codeplug;
                    UpdateCodeplug();
                    NormalStatus.Text = _codeplug.GetTextDump();
                }
            }
            finally
            {
                _isBusy = false;
            }
        }

        private async void NormalWriteButton_Click(object sender, EventArgs e)
        {
            if (_isBusy || _codeplug == null) return;
            try
            {
                _isBusy = true;
                NormalStatus.Clear();
                var codeplug = await _codeplug.WriteToRadio((String)NormalComPortComboBox.SelectedValue, new Progress<string>(s => NormalStatus.AppendText(s + Environment.NewLine)));
                NormalStatus.AppendText("Done!");
            }
            finally
            {
                _isBusy = false;
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

        private void NormalOpenButton_Click(object sender, EventArgs e)
        {
            if (_isBusy) return;

            using OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = "bin files (*.bin)|*.bin|All files (*.*)|*.*",
                InitialDirectory = Application.StartupPath,
                Title = "Open Codeplug File"
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    NormalStatus.Clear();
                    var codeplug = new Codeplug(fileDialog.FileName);

                    if (codeplug != null)
                    {
                        _codeplug = codeplug;
                        UpdateCodeplug();
                        NormalStatus.Text = _codeplug.GetTextDump();
                    }
                }
                catch (Exception ex)
                {
                    NormalStatus.Text = $"Error opening file {fileDialog.FileName}\r\n{ex}";
                }
            }
        }

        private void NormalSaveButton_Click(object sender, EventArgs e)
        {
            if (_isBusy || _codeplug == null) return;

            using var fileDialog = new SaveFileDialog
            {
                Filter = "bin files (*.bin)|*.bin|All files (*.*)|*.*",
                InitialDirectory = Application.StartupPath,
                Title = "Save Codeplug File",
                CheckFileExists = true,
                DefaultExt = "bin",
                AddExtension = false,
                FileName = _codeplug.GetProposedFileName(),
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllBytes(fileDialog.FileName, _codeplug.Serialize());
                }
                catch (Exception ex)
                {
                    NormalStatus.Text = $"Error saving file {fileDialog.FileName}\r\n{ex}";
                }
            }
        }
        #endregion

        #region Codeplug Explorer
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
            NormalStatus.Text = _codeplug.InternalCodeplug.ToString() + "\n\n" + _codeplug.ExternalCodeplug.ToString();
            propertyGrid1.SelectedObject = _codeplug.ExternalCodeplug;
            CodeplugView.Nodes[0].Nodes[0].Collapse();
        }

        private void CodeplugView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            propertyGrid1.SelectedObject = e.Node.Tag;
        }
        #endregion

        #region Analysis / Fixes
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
        #endregion

        #region Flash Read / Write
        private async void FlashReadButton_Click(object sender, EventArgs e)
        {
            if (_isBusy) return;
            try
            {
                _isBusy = true;
                FlashStatus.Clear();
                _codeplug = null;

                var flash = new Flash();

                await flash.ReadFromRadio((String)FlashComPortComboBox.SelectedValue, new Progress<string>(s => FlashStatus.AppendText(s + Environment.NewLine)));

                //if (codeplug != null)
                //{
                //    _codeplug = codeplug;
                //    UpdateCodeplug();
                //    NormalStatus.Text = _codeplug.GetTextDump();
                //}
            }
            finally
            {
                _isBusy = false;
            }
        }
        #endregion

        #region Emulator
        private void EmulatorButton_Click(object sender, EventArgs e)
        {
            if (_emulator == null)
            {
                _isBusy = true;
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
                _isBusy = false;
            }
        }

        private void _emulator_StatusUpdate(object sender, EmulatorStatusUpdateEventArgs e)
        {
            EmulatorStatus.BeginInvoke(new Action(() => EmulatorStatus.AppendText(e.Status + Environment.NewLine)));
        }
        #endregion

        private void ComPortComboBox_DropDown(object sender, EventArgs e)
        {
            NormalComPortComboBox.DataSource = SerialPort.GetPortNames();
        }

    }
}
