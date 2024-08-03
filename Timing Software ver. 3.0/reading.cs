using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace RaceManager
{
    public partial class reading : Form
    {
        private string connectionString;
        private string selectedFile;
        private long lastPosition;
        private int previousRecordCount;
        private SoundPlayer soundPlayer;
        private Dictionary<string, DateTime> lastReadTimes; // Tracks last read times for each RFID
        private Dictionary<string, int> remainingLaps; // Tracks remaining laps for each RFID

        public reading()
        {
            InitializeComponent();
            connectionString = GetConnectionString("../../../../dbconfig.txt");
            soundPlayer = new SoundPlayer("../../../../beep.wav");
            lastReadTimes = new Dictionary<string, DateTime>();
            remainingLaps = new Dictionary<string, int>();
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

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                selectedFile = openFileDialog1.FileName;
                lastPosition = 0;
                previousRecordCount = GetRecordCount();
                timer1.Start();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedFile))
            {
                ReadNewLines();
                LoadResults();
            }
            UpdateElapsedTime();
        }

        private void ReadNewLines()
        {
            using (var fs = new FileStream(selectedFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                fs.Seek(lastPosition, SeekOrigin.Begin);
                using (var sr = new StreamReader(fs))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        ProcessLine(line);
                    }
                    lastPosition = fs.Position;
                }
            }
        }

        private void ProcessLine(string line)
        {
            var parts = line.Split(',');
            if (parts.Length < 3) return;

            var gap = float.Parse(parts[0]);
            var timestamp = DateTime.Parse(parts[1]);
            var rfid = parts[2];

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM runners WHERE rfid = @rfid", conn);
                cmd.Parameters.AddWithValue("@rfid", rfid);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var distanceIntervals = Convert.ToInt32(reader["distance_intervals"]);
                    var distanceLaps = Convert.ToInt32(reader["distance_laps"]);
                    var distanceId = (int)reader["distance_id"];
                    var firstName = reader["first_name"].ToString();
                    var lastName = reader["last_name"].ToString();
                    var gender = reader["gender"].ToString();
                    var birthday = (DateTime)reader["birthday"];
                    var age = (int)reader["age"];
                    var raceName = reader["race_name"].ToString();
                    var distanceName = reader["distance_name"].ToString();
                    var raceDate = (DateTime)reader["race_date"];
                    var category = reader["category"] != DBNull.Value ? reader["category"].ToString() : "Unknown"; // Get the category from the runners table

                    // Debug output to verify category retrieval
                    Console.WriteLine($"RFID: {rfid}, Category: {category}");

                    // Check if logging for specific distance is enabled and if the RFID belongs to the selected distance
                    if (chkLogSpecificDistance.Checked && cmbDistances.SelectedValue != null)
                    {
                        int selectedDistanceId = (int)cmbDistances.SelectedValue;
                        if (distanceId != selectedDistanceId)
                        {
                            reader.Close();
                            return;
                        }
                    }

                    // Check if the RFID has been read recently within the distance_intervals
                    if (lastReadTimes.ContainsKey(rfid) && (timestamp - lastReadTimes[rfid]).TotalSeconds < distanceIntervals)
                    {
                        reader.Close();
                        return;
                    }

                    // Check and update the remaining laps for the RFID
                    if (remainingLaps.ContainsKey(rfid))
                    {
                        if (remainingLaps[rfid] == 0)
                        {
                            reader.Close();
                            return;
                        }

                        remainingLaps[rfid]--;
                    }
                    else
                    {
                        remainingLaps[rfid] = distanceLaps - 1;
                    }

                    lastReadTimes[rfid] = timestamp;

                    // Close the reader before opening a new one
                    reader.Close();

                    // Calculate elapsed time
                    var startTimeCmd = new MySqlCommand("SELECT start_time FROM distances WHERE id = @id", conn);
                    startTimeCmd.Parameters.AddWithValue("@id", distanceId);
                    var result = startTimeCmd.ExecuteScalar();
                    string elapsedTime = "00:00:00:00"; // Default elapsed time if start_time is null
                    if (result != null && result != DBNull.Value)
                    {
                        DateTime startTime = (DateTime)result;
                        TimeSpan timeDiff = timestamp - startTime;
                        elapsedTime = $"{timeDiff.Days:D2}:{timeDiff.Hours:D2}:{timeDiff.Minutes:D2}:{timeDiff.Seconds:D2}";
                    }

                    using (var insertConn = new MySqlConnection(connectionString))
                    {
                        insertConn.Open();
                        var insertCmd = new MySqlCommand(
                            "INSERT INTO results (rfid, timestamp, gap, first_name, last_name, gender, birthday, age, race_name, distance_name, race_date, distance_laps, distance_intervals, category, elapsed_time) " +
                            "VALUES (@rfid, @timestamp, @gap, @first_name, @last_name, @gender, @birthday, @age, @race_name, @distance_name, @race_date, @distance_laps, @distance_intervals, @category, @elapsed_time)",
                            insertConn);
                        insertCmd.Parameters.AddWithValue("@rfid", rfid);
                        insertCmd.Parameters.AddWithValue("@timestamp", timestamp);
                        insertCmd.Parameters.AddWithValue("@gap", gap);
                        insertCmd.Parameters.AddWithValue("@first_name", firstName);
                        insertCmd.Parameters.AddWithValue("@last_name", lastName);
                        insertCmd.Parameters.AddWithValue("@gender", gender);
                        insertCmd.Parameters.AddWithValue("@birthday", birthday);
                        insertCmd.Parameters.AddWithValue("@age", age);
                        insertCmd.Parameters.AddWithValue("@race_name", raceName);
                        insertCmd.Parameters.AddWithValue("@distance_name", distanceName);
                        insertCmd.Parameters.AddWithValue("@race_date", raceDate);
                        insertCmd.Parameters.AddWithValue("@distance_laps", distanceLaps);
                        insertCmd.Parameters.AddWithValue("@distance_intervals", distanceIntervals);
                        insertCmd.Parameters.AddWithValue("@category", category); // Use the retrieved category
                        insertCmd.Parameters.AddWithValue("@elapsed_time", elapsedTime);
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
        }




        private void LoadResults()
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM results", conn);
                var adapter = new MySqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;

                // Hide the distance_id and race_id columns
                dataGridView1.Columns["distance_id"].Visible = false;
                dataGridView1.Columns["race_id"].Visible = false;

                int currentRecordCount = dt.Rows.Count;
                if (currentRecordCount > previousRecordCount)
                {
                    soundPlayer.Play();
                    previousRecordCount = currentRecordCount;
                }
            }
        }

        private int GetRecordCount()
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT COUNT(*) FROM results", conn);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void LoadRaces()
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT id, name FROM races", conn);
                var reader = cmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(reader);

                cmbRaces.DisplayMember = "name";
                cmbRaces.ValueMember = "id";
                cmbRaces.DataSource = dt;
            }
        }

        private void LoadDistances(int raceId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT id, name FROM distances WHERE race_id = @race_id", conn);
                cmd.Parameters.AddWithValue("@race_id", raceId);
                var reader = cmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(reader);

                cmbDistances.DisplayMember = "name";
                cmbDistances.ValueMember = "id";
                cmbDistances.DataSource = dt;
            }
        }

        private void cmbRaces_SelectedIndexChanged(object sender, EventArgs e)
        {
            int raceId = (int)cmbRaces.SelectedValue;
            LoadDistances(raceId);
        }

        private void cmbDistances_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateElapsedTime();
        }

        private void btnSetStartTime_Click(object sender, EventArgs e)
        {
            if (cmbDistances.SelectedValue != null)
            {
                int distanceId = (int)cmbDistances.SelectedValue;
                DateTime startTime = dtpStartTime.Value;

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("UPDATE distances SET start_time = @start_time WHERE id = @id", conn);
                    cmd.Parameters.AddWithValue("@start_time", startTime);
                    cmd.Parameters.AddWithValue("@id", distanceId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Start time updated successfully.");
                }
            }
            else
            {
                MessageBox.Show("Please select a distance.");
            }
        }

        private void btnSetCurrentTimestamp_Click(object sender, EventArgs e)
        {
            if (cmbDistances.SelectedValue != null)
            {
                int distanceId = (int)cmbDistances.SelectedValue;
                DateTime currentTimestamp = DateTime.Now;

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("UPDATE distances SET start_time = @start_time WHERE id = @id", conn);
                    cmd.Parameters.AddWithValue("@start_time", currentTimestamp);
                    cmd.Parameters.AddWithValue("@id", distanceId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Start time updated to current timestamp successfully.");
                }
            }
            else
            {
                MessageBox.Show("Please select a distance.");
            }
        }

        private void UpdateElapsedTime()
        {
            if (cmbDistances.SelectedValue != null)
            {
                int distanceId = (int)cmbDistances.SelectedValue;

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT start_time FROM distances WHERE id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", distanceId);
                    var result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        DateTime startTime = (DateTime)result;
                        TimeSpan elapsedTime = DateTime.Now - startTime;

                        lblElapsedTime.Text = $"{elapsedTime.Days:D2}:{elapsedTime.Hours:D2}:{elapsedTime.Minutes:D2}:{elapsedTime.Seconds:D2}";
                    }
                    else
                    {
                        lblElapsedTime.Text = "Not Started";
                    }
                }
            }
        }
    }
}