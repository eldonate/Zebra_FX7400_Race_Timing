namespace RaceManager
{
    partial class NewParticipantsForm
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
            this.btnImportParticipants = new System.Windows.Forms.Button();
            this.lblRfid = new System.Windows.Forms.Label();
            this.lblBib = new System.Windows.Forms.Label();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.lblLastName = new System.Windows.Forms.Label();
            this.lblGender = new System.Windows.Forms.Label();
            this.lblBirthday = new System.Windows.Forms.Label();
            this.lblRaces = new System.Windows.Forms.Label();
            this.lblDistances = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTeam = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cmbRaces
            // 
            this.cmbRaces.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRaces.FormattingEnabled = true;
            this.cmbRaces.Location = new System.Drawing.Point(100, 20);
            this.cmbRaces.Name = "cmbRaces";
            this.cmbRaces.Size = new System.Drawing.Size(200, 21);
            this.cmbRaces.TabIndex = 0;
            this.cmbRaces.SelectedIndexChanged += new System.EventHandler(this.cmbRaces_SelectedIndexChanged);
            // 
            // cmbDistances
            // 
            this.cmbDistances.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDistances.FormattingEnabled = true;
            this.cmbDistances.Location = new System.Drawing.Point(100, 50);
            this.cmbDistances.Name = "cmbDistances";
            this.cmbDistances.Size = new System.Drawing.Size(200, 21);
            this.cmbDistances.TabIndex = 1;
            this.cmbDistances.SelectedIndexChanged += new System.EventHandler(this.cmbDistances_SelectedIndexChanged);
            // 
            // txtRfid
            // 
            this.txtRfid.Location = new System.Drawing.Point(100, 80);
            this.txtRfid.Name = "txtRfid";
            this.txtRfid.Size = new System.Drawing.Size(200, 20);
            this.txtRfid.TabIndex = 2;
            // 
            // txtBib
            // 
            this.txtBib.Location = new System.Drawing.Point(100, 110);
            this.txtBib.Name = "txtBib";
            this.txtBib.Size = new System.Drawing.Size(200, 20);
            this.txtBib.TabIndex = 3;
            // 
            // txtFirstName
            // 
            this.txtFirstName.Location = new System.Drawing.Point(100, 140);
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(200, 20);
            this.txtFirstName.TabIndex = 4;
            // 
            // txtLastName
            // 
            this.txtLastName.Location = new System.Drawing.Point(100, 170);
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(200, 20);
            this.txtLastName.TabIndex = 5;
            // 
            // cmbGender
            // 
            this.cmbGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGender.FormattingEnabled = true;
            this.cmbGender.Items.AddRange(new object[] {
            "Male",
            "Female"});
            this.cmbGender.Location = new System.Drawing.Point(100, 230);
            this.cmbGender.Name = "cmbGender";
            this.cmbGender.Size = new System.Drawing.Size(200, 21);
            this.cmbGender.TabIndex = 6;
            // 
            // dtpBirthday
            // 
            this.dtpBirthday.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpBirthday.Location = new System.Drawing.Point(100, 260);
            this.dtpBirthday.Name = "dtpBirthday";
            this.dtpBirthday.Size = new System.Drawing.Size(200, 20);
            this.dtpBirthday.TabIndex = 7;
            // 
            // btnAddParticipant
            // 
            this.btnAddParticipant.Location = new System.Drawing.Point(100, 290);
            this.btnAddParticipant.Name = "btnAddParticipant";
            this.btnAddParticipant.Size = new System.Drawing.Size(200, 23);
            this.btnAddParticipant.TabIndex = 8;
            this.btnAddParticipant.Text = "Add Participant";
            this.btnAddParticipant.UseVisualStyleBackColor = true;
            this.btnAddParticipant.Click += new System.EventHandler(this.btnAddParticipant_Click);
            // 
            // btnImportParticipants
            // 
            this.btnImportParticipants.Location = new System.Drawing.Point(100, 320);
            this.btnImportParticipants.Name = "btnImportParticipants";
            this.btnImportParticipants.Size = new System.Drawing.Size(200, 23);
            this.btnImportParticipants.TabIndex = 9;
            this.btnImportParticipants.Text = "Import Participants";
            this.btnImportParticipants.UseVisualStyleBackColor = true;
            this.btnImportParticipants.Click += new System.EventHandler(this.btnImportParticipants_Click);
            // 
            // lblRfid
            // 
            this.lblRfid.AutoSize = true;
            this.lblRfid.Location = new System.Drawing.Point(20, 83);
            this.lblRfid.Name = "lblRfid";
            this.lblRfid.Size = new System.Drawing.Size(32, 13);
            this.lblRfid.TabIndex = 10;
            this.lblRfid.Text = "RFID";
            // 
            // lblBib
            // 
            this.lblBib.AutoSize = true;
            this.lblBib.Location = new System.Drawing.Point(20, 113);
            this.lblBib.Name = "lblBib";
            this.lblBib.Size = new System.Drawing.Size(24, 13);
            this.lblBib.TabIndex = 11;
            this.lblBib.Text = "BIB";
            // 
            // lblFirstName
            // 
            this.lblFirstName.AutoSize = true;
            this.lblFirstName.Location = new System.Drawing.Point(20, 143);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(57, 13);
            this.lblFirstName.TabIndex = 12;
            this.lblFirstName.Text = "First Name";
            // 
            // lblLastName
            // 
            this.lblLastName.AutoSize = true;
            this.lblLastName.Location = new System.Drawing.Point(20, 173);
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size(58, 13);
            this.lblLastName.TabIndex = 13;
            this.lblLastName.Text = "Last Name";
            // 
            // lblGender
            // 
            this.lblGender.AutoSize = true;
            this.lblGender.Location = new System.Drawing.Point(20, 233);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(42, 13);
            this.lblGender.TabIndex = 14;
            this.lblGender.Text = "Gender";
            // 
            // lblBirthday
            // 
            this.lblBirthday.AutoSize = true;
            this.lblBirthday.Location = new System.Drawing.Point(20, 263);
            this.lblBirthday.Name = "lblBirthday";
            this.lblBirthday.Size = new System.Drawing.Size(45, 13);
            this.lblBirthday.TabIndex = 15;
            this.lblBirthday.Text = "Birthday";
            // 
            // lblRaces
            // 
            this.lblRaces.AutoSize = true;
            this.lblRaces.Location = new System.Drawing.Point(20, 23);
            this.lblRaces.Name = "lblRaces";
            this.lblRaces.Size = new System.Drawing.Size(38, 13);
            this.lblRaces.TabIndex = 16;
            this.lblRaces.Text = "Races";
            // 
            // lblDistances
            // 
            this.lblDistances.AutoSize = true;
            this.lblDistances.Location = new System.Drawing.Point(20, 53);
            this.lblDistances.Name = "lblDistances";
            this.lblDistances.Size = new System.Drawing.Size(54, 13);
            this.lblDistances.TabIndex = 17;
            this.lblDistances.Text = "Distances";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 204);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Team";
            // 
            // txtTeam
            // 
            this.txtTeam.Location = new System.Drawing.Point(100, 201);
            this.txtTeam.Name = "txtTeam";
            this.txtTeam.Size = new System.Drawing.Size(200, 20);
            this.txtTeam.TabIndex = 19;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 363);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(260, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "RFID,Bib,FirstName,LastName,Gender,Birthday,Team";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 385);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(346, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "The last line has an empty team field, which should default to \"Individual";
            // 
            // NewParticipantsForm
            // 
            this.ClientSize = new System.Drawing.Size(356, 435);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtTeam);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblDistances);
            this.Controls.Add(this.lblRaces);
            this.Controls.Add(this.lblBirthday);
            this.Controls.Add(this.lblGender);
            this.Controls.Add(this.lblLastName);
            this.Controls.Add(this.lblFirstName);
            this.Controls.Add(this.lblBib);
            this.Controls.Add(this.lblRfid);
            this.Controls.Add(this.btnImportParticipants);
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
            this.Text = "New Participants";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.ComboBox cmbRaces;
        private System.Windows.Forms.ComboBox cmbDistances;
        private System.Windows.Forms.TextBox txtRfid;
        private System.Windows.Forms.TextBox txtBib;
        private System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.TextBox txtLastName;
        private System.Windows.Forms.ComboBox cmbGender;
        private System.Windows.Forms.DateTimePicker dtpBirthday;
        private System.Windows.Forms.Button btnAddParticipant;
        private System.Windows.Forms.Button btnImportParticipants;
        private System.Windows.Forms.Label lblRfid;
        private System.Windows.Forms.Label lblBib;
        private System.Windows.Forms.Label lblFirstName;
        private System.Windows.Forms.Label lblLastName;
        private System.Windows.Forms.Label lblGender;
        private System.Windows.Forms.Label lblBirthday;
        private System.Windows.Forms.Label lblRaces;
        private System.Windows.Forms.Label lblDistances;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTeam;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}
