namespace JediComlink
{
    partial class Home
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Label label1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Home));
            this.NormalReadButton = new System.Windows.Forms.Button();
            this.NormalWriteButton = new System.Windows.Forms.Button();
            this.NormalStatus = new System.Windows.Forms.TextBox();
            this.NormalComPortComboBox = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.CodeplugView = new System.Windows.Forms.TreeView();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.FlashStatus = new System.Windows.Forms.TextBox();
            this.FlashComPortComboBox = new System.Windows.Forms.ComboBox();
            this.FlashReadButton = new System.Windows.Forms.Button();
            this.FlashWriteButton = new System.Windows.Forms.Button();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.EmulatorComPortComboBox = new System.Windows.Forms.ComboBox();
            this.EmulatorButton = new System.Windows.Forms.Button();
            this.EmulatorStatus = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Tahoma", 21.75F, ((System.Drawing.FontStyle)(((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic) 
                | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label1.ForeColor = System.Drawing.Color.Crimson;
            label1.Location = new System.Drawing.Point(280, 14);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(209, 35);
            label1.TabIndex = 1;
            label1.Text = "Whore It Out";
            // 
            // NormalReadButton
            // 
            this.NormalReadButton.Location = new System.Drawing.Point(7, 5);
            this.NormalReadButton.Margin = new System.Windows.Forms.Padding(2);
            this.NormalReadButton.Name = "NormalReadButton";
            this.NormalReadButton.Size = new System.Drawing.Size(52, 26);
            this.NormalReadButton.TabIndex = 0;
            this.NormalReadButton.Text = "Read";
            this.NormalReadButton.UseVisualStyleBackColor = true;
            this.NormalReadButton.Click += new System.EventHandler(this.NormalReadButton_Click);
            // 
            // NormalWriteButton
            // 
            this.NormalWriteButton.Location = new System.Drawing.Point(76, 5);
            this.NormalWriteButton.Margin = new System.Windows.Forms.Padding(2);
            this.NormalWriteButton.Name = "NormalWriteButton";
            this.NormalWriteButton.Size = new System.Drawing.Size(52, 26);
            this.NormalWriteButton.TabIndex = 1;
            this.NormalWriteButton.Text = "Write";
            this.NormalWriteButton.UseVisualStyleBackColor = true;
            this.NormalWriteButton.Click += new System.EventHandler(this.NormalWriteButton_Click);
            // 
            // NormalStatus
            // 
            this.NormalStatus.Location = new System.Drawing.Point(7, 65);
            this.NormalStatus.Margin = new System.Windows.Forms.Padding(2);
            this.NormalStatus.Multiline = true;
            this.NormalStatus.Name = "NormalStatus";
            this.NormalStatus.ReadOnly = true;
            this.NormalStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.NormalStatus.Size = new System.Drawing.Size(807, 407);
            this.NormalStatus.TabIndex = 2;
            // 
            // NormalComPortComboBox
            // 
            this.NormalComPortComboBox.FormattingEnabled = true;
            this.NormalComPortComboBox.Location = new System.Drawing.Point(705, 5);
            this.NormalComPortComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.NormalComPortComboBox.Name = "NormalComPortComboBox";
            this.NormalComPortComboBox.Size = new System.Drawing.Size(98, 21);
            this.NormalComPortComboBox.TabIndex = 4;
            this.NormalComPortComboBox.DropDown += new System.EventHandler(this.ComPortComboBox_DropDown);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(824, 489);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.NormalStatus);
            this.tabPage1.Controls.Add(this.NormalComPortComboBox);
            this.tabPage1.Controls.Add(this.NormalReadButton);
            this.tabPage1.Controls.Add(this.NormalWriteButton);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(816, 463);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Normal Read/Wrtie";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.splitContainer1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(816, 463);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Codeplug Explorer";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.CodeplugView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer1.Size = new System.Drawing.Size(810, 457);
            this.splitContainer1.SplitterDistance = 232;
            this.splitContainer1.TabIndex = 0;
            // 
            // CodeplugView
            // 
            this.CodeplugView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CodeplugView.Location = new System.Drawing.Point(0, 0);
            this.CodeplugView.Name = "CodeplugView";
            this.CodeplugView.Size = new System.Drawing.Size(232, 457);
            this.CodeplugView.TabIndex = 0;
            this.CodeplugView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.CodeplugView_AfterSelect);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGrid1.Size = new System.Drawing.Size(574, 457);
            this.propertyGrid1.TabIndex = 0;
            this.propertyGrid1.ToolbarVisible = false;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.panel1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(816, 463);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Analysis / Fixes";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pictureBox7);
            this.panel1.Controls.Add(this.pictureBox6);
            this.panel1.Controls.Add(this.pictureBox5);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(label1);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 156);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(816, 307);
            this.panel1.TabIndex = 2;
            // 
            // pictureBox7
            // 
            this.pictureBox7.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox7.Image")));
            this.pictureBox7.Location = new System.Drawing.Point(267, 171);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(39, 37);
            this.pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox7.TabIndex = 10;
            this.pictureBox7.TabStop = false;
            // 
            // pictureBox6
            // 
            this.pictureBox6.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox6.Image")));
            this.pictureBox6.Location = new System.Drawing.Point(267, 119);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(39, 37);
            this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox6.TabIndex = 9;
            this.pictureBox6.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox5.Image")));
            this.pictureBox5.Location = new System.Drawing.Point(267, 66);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(39, 37);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox5.TabIndex = 8;
            this.pictureBox5.TabStop = false;
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.Color.Crimson;
            this.button3.Location = new System.Drawing.Point(323, 171);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(178, 37);
            this.button3.TabIndex = 4;
            this.button3.Text = "Add MDC";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.Crimson;
            this.button2.Location = new System.Drawing.Point(323, 119);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(178, 37);
            this.button2.TabIndex = 3;
            this.button2.Text = "Add Quik-Call";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.Crimson;
            this.button1.Location = new System.Drawing.Point(323, 66);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(178, 37);
            this.button1.TabIndex = 2;
            this.button1.Text = "Add Trunking";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(-5, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(256, 308);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.FlashStatus);
            this.tabPage4.Controls.Add(this.FlashComPortComboBox);
            this.tabPage4.Controls.Add(this.FlashReadButton);
            this.tabPage4.Controls.Add(this.FlashWriteButton);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(816, 463);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Flash Read/Write";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // FlashStatus
            // 
            this.FlashStatus.Location = new System.Drawing.Point(7, 52);
            this.FlashStatus.Margin = new System.Windows.Forms.Padding(2);
            this.FlashStatus.Multiline = true;
            this.FlashStatus.Name = "FlashStatus";
            this.FlashStatus.ReadOnly = true;
            this.FlashStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.FlashStatus.Size = new System.Drawing.Size(802, 424);
            this.FlashStatus.TabIndex = 7;
            // 
            // FlashComPortComboBox
            // 
            this.FlashComPortComboBox.FormattingEnabled = true;
            this.FlashComPortComboBox.Location = new System.Drawing.Point(705, 9);
            this.FlashComPortComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.FlashComPortComboBox.Name = "FlashComPortComboBox";
            this.FlashComPortComboBox.Size = new System.Drawing.Size(98, 21);
            this.FlashComPortComboBox.TabIndex = 8;
            this.FlashComPortComboBox.DropDown += new System.EventHandler(this.ComPortComboBox_DropDown);
            // 
            // FlashReadButton
            // 
            this.FlashReadButton.Location = new System.Drawing.Point(7, 9);
            this.FlashReadButton.Margin = new System.Windows.Forms.Padding(2);
            this.FlashReadButton.Name = "FlashReadButton";
            this.FlashReadButton.Size = new System.Drawing.Size(52, 26);
            this.FlashReadButton.TabIndex = 5;
            this.FlashReadButton.Text = "Read";
            this.FlashReadButton.UseVisualStyleBackColor = true;
            this.FlashReadButton.Click += new System.EventHandler(this.FlashReadButton_Click);
            // 
            // FlashWriteButton
            // 
            this.FlashWriteButton.Location = new System.Drawing.Point(76, 9);
            this.FlashWriteButton.Margin = new System.Windows.Forms.Padding(2);
            this.FlashWriteButton.Name = "FlashWriteButton";
            this.FlashWriteButton.Size = new System.Drawing.Size(52, 26);
            this.FlashWriteButton.TabIndex = 6;
            this.FlashWriteButton.Text = "Write";
            this.FlashWriteButton.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.EmulatorComPortComboBox);
            this.tabPage5.Controls.Add(this.EmulatorButton);
            this.tabPage5.Controls.Add(this.EmulatorStatus);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(816, 463);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Emulator";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // EmulatorComPortComboBox
            // 
            this.EmulatorComPortComboBox.FormattingEnabled = true;
            this.EmulatorComPortComboBox.Location = new System.Drawing.Point(684, 2);
            this.EmulatorComPortComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.EmulatorComPortComboBox.Name = "EmulatorComPortComboBox";
            this.EmulatorComPortComboBox.Size = new System.Drawing.Size(98, 21);
            this.EmulatorComPortComboBox.TabIndex = 9;
            this.EmulatorComPortComboBox.DropDown += new System.EventHandler(this.ComPortComboBox_DropDown);
            // 
            // EmulatorButton
            // 
            this.EmulatorButton.Location = new System.Drawing.Point(38, 2);
            this.EmulatorButton.Margin = new System.Windows.Forms.Padding(2);
            this.EmulatorButton.Name = "EmulatorButton";
            this.EmulatorButton.Size = new System.Drawing.Size(52, 26);
            this.EmulatorButton.TabIndex = 4;
            this.EmulatorButton.Text = "Start";
            this.EmulatorButton.UseVisualStyleBackColor = true;
            this.EmulatorButton.Click += new System.EventHandler(this.EmulatorButton_Click);
            // 
            // EmulatorStatus
            // 
            this.EmulatorStatus.Location = new System.Drawing.Point(5, 28);
            this.EmulatorStatus.Margin = new System.Windows.Forms.Padding(2);
            this.EmulatorStatus.Multiline = true;
            this.EmulatorStatus.Name = "EmulatorStatus";
            this.EmulatorStatus.ReadOnly = true;
            this.EmulatorStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.EmulatorStatus.Size = new System.Drawing.Size(807, 407);
            this.EmulatorStatus.TabIndex = 3;
            // 
            // Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 489);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Home";
            this.Text = "Jedi Comlink";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button NormalReadButton;
        private System.Windows.Forms.Button NormalWriteButton;
        private System.Windows.Forms.TextBox NormalStatus;
        private System.Windows.Forms.ComboBox NormalComPortComboBox;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView CodeplugView;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Button EmulatorButton;
        private System.Windows.Forms.TextBox EmulatorStatus;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox7;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox FlashStatus;
        private System.Windows.Forms.ComboBox FlashComPortComboBox;
        private System.Windows.Forms.Button FlashReadButton;
        private System.Windows.Forms.Button FlashWriteButton;
        private System.Windows.Forms.ComboBox EmulatorComPortComboBox;
    }
}

