namespace Timing_Software_ver._3._0
{
    partial class timetrial
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label labelConnectionStatus;
        private System.Windows.Forms.Button SelectFileButton;
        private System.Windows.Forms.Button StartMonitoringButton;
        private System.Windows.Forms.Button StopMonitoringButton;
        private System.Windows.Forms.Button IncrementLapButton;
        private System.Windows.Forms.Label labelCurrentLap;
        private System.Windows.Forms.TextBox textBoxLapNumber;
        private System.Windows.Forms.Label labelLapNumber;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                soundPlayer.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Initializes the form's components.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelConnectionStatus = new System.Windows.Forms.Label();
            this.SelectFileButton = new System.Windows.Forms.Button();
            this.StartMonitoringButton = new System.Windows.Forms.Button();
            this.StopMonitoringButton = new System.Windows.Forms.Button();
            this.IncrementLapButton = new System.Windows.Forms.Button();
            this.labelCurrentLap = new System.Windows.Forms.Label();
            this.textBoxLapNumber = new System.Windows.Forms.TextBox();
            this.labelLapNumber = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelConnectionStatus
            // 
            this.labelConnectionStatus.AutoSize = true;
            this.labelConnectionStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.labelCurrentLap.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            // timetrial Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.ClientSize = new System.Drawing.Size(560, 270);
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
