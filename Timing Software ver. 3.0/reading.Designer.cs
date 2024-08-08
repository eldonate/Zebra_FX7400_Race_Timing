namespace RaceManager
{
    partial class reading
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.cmbRaces = new System.Windows.Forms.ComboBox();
            this.cmbDistances = new System.Windows.Forms.ComboBox();
            this.lblElapsedTime = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnSetStartTime = new System.Windows.Forms.Button();
            this.btnSetCurrentTimestamp = new System.Windows.Forms.Button();
            this.dtpStartTime = new System.Windows.Forms.DateTimePicker();
            this.chkLogSpecificDistance = new System.Windows.Forms.CheckBox();
            this.comboBoxEvents = new System.Windows.Forms.ComboBox();
            this.btnTransferRecords = new System.Windows.Forms.Button();
            this.textBoxEventId = new System.Windows.Forms.TextBox();
            this.chkAddLap = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 150);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(982, 300);
            this.dataGridView1.TabIndex = 0;
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(12, 13);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(100, 23);
            this.btnSelectFile.TabIndex = 1;
            this.btnSelectFile.Text = "Select File";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // cmbRaces
            // 
            this.cmbRaces.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRaces.FormattingEnabled = true;
            this.cmbRaces.Location = new System.Drawing.Point(130, 12);
            this.cmbRaces.Name = "cmbRaces";
            this.cmbRaces.Size = new System.Drawing.Size(200, 21);
            this.cmbRaces.TabIndex = 2;
            this.cmbRaces.SelectedIndexChanged += new System.EventHandler(this.cmbRaces_SelectedIndexChanged);
            // 
            // cmbDistances
            // 
            this.cmbDistances.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDistances.FormattingEnabled = true;
            this.cmbDistances.Location = new System.Drawing.Point(350, 12);
            this.cmbDistances.Name = "cmbDistances";
            this.cmbDistances.Size = new System.Drawing.Size(200, 21);
            this.cmbDistances.TabIndex = 3;
            this.cmbDistances.SelectedIndexChanged += new System.EventHandler(this.cmbDistances_SelectedIndexChanged);
            // 
            // lblElapsedTime
            // 
            this.lblElapsedTime.AutoSize = true;
            this.lblElapsedTime.Location = new System.Drawing.Point(12, 51);
            this.lblElapsedTime.Name = "lblElapsedTime";
            this.lblElapsedTime.Size = new System.Drawing.Size(77, 13);
            this.lblElapsedTime.TabIndex = 4;
            this.lblElapsedTime.Text = "Elapsed Time: ";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnSetStartTime
            // 
            this.btnSetStartTime.Location = new System.Drawing.Point(680, 12);
            this.btnSetStartTime.Name = "btnSetStartTime";
            this.btnSetStartTime.Size = new System.Drawing.Size(100, 23);
            this.btnSetStartTime.TabIndex = 5;
            this.btnSetStartTime.Text = "Set Start Time";
            this.btnSetStartTime.UseVisualStyleBackColor = true;
            this.btnSetStartTime.Click += new System.EventHandler(this.btnSetStartTime_Click);
            // 
            // btnSetCurrentTimestamp
            // 
            this.btnSetCurrentTimestamp.Location = new System.Drawing.Point(680, 41);
            this.btnSetCurrentTimestamp.Name = "btnSetCurrentTimestamp";
            this.btnSetCurrentTimestamp.Size = new System.Drawing.Size(100, 23);
            this.btnSetCurrentTimestamp.TabIndex = 6;
            this.btnSetCurrentTimestamp.Text = "Set Current Time";
            this.btnSetCurrentTimestamp.UseVisualStyleBackColor = true;
            this.btnSetCurrentTimestamp.Click += new System.EventHandler(this.btnSetCurrentTimestamp_Click);
            // 
            // dtpStartTime
            // 
            this.dtpStartTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtpStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartTime.Location = new System.Drawing.Point(570, 12);
            this.dtpStartTime.Name = "dtpStartTime";
            this.dtpStartTime.Size = new System.Drawing.Size(100, 20);
            this.dtpStartTime.TabIndex = 7;
            // 
            // chkLogSpecificDistance
            // 
            this.chkLogSpecificDistance.AutoSize = true;
            this.chkLogSpecificDistance.Location = new System.Drawing.Point(15, 77);
            this.chkLogSpecificDistance.Name = "chkLogSpecificDistance";
            this.chkLogSpecificDistance.Size = new System.Drawing.Size(130, 17);
            this.chkLogSpecificDistance.TabIndex = 8;
            this.chkLogSpecificDistance.Text = "Log Specific Distance";
            this.chkLogSpecificDistance.UseVisualStyleBackColor = true;
            // 
            // comboBoxEvents
            // 
            this.comboBoxEvents.FormattingEnabled = true;
            this.comboBoxEvents.Location = new System.Drawing.Point(667, 123);
            this.comboBoxEvents.Name = "comboBoxEvents";
            this.comboBoxEvents.Size = new System.Drawing.Size(159, 21);
            this.comboBoxEvents.TabIndex = 9;
            // 
            // btnTransferRecords
            // 
            this.btnTransferRecords.Location = new System.Drawing.Point(883, 124);
            this.btnTransferRecords.Name = "btnTransferRecords";
            this.btnTransferRecords.Size = new System.Drawing.Size(112, 23);
            this.btnTransferRecords.TabIndex = 10;
            this.btnTransferRecords.Text = "Publish Results";
            this.btnTransferRecords.UseVisualStyleBackColor = true;
            this.btnTransferRecords.Click += new System.EventHandler(this.btnTransferRecords_Click);
            // 
            // textBoxEventId
            // 
            this.textBoxEventId.Location = new System.Drawing.Point(832, 124);
            this.textBoxEventId.Name = "textBoxEventId";
            this.textBoxEventId.ReadOnly = true;
            this.textBoxEventId.Size = new System.Drawing.Size(45, 20);
            this.textBoxEventId.TabIndex = 11;
            // 
            // chkAddLap
            // 
            this.chkAddLap.AutoSize = true;
            this.chkAddLap.Location = new System.Drawing.Point(15, 100);
            this.chkAddLap.Name = "chkAddLap";
            this.chkAddLap.Size = new System.Drawing.Size(110, 17);
            this.chkAddLap.TabIndex = 12;
            this.chkAddLap.Text = "Add Starting Laps";
            this.chkAddLap.UseVisualStyleBackColor = true;
            // 
            // reading
            // 
            this.ClientSize = new System.Drawing.Size(1006, 461);
            this.Controls.Add(this.chkAddLap);
            this.Controls.Add(this.textBoxEventId);
            this.Controls.Add(this.btnTransferRecords);
            this.Controls.Add(this.comboBoxEvents);
            this.Controls.Add(this.chkLogSpecificDistance);
            this.Controls.Add(this.dtpStartTime);
            this.Controls.Add(this.btnSetCurrentTimestamp);
            this.Controls.Add(this.btnSetStartTime);
            this.Controls.Add(this.lblElapsedTime);
            this.Controls.Add(this.cmbDistances);
            this.Controls.Add(this.cmbRaces);
            this.Controls.Add(this.btnSelectFile);
            this.Controls.Add(this.dataGridView1);
            this.Name = "reading";
            this.Text = "Race Manager";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.ComboBox cmbRaces;
        private System.Windows.Forms.ComboBox cmbDistances;
        private System.Windows.Forms.Label lblElapsedTime;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnSetStartTime;
        private System.Windows.Forms.Button btnSetCurrentTimestamp;
        private System.Windows.Forms.DateTimePicker dtpStartTime;
        private System.Windows.Forms.CheckBox chkLogSpecificDistance;
        private System.Windows.Forms.ComboBox comboBoxEvents;
        private System.Windows.Forms.Button btnTransferRecords;
        private System.Windows.Forms.TextBox textBoxEventId;
        private System.Windows.Forms.CheckBox chkAddLap;
    }
}
