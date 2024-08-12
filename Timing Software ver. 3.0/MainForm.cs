using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Timing_Software_ver._3._0;

namespace RaceManager
{
    public partial class MainForm : Form
    {
        private string connectionString;

        public MainForm()
        {
            InitializeComponent();
            connectionString = GetConnectionString("../../../../dbconfig.txt");
            LoadRaces();
        }

        private string GetConnectionString(string configFile)
        {
            var lines = File.ReadAllLines(configFile);
            var connectionStringBuilder = new MySqlConnectionStringBuilder();

            foreach (var line in lines)
            {
                var parts = line.Split('=');
                if (parts.Length == 2)
                {
                    switch (parts[0].Trim().ToLower())
                    {
                        case "server":
                            connectionStringBuilder.Server = parts[1].Trim();
                            break;
                        case "user":
                            connectionStringBuilder.UserID = parts[1].Trim();
                            break;
                        case "database":
                            connectionStringBuilder.Database = parts[1].Trim();
                            break;
                        case "port":
                            connectionStringBuilder.Port = uint.Parse(parts[1].Trim());
                            break;
                        case "password":
                            connectionStringBuilder.Password = parts[1].Trim();
                            break;
                    }
                }
            }

            return connectionStringBuilder.ToString();
        }

        private void LoadRaces()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT id, name FROM races", conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                cmbRaces.DisplayMember = "name";
                cmbRaces.ValueMember = "id";
                cmbRaces.DataSource = dt;
            }
        }

        private void LoadDistances(int raceId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT id, name FROM distances WHERE race_id = @race_id", conn);
                cmd.Parameters.AddWithValue("@race_id", raceId);
                MySqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                cmbDistances.DisplayMember = "name";
                cmbDistances.ValueMember = "id";
                cmbDistances.DataSource = dt;
            }
        }

        private void DisplayRaceDetails(int raceId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM races WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", raceId);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtRaceDetails.Text = $"ID: {reader["id"]}\r\nName: {reader["name"]}\r\nDate: {reader["date"]}";
                }
            }
        }

        private void DisplayDistanceDetails(int distanceId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM distances WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", distanceId);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var detailsBuilder = new StringBuilder();
                    detailsBuilder.AppendLine($"ID: {reader["id"]}");
                    detailsBuilder.AppendLine($"Name: {reader["name"]}");
                    detailsBuilder.AppendLine($"Laps: {reader["laps"]}");
                    detailsBuilder.AppendLine($"Intervals: {reader["intervals"]}");
                    detailsBuilder.AppendLine($"Start RFID: {reader["start_rfid"]}");
                    detailsBuilder.AppendLine($"End RFID: {reader["end_rfid"]}");

                    reader.Close();

                    cmd = new MySqlCommand("SELECT * FROM categories WHERE distance_id = @distance_id", conn);
                    cmd.Parameters.AddWithValue("@distance_id", distanceId);
                    reader = cmd.ExecuteReader();
                    detailsBuilder.AppendLine("Categories:");
                    while (reader.Read())
                    {
                        detailsBuilder.AppendLine($"  - Name: {reader["name"]}, Start Age: {reader["start_age"]}, End Age: {reader["end_age"]}");
                    }

                    txtDistanceDetails.Text = detailsBuilder.ToString();
                }
            }
        }

        private void btnCreateRace_Click(object sender, EventArgs e)
        {
            string raceName = txtRaceName.Text;
            DateTime raceDate = dtpRaceDate.Value;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO races (name, date) VALUES (@name, @date)", conn);
                cmd.Parameters.AddWithValue("@name", raceName);
                cmd.Parameters.AddWithValue("@date", raceDate);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Race created successfully!");
                LoadRaces();
            }
        }

        private void btnAddDistance_Click(object sender, EventArgs e)
        {
            int raceId = (int)cmbRaces.SelectedValue;
            string distanceName = txtDistanceName.Text;
            int laps = int.Parse(txtLaps.Text);
            int intervals = int.Parse(txtIntervals.Text);
            string startRfid = txtStartRfid.Text;
            string endRfid = txtEndRfid.Text;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO distances (race_id, name, laps, intervals, start_rfid, end_rfid) VALUES (@race_id, @name, @laps, @intervals, @start_rfid, @end_rfid)", conn);
                cmd.Parameters.AddWithValue("@race_id", raceId);
                cmd.Parameters.AddWithValue("@name", distanceName);
                cmd.Parameters.AddWithValue("@laps", laps);
                cmd.Parameters.AddWithValue("@intervals", intervals);
                cmd.Parameters.AddWithValue("@start_rfid", startRfid);
                cmd.Parameters.AddWithValue("@end_rfid", endRfid);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Distance added successfully!");
                LoadDistances(raceId);
            }
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            int distanceId = (int)cmbDistances.SelectedValue;
            string categoryName = txtCategoryName.Text;
            int startAge = int.Parse(txtStartAge.Text);
            int endAge = int.Parse(txtEndAge.Text);

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO categories (distance_id, name, start_age, end_age) VALUES (@distance_id, @name, @start_age, @end_age)", conn);
                cmd.Parameters.AddWithValue("@distance_id", distanceId);
                cmd.Parameters.AddWithValue("@name", categoryName);
                cmd.Parameters.AddWithValue("@start_age", startAge);
                cmd.Parameters.AddWithValue("@end_age", endAge);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Category added successfully!");
                DisplayDistanceDetails(distanceId);
            }
        }

        private void cmbRaces_SelectedIndexChanged(object sender, EventArgs e)
        {
            int raceId = (int)cmbRaces.SelectedValue;
            DisplayRaceDetails(raceId);
            LoadDistances(raceId);
        }

        private void cmbDistances_SelectedIndexChanged(object sender, EventArgs e)
        {
            int distanceId = (int)cmbDistances.SelectedValue;
            DisplayDistanceDetails(distanceId);
        }

        private void btnAddParticipant_Click(object sender, EventArgs e)
        {
            if (cmbRaces.SelectedValue != null && cmbDistances.SelectedValue != null)
            {
                int raceId = (int)cmbRaces.SelectedValue;
                int distanceId = (int)cmbDistances.SelectedValue;
                string raceName = cmbRaces.Text;
                string distanceName = cmbDistances.Text;
                DateTime raceDate = GetRaceDate(raceId);

                ParticipantForm participantForm = new ParticipantForm(raceId, distanceId, raceDate, raceName, distanceName);
                participantForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a race and a distance.");
            }
        }

        private void btnOpenReadingForm_Click(object sender, EventArgs e)
        {
            reading readingForm = new reading();
            readingForm.ShowDialog();
        }

        private void btnNewParticipants_Click(object sender, EventArgs e)
        {
            NewParticipantsForm newParticipantsForm = new NewParticipantsForm();
            newParticipantsForm.ShowDialog();
        }

        private void btnNewForm_Click(object sender, EventArgs e)
        {
            reading readingForm = new reading();
            readingForm.ShowDialog();
        }

        private DateTime GetRaceDate(int raceId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT date FROM races WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", raceId);
                return (DateTime)cmd.ExecuteScalar();
            }
        }

        private void addSplit_Click(object sender, EventArgs e)
        {
            // Ensure the race and distance are selected and split names are entered
            if (cmbRaces.SelectedValue == null || cmbDistances.SelectedValue == null || string.IsNullOrEmpty(txtSplitNames.Text))
            {
                MessageBox.Show("Please select a race, a distance, and enter the split names.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get race_name and distance_name from the ComboBoxes
            string raceName = ((DataRowView)cmbRaces.SelectedItem)["name"].ToString().Trim();
            string distanceName = ((DataRowView)cmbDistances.SelectedItem)["name"].ToString().Trim();
            string newSplitNames = txtSplitNames.Text.Trim();

            // Specify the delimiter
            string delimiter = "|";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Check if there is an existing record for the given race_name and distance_name
                MySqlCommand cmdCheck = new MySqlCommand(
                    "SELECT split_names FROM race_splits WHERE race_name = @raceName AND distance_name = @distanceName", conn);
                cmdCheck.Parameters.AddWithValue("@raceName", raceName);
                cmdCheck.Parameters.AddWithValue("@distanceName", distanceName);

                object existingSplitsObj = cmdCheck.ExecuteScalar();
                string updatedSplits;

                if (existingSplitsObj != null)
                {
                    // If the record exists, append the new split names to the existing ones
                    string existingSplits = existingSplitsObj.ToString();
                    updatedSplits = existingSplits + delimiter + newSplitNames;

                    // Update the existing record with the new split names
                    MySqlCommand cmdUpdate = new MySqlCommand(
                        "UPDATE race_splits SET split_names = @splitNames WHERE race_name = @raceName AND distance_name = @distanceName", conn);
                    cmdUpdate.Parameters.AddWithValue("@splitNames", updatedSplits);
                    cmdUpdate.Parameters.AddWithValue("@raceName", raceName);
                    cmdUpdate.Parameters.AddWithValue("@distanceName", distanceName);

                    cmdUpdate.ExecuteNonQuery();
                    MessageBox.Show("Split names successfully updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // If the record does not exist, insert a new record
                    MySqlCommand cmdInsert = new MySqlCommand(
                        "INSERT INTO race_splits (race_name, distance_name, split_names) " +
                        "VALUES (@raceName, @distanceName, @splitNames)", conn);
                    cmdInsert.Parameters.AddWithValue("@raceName", raceName);
                    cmdInsert.Parameters.AddWithValue("@distanceName", distanceName);
                    cmdInsert.Parameters.AddWithValue("@splitNames", newSplitNames);

                    cmdInsert.ExecuteNonQuery();
                    MessageBox.Show("Split names successfully saved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
