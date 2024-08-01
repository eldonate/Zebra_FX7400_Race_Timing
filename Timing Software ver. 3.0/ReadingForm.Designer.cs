namespace RaceManager
{
    partial class ReadingForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.ComboBox cmbRaces;
        private System.Windows.Forms.ComboBox cmbDistances;
        private System.Windows.Forms.Button btnSetCurrentTime;
        private System.Windows.Forms.Button btnSetSelectedRecordTime;
        private System.Windows.Forms.Button btnClearDataGrid;
        private System.Windows.Forms.Label lblRealTimeDuration;
        private System.Windows.Forms.Button positions_update;
        private System.Windows.Forms.DateTimePicker dtpRaceStartTime;
        private System.Windows.Forms.Button btnSetRaceStartTime;
        private System.Windows.Forms.Label lblRaceStartTime;

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
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.cmbRaces = new System.Windows.Forms.ComboBox();
            this.cmbDistances = new System.Windows.Forms.ComboBox();
            this.btnSetCurrentTime = new System.Windows.Forms.Button();
            this.btnSetSelectedRecordTime = new System.Windows.Forms.Button();
            this.btnClearDataGrid = new System.Windows.Forms.Button();
            this.lblRealTimeDuration = new System.Windows.Forms.Label();
            this.positions_update = new System.Windows.Forms.Button();
            this.dtpRaceStartTime = new System.Windows.Forms.DateTimePicker();
            this.btnSetRaceStartTime = new System.Windows.Forms.Button();
            this.lblRaceStartTime = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(12, 120);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(760, 357);
            this.dataGridView.TabIndex = 0;
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(12, 12);
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
            // btnSetCurrentTime
            // 
            this.btnSetCurrentTime.Location = new System.Drawing.Point(570, 10);
            this.btnSetCurrentTime.Name = "btnSetCurrentTime";
            this.btnSetCurrentTime.Size = new System.Drawing.Size(100, 23);
            this.btnSetCurrentTime.TabIndex = 4;
            this.btnSetCurrentTime.Text = "Set Current Time";
            this.btnSetCurrentTime.UseVisualStyleBackColor = true;
            this.btnSetCurrentTime.Click += new System.EventHandler(this.btnSetCurrentTime_Click);
            // 
            // btnSetSelectedRecordTime
            // 
            this.btnSetSelectedRecordTime.Location = new System.Drawing.Point(680, 10);
            this.btnSetSelectedRecordTime.Name = "btnSetSelectedRecordTime";
            this.btnSetSelectedRecordTime.Size = new System.Drawing.Size(100, 23);
            this.btnSetSelectedRecordTime.TabIndex = 5;
            this.btnSetSelectedRecordTime.Text = "Set Record Time";
            this.btnSetSelectedRecordTime.UseVisualStyleBackColor = true;
            this.btnSetSelectedRecordTime.Click += new System.EventHandler(this.btnSetSelectedRecordTime_Click);
            // 
            // btnClearDataGrid
            // 
            this.btnClearDataGrid.Location = new System.Drawing.Point(680, 40);
            this.btnClearDataGrid.Name = "btnClearDataGrid";
            this.btnClearDataGrid.Size = new System.Drawing.Size(100, 23);
            this.btnClearDataGrid.TabIndex = 7;
            this.btnClearDataGrid.Text = "Clear Data Grid";
            this.btnClearDataGrid.UseVisualStyleBackColor = true;
            this.btnClearDataGrid.Click += new System.EventHandler(this.btnClearDataGrid_Click);
            // 
            // lblRealTimeDuration
            // 
            this.lblRealTimeDuration.AutoSize = true;
            this.lblRealTimeDuration.Location = new System.Drawing.Point(150, 41);
            this.lblRealTimeDuration.Name = "lblRealTimeDuration";
            this.lblRealTimeDuration.Size = new System.Drawing.Size(0, 13);
            this.lblRealTimeDuration.TabIndex = 8;
            // 
            // positions_update
            // 
            this.positions_update.Location = new System.Drawing.Point(570, 40);
            this.positions_update.Name = "positions_update";
            this.positions_update.Size = new System.Drawing.Size(100, 23);
            this.positions_update.TabIndex = 9;
            this.positions_update.Text = "Update Positions";
            this.positions_update.UseVisualStyleBackColor = true;
            this.positions_update.Click += new System.EventHandler(this.positions_update_Click);
            // 
            // dtpRaceStartTime
            // 
            this.dtpRaceStartTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtpRaceStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpRaceStartTime.Location = new System.Drawing.Point(15, 67);
            this.dtpRaceStartTime.Name = "dtpRaceStartTime";
            this.dtpRaceStartTime.Size = new System.Drawing.Size(200, 20);
            this.dtpRaceStartTime.TabIndex = 10;
            // 
            // btnSetRaceStartTime
            // 
            this.btnSetRaceStartTime.Location = new System.Drawing.Point(221, 68);
            this.btnSetRaceStartTime.Name = "btnSetRaceStartTime";
            this.btnSetRaceStartTime.Size = new System.Drawing.Size(200, 23);
            this.btnSetRaceStartTime.TabIndex = 11;
            this.btnSetRaceStartTime.Text = "Set Race Start Time";
            this.btnSetRaceStartTime.UseVisualStyleBackColor = true;
            this.btnSetRaceStartTime.Click += new System.EventHandler(this.btnSetRaceStartTime_Click);
            // 
            // lblRaceStartTime
            // 
            this.lblRaceStartTime.AutoSize = true;
            this.lblRaceStartTime.Location = new System.Drawing.Point(12, 45);
            this.lblRaceStartTime.Name = "lblRaceStartTime";
            this.lblRaceStartTime.Size = new System.Drawing.Size(87, 13);
            this.lblRaceStartTime.TabIndex = 12;
            this.lblRaceStartTime.Text = "Race Start Time:";
            // 
            // ReadingForm
            // 
            this.ClientSize = new System.Drawing.Size(784, 489);
            this.Controls.Add(this.lblRaceStartTime);
            this.Controls.Add(this.btnSetRaceStartTime);
            this.Controls.Add(this.dtpRaceStartTime);
            this.Controls.Add(this.positions_update);
            this.Controls.Add(this.lblRealTimeDuration);
            this.Controls.Add(this.btnClearDataGrid);
            this.Controls.Add(this.btnSetSelectedRecordTime);
            this.Controls.Add(this.btnSetCurrentTime);
            this.Controls.Add(this.cmbDistances);
            this.Controls.Add(this.cmbRaces);
            this.Controls.Add(this.btnSelectFile);
            this.Controls.Add(this.dataGridView);
            this.Name = "ReadingForm";
            this.Text = "Reading Form";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
