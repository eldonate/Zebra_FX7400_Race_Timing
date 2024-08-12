namespace RaceManager
{
    partial class MainForm
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
            this.txtRaceName = new System.Windows.Forms.TextBox();
            this.dtpRaceDate = new System.Windows.Forms.DateTimePicker();
            this.btnCreateRace = new System.Windows.Forms.Button();
            this.txtRaceId = new System.Windows.Forms.TextBox();
            this.txtDistanceName = new System.Windows.Forms.TextBox();
            this.txtLaps = new System.Windows.Forms.TextBox();
            this.txtIntervals = new System.Windows.Forms.TextBox();
            this.txtStartRfid = new System.Windows.Forms.TextBox();
            this.txtEndRfid = new System.Windows.Forms.TextBox();
            this.btnAddDistance = new System.Windows.Forms.Button();
            this.txtDistanceId = new System.Windows.Forms.TextBox();
            this.txtCategoryName = new System.Windows.Forms.TextBox();
            this.txtStartAge = new System.Windows.Forms.TextBox();
            this.txtEndAge = new System.Windows.Forms.TextBox();
            this.btnAddCategory = new System.Windows.Forms.Button();
            this.cmbRaces = new System.Windows.Forms.ComboBox();
            this.cmbDistances = new System.Windows.Forms.ComboBox();
            this.txtRaceDetails = new System.Windows.Forms.TextBox();
            this.txtDistanceDetails = new System.Windows.Forms.TextBox();
            this.btnAddParticipant = new System.Windows.Forms.Button();
            this.btnOpenReadingForm = new System.Windows.Forms.Button();
            this.btnNewParticipants = new System.Windows.Forms.Button();
            this.btnNewForm = new System.Windows.Forms.Button();
            this.txtSplitNames = new System.Windows.Forms.TextBox();
            this.addSplit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtRaceName
            // 
            this.txtRaceName.Location = new System.Drawing.Point(12, 12);
            this.txtRaceName.Name = "txtRaceName";
            this.txtRaceName.Size = new System.Drawing.Size(200, 20);
            this.txtRaceName.TabIndex = 0;
            this.txtRaceName.Text = "Race Name";
            // 
            // dtpRaceDate
            // 
            this.dtpRaceDate.Location = new System.Drawing.Point(12, 38);
            this.dtpRaceDate.Name = "dtpRaceDate";
            this.dtpRaceDate.Size = new System.Drawing.Size(200, 20);
            this.dtpRaceDate.TabIndex = 1;
            // 
            // btnCreateRace
            // 
            this.btnCreateRace.Location = new System.Drawing.Point(12, 64);
            this.btnCreateRace.Name = "btnCreateRace";
            this.btnCreateRace.Size = new System.Drawing.Size(200, 23);
            this.btnCreateRace.TabIndex = 2;
            this.btnCreateRace.Text = "Create Race";
            this.btnCreateRace.UseVisualStyleBackColor = true;
            this.btnCreateRace.Click += new System.EventHandler(this.btnCreateRace_Click);
            // 
            // txtRaceId
            // 
            this.txtRaceId.Location = new System.Drawing.Point(12, 93);
            this.txtRaceId.Name = "txtRaceId";
            this.txtRaceId.Size = new System.Drawing.Size(200, 20);
            this.txtRaceId.TabIndex = 3;
            this.txtRaceId.Text = "Race ID";
            // 
            // txtDistanceName
            // 
            this.txtDistanceName.Location = new System.Drawing.Point(12, 119);
            this.txtDistanceName.Name = "txtDistanceName";
            this.txtDistanceName.Size = new System.Drawing.Size(200, 20);
            this.txtDistanceName.TabIndex = 4;
            this.txtDistanceName.Text = "Distance Name";
            // 
            // txtLaps
            // 
            this.txtLaps.Location = new System.Drawing.Point(12, 145);
            this.txtLaps.Name = "txtLaps";
            this.txtLaps.Size = new System.Drawing.Size(200, 20);
            this.txtLaps.TabIndex = 5;
            this.txtLaps.Text = "Laps";
            // 
            // txtIntervals
            // 
            this.txtIntervals.Location = new System.Drawing.Point(12, 171);
            this.txtIntervals.Name = "txtIntervals";
            this.txtIntervals.Size = new System.Drawing.Size(200, 20);
            this.txtIntervals.TabIndex = 6;
            this.txtIntervals.Text = "Intervals";
            // 
            // txtStartRfid
            // 
            this.txtStartRfid.Location = new System.Drawing.Point(12, 197);
            this.txtStartRfid.Name = "txtStartRfid";
            this.txtStartRfid.Size = new System.Drawing.Size(200, 20);
            this.txtStartRfid.TabIndex = 7;
            this.txtStartRfid.Text = "Start RFID";
            // 
            // txtEndRfid
            // 
            this.txtEndRfid.Location = new System.Drawing.Point(12, 223);
            this.txtEndRfid.Name = "txtEndRfid";
            this.txtEndRfid.Size = new System.Drawing.Size(200, 20);
            this.txtEndRfid.TabIndex = 8;
            this.txtEndRfid.Text = "End RFID";
            // 
            // btnAddDistance
            // 
            this.btnAddDistance.Location = new System.Drawing.Point(12, 249);
            this.btnAddDistance.Name = "btnAddDistance";
            this.btnAddDistance.Size = new System.Drawing.Size(200, 23);
            this.btnAddDistance.TabIndex = 9;
            this.btnAddDistance.Text = "Add Distance";
            this.btnAddDistance.UseVisualStyleBackColor = true;
            this.btnAddDistance.Click += new System.EventHandler(this.btnAddDistance_Click);
            // 
            // txtDistanceId
            // 
            this.txtDistanceId.Location = new System.Drawing.Point(12, 278);
            this.txtDistanceId.Name = "txtDistanceId";
            this.txtDistanceId.Size = new System.Drawing.Size(200, 20);
            this.txtDistanceId.TabIndex = 10;
            this.txtDistanceId.Text = "Distance ID";
            // 
            // txtCategoryName
            // 
            this.txtCategoryName.Location = new System.Drawing.Point(12, 304);
            this.txtCategoryName.Name = "txtCategoryName";
            this.txtCategoryName.Size = new System.Drawing.Size(200, 20);
            this.txtCategoryName.TabIndex = 11;
            this.txtCategoryName.Text = "Category Name";
            // 
            // txtStartAge
            // 
            this.txtStartAge.Location = new System.Drawing.Point(12, 330);
            this.txtStartAge.Name = "txtStartAge";
            this.txtStartAge.Size = new System.Drawing.Size(200, 20);
            this.txtStartAge.TabIndex = 12;
            this.txtStartAge.Text = "Start Age";
            // 
            // txtEndAge
            // 
            this.txtEndAge.Location = new System.Drawing.Point(12, 356);
            this.txtEndAge.Name = "txtEndAge";
            this.txtEndAge.Size = new System.Drawing.Size(200, 20);
            this.txtEndAge.TabIndex = 13;
            this.txtEndAge.Text = "End Age";
            // 
            // btnAddCategory
            // 
            this.btnAddCategory.Location = new System.Drawing.Point(12, 382);
            this.btnAddCategory.Name = "btnAddCategory";
            this.btnAddCategory.Size = new System.Drawing.Size(200, 23);
            this.btnAddCategory.TabIndex = 14;
            this.btnAddCategory.Text = "Add Category";
            this.btnAddCategory.UseVisualStyleBackColor = true;
            this.btnAddCategory.Click += new System.EventHandler(this.btnAddCategory_Click);
            // 
            // cmbRaces
            // 
            this.cmbRaces.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRaces.FormattingEnabled = true;
            this.cmbRaces.Location = new System.Drawing.Point(250, 12);
            this.cmbRaces.Name = "cmbRaces";
            this.cmbRaces.Size = new System.Drawing.Size(200, 21);
            this.cmbRaces.TabIndex = 15;
            this.cmbRaces.SelectedIndexChanged += new System.EventHandler(this.cmbRaces_SelectedIndexChanged);
            // 
            // cmbDistances
            // 
            this.cmbDistances.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDistances.FormattingEnabled = true;
            this.cmbDistances.Location = new System.Drawing.Point(250, 93);
            this.cmbDistances.Name = "cmbDistances";
            this.cmbDistances.Size = new System.Drawing.Size(200, 21);
            this.cmbDistances.TabIndex = 16;
            this.cmbDistances.SelectedIndexChanged += new System.EventHandler(this.cmbDistances_SelectedIndexChanged);
            // 
            // txtRaceDetails
            // 
            this.txtRaceDetails.Location = new System.Drawing.Point(250, 40);
            this.txtRaceDetails.Multiline = true;
            this.txtRaceDetails.Name = "txtRaceDetails";
            this.txtRaceDetails.ReadOnly = true;
            this.txtRaceDetails.Size = new System.Drawing.Size(200, 47);
            this.txtRaceDetails.TabIndex = 17;
            // 
            // txtDistanceDetails
            // 
            this.txtDistanceDetails.Location = new System.Drawing.Point(250, 121);
            this.txtDistanceDetails.Multiline = true;
            this.txtDistanceDetails.Name = "txtDistanceDetails";
            this.txtDistanceDetails.ReadOnly = true;
            this.txtDistanceDetails.Size = new System.Drawing.Size(200, 121);
            this.txtDistanceDetails.TabIndex = 18;
            // 
            // btnAddParticipant
            // 
            this.btnAddParticipant.Location = new System.Drawing.Point(250, 360);
            this.btnAddParticipant.Name = "btnAddParticipant";
            this.btnAddParticipant.Size = new System.Drawing.Size(200, 23);
            this.btnAddParticipant.TabIndex = 19;
            this.btnAddParticipant.Text = "Add Participant";
            this.btnAddParticipant.UseVisualStyleBackColor = true;
            this.btnAddParticipant.Click += new System.EventHandler(this.btnAddParticipant_Click);
            // 
            // btnOpenReadingForm
            // 
            this.btnOpenReadingForm.Location = new System.Drawing.Point(250, 386);
            this.btnOpenReadingForm.Name = "btnOpenReadingForm";
            this.btnOpenReadingForm.Size = new System.Drawing.Size(200, 23);
            this.btnOpenReadingForm.TabIndex = 20;
            this.btnOpenReadingForm.Text = "Open Reading Form";
            this.btnOpenReadingForm.UseVisualStyleBackColor = true;
            this.btnOpenReadingForm.Click += new System.EventHandler(this.btnOpenReadingForm_Click);
            // 
            // btnNewParticipants
            // 
            this.btnNewParticipants.Location = new System.Drawing.Point(250, 249);
            this.btnNewParticipants.Name = "btnNewParticipants";
            this.btnNewParticipants.Size = new System.Drawing.Size(200, 23);
            this.btnNewParticipants.TabIndex = 21;
            this.btnNewParticipants.Text = "Participants";
            this.btnNewParticipants.UseVisualStyleBackColor = true;
            this.btnNewParticipants.Click += new System.EventHandler(this.btnNewParticipants_Click);
            // 
            // btnNewForm
            // 
            this.btnNewForm.Location = new System.Drawing.Point(250, 278);
            this.btnNewForm.Name = "btnNewForm";
            this.btnNewForm.Size = new System.Drawing.Size(200, 23);
            this.btnNewForm.TabIndex = 22;
            this.btnNewForm.Text = "Reading";
            this.btnNewForm.UseVisualStyleBackColor = true;
            this.btnNewForm.Click += new System.EventHandler(this.btnNewForm_Click);
            // 
            // txtSplitNames
            // 
            this.txtSplitNames.Location = new System.Drawing.Point(12, 412);
            this.txtSplitNames.Name = "txtSplitNames";
            this.txtSplitNames.Size = new System.Drawing.Size(200, 20);
            this.txtSplitNames.TabIndex = 23;
            // 
            // addSplit
            // 
            this.addSplit.Location = new System.Drawing.Point(12, 439);
            this.addSplit.Name = "addSplit";
            this.addSplit.Size = new System.Drawing.Size(200, 23);
            this.addSplit.TabIndex = 24;
            this.addSplit.Text = "button1";
            this.addSplit.UseVisualStyleBackColor = true;
            this.addSplit.Click += new System.EventHandler(this.addSplit_Click);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(464, 484);
            this.Controls.Add(this.addSplit);
            this.Controls.Add(this.txtSplitNames);
            this.Controls.Add(this.btnNewForm);
            this.Controls.Add(this.btnNewParticipants);
            this.Controls.Add(this.btnOpenReadingForm);
            this.Controls.Add(this.btnAddParticipant);
            this.Controls.Add(this.txtDistanceDetails);
            this.Controls.Add(this.txtRaceDetails);
            this.Controls.Add(this.cmbDistances);
            this.Controls.Add(this.cmbRaces);
            this.Controls.Add(this.btnAddCategory);
            this.Controls.Add(this.txtEndAge);
            this.Controls.Add(this.txtStartAge);
            this.Controls.Add(this.txtCategoryName);
            this.Controls.Add(this.txtDistanceId);
            this.Controls.Add(this.btnAddDistance);
            this.Controls.Add(this.txtEndRfid);
            this.Controls.Add(this.txtStartRfid);
            this.Controls.Add(this.txtIntervals);
            this.Controls.Add(this.txtLaps);
            this.Controls.Add(this.txtDistanceName);
            this.Controls.Add(this.txtRaceId);
            this.Controls.Add(this.btnCreateRace);
            this.Controls.Add(this.dtpRaceDate);
            this.Controls.Add(this.txtRaceName);
            this.Name = "MainForm";
            this.Text = "Race Manager";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.TextBox txtRaceName;
        private System.Windows.Forms.DateTimePicker dtpRaceDate;
        private System.Windows.Forms.Button btnCreateRace;
        private System.Windows.Forms.TextBox txtRaceId;
        private System.Windows.Forms.TextBox txtDistanceName;
        private System.Windows.Forms.TextBox txtLaps;
        private System.Windows.Forms.TextBox txtIntervals;
        private System.Windows.Forms.TextBox txtStartRfid;
        private System.Windows.Forms.TextBox txtEndRfid;
        private System.Windows.Forms.Button btnAddDistance;
        private System.Windows.Forms.TextBox txtDistanceId;
        private System.Windows.Forms.TextBox txtCategoryName;
        private System.Windows.Forms.TextBox txtStartAge;
        private System.Windows.Forms.TextBox txtEndAge;
        private System.Windows.Forms.Button btnAddCategory;
        private System.Windows.Forms.ComboBox cmbRaces;
        private System.Windows.Forms.ComboBox cmbDistances;
        private System.Windows.Forms.TextBox txtRaceDetails;
        private System.Windows.Forms.TextBox txtDistanceDetails;
        private System.Windows.Forms.Button btnAddParticipant;
        private System.Windows.Forms.Button btnOpenReadingForm;
        private System.Windows.Forms.Button btnNewParticipants;
        private System.Windows.Forms.Button btnNewForm; // Declare the new button

        #endregion

        private System.Windows.Forms.TextBox txtSplitNames;
        private System.Windows.Forms.Button addSplit;
    }
}
