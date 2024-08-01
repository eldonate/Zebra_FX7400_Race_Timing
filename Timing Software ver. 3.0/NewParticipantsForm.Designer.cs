namespace RaceManager
{
    partial class NewParticipantsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ComboBox cmbRaces;
        private System.Windows.Forms.ComboBox cmbDistances;
        private System.Windows.Forms.TextBox txtRfid;
        private System.Windows.Forms.TextBox txtBib;
        private System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.TextBox txtLastName;
        private System.Windows.Forms.ComboBox cmbGender;
        private System.Windows.Forms.DateTimePicker dtpBirthday;
        private System.Windows.Forms.Button btnAddParticipant;
        private System.Windows.Forms.Label lblRace;
        private System.Windows.Forms.Label lblDistance;
        private System.Windows.Forms.Label lblRfid;
        private System.Windows.Forms.Label lblBib;
        private System.Windows.Forms.Label lblFirstName;
        private System.Windows.Forms.Label lblLastName;
        private System.Windows.Forms.Label lblGender;
        private System.Windows.Forms.Label lblBirthday;

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
            this.cmbRaces = new System.Windows.Forms.ComboBox();
            this.cmbDistances = new System.Windows.Forms.ComboBox();
            this.txtRfid = new System.Windows.Forms.TextBox();
            this.txtBib = new System.Windows.Forms.TextBox();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.cmbGender = new System.Windows.Forms.ComboBox();
            this.dtpBirthday = new System.Windows.Forms.DateTimePicker();
            this.btnAddParticipant = new System.Windows.Forms.Button();
            this.lblRace = new System.Windows.Forms.Label();
            this.lblDistance = new System.Windows.Forms.Label();
            this.lblRfid = new System.Windows.Forms.Label();
            this.lblBib = new System.Windows.Forms.Label();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.lblLastName = new System.Windows.Forms.Label();
            this.lblGender = new System.Windows.Forms.Label();
            this.lblBirthday = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // 
            // cmbRaces
            // 
            this.cmbRaces.FormattingEnabled = true;
            this.cmbRaces.Location = new System.Drawing.Point(120, 12);
            this.cmbRaces.Name = "cmbRaces";
            this.cmbRaces.Size = new System.Drawing.Size(200, 21);
            this.cmbRaces.TabIndex = 0;
            this.cmbRaces.SelectedIndexChanged += new System.EventHandler(this.cmbRaces_SelectedIndexChanged);

            // 
            // cmbDistances
            // 
            this.cmbDistances.FormattingEnabled = true;
            this.cmbDistances.Location = new System.Drawing.Point(120, 39);
            this.cmbDistances.Name = "cmbDistances";
            this.cmbDistances.Size = new System.Drawing.Size(200, 21);
            this.cmbDistances.TabIndex = 1;
            this.cmbDistances.SelectedIndexChanged += new System.EventHandler(this.cmbDistances_SelectedIndexChanged);

            // 
            // txtRfid
            // 
            this.txtRfid.Location = new System.Drawing.Point(120, 66);
            this.txtRfid.Name = "txtRfid";
            this.txtRfid.Size = new System.Drawing.Size(200, 20);
            this.txtRfid.TabIndex = 2;

            // 
            // txtBib
            // 
            this.txtBib.Location = new System.Drawing.Point(120, 92);
            this.txtBib.Name = "txtBib";
            this.txtBib.Size = new System.Drawing.Size(200, 20);
            this.txtBib.TabIndex = 3;

            // 
            // txtFirstName
            // 
            this.txtFirstName.Location = new System.Drawing.Point(120, 118);
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(200, 20);
            this.txtFirstName.TabIndex = 4;

            // 
            // txtLastName
            // 
            this.txtLastName.Location = new System.Drawing.Point(120, 144);
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(200, 20);
            this.txtLastName.TabIndex = 5;

            // 
            // cmbGender
            // 
            this.cmbGender.FormattingEnabled = true;
            this.cmbGender.Items.AddRange(new object[] {
            "Male",
            "Female"});
            this.cmbGender.Location = new System.Drawing.Point(120, 170);
            this.cmbGender.Name = "cmbGender";
            this.cmbGender.Size = new System.Drawing.Size(200, 21);
            this.cmbGender.TabIndex = 6;

            // 
            // dtpBirthday
            // 
            this.dtpBirthday.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpBirthday.Location = new System.Drawing.Point(120, 197);
            this.dtpBirthday.Name = "dtpBirthday";
            this.dtpBirthday.Size = new System.Drawing.Size(200, 20);
            this.dtpBirthday.TabIndex = 7;

            // 
            // btnAddParticipant
            // 
            this.btnAddParticipant.Location = new System.Drawing.Point(120, 223);
            this.btnAddParticipant.Name = "btnAddParticipant";
            this.btnAddParticipant.Size = new System.Drawing.Size(200, 23);
            this.btnAddParticipant.TabIndex = 8;
            this.btnAddParticipant.Text = "Add Participant";
            this.btnAddParticipant.UseVisualStyleBackColor = true;
            this.btnAddParticipant.Click += new System.EventHandler(this.btnAddParticipant_Click);

            // 
            // lblRace
            // 
            this.lblRace.AutoSize = true;
            this.lblRace.Location = new System.Drawing.Point(12, 15);
            this.lblRace.Name = "lblRace";
            this.lblRace.Size = new System.Drawing.Size(32, 13);
            this.lblRace.TabIndex = 9;
            this.lblRace.Text = "Race:";

            // 
            // lblDistance
            // 
            this.lblDistance.AutoSize = true;
            this.lblDistance.Location = new System.Drawing.Point(12, 42);
            this.lblDistance.Name = "lblDistance";
            this.lblDistance.Size = new System.Drawing.Size(54, 13);
            this.lblDistance.TabIndex = 10;
            this.lblDistance.Text = "Distance:";

            // 
            // lblRfid
            // 
            this.lblRfid.AutoSize = true;
            this.lblRfid.Location = new System.Drawing.Point(12, 69);
            this.lblRfid.Name = "lblRfid";
            this.lblRfid.Size = new System.Drawing.Size(31, 13);
            this.lblRfid.TabIndex = 11;
            this.lblRfid.Text = "RFID:";

            // 
            // lblBib
            // 
            this.lblBib.AutoSize = true;
            this.lblBib.Location = new System.Drawing.Point(12, 95);
            this.lblBib.Name = "lblBib";
            this.lblBib.Size = new System.Drawing.Size(27, 13);
            this.lblBib.TabIndex = 12;
            this.lblBib.Text = "Bib:";

            // 
            // lblFirstName
            // 
            this.lblFirstName.AutoSize = true;
            this.lblFirstName.Location = new System.Drawing.Point(12, 121);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(60, 13);
            this.lblFirstName.TabIndex = 13;
            this.lblFirstName.Text = "First Name:";

            // 
            // lblLastName
            // 
            this.lblLastName.AutoSize = true;
            this.lblLastName.Location = new System.Drawing.Point(12, 147);
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size(61, 13);
            this.lblLastName.TabIndex = 14;
            this.lblLastName.Text = "Last Name:";

            // 
            // lblGender
            // 
            this.lblGender.AutoSize = true;
            this.lblGender.Location = new System.Drawing.Point(12, 173);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(45, 13);
            this.lblGender.TabIndex = 15;
            this.lblGender.Text = "Gender:";

            // 
            // lblBirthday
            // 
            this.lblBirthday.AutoSize = true;
            this.lblBirthday.Location = new System.Drawing.Point(12, 203);
            this.lblBirthday.Name = "lblBirthday";
            this.lblBirthday.Size = new System.Drawing.Size(48, 13);
            this.lblBirthday.TabIndex = 16;
            this.lblBirthday.Text = "Birthday:";

            // 
            // NewParticipantsForm
            // 
            this.ClientSize = new System.Drawing.Size(334, 261);
            this.Controls.Add(this.lblBirthday);
            this.Controls.Add(this.lblGender);
            this.Controls.Add(this.lblLastName);
            this.Controls.Add(this.lblFirstName);
            this.Controls.Add(this.lblBib);
            this.Controls.Add(this.lblRfid);
            this.Controls.Add(this.lblDistance);
            this.Controls.Add(this.lblRace);
            this.Controls.Add(this.btnAddParticipant);
            this.Controls.Add(this.dtpBirthday);
            this.Controls.Add(this.cmbGender);
            this.Controls.Add(this.txtLastName);
            this.Controls.Add(this.txtFirstName);
            this.Controls.Add(this.txtBib);
            this.Controls.Add(this.txtRfid);
            this.Controls.Add(this.cmbDistances);
            this.Controls.Add(this.cmbRaces);
            this.Name = "NewParticipantsForm";
            this.Text = "Add New Participant";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
