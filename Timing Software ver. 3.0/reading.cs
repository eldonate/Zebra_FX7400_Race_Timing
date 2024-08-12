using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
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
            // Initialize CheckBox

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
                    var bib = reader["bib"].ToString(); // Retrieve the bib value

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

                    // Adjust laps if the CheckBox is checked
                    if (chkAddLap.Checked)
                    {
                        distanceLaps += 1;
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

                    // Calculate the position, category_position, and gender_position for the current RFID and distance if it's the last lap
                    int? position = null;
                    int? categoryPosition = null;
                    int? genderPosition = null;
                    if (distanceLaps == 1 || remainingLaps[rfid] == 0)
                    {
                        position = CalculateOverallPosition(rfid, distanceName, distanceLaps);
                        categoryPosition = CalculateCategoryPosition(rfid, category, gender, distanceName);
                        genderPosition = CalculateGenderPosition(rfid, gender, distanceName);
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
                            "INSERT INTO results (rfid, timestamp, gap, first_name, last_name, gender, birthday, age, race_name, distance_name, race_date, distance_laps, distance_intervals, category, elapsed_time, position, category_position, gender_position, bib) " + // Added bib here
                            "VALUES (@rfid, @timestamp, @gap, @first_name, @last_name, @gender, @birthday, @age, @race_name, @distance_name, @race_date, @distance_laps, @distance_intervals, @category, @elapsed_time, @position, @category_position, @gender_position, @bib)", // Added @bib here
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
                        insertCmd.Parameters.AddWithValue("@category_position", categoryPosition.HasValue ? (object)categoryPosition.Value : DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@gender_position", genderPosition.HasValue ? (object)genderPosition.Value : DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@bib", bib); // Set bib value here
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        // New method to calculate the gender position
        private int CalculateGenderPosition(string rfid, string gender, string distanceName)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    "SELECT COUNT(DISTINCT rfid) FROM results WHERE gender = @gender AND distance_name = @distance_name AND (distance_laps = 1 OR (distance_laps > 1 AND gender_position IS NOT NULL))",
                    conn);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.Parameters.AddWithValue("@distance_name", distanceName);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count + 1;
            }
        }

        // Renamed to CalculateOverallPosition
        private int CalculateOverallPosition(string rfid, string distanceName, int distanceLaps)
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

        // Renamed to CalculateCategoryPosition
        private int CalculateCategoryPosition(string rfid, string category, string gender, string distanceName)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    "SELECT COUNT(DISTINCT rfid) FROM results WHERE category = @category AND gender = @gender AND distance_name = @distance_name AND (distance_laps = 1 OR (distance_laps > 1 AND category_position IS NOT NULL))",
                    conn);
                cmd.Parameters.AddWithValue("@category", category);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.Parameters.AddWithValue("@distance_name", distanceName);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count + 1;
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



        







        private void button1_Click(object sender, EventArgs e)
        {
            if (cmbRaces.SelectedValue == null || cmbDistances.SelectedValue == null)
            {
                MessageBox.Show("Please select both a race and a distance.");
                return;
            }

            int selectedRaceId = (int)cmbRaces.SelectedValue;
            int selectedDistanceId = (int)cmbDistances.SelectedValue;
            string distanceName = string.Empty;
            DateTime? startTime = null;

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Fetch the distance_name and start_time using the selected distance_id from cmbDistances
                    var cmdGetDistanceDetails = new MySqlCommand(
                        "SELECT name, start_time FROM distances WHERE id = @distance_id", conn);
                    cmdGetDistanceDetails.Parameters.AddWithValue("@distance_id", selectedDistanceId);

                    using (var reader = cmdGetDistanceDetails.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            distanceName = reader["name"].ToString();
                            startTime = reader["start_time"] != DBNull.Value ? (DateTime?)reader["start_time"] : null;
                        }
                        else
                        {
                            MessageBox.Show("Invalid distance selected. No matching distance found.");
                            return;
                        }
                    }
                }

                if (startTime == null)
                {
                    MessageBox.Show("The selected distance does not have a start time set.");
                    return;
                }

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Fetch runners with the matching distance_name
                    var cmdGetRunners = new MySqlCommand(
                        "SELECT rfid, first_name, last_name, gender, birthday, age, race_name, distance_name, race_date, distance_laps, distance_intervals, category " +
                        "FROM runners WHERE distance_name = @distance_name", conn);
                    cmdGetRunners.Parameters.AddWithValue("@distance_name", distanceName);

                    var reader = cmdGetRunners.ExecuteReader();
                    while (reader.Read())
                    {
                        string rfid = reader["rfid"].ToString();
                        string firstName = reader["first_name"].ToString();
                        string lastName = reader["last_name"].ToString();
                        string gender = reader["gender"].ToString();
                        DateTime birthday = (DateTime)reader["birthday"];
                        int age = (int)reader["age"];
                        string raceName = reader["race_name"].ToString();
                        DateTime raceDate = (DateTime)reader["race_date"];
                        int distanceLaps = (int)reader["distance_laps"];
                        int distanceIntervals = (int)reader["distance_intervals"];
                        string category = reader["category"] != DBNull.Value ? reader["category"].ToString() : "Unknown";

                        // Calculate the elapsed time based on the start_time and the start_time itself (as the timestamp)
                        TimeSpan elapsedTimeSpan = startTime.Value - startTime.Value;
                        string elapsedTime = $"{elapsedTimeSpan.Days:D2}:{elapsedTimeSpan.Hours:D2}:{elapsedTimeSpan.Minutes:D2}:{elapsedTimeSpan.Seconds:D2}";

                        using (var insertConn = new MySqlConnection(connectionString))
                        {
                            insertConn.Open();
                            var insertCmd = new MySqlCommand(
                                "INSERT INTO results (rfid, timestamp, gap, first_name, last_name, gender, birthday, age, race_name, distance_name, race_date, distance_laps, distance_intervals, category, elapsed_time, position, category_position, gender_position) " +
                                "VALUES (@rfid, @timestamp, @gap, @first_name, @last_name, @gender, @birthday, @age, @race_name, @distance_name, @race_date, @distance_laps, @distance_intervals, @category, @elapsed_time, @position, @category_position, @gender_position)",
                                insertConn);

                            insertCmd.Parameters.AddWithValue("@rfid", rfid);
                            insertCmd.Parameters.AddWithValue("@timestamp", startTime); // Use start_time as the timestamp
                            insertCmd.Parameters.AddWithValue("@gap", 0); // Assuming a gap of 0 for the new lap
                            insertCmd.Parameters.AddWithValue("@first_name", firstName);
                            insertCmd.Parameters.AddWithValue("@last_name", lastName);
                            insertCmd.Parameters.AddWithValue("@gender", gender);
                            insertCmd.Parameters.AddWithValue("@birthday", birthday);
                            insertCmd.Parameters.AddWithValue("@age", age);
                            insertCmd.Parameters.AddWithValue("@race_name", raceName);
                            insertCmd.Parameters.AddWithValue("@distance_name", distanceName);
                            insertCmd.Parameters.AddWithValue("@race_date", raceDate);
                            insertCmd.Parameters.AddWithValue("@distance_laps", distanceLaps + 1); // Adding one lap
                            insertCmd.Parameters.AddWithValue("@distance_intervals", distanceIntervals);
                            insertCmd.Parameters.AddWithValue("@category", category);
                            insertCmd.Parameters.AddWithValue("@elapsed_time", elapsedTime); // Set the calculated elapsed time
                            insertCmd.Parameters.AddWithValue("@position", DBNull.Value); // Position is null for this entry
                            insertCmd.Parameters.AddWithValue("@category_position", DBNull.Value); // Category position is null for this entry
                            insertCmd.Parameters.AddWithValue("@gender_position", DBNull.Value); // Gender position is null for this entry

                            insertCmd.ExecuteNonQuery();
                        }
                    }
                    reader.Close();
                }
                MessageBox.Show("Lap added to all participants successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding lap to all participants: " + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string remoteConnectionString = GetConnectionString("../../../../dbconfig.txt", true); // True for remote DB

            try
            {
                using (var localConn = new MySqlConnection(connectionString)) // Use the local connection string
                {
                    localConn.Open();

                    using (var cmd = new MySqlCommand("SELECT * FROM results", localConn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            using (var remoteConn = new MySqlConnection(remoteConnectionString))
                            {
                                remoteConn.Open();

                                // Insert data into the remote "results_zebra" table
                                while (reader.Read())
                                {
                                    using (var insertCmd = new MySqlCommand(
                                        "INSERT INTO results_zebra (rfid, timestamp, gap, first_name, last_name, gender, birthday, age, race_name, distance_name, race_date, distance_laps, distance_intervals, category, elapsed_time, position, category_position, gender_position, event_id, bib) " + // Added bib here
                                        "VALUES (@rfid, @timestamp, @gap, @first_name, @last_name, @gender, @birthday, @age, @race_name, @distance_name, @race_date, @distance_laps, @distance_intervals, @category, @elapsed_time, @position, @category_position, @gender_position, @event_id, @bib)", // Added @bib here
                                        remoteConn))
                                    {
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
                                        insertCmd.Parameters.AddWithValue("@position", reader["position"] != DBNull.Value ? reader["position"] : (object)DBNull.Value);

                                        var categoryPosition = reader["category_position"] != DBNull.Value ? reader["category_position"] : null;
                                        var genderPosition = reader["gender_position"] != DBNull.Value ? reader["gender_position"] : null;

                                        insertCmd.Parameters.AddWithValue("@category_position", categoryPosition != null ? categoryPosition : (object)DBNull.Value);
                                        insertCmd.Parameters.AddWithValue("@gender_position", genderPosition != null ? genderPosition : (object)DBNull.Value);
                                        insertCmd.Parameters.AddWithValue("@event_id", Convert.ToInt32(textBoxEventId.Text)); // Add the event ID
                                        insertCmd.Parameters.AddWithValue("@bib", reader["bib"]); // Add the bib value

                                        insertCmd.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                }

                MessageBox.Show("Data transferred successfully to the remote results_zebra table.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error transferring data: " + ex.Message);
            }
        }

    }
}
