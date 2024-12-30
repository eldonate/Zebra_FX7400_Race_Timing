namespace Timing_Software_ver._3._0
{
    partial class timetrial
    {
        private System.ComponentModel.IContainer components = null;

        // Existing controls
        private System.Windows.Forms.Label labelConnectionStatus;
        private System.Windows.Forms.Button SelectFileButton;
        private System.Windows.Forms.Button StartMonitoringButton;
        private System.Windows.Forms.Button StopMonitoringButton;
        private System.Windows.Forms.Button IncrementLapButton;
        private System.Windows.Forms.Label labelCurrentLap;
        private System.Windows.Forms.TextBox textBoxLapNumber;
        private System.Windows.Forms.Label labelLapNumber;

        // NEW: Additional controls for remote upload
        private System.Windows.Forms.Button button3;         // "Upload to Remote" button
        private System.Windows.Forms.TextBox ReaderPosition; // Reader position TextBox
        private System.Windows.Forms.Label labelReaderPosition;
        private System.Windows.Forms.TextBox textBoxEventId; // Event ID TextBox
        private System.Windows.Forms.Label labelEventId;

        /// <summary>
        /// Dispose of any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                soundPlayer.Dispose(); // Dispose SoundPlayer too
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Initializes the form's components.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // ========== EXISTING CONTROLS ==========

            this.labelConnectionStatus = new System.Windows.Forms.Label();
            this.SelectFileButton = new System.Windows.Forms.Button();
            this.StartMonitoringButton = new System.Windows.Forms.Button();
            this.StopMonitoringButton = new System.Windows.Forms.Button();
            this.IncrementLapButton = new System.Windows.Forms.Button();
            this.labelCurrentLap = new System.Windows.Forms.Label();
            this.textBoxLapNumber = new System.Windows.Forms.TextBox();
            this.labelLapNumber = new System.Windows.Forms.Label();

            // ========== NEW CONTROLS ==========

            this.button3 = new System.Windows.Forms.Button();
            this.ReaderPosition = new System.Windows.Forms.TextBox();
            this.labelReaderPosition = new System.Windows.Forms.Label();
            this.textBoxEventId = new System.Windows.Forms.TextBox();
            this.labelEventId = new System.Windows.Forms.Label();

            // 
            // labelConnectionStatus
            // 
            this.labelConnectionStatus.AutoSize = true;
            this.labelConnectionStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.labelConnectionStatus.Location = new System.Drawing.Point(20, 20);
            this.labelConnectionStatus.Name = "labelConnectionStatus";
            this.labelConnectionStatus.Size = new System.Drawing.Size(142, 25);
            this.labelConnectionStatus.TabIndex = 0;
            this.labelConnectionStatus.Text = "Not Connected";
            // 
            // SelectFileButton
            // 
            this.SelectFileButton.Location = new System.Drawing.Point(20, 60);
            this.SelectFileButton.Name = "SelectFileButton";
            this.SelectFileButton.Size = new System.Drawing.Size(150, 50);
            this.SelectFileButton.TabIndex = 1;
            this.SelectFileButton.Text = "Select File";
            this.SelectFileButton.UseVisualStyleBackColor = true;
            this.SelectFileButton.Click += new System.EventHandler(this.SelectFileButton_Click);
            // 
            // StartMonitoringButton
            // 
            this.StartMonitoringButton.Location = new System.Drawing.Point(200, 60);
            this.StartMonitoringButton.Name = "StartMonitoringButton";
            this.StartMonitoringButton.Size = new System.Drawing.Size(150, 50);
            this.StartMonitoringButton.TabIndex = 2;
            this.StartMonitoringButton.Text = "Start Monitoring";
            this.StartMonitoringButton.UseVisualStyleBackColor = true;
            this.StartMonitoringButton.Click += new System.EventHandler(this.StartMonitoringButton_Click);
            // 
            // StopMonitoringButton
            // 
            this.StopMonitoringButton.Location = new System.Drawing.Point(380, 60);
            this.StopMonitoringButton.Name = "StopMonitoringButton";
            this.StopMonitoringButton.Size = new System.Drawing.Size(150, 50);
            this.StopMonitoringButton.TabIndex = 3;
            this.StopMonitoringButton.Text = "Stop Monitoring";
            this.StopMonitoringButton.UseVisualStyleBackColor = true;
            this.StopMonitoringButton.Click += new System.EventHandler(this.StopMonitoringButton_Click);
            // 
            // IncrementLapButton
            // 
            this.IncrementLapButton.Location = new System.Drawing.Point(20, 140);
            this.IncrementLapButton.Name = "IncrementLapButton";
            this.IncrementLapButton.Size = new System.Drawing.Size(150, 50);
            this.IncrementLapButton.TabIndex = 4;
            this.IncrementLapButton.Text = "Increment Lap";
            this.IncrementLapButton.UseVisualStyleBackColor = true;
            this.IncrementLapButton.Click += new System.EventHandler(this.IncrementLapButton_Click);
            // 
            // labelCurrentLap
            // 
            this.labelCurrentLap.AutoSize = true;
            this.labelCurrentLap.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.labelCurrentLap.Location = new System.Drawing.Point(200, 160);
            this.labelCurrentLap.Name = "labelCurrentLap";
            this.labelCurrentLap.Size = new System.Drawing.Size(149, 25);
            this.labelCurrentLap.TabIndex = 5;
            this.labelCurrentLap.Text = "Current Lap: 1";
            // 
            // textBoxLapNumber
            // 
            this.textBoxLapNumber.Location = new System.Drawing.Point(20, 220);
            this.textBoxLapNumber.Name = "textBoxLapNumber";
            this.textBoxLapNumber.Size = new System.Drawing.Size(150, 26);
            this.textBoxLapNumber.TabIndex = 6;
            this.textBoxLapNumber.TextChanged += new System.EventHandler(this.TextBoxLapNumber_TextChanged);
            // 
            // labelLapNumber
            // 
            this.labelLapNumber.AutoSize = true;
            this.labelLapNumber.Location = new System.Drawing.Point(200, 223);
            this.labelLapNumber.Name = "labelLapNumber";
            this.labelLapNumber.Size = new System.Drawing.Size(176, 20);
            this.labelLapNumber.TabIndex = 7;
            this.labelLapNumber.Text = "Set Lap Number (Manual)";
            // 
            // button3 (Upload to Remote)
            // 
            this.button3.Location = new System.Drawing.Point(380, 140);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(150, 50);
            this.button3.TabIndex = 8;
            this.button3.Text = "Upload to Remote";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // labelReaderPosition
            // 
            this.labelReaderPosition.AutoSize = true;
            this.labelReaderPosition.Location = new System.Drawing.Point(20, 270);
            this.labelReaderPosition.Name = "labelReaderPosition";
            this.labelReaderPosition.Size = new System.Drawing.Size(120, 20);
            this.labelReaderPosition.TabIndex = 9;
            this.labelReaderPosition.Text = "Reader Position:";
            // 
            // ReaderPosition
            // 
            this.ReaderPosition.Location = new System.Drawing.Point(150, 267);
            this.ReaderPosition.Name = "ReaderPosition";
            this.ReaderPosition.Size = new System.Drawing.Size(150, 26);
            this.ReaderPosition.TabIndex = 10;
            // 
            // labelEventId
            // 
            this.labelEventId.AutoSize = true;
            this.labelEventId.Location = new System.Drawing.Point(20, 320);
            this.labelEventId.Name = "labelEventId";
            this.labelEventId.Size = new System.Drawing.Size(63, 20);
            this.labelEventId.TabIndex = 11;
            this.labelEventId.Text = "Event ID";
            // 
            // textBoxEventId
            // 
            this.textBoxEventId.Location = new System.Drawing.Point(150, 317);
            this.textBoxEventId.Name = "textBoxEventId";
            this.textBoxEventId.Size = new System.Drawing.Size(150, 26);
            this.textBoxEventId.TabIndex = 12;
            // 
            // timetrial (Form settings)
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.ClientSize = new System.Drawing.Size(560, 370);
            this.Controls.Add(this.textBoxEventId);
            this.Controls.Add(this.labelEventId);
            this.Controls.Add(this.ReaderPosition);
            this.Controls.Add(this.labelReaderPosition);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.labelLapNumber);
            this.Controls.Add(this.textBoxLapNumber);
            this.Controls.Add(this.labelCurrentLap);
            this.Controls.Add(this.IncrementLapButton);
            this.Controls.Add(this.StopMonitoringButton);
            this.Controls.Add(this.StartMonitoringButton);
            this.Controls.Add(this.SelectFileButton);
            this.Controls.Add(this.labelConnectionStatus);
            this.Name = "timetrial";
            this.Text = "Timing Software ver. 3.0";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
