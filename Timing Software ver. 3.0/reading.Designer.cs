namespace RaceManager
{
    partial class reading
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
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
            this.lblRunnersStarted = new System.Windows.Forms.Label();
            this.lblRunnersFinished = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnSetStartTime = new System.Windows.Forms.Button();
            this.btnSetCurrentTimestamp = new System.Windows.Forms.Button();
            this.dtpStartTime = new System.Windows.Forms.DateTimePicker();
            this.chkLogSpecificDistance = new System.Windows.Forms.CheckBox();
            this.comboBoxEvents = new System.Windows.Forms.ComboBox();
            this.textBoxEventId = new System.Windows.Forms.TextBox();
            this.chkAddLap = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.publishtoremote = new System.Windows.Forms.Button();
            this.ReaderPosition = new System.Windows.Forms.TextBox();
            this.chkAutoUpload = new System.Windows.Forms.CheckBox();
            this.txtUploadInterval = new System.Windows.Forms.TextBox();
            this.lblLastPush = new System.Windows.Forms.Label();
            this.cmbCategories = new System.Windows.Forms.ComboBox();
            this.dgvTopRunners = new System.Windows.Forms.DataGridView();
            this.btnRefresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTopRunners)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 403);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(982, 47);
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
            this.cmbRaces.Location = new System.Drawing.Point(130, 13);
            this.cmbRaces.Name = "cmbRaces";
            this.cmbRaces.Size = new System.Drawing.Size(200, 21);
            this.cmbRaces.TabIndex = 2;
            this.cmbRaces.SelectedIndexChanged += new System.EventHandler(this.cmbRaces_SelectedIndexChanged);
            // 
            // cmbDistances
            // 
            this.cmbDistances.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDistances.FormattingEnabled = true;
            this.cmbDistances.Location = new System.Drawing.Point(350, 13);
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
            // lblRunnersStarted
            // 
            this.lblRunnersStarted.AutoSize = true;
            this.lblRunnersStarted.Location = new System.Drawing.Point(12, 70);
            this.lblRunnersStarted.Name = "lblRunnersStarted";
            this.lblRunnersStarted.Size = new System.Drawing.Size(96, 13);
            this.lblRunnersStarted.TabIndex = 5;
            this.lblRunnersStarted.Text = "Runners Started: 0";
            // 
            // lblRunnersFinished
            // 
            this.lblRunnersFinished.AutoSize = true;
            this.lblRunnersFinished.Location = new System.Drawing.Point(130, 70);
            this.lblRunnersFinished.Name = "lblRunnersFinished";
            this.lblRunnersFinished.Size = new System.Drawing.Size(101, 13);
            this.lblRunnersFinished.TabIndex = 6;
            this.lblRunnersFinished.Text = "Runners Finished: 0";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnSetStartTime
            // 
            this.btnSetStartTime.Location = new System.Drawing.Point(726, 13);
            this.btnSetStartTime.Name = "btnSetStartTime";
            this.btnSetStartTime.Size = new System.Drawing.Size(100, 23);
            this.btnSetStartTime.TabIndex = 7;
            this.btnSetStartTime.Text = "Set Start Time";
            this.btnSetStartTime.UseVisualStyleBackColor = true;
            this.btnSetStartTime.Click += new System.EventHandler(this.btnSetStartTime_Click);
            // 
            // btnSetCurrentTimestamp
            // 
            this.btnSetCurrentTimestamp.Location = new System.Drawing.Point(726, 42);
            this.btnSetCurrentTimestamp.Name = "btnSetCurrentTimestamp";
            this.btnSetCurrentTimestamp.Size = new System.Drawing.Size(100, 23);
            this.btnSetCurrentTimestamp.TabIndex = 8;
            this.btnSetCurrentTimestamp.Text = "Set Current Time";
            this.btnSetCurrentTimestamp.UseVisualStyleBackColor = true;
            this.btnSetCurrentTimestamp.Click += new System.EventHandler(this.btnSetCurrentTimestamp_Click);
            // 
            // dtpStartTime
            // 
            this.dtpStartTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtpStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartTime.Location = new System.Drawing.Point(570, 13);
            this.dtpStartTime.Name = "dtpStartTime";
            this.dtpStartTime.Size = new System.Drawing.Size(150, 20);
            this.dtpStartTime.TabIndex = 9;
            // 
            // chkLogSpecificDistance
            // 
            this.chkLogSpecificDistance.AutoSize = true;
            this.chkLogSpecificDistance.Location = new System.Drawing.Point(15, 90);
            this.chkLogSpecificDistance.Name = "chkLogSpecificDistance";
            this.chkLogSpecificDistance.Size = new System.Drawing.Size(130, 17);
            this.chkLogSpecificDistance.TabIndex = 10;
            this.chkLogSpecificDistance.Text = "Log Specific Distance";
            this.chkLogSpecificDistance.UseVisualStyleBackColor = true;
            // 
            // comboBoxEvents
            // 
            this.comboBoxEvents.FormattingEnabled = true;
            this.comboBoxEvents.Location = new System.Drawing.Point(667, 123);
            this.comboBoxEvents.Name = "comboBoxEvents";
            this.comboBoxEvents.Size = new System.Drawing.Size(159, 21);
            this.comboBoxEvents.TabIndex = 11;
            // 
            // textBoxEventId
            // 
            this.textBoxEventId.Location = new System.Drawing.Point(832, 123);
            this.textBoxEventId.Name = "textBoxEventId";
            this.textBoxEventId.ReadOnly = true;
            this.textBoxEventId.Size = new System.Drawing.Size(45, 20);
            this.textBoxEventId.TabIndex = 12;
            // 
            // chkAddLap
            // 
            this.chkAddLap.AutoSize = true;
            this.chkAddLap.Location = new System.Drawing.Point(15, 113);
            this.chkAddLap.Name = "chkAddLap";
            this.chkAddLap.Size = new System.Drawing.Size(222, 17);
            this.chkAddLap.TabIndex = 13;
            this.chkAddLap.Text = "Add Starting Laps (to count starting times)";
            this.chkAddLap.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 134);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(130, 23);
            this.button1.TabIndex = 14;
            this.button1.Text = "Add zero starting times";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(151, 139);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(247, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "( NOT counting starting times for the selected race)";
            // 
            // publishtoremote
            // 
            this.publishtoremote.Location = new System.Drawing.Point(892, 123);
            this.publishtoremote.Name = "publishtoremote";
            this.publishtoremote.Size = new System.Drawing.Size(102, 23);
            this.publishtoremote.TabIndex = 16;
            this.publishtoremote.Text = "Publish Results";
            this.publishtoremote.UseVisualStyleBackColor = true;
            this.publishtoremote.Click += new System.EventHandler(this.button3_Click);
            // 
            // ReaderPosition
            // 
            this.ReaderPosition.Location = new System.Drawing.Point(667, 98);
            this.ReaderPosition.Name = "ReaderPosition";
            this.ReaderPosition.Size = new System.Drawing.Size(158, 20);
            this.ReaderPosition.TabIndex = 17;
            // 
            // chkAutoUpload
            // 
            this.chkAutoUpload.AutoSize = true;
            this.chkAutoUpload.Location = new System.Drawing.Point(667, 75);
            this.chkAutoUpload.Name = "chkAutoUpload";
            this.chkAutoUpload.Size = new System.Drawing.Size(75, 17);
            this.chkAutoUpload.TabIndex = 18;
            this.chkAutoUpload.Text = "Auto Sync";
            this.chkAutoUpload.UseVisualStyleBackColor = true;
            this.chkAutoUpload.CheckedChanged += new System.EventHandler(this.chkAutoUpload_CheckedChanged);
            // 
            // txtUploadInterval
            // 
            this.txtUploadInterval.Location = new System.Drawing.Point(748, 73);
            this.txtUploadInterval.Name = "txtUploadInterval";
            this.txtUploadInterval.Size = new System.Drawing.Size(45, 20);
            this.txtUploadInterval.TabIndex = 19;
            this.txtUploadInterval.Text = "5";
            // 
            // lblLastPush
            // 
            this.lblLastPush.AutoSize = true;
            this.lblLastPush.Location = new System.Drawing.Point(809, 76);
            this.lblLastPush.Name = "lblLastPush";
            this.lblLastPush.Size = new System.Drawing.Size(54, 13);
            this.lblLastPush.TabIndex = 20;
            this.lblLastPush.Text = "Last Push";
            // 
            // cmbCategories
            // 
            this.cmbCategories.FormattingEnabled = true;
            this.cmbCategories.Location = new System.Drawing.Point(24, 212);
            this.cmbCategories.Name = "cmbCategories";
            this.cmbCategories.Size = new System.Drawing.Size(121, 21);
            this.cmbCategories.TabIndex = 21;
            this.cmbCategories.SelectedIndexChanged += new System.EventHandler(this.cmbCategories_SelectedIndexChanged);
            // 
            // dgvTopRunners
            // 
            this.dgvTopRunners.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvTopRunners.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvTopRunners.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTopRunners.Location = new System.Drawing.Point(154, 212);
            this.dgvTopRunners.Name = "dgvTopRunners";
            this.dgvTopRunners.Size = new System.Drawing.Size(840, 168);
            this.dgvTopRunners.TabIndex = 22;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(24, 239);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 23;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // reading
            // 
            this.ClientSize = new System.Drawing.Size(1006, 461);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.dgvTopRunners);
            this.Controls.Add(this.cmbCategories);
            this.Controls.Add(this.lblLastPush);
            this.Controls.Add(this.txtUploadInterval);
            this.Controls.Add(this.chkAutoUpload);
            this.Controls.Add(this.ReaderPosition);
            this.Controls.Add(this.publishtoremote);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.chkAddLap);
            this.Controls.Add(this.textBoxEventId);
            this.Controls.Add(this.comboBoxEvents);
            this.Controls.Add(this.chkLogSpecificDistance);
            this.Controls.Add(this.dtpStartTime);
            this.Controls.Add(this.btnSetCurrentTimestamp);
            this.Controls.Add(this.btnSetStartTime);
            this.Controls.Add(this.lblRunnersFinished);
            this.Controls.Add(this.lblRunnersStarted);
            this.Controls.Add(this.lblElapsedTime);
            this.Controls.Add(this.cmbDistances);
            this.Controls.Add(this.cmbRaces);
            this.Controls.Add(this.btnSelectFile);
            this.Controls.Add(this.dataGridView1);
            this.Name = "reading";
            this.Text = "Race Manager";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTopRunners)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.ComboBox cmbRaces;
        private System.Windows.Forms.ComboBox cmbDistances;
        private System.Windows.Forms.Label lblElapsedTime;
        private System.Windows.Forms.Label lblRunnersStarted;
        private System.Windows.Forms.Label lblRunnersFinished;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnSetStartTime;
        private System.Windows.Forms.Button btnSetCurrentTimestamp;
        private System.Windows.Forms.DateTimePicker dtpStartTime;
        private System.Windows.Forms.CheckBox chkLogSpecificDistance;
        private System.Windows.Forms.ComboBox comboBoxEvents;
        private System.Windows.Forms.TextBox textBoxEventId;
        private System.Windows.Forms.CheckBox chkAddLap;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button publishtoremote;
        private System.Windows.Forms.TextBox ReaderPosition;
        private System.Windows.Forms.CheckBox chkAutoUpload;
        private System.Windows.Forms.TextBox txtUploadInterval;
        private System.Windows.Forms.Label lblLastPush;
        private System.Windows.Forms.ComboBox cmbCategories;
        private System.Windows.Forms.DataGridView dgvTopRunners;
        private System.Windows.Forms.Button btnRefresh;
    }
}
