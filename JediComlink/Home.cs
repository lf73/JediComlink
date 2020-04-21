using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
            //_codeplug = new Codeplug(@"MTS2000-2020-04-17_19-15-43.hex");
            //UpdateCodeplug();
            //Status.Text = _codeplug.GetTextDump();

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
                _emulator = new Emulator(File.ReadAllBytes(@"MTS2000-2020-04-17_19-15-43.hex"));
                _emulator.StatusUpdate += _emulator_StatusUpdate;

                _emulator.Start();
                EmulatorButton.Text = "Stop";
            }
            else
            {
                _emulator.Stop();
                _emulator = null;
                EmulatorButton.Text = "Start";
            }

        }

        private void _emulator_StatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            EmulatorStatus.BeginInvoke(new Action(() => EmulatorStatus.AppendText(e.Status + Environment.NewLine)));
        }
    }
}
