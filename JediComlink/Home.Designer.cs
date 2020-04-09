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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Home));
            this.ReadButton = new System.Windows.Forms.Button();
            this.WriteButton = new System.Windows.Forms.Button();
            this.Status = new System.Windows.Forms.TextBox();
            this.serialPort = new System.IO.Ports.SerialPort(this.components);
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.ParseButton = new System.Windows.Forms.Button();
            this.ComPortComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // ReadButton
            // 
            this.ReadButton.Location = new System.Drawing.Point(38, 26);
            this.ReadButton.Name = "ReadButton";
            this.ReadButton.Size = new System.Drawing.Size(78, 40);
            this.ReadButton.TabIndex = 0;
            this.ReadButton.Text = "Read";
            this.ReadButton.UseVisualStyleBackColor = true;
            this.ReadButton.Click += new System.EventHandler(this.ReadButton_Click);
            // 
            // WriteButton
            // 
            this.WriteButton.Location = new System.Drawing.Point(154, 26);
            this.WriteButton.Name = "WriteButton";
            this.WriteButton.Size = new System.Drawing.Size(78, 40);
            this.WriteButton.TabIndex = 1;
            this.WriteButton.Text = "Write";
            this.WriteButton.UseVisualStyleBackColor = true;
            this.WriteButton.Click += new System.EventHandler(this.WriteButton_Click);
            // 
            // Status
            // 
            this.Status.Location = new System.Drawing.Point(16, 94);
            this.Status.Multiline = true;
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            this.Status.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Status.Size = new System.Drawing.Size(1208, 663);
            this.Status.TabIndex = 2;
            // 
            // serialPort
            // 
            this.serialPort.PortName = "COM4";
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            // 
            // ParseButton
            // 
            this.ParseButton.Location = new System.Drawing.Point(277, 26);
            this.ParseButton.Name = "ParseButton";
            this.ParseButton.Size = new System.Drawing.Size(78, 40);
            this.ParseButton.TabIndex = 3;
            this.ParseButton.Text = "Parse";
            this.ParseButton.UseVisualStyleBackColor = true;
            this.ParseButton.Click += new System.EventHandler(this.ParseButton_Click);
            // 
            // ComPortComboBox
            // 
            this.ComPortComboBox.FormattingEnabled = true;
            this.ComPortComboBox.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9"});
            this.ComPortComboBox.Location = new System.Drawing.Point(932, 40);
            this.ComPortComboBox.Name = "ComPortComboBox";
            this.ComPortComboBox.Size = new System.Drawing.Size(206, 28);
            this.ComPortComboBox.TabIndex = 4;
            this.ComPortComboBox.Text = "COM1";
            this.ComPortComboBox.SelectedValueChanged += new System.EventHandler(this.ComPortComboBox_SelectedValueChanged);
            // 
            // Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1236, 769);
            this.Controls.Add(this.ComPortComboBox);
            this.Controls.Add(this.ParseButton);
            this.Controls.Add(this.Status);
            this.Controls.Add(this.WriteButton);
            this.Controls.Add(this.ReadButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Home";
            this.Text = "Jedi Comlink";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ReadButton;
        private System.Windows.Forms.Button WriteButton;
        private System.Windows.Forms.TextBox Status;
        private System.IO.Ports.SerialPort serialPort;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.Button ParseButton;
        private System.Windows.Forms.ComboBox ComPortComboBox;
    }
}

