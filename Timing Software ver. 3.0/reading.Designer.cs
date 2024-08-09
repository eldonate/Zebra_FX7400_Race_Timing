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
            this.textBoxEventId = new System.Windows.Forms.TextBox();
            this.chkAddLap = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
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
            this.btnSetStartTime.Location = new System.Drawing.Point(726, 12);
            this.btnSetStartTime.Name = "btnSetStartTime";
            this.btnSetStartTime.Size = new System.Drawing.Size(100, 23);
            this.btnSetStartTime.TabIndex = 5;
            this.btnSetStartTime.Text = "Set Start Time";
            this.btnSetStartTime.UseVisualStyleBackColor = true;
            this.btnSetStartTime.Click += new System.EventHandler(this.btnSetStartTime_Click);
            // 
            // btnSetCurrentTimestamp
            // 
            this.btnSetCurrentTimestamp.Location = new System.Drawing.Point(726, 41);
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
            this.dtpStartTime.Size = new System.Drawing.Size(150, 20);
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
            this.chkAddLap.Size = new System.Drawing.Size(225, 17);
            this.chkAddLap.TabIndex = 12;
            this.chkAddLap.Text = "Add Starting Laps ( to count starting times)";
            this.chkAddLap.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 121);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(130, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "Add zero starting times";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(151, 126);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(247, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "( NOT counting starting times for the selected race)";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(883, 121);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(110, 23);
            this.button2.TabIndex = 15;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // reading
            // 
            this.ClientSize = new System.Drawing.Size(1006, 461);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.chkAddLap);
            this.Controls.Add(this.textBoxEventId);
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
        private System.Windows.Forms.TextBox textBoxEventId;
        private System.Windows.Forms.CheckBox chkAddLap;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
    }
}
