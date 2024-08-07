using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Media;
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

        private MySqlConnection remoteConnection;
        private int selectedEventId;

        public reading()
        {
            InitializeComponent();
            connectionString = GetConnectionString("../../../../dbconfig.txt", false); // Pass false for local DB
            soundPlayer = new SoundPlayer("../../../../beep.wav");
            lastReadTimes = new Dictionary<string, DateTime>();
            remainingLaps = new Dictionary<string, int>();
            comboBoxEvents.SelectedIndexChanged += comboBoxEvents_SelectedIndexChanged; // Subscribe to event
            LoadRaces();
            LoadEvents();
        }



        private string GetConnectionString(string configFile, bool isRemote)
        {
            var lines = File.ReadAllLines(configFile);
            var connectionStringBuilder = new MySqlConnectionStringBuilder();

            foreach (var line in lines)
            {
                var parts = line.Split('=');
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim().ToLower();
                    var value = parts[1].Trim();

                    if (isRemote)
                    {
                        switch (key)
                        {
                            case "remote_server":
                                connectionStringBuilder.Server = value;
                                break;
                            case "remote_user":
                                connectionStringBuilder.UserID = value;
                                break;
                            case "remote_database":
                                connectionStringBuilder.Database = value;
                                break;
                            case "remote_port":
                                connectionStringBuilder.Port = uint.Parse(value);
                                break;
                            case "remote_password":
                                connectionStringBuilder.Password = value;
                                break;
                        }
                    }
                    else
                    {
                        switch (key)
                        {
                            case "server":
                                connectionStringBuilder.Server = value;
                                break;
                            case "user":
                                connectionStringBuilder.UserID = value;
                                break;
                            case "database":
                                connectionStringBuilder.Database = value;
                                break;
                            case "port":
                                connectionStringBuilder.Port = uint.Parse(value);
                                break;
                            case "password":
                                connectionStringBuilder.Password = value;
                                break;
                        }
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
                    var category = reader["category"] != DBNull.Value ? reader["category"].ToString() : "Unknown";

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

                    // Calculate the position for the current RFID and distance if it's the last lap
                    int? position = null;
                    if (distanceLaps == 1 || remainingLaps[rfid] == 0)
                    {
                        position = CalculatePosition(rfid, distanceName, distanceLaps);
                    }

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
                            "INSERT INTO results (rfid, timestamp, gap, first_name, last_name, gender, birthday, age, race_name, distance_name, race_date, distance_laps, distance_intervals, category, elapsed_time, position) " +
                            "VALUES (@rfid, @timestamp, @gap, @first_name, @last_name, @gender, @birthday, @age, @race_name, @distance_name, @race_date, @distance_laps, @distance_intervals, @category, @elapsed_time, @position)",
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
                        insertCmd.Parameters.AddWithValue("@category", category);
                        insertCmd.Parameters.AddWithValue("@elapsed_time", elapsedTime);
                        insertCmd.Parameters.AddWithValue("@position", position.HasValue ? (object)position.Value : DBNull.Value);
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        // Method to calculate the position based on RFID and distance name
        private int CalculatePosition(string rfid, string distanceName, int distanceLaps)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    "SELECT COUNT(DISTINCT rfid) FROM results WHERE distance_name = @distance_name AND (distance_laps = 1 OR (distance_laps > 1 AND position IS NOT NULL))",
                    conn);
                cmd.Parameters.AddWithValue("@distance_name", distanceName);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count + 1;
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
            if (cmbRaces.SelectedValue != null)
            {
                int raceId = (int)cmbRaces.SelectedValue;
                LoadDistances(raceId);
            }
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

        private void LoadEvents()
        {
            string connectionString = GetConnectionString("../../../../dbconfig.txt", true);
            try
            {
                using (remoteConnection = new MySqlConnection(connectionString))
                {
                    remoteConnection.Open();
                    string query = "SELECT id, event_name FROM events";
                    MySqlCommand cmd = new MySqlCommand(query, remoteConnection);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    // Add a new column to format the display
                    dt.Columns.Add("Display", typeof(string), "event_name + ' (' + CONVERT(id, 'System.String') + ')'");

                    comboBoxEvents.DisplayMember = "Display"; // Show event_name and id
                    comboBoxEvents.ValueMember = "id"; // Set id as the value
                    comboBoxEvents.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading events: " + ex.Message);
            }
        }





        private void comboBoxEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEvents.SelectedValue != null)
            {
                selectedEventId = (int)comboBoxEvents.SelectedValue;

                // Populate the TextBox with the selected Event ID
                textBoxEventId.Text = selectedEventId.ToString();
            }
            else
            {
                MessageBox.Show("No event selected.");
            }
        }



        private void btnTransferRecords_Click(object sender, EventArgs e)
        {
            string localConnectionString = GetConnectionString("../../../../dbconfig.txt", false);
            string remoteConnectionString = GetConnectionString("../../../../dbconfig.txt", true);

            try
            {
                // Validate the event_id input
                if (!int.TryParse(textBoxEventId.Text, out int manualEventId))
                {
                    MessageBox.Show("Please enter a valid numeric event ID.");
                    return;
                }

                using (var localConnection = new MySqlConnection(localConnectionString))
                using (var remoteConnection = new MySqlConnection(remoteConnectionString))
                {
                    localConnection.Open();
                    remoteConnection.Open();

                    string selectQuery = "SELECT * FROM results";
                    MySqlCommand selectCmd = new MySqlCommand(selectQuery, localConnection);
                    MySqlDataReader reader = selectCmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Console.WriteLine($"Debug: Preparing to insert with Event ID: {manualEventId}");

                        string insertQuery = "INSERT INTO results_zebra (id, rfid, timestamp, gap, first_name, last_name, gender, birthday, age, race_name, distance_name, race_date, distance_laps, distance_intervals, category, elapsed_time, position, event_id) " +
                                             "VALUES (@id, @rfid, @timestamp, @gap, @first_name, @last_name, @gender, @birthday, @age, @race_name, @distance_name, @race_date, @distance_laps, @distance_intervals, @category, @elapsed_time, @position, @event_id)";

                        using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, remoteConnection))
                        {
                            insertCmd.Parameters.AddWithValue("@id", reader["id"]);
                            insertCmd.Parameters.AddWithValue("@rfid", reader["rfid"]);
                            insertCmd.Parameters.AddWithValue("@timestamp", reader["timestamp"]);
                            insertCmd.Parameters.AddWithValue("@gap", reader["gap"]);
                            insertCmd.Parameters.AddWithValue("@first_name", reader["first_name"]);
                            insertCmd.Parameters.AddWithValue("@last_name", reader["last_name"]);
                            insertCmd.Parameters.AddWithValue("@gender", reader["gender"]);
                            insertCmd.Parameters.AddWithValue("@birthday", reader["birthday"]);
                            insertCmd.Parameters.AddWithValue("@age", reader["age"]);
                            insertCmd.Parameters.AddWithValue("@race_name", reader["race_name"]);
                            insertCmd.Parameters.AddWithValue("@distance_name", reader["distance_name"]);
                            insertCmd.Parameters.AddWithValue("@race_date", reader["race_date"]);
                            insertCmd.Parameters.AddWithValue("@distance_laps", reader["distance_laps"]);
                            insertCmd.Parameters.AddWithValue("@distance_intervals", reader["distance_intervals"]);
                            insertCmd.Parameters.AddWithValue("@category", reader["category"]);
                            insertCmd.Parameters.AddWithValue("@elapsed_time", reader["elapsed_time"]);
                            insertCmd.Parameters.AddWithValue("@position", reader["position"]);
                            insertCmd.Parameters.AddWithValue("@event_id", manualEventId); // Use the value from the TextBox

                            // Debug: Log the value of event_id before executing the query
                            Console.WriteLine($"Debug: Executing insert with Event ID: {manualEventId}");

                            // Execute the insert command
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Records transferred successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Error transferring records: {ex.Message}");
                MessageBox.Show($"Error transferring records: {ex.Message}");
            }
        }








    }
}
