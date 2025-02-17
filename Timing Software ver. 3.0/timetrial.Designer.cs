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

        // Controls for remote upload
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox ReaderPosition;
        private System.Windows.Forms.Label labelReaderPosition;
        private System.Windows.Forms.TextBox textBoxEventId;
        private System.Windows.Forms.Label labelEventId;

        // Controls for manual bib submission
        private System.Windows.Forms.Label labelBibNumber;
        private System.Windows.Forms.TextBox textBoxBibNumber;
        private System.Windows.Forms.Label labelManualTime;
        private System.Windows.Forms.TextBox textBoxManualTime;
        private System.Windows.Forms.Button buttonSubmitLap;

        // Controls for started/finished counters
        private System.Windows.Forms.Label labelStarted;
        private System.Windows.Forms.TextBox textBoxStartedCount;
        private System.Windows.Forms.Label labelFinished;
        private System.Windows.Forms.TextBox textBoxFinishedCount;

        // Controls for automatic lap increment
        private System.Windows.Forms.CheckBox checkBoxAutoLap;
        private System.Windows.Forms.Label labelAutoLap;
        private System.Windows.Forms.TextBox textBoxAutoLapDuration;

        // NEW: Countdown display label.
        private System.Windows.Forms.Label labelCountdown;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // Existing controls
            this.labelConnectionStatus = new System.Windows.Forms.Label();
            this.SelectFileButton = new System.Windows.Forms.Button();
            this.StartMonitoringButton = new System.Windows.Forms.Button();
            this.StopMonitoringButton = new System.Windows.Forms.Button();
            this.IncrementLapButton = new System.Windows.Forms.Button();
            this.labelCurrentLap = new System.Windows.Forms.Label();
            this.textBoxLapNumber = new System.Windows.Forms.TextBox();
            this.labelLapNumber = new System.Windows.Forms.Label();

            // Controls for remote upload
            this.button3 = new System.Windows.Forms.Button();
            this.ReaderPosition = new System.Windows.Forms.TextBox();
            this.labelReaderPosition = new System.Windows.Forms.Label();
            this.textBoxEventId = new System.Windows.Forms.TextBox();
            this.labelEventId = new System.Windows.Forms.Label();

            // Controls for manual bib submission
            this.labelBibNumber = new System.Windows.Forms.Label();
            this.textBoxBibNumber = new System.Windows.Forms.TextBox();
            this.labelManualTime = new System.Windows.Forms.Label();
            this.textBoxManualTime = new System.Windows.Forms.TextBox();
            this.buttonSubmitLap = new System.Windows.Forms.Button();

            // Controls for started/finished counters
            this.labelStarted = new System.Windows.Forms.Label();
            this.textBoxStartedCount = new System.Windows.Forms.TextBox();
            this.labelFinished = new System.Windows.Forms.Label();
            this.textBoxFinishedCount = new System.Windows.Forms.TextBox();

            // Controls for automatic lap increment
            this.checkBoxAutoLap = new System.Windows.Forms.CheckBox();
            this.labelAutoLap = new System.Windows.Forms.Label();
            this.textBoxAutoLapDuration = new System.Windows.Forms.TextBox();

            // NEW: Countdown label.
            this.labelCountdown = new System.Windows.Forms.Label();

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
            // labelBibNumber
            // 
            this.labelBibNumber.AutoSize = true;
            this.labelBibNumber.Location = new System.Drawing.Point(20, 370);
            this.labelBibNumber.Name = "labelBibNumber";
            this.labelBibNumber.Size = new System.Drawing.Size(87, 20);
            this.labelBibNumber.TabIndex = 13;
            this.labelBibNumber.Text = "Bib Number";
            // 
            // textBoxBibNumber
            // 
            this.textBoxBibNumber.Location = new System.Drawing.Point(150, 367);
            this.textBoxBibNumber.Name = "textBoxBibNumber";
            this.textBoxBibNumber.Size = new System.Drawing.Size(150, 26);
            this.textBoxBibNumber.TabIndex = 14;
            // 
            // labelManualTime
            // 
            this.labelManualTime.AutoSize = true;
            this.labelManualTime.Location = new System.Drawing.Point(20, 410);
            this.labelManualTime.Name = "labelManualTime";
            this.labelManualTime.Size = new System.Drawing.Size(101, 20);
            this.labelManualTime.TabIndex = 15;
            this.labelManualTime.Text = "Manual Time";
            // 
            // textBoxManualTime
            // 
            this.textBoxManualTime.Location = new System.Drawing.Point(150, 407);
            this.textBoxManualTime.Name = "textBoxManualTime";
            this.textBoxManualTime.Size = new System.Drawing.Size(150, 26);
            this.textBoxManualTime.TabIndex = 16;
            // 
            // buttonSubmitLap
            // 
            this.buttonSubmitLap.Location = new System.Drawing.Point(320, 380);
            this.buttonSubmitLap.Name = "buttonSubmitLap";
            this.buttonSubmitLap.Size = new System.Drawing.Size(150, 40);
            this.buttonSubmitLap.TabIndex = 17;
            this.buttonSubmitLap.Text = "Submit Lap";
            this.buttonSubmitLap.UseVisualStyleBackColor = true;
            this.buttonSubmitLap.Click += new System.EventHandler(this.buttonSubmitLap_Click);
            // 
            // labelStarted
            // 
            this.labelStarted.AutoSize = true;
            this.labelStarted.Location = new System.Drawing.Point(20, 450);
            this.labelStarted.Name = "labelStarted";
            this.labelStarted.Size = new System.Drawing.Size(148, 20);
            this.labelStarted.TabIndex = 18;
            this.labelStarted.Text = "Runners Started Lap";
            // 
            // textBoxStartedCount
            // 
            this.textBoxStartedCount.Location = new System.Drawing.Point(200, 447);
            this.textBoxStartedCount.Name = "textBoxStartedCount";
            this.textBoxStartedCount.Size = new System.Drawing.Size(60, 26);
            this.textBoxStartedCount.TabIndex = 19;
            this.textBoxStartedCount.ReadOnly = true;
            this.textBoxStartedCount.BackColor = System.Drawing.Color.LightYellow;
            // 
            // labelFinished
            // 
            this.labelFinished.AutoSize = true;
            this.labelFinished.Location = new System.Drawing.Point(20, 490);
            this.labelFinished.Name = "labelFinished";
            this.labelFinished.Size = new System.Drawing.Size(153, 20);
            this.labelFinished.TabIndex = 20;
            this.labelFinished.Text = "Runners Finished Lap";
            // 
            // textBoxFinishedCount
            // 
            this.textBoxFinishedCount.Location = new System.Drawing.Point(200, 487);
            this.textBoxFinishedCount.Name = "textBoxFinishedCount";
            this.textBoxFinishedCount.Size = new System.Drawing.Size(60, 26);
            this.textBoxFinishedCount.TabIndex = 21;
            this.textBoxFinishedCount.ReadOnly = true;
            this.textBoxFinishedCount.BackColor = System.Drawing.Color.LightYellow;
            // 
            // checkBoxAutoLap
            // 
            this.checkBoxAutoLap.AutoSize = true;
            this.checkBoxAutoLap.Location = new System.Drawing.Point(20, 530);
            this.checkBoxAutoLap.Name = "checkBoxAutoLap";
            this.checkBoxAutoLap.Size = new System.Drawing.Size(20, 19);
            this.checkBoxAutoLap.TabIndex = 22;
            this.checkBoxAutoLap.UseVisualStyleBackColor = true;
            this.checkBoxAutoLap.CheckedChanged += new System.EventHandler(this.checkBoxAutoLap_CheckedChanged);
            // 
            // labelAutoLap
            // 
            this.labelAutoLap.AutoSize = true;
            this.labelAutoLap.Location = new System.Drawing.Point(50, 530);
            this.labelAutoLap.Name = "labelAutoLap";
            this.labelAutoLap.Size = new System.Drawing.Size(165, 20);
            this.labelAutoLap.TabIndex = 23;
            this.labelAutoLap.Text = "Enable Auto Lap Increment";
            // 
            // textBoxAutoLapDuration
            // 
            this.textBoxAutoLapDuration.Location = new System.Drawing.Point(230, 527);
            this.textBoxAutoLapDuration.Name = "textBoxAutoLapDuration";
            this.textBoxAutoLapDuration.Size = new System.Drawing.Size(60, 26);
            this.textBoxAutoLapDuration.TabIndex = 24;
            this.textBoxAutoLapDuration.Text = "60";
            // 
            // labelCountdown (NEW)
            // 
            this.labelCountdown.AutoSize = true;
            this.labelCountdown.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.labelCountdown.Location = new System.Drawing.Point(320, 530);
            this.labelCountdown.Name = "labelCountdown";
            this.labelCountdown.Size = new System.Drawing.Size(190, 25);
            this.labelCountdown.TabIndex = 25;
            this.labelCountdown.Text = "Countdown: 00:00";
            // 
            // timetrial (Form settings)
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.ClientSize = new System.Drawing.Size(560, 600);
            this.Controls.Add(this.labelCountdown);
            this.Controls.Add(this.textBoxAutoLapDuration);
            this.Controls.Add(this.labelAutoLap);
            this.Controls.Add(this.checkBoxAutoLap);
            this.Controls.Add(this.labelFinished);
            this.Controls.Add(this.textBoxFinishedCount);
            this.Controls.Add(this.labelStarted);
            this.Controls.Add(this.textBoxStartedCount);
            this.Controls.Add(this.buttonSubmitLap);
            this.Controls.Add(this.textBoxManualTime);
            this.Controls.Add(this.labelManualTime);
            this.Controls.Add(this.textBoxBibNumber);
            this.Controls.Add(this.labelBibNumber);
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
