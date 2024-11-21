using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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


        private void button1_Click(object sender, EventArgs e)
        {
            timetrial secondForm = new timetrial();
            secondForm.Show(); // Use ShowDialog() if you want the first form to be inactive until the second one closes
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string url = "https://127.0.0.1/"; // Replace with your desired URL
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true // Ensures the default browser is used
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetConfigValue(string key, string configFilePath)
        {
            if (!File.Exists(configFilePath))
                throw new FileNotFoundException("Configuration file not found.");

            foreach (var line in File.ReadLines(configFilePath))
            {
                if (line.StartsWith(key + "=") && !line.TrimStart().StartsWith("#")) // Skip commented lines
                {
                    string value = line.Substring(key.Length + 1).Trim();
                    Console.WriteLine($"Key: {key}, Value: {value}"); // Debug output
                    return value;
                }
            }
            throw new KeyNotFoundException($"Key '{key}' not found in the configuration file.");
        }
        private void button3_Click(object sender, EventArgs e)
        {
            OpenExecutable("zebra_reader_path");
        }
        private void button4_Click(object sender, EventArgs e)
        {
            OpenExecutable("writer_path");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenExecutable("heidisql_path");
        }
        private void OpenExecutable(string configKey)
        {
            try
            {
                string configFilePath = Path.GetFullPath("../../../../dbconfig.txt");
                string relativePath = GetConfigValue(configKey, configFilePath);
                string executablePath = Path.GetFullPath(relativePath); // Resolve to absolute path

                Console.WriteLine($"Resolved path for {configKey}: {executablePath}"); // Debug log

                if (!File.Exists(executablePath))
                {
                    MessageBox.Show($"Executable not found at the specified path: {executablePath}",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Process.Start(executablePath);
            }
            catch (FileNotFoundException fnfEx)
            {
                MessageBox.Show(fnfEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (KeyNotFoundException knfEx)
            {
                MessageBox.Show(knfEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open executable: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
