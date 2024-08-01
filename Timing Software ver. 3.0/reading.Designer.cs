namespace RaceManager
{
    partial class reading
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ComboBox cmbRaces;
        private System.Windows.Forms.ComboBox cmbDistances;
        private System.Windows.Forms.DateTimePicker dtpStartTime;
        private System.Windows.Forms.Button btnSetStartTime;
        private System.Windows.Forms.Button btnSetCurrentTimestamp;
        private System.Windows.Forms.Label lblElapsedTime;
        private System.Windows.Forms.CheckBox chkLogSpecificDistance;

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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.cmbRaces = new System.Windows.Forms.ComboBox();
            this.cmbDistances = new System.Windows.Forms.ComboBox();
            this.dtpStartTime = new System.Windows.Forms.DateTimePicker();
            this.btnSetStartTime = new System.Windows.Forms.Button();
            this.btnSetCurrentTimestamp = new System.Windows.Forms.Button();
            this.lblElapsedTime = new System.Windows.Forms.Label();
            this.chkLogSpecificDistance = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 41);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(776, 300);
            this.dataGridView1.TabIndex = 0;
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(12, 12);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(75, 23);
            this.btnSelectFile.TabIndex = 1;
            this.btnSelectFile.Text = "Select File";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000; // Check every second
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // cmbRaces
            // 
            this.cmbRaces.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRaces.FormattingEnabled = true;
            this.cmbRaces.Location = new System.Drawing.Point(12, 358);
            this.cmbRaces.Name = "cmbRaces";
            this.cmbRaces.Size = new System.Drawing.Size(200, 21);
            this.cmbRaces.TabIndex = 2;
            this.cmbRaces.SelectedIndexChanged += new System.EventHandler(this.cmbRaces_SelectedIndexChanged);
            // 
            // cmbDistances
            // 
            this.cmbDistances.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDistances.FormattingEnabled = true;
            this.cmbDistances.Location = new System.Drawing.Point(12, 385);
            this.cmbDistances.Name = "cmbDistances";
            this.cmbDistances.Size = new System.Drawing.Size(200, 21);
            this.cmbDistances.TabIndex = 3;
            this.cmbDistances.SelectedIndexChanged += new System.EventHandler(this.cmbDistances_SelectedIndexChanged);
            // 
            // dtpStartTime
            // 
            this.dtpStartTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtpStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartTime.Location = new System.Drawing.Point(218, 358);
            this.dtpStartTime.Name = "dtpStartTime";
            this.dtpStartTime.Size = new System.Drawing.Size(200, 20);
            this.dtpStartTime.TabIndex = 4;
            // 
            // btnSetStartTime
            // 
            this.btnSetStartTime.Location = new System.Drawing.Point(424, 356);
            this.btnSetStartTime.Name = "btnSetStartTime";
            this.btnSetStartTime.Size = new System.Drawing.Size(75, 23);
            this.btnSetStartTime.TabIndex = 5;
            this.btnSetStartTime.Text = "Set Start Time";
            this.btnSetStartTime.UseVisualStyleBackColor = true;
            this.btnSetStartTime.Click += new System.EventHandler(this.btnSetStartTime_Click);
            // 
            // btnSetCurrentTimestamp
            // 
            this.btnSetCurrentTimestamp.Location = new System.Drawing.Point(505, 356);
            this.btnSetCurrentTimestamp.Name = "btnSetCurrentTimestamp";
            this.btnSetCurrentTimestamp.Size = new System.Drawing.Size(125, 23);
            this.btnSetCurrentTimestamp.TabIndex = 6;
            this.btnSetCurrentTimestamp.Text = "Set Current Timestamp";
            this.btnSetCurrentTimestamp.UseVisualStyleBackColor = true;
            this.btnSetCurrentTimestamp.Click += new System.EventHandler(this.btnSetCurrentTimestamp_Click);
            // 
            // lblElapsedTime
            // 
            this.lblElapsedTime.AutoSize = true;
            this.lblElapsedTime.Location = new System.Drawing.Point(636, 361);
            this.lblElapsedTime.Name = "lblElapsedTime";
            this.lblElapsedTime.Size = new System.Drawing.Size(70, 13);
            this.lblElapsedTime.TabIndex = 7;
            this.lblElapsedTime.Text = "Elapsed Time";
            // 
            // chkLogSpecificDistance
            // 
            this.chkLogSpecificDistance.AutoSize = true;
            this.chkLogSpecificDistance.Location = new System.Drawing.Point(218, 387);
            this.chkLogSpecificDistance.Name = "chkLogSpecificDistance";
            this.chkLogSpecificDistance.Size = new System.Drawing.Size(134, 17);
            this.chkLogSpecificDistance.TabIndex = 8;
            this.chkLogSpecificDistance.Text = "Log Specific Distance";
            this.chkLogSpecificDistance.UseVisualStyleBackColor = true;
            // 
            // reading
            // 
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.chkLogSpecificDistance);
            this.Controls.Add(this.lblElapsedTime);
            this.Controls.Add(this.btnSetCurrentTimestamp);
            this.Controls.Add(this.btnSetStartTime);
            this.Controls.Add(this.dtpStartTime);
            this.Controls.Add(this.cmbDistances);
            this.Controls.Add(this.cmbRaces);
            this.Controls.Add(this.btnSelectFile);
            this.Controls.Add(this.dataGridView1);
            this.Name = "reading";
            this.Text = "Reading";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
