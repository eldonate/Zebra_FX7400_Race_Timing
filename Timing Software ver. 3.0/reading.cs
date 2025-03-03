using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Media;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Collections.Concurrent;
using System.Threading;

namespace RaceManager
{
    public partial class reading : Form
    {
        // Connection, file, and sound
        private string connectionString;
        private string selectedFile;
        private long lastPosition; // (CS0414: if you don't use it further, you can suppress this warning)
        private int previousRecordCount;
        private SoundPlayer soundPlayer;

        // In-memory state for interval checks and caching
        private Dictionary<string, DateTime> lastReadTimes;      // Last read timestamp per RFID
        private Dictionary<string, int> remainingLaps;           // Remaining laps per RFID
        private Dictionary<string, RunnerInfo> runnerCache;      // Runner details cached from DB (keyed by RFID)
        private Dictionary<int, DateTime> distanceStartTimeCache;  // Cache for distances' start times

        // Batch processing using a thread-safe queue
        private ConcurrentQueue<ResultRecord> recordQueue;
        private System.Threading.Timer batchTimer;  // Flushes the queue every 1 second

        // Remote upload timer (for auto push)
        private System.Windows.Forms.Timer uploadTimer;

        // Other fields
        private MySqlConnection remoteConnection;
        private int selectedEventId;

        public reading()
        {
            InitializeComponent();

            // Build connection string (with pooling enabled)
            connectionString = GetConnectionString("../../../../dbconfig.txt", false);
            soundPlayer = new SoundPlayer("../../../../beep.wav");

            // Initialize in-memory dictionaries and queue.
            lastReadTimes = new Dictionary<string, DateTime>();
            remainingLaps = new Dictionary<string, int>();
            runnerCache = new Dictionary<string, RunnerInfo>();
            distanceStartTimeCache = new Dictionary<int, DateTime>();
            recordQueue = new ConcurrentQueue<ResultRecord>();

            // Preload runner data from the DB into memory.
            LoadRunnerCache();

            // Subscribe to UI events.
            comboBoxEvents.SelectedIndexChanged += comboBoxEvents_SelectedIndexChanged;
            cmbRaces.SelectedIndexChanged += cmbRaces_SelectedIndexChanged;
            cmbDistances.SelectedIndexChanged += cmbDistances_SelectedIndexChanged;
            LoadRaces();
            LoadEvents();

            // Hide heavy UI elements.
            dataGridView1.Visible = false;

            // Start file reading timer.
            timer1.Start();

            // Start batch timer – flushes every 1 second.
            batchTimer = new System.Threading.Timer(BatchInsertCallback, null, 1000, 1000);

            // Initialize remote upload timer (UI timer) for auto-upload.
            uploadTimer = new System.Windows.Forms.Timer();
            uploadTimer.Tick += UploadTimer_Tick;
        }

        #region Connection and Caching

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
            // Ensure pooling is enabled.
            string connStr = connectionStringBuilder.ToString();
            if (!connStr.ToLower().Contains("pooling"))
                connStr += ";Pooling=true;";
            return connStr;
        }

        private void LoadRunnerCache()
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand(
                        "SELECT rfid, distance_intervals, distance_laps, distance_id, first_name, last_name, gender, birthday, age, race_name, distance_name, race_date, category, bib, team FROM runners",
                        conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string rfid = reader["rfid"].ToString();
                            RunnerInfo info = new RunnerInfo
                            {
                                DistanceIntervals = Convert.ToInt32(reader["distance_intervals"]),
                                DistanceLaps = Convert.ToInt32(reader["distance_laps"]),
                                DistanceId = Convert.ToInt32(reader["distance_id"]),
                                FirstName = reader["first_name"].ToString(),
                                LastName = reader["last_name"].ToString(),
                                Gender = reader["gender"].ToString(),
                                Birthday = (DateTime)reader["birthday"],
                                Age = Convert.ToInt32(reader["age"]),
                                RaceName = reader["race_name"].ToString(),
                                DistanceName = reader["distance_name"].ToString(),
                                RaceDate = (DateTime)reader["race_date"],
                                Category = reader["category"] != DBNull.Value ? reader["category"].ToString() : "Unknown",
                                Bib = reader["bib"].ToString(),
                                Team = reader["team"] != DBNull.Value ? reader["team"].ToString() : "Individual"
                            };
                            runnerCache[rfid] = info;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading runner cache: " + ex.Message);
            }
        }

        #endregion

        #region File Reading and Record Processing

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
                UpdateRunnerCounts();
            }
            UpdateElapsedTime();
        }

        private void ReadNewLines()
        {
            using (var fs = new FileStream(selectedFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                // Check if the file was truncated
                if (fs.Length < lastPosition)
                {
                    lastPosition = 0;
                }
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
            try
            {
                var parts = line.Split(',');
                if (parts.Length < 3)
                    return;

                if (!float.TryParse(parts[0], out var gap))
                {
                    NotifyUser($"Invalid gap value: {parts[0]}");
                    return;
                }
                if (!DateTime.TryParse(parts[1], out var timestamp))
                {
                    NotifyUser($"Invalid timestamp value: {parts[1]}");
                    return;
                }
                string rfid = parts[2];

                double interval = GetIntervalForRFID(rfid);
                if (lastReadTimes.ContainsKey(rfid) && (timestamp - lastReadTimes[rfid]).TotalSeconds < interval)
                    return;
                lastReadTimes[rfid] = timestamp;

                if (!runnerCache.TryGetValue(rfid, out RunnerInfo runner))
                    return;

                if (chkLogSpecificDistance.Checked && cmbDistances.SelectedValue != null)
                {
                    int selectedDistanceId = (int)cmbDistances.SelectedValue;
                    if (runner.DistanceId != selectedDistanceId)
                        return;
                }

                int distanceIntervals = runner.DistanceIntervals;
                int distanceLaps = runner.DistanceLaps;
                if (chkAddLap.Checked)
                    distanceLaps += 1;

                if (remainingLaps.ContainsKey(rfid))
                {
                    if (remainingLaps[rfid] == 0)
                        return;
                    remainingLaps[rfid]--;
                }
                else
                {
                    remainingLaps[rfid] = distanceLaps - 1;
                }

                // Mark as finish if first lap or no remaining laps.
                bool isFinish = (distanceLaps == 1 || remainingLaps[rfid] == 0);

                int? position = null;
                int? categoryPosition = null;
                int? genderPosition = null;

                string elapsedTime = GetElapsedTime(runner.DistanceId, timestamp);

                ResultRecord record = new ResultRecord
                {
                    RFID = rfid,
                    Timestamp = timestamp,
                    Gap = gap,
                    FirstName = runner.FirstName,
                    LastName = runner.LastName,
                    Gender = runner.Gender,
                    Birthday = runner.Birthday,
                    Age = runner.Age,
                    RaceName = runner.RaceName,
                    DistanceName = runner.DistanceName,
                    RaceDate = runner.RaceDate,
                    DistanceLaps = distanceLaps,
                    DistanceIntervals = runner.DistanceIntervals,
                    Category = runner.Category,
                    ElapsedTime = elapsedTime,
                    Position = position,
                    CategoryPosition = categoryPosition,
                    GenderPosition = genderPosition,
                    Bib = runner.Bib,
                    Team = runner.Team,
                    IsFinish = isFinish
                };

                recordQueue.Enqueue(record);
            }
            catch (Exception ex)
            {
                NotifyUser($"Unexpected error: {ex.Message}");
            }
        }

        private double GetIntervalForRFID(string rfid)
        {
            if (runnerCache.TryGetValue(rfid, out RunnerInfo runner))
                return runner.DistanceIntervals;
            return 0;
        }

        private string GetElapsedTime(int distanceId, DateTime timestamp)
        {
            DateTime startTime;
            if (!distanceStartTimeCache.TryGetValue(distanceId, out startTime))
            {
                try
                {
                    using (var conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        var cmd = new MySqlCommand("SELECT start_time FROM distances WHERE id = @id", conn);
                        cmd.Parameters.AddWithValue("@id", distanceId);
                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            startTime = (DateTime)result;
                            distanceStartTimeCache[distanceId] = startTime;
                        }
                        else
                        {
                            return "00:00:00:00";
                        }
                    }
                }
                catch
                {
                    return "00:00:00:00";
                }
            }
            TimeSpan diff = timestamp - startTime;
            return $"{diff.Days:D2}:{diff.Hours:D2}:{diff.Minutes:D2}:{diff.Seconds:D2}";
        }

        #endregion

        #region Batch Insertion and Position Calculation

        private void BatchInsertCallback(object state)
        {
            List<ResultRecord> batch = new List<ResultRecord>();
            while (recordQueue.TryDequeue(out ResultRecord rec))
            {
                batch.Add(rec);
            }
            if (batch.Count == 0)
                return;

            var finishRecords = batch.Where(r => r.IsFinish).ToList();
            if (finishRecords.Any())
            {
                // Overall positions grouped by DistanceName.
                var overallGroups = finishRecords.GroupBy(r => r.DistanceName);
                foreach (var group in overallGroups)
                {
                    int baseOverall = GetOverallBaseCount(group.Key);
                    var ordered = group.OrderBy(r => r.Timestamp).ToList();
                    for (int i = 0; i < ordered.Count; i++)
                        ordered[i].Position = baseOverall + i + 1;
                }
                // Category positions grouped by DistanceName, Category, and Gender.
                var categoryGroups = finishRecords.GroupBy(r => new { r.DistanceName, r.Category, r.Gender });
                foreach (var group in categoryGroups)
                {
                    int baseCategory = GetCategoryBaseCount(group.Key.DistanceName, group.Key.Category, group.Key.Gender);
                    var ordered = group.OrderBy(r => r.Timestamp).ToList();
                    for (int i = 0; i < ordered.Count; i++)
                        ordered[i].CategoryPosition = baseCategory + i + 1;
                }
                // Gender positions grouped by DistanceName and Gender.
                var genderGroups = finishRecords.GroupBy(r => new { r.DistanceName, r.Gender });
                foreach (var group in genderGroups)
                {
                    int baseGender = GetGenderBaseCount(group.Key.DistanceName, group.Key.Gender);
                    var ordered = group.OrderBy(r => r.Timestamp).ToList();
                    for (int i = 0; i < ordered.Count; i++)
                        ordered[i].GenderPosition = baseGender + i + 1;
                }
            }

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach (var rec in batch)
                        {
                            var insertCmd = new MySqlCommand(
                                "INSERT INTO results (rfid, timestamp, gap, first_name, last_name, gender, birthday, age, race_name, distance_name, race_date, distance_laps, distance_intervals, category, elapsed_time, position, category_position, gender_position, bib, team) " +
                                "VALUES (@rfid, @timestamp, @gap, @first_name, @last_name, @gender, @birthday, @age, @race_name, @distance_name, @race_date, @distance_laps, @distance_intervals, @category, @elapsed_time, @position, @category_position, @gender_position, @bib, @team)", conn, transaction);

                            insertCmd.Parameters.AddWithValue("@rfid", rec.RFID);
                            insertCmd.Parameters.AddWithValue("@timestamp", rec.Timestamp);
                            insertCmd.Parameters.AddWithValue("@gap", rec.Gap);
                            insertCmd.Parameters.AddWithValue("@first_name", rec.FirstName);
                            insertCmd.Parameters.AddWithValue("@last_name", rec.LastName);
                            insertCmd.Parameters.AddWithValue("@gender", rec.Gender);
                            insertCmd.Parameters.AddWithValue("@birthday", rec.Birthday);
                            insertCmd.Parameters.AddWithValue("@age", rec.Age);
                            insertCmd.Parameters.AddWithValue("@race_name", rec.RaceName);
                            insertCmd.Parameters.AddWithValue("@distance_name", rec.DistanceName);
                            insertCmd.Parameters.AddWithValue("@race_date", rec.RaceDate);
                            insertCmd.Parameters.AddWithValue("@distance_laps", rec.DistanceLaps);
                            insertCmd.Parameters.AddWithValue("@distance_intervals", rec.DistanceIntervals);
                            insertCmd.Parameters.AddWithValue("@category", rec.Category);
                            insertCmd.Parameters.AddWithValue("@elapsed_time", rec.ElapsedTime);
                            insertCmd.Parameters.AddWithValue("@position", rec.Position.HasValue ? (object)rec.Position.Value : DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@category_position", rec.CategoryPosition.HasValue ? (object)rec.CategoryPosition.Value : DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@gender_position", rec.GenderPosition.HasValue ? (object)rec.GenderPosition.Value : DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@bib", rec.Bib);
                            insertCmd.Parameters.AddWithValue("@team", rec.Team);
                            insertCmd.ExecuteNonQuery();

                            // Play beep for each inserted record.
                            soundPlayer.Play();
                        }
                        transaction.Commit();
                    }
                }
            }
            catch (Exception)
            {
                // Optionally log error.
            }
        }

        private int GetOverallBaseCount(string distanceName)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    "SELECT COUNT(DISTINCT rfid) FROM results WHERE distance_name = @distance_name AND (distance_laps = 1 OR (distance_laps > 1 AND position IS NOT NULL))",
                    conn);
                cmd.Parameters.AddWithValue("@distance_name", distanceName);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private int GetCategoryBaseCount(string distanceName, string category, string gender)
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
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private int GetGenderBaseCount(string distanceName, string gender)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    "SELECT COUNT(DISTINCT rfid) FROM results WHERE gender = @gender AND distance_name = @distance_name AND (distance_laps = 1 OR (distance_laps > 1 AND gender_position IS NOT NULL))",
                    conn);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.Parameters.AddWithValue("@distance_name", distanceName);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // Single definition of GetRecordCount.
        private int GetRecordCount()
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT COUNT(*) FROM results", conn);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        #endregion

        #region UI Updates and Additional Button Handlers

        private void UpdateRunnerCounts()
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmdStarted = new MySqlCommand("SELECT COUNT(DISTINCT rfid) FROM results", conn);
                    int runnersStarted = Convert.ToInt32(cmdStarted.ExecuteScalar());
                    var cmdFinished = new MySqlCommand("SELECT COUNT(DISTINCT rfid) FROM results WHERE position IS NOT NULL", conn);
                    int runnersFinished = Convert.ToInt32(cmdFinished.ExecuteScalar());
                    lblRunnersStarted.Text = $"Runners Started: {runnersStarted}";
                    lblRunnersFinished.Text = $"Runners Finished: {runnersFinished}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating runner counts: " + ex.Message);
            }
        }

        private void UpdateElapsedTime()
        {
            if (cmbDistances.SelectedValue != null)
            {
                int distanceId = (int)cmbDistances.SelectedValue;
                try
                {
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
                catch { }
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
                try
                {
                    using (var conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        var cmd = new MySqlCommand("UPDATE distances SET start_time = @start_time WHERE id = @id", conn);
                        cmd.Parameters.AddWithValue("@start_time", startTime);
                        cmd.Parameters.AddWithValue("@id", distanceId);
                        cmd.ExecuteNonQuery();
                        // Refresh the cache so new elapsed times are computed correctly.
                        distanceStartTimeCache[distanceId] = startTime;
                        MessageBox.Show("Start time updated successfully.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
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
                try
                {
                    using (var conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        var cmd = new MySqlCommand("UPDATE distances SET start_time = @start_time WHERE id = @id", conn);
                        cmd.Parameters.AddWithValue("@start_time", currentTimestamp);
                        cmd.Parameters.AddWithValue("@id", distanceId);
                        cmd.ExecuteNonQuery();
                        distanceStartTimeCache[distanceId] = currentTimestamp;
                        MessageBox.Show("Start time updated to current timestamp successfully.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please select a distance.");
            }
        }

        // button1_Click: Add zero starting times to all participants.
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
                    var cmdGetDistanceDetails = new MySqlCommand("SELECT name, start_time FROM distances WHERE id = @distance_id", conn);
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
                    var cmdGetRunners = new MySqlCommand("SELECT rfid, first_name, last_name, gender, birthday, age, race_name, distance_name, race_date, distance_laps, distance_intervals, category, bib, team FROM runners WHERE distance_name = @distance_name", conn);
                    cmdGetRunners.Parameters.AddWithValue("@distance_name", distanceName);
                    using (var reader = cmdGetRunners.ExecuteReader())
                    {
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
                            string bib = reader["bib"] != DBNull.Value ? reader["bib"].ToString() : string.Empty;
                            string team = reader["team"] != DBNull.Value ? reader["team"].ToString() : "Individual";
                            string elapsedTime = "00:00:00:00"; // Placeholder

                            using (var insertConn = new MySqlConnection(connectionString))
                            {
                                insertConn.Open();
                                var insertCmd = new MySqlCommand(
                                    "INSERT INTO results (rfid, timestamp, gap, first_name, last_name, gender, birthday, age, race_name, distance_name, race_date, distance_laps, distance_intervals, category, elapsed_time, position, category_position, gender_position, bib, team) " +
                                    "VALUES (@rfid, @timestamp, @gap, @first_name, @last_name, @gender, @birthday, @age, @race_name, @distance_name, @race_date, @distance_laps, @distance_intervals, @category, @elapsed_time, @position, @category_position, @gender_position, @bib, @team)", insertConn);
                                insertCmd.Parameters.AddWithValue("@rfid", rfid);
                                insertCmd.Parameters.AddWithValue("@timestamp", startTime);
                                insertCmd.Parameters.AddWithValue("@gap", 0);
                                insertCmd.Parameters.AddWithValue("@first_name", firstName);
                                insertCmd.Parameters.AddWithValue("@last_name", lastName);
                                insertCmd.Parameters.AddWithValue("@gender", gender);
                                insertCmd.Parameters.AddWithValue("@birthday", birthday);
                                insertCmd.Parameters.AddWithValue("@age", age);
                                insertCmd.Parameters.AddWithValue("@race_name", raceName);
                                insertCmd.Parameters.AddWithValue("@distance_name", distanceName);
                                insertCmd.Parameters.AddWithValue("@race_date", raceDate);
                                insertCmd.Parameters.AddWithValue("@distance_laps", distanceLaps + 1);
                                insertCmd.Parameters.AddWithValue("@distance_intervals", distanceIntervals);
                                insertCmd.Parameters.AddWithValue("@category", category);
                                insertCmd.Parameters.AddWithValue("@elapsed_time", elapsedTime);
                                insertCmd.Parameters.AddWithValue("@position", DBNull.Value);
                                insertCmd.Parameters.AddWithValue("@category_position", DBNull.Value);
                                insertCmd.Parameters.AddWithValue("@gender_position", DBNull.Value);
                                insertCmd.Parameters.AddWithValue("@bib", bib);
                                insertCmd.Parameters.AddWithValue("@team", team);
                                insertCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                MessageBox.Show("Lap added to all participants successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding lap to all participants: " + ex.Message);
            }
        }

        // button3_Click: Manual push to remote.
        private void button3_Click(object sender, EventArgs e)
        {
            Task.Run(() => PushDataToRemote());
        }

        // Optimized: Pushes data from the local results table to the remote DB.
        private void PushDataToRemote()
        {
            string remoteConnectionString = GetConnectionString("../../../../dbconfig.txt", true);
            try
            {
                using (var localConn = new MySqlConnection(connectionString))
                {
                    localConn.Open();

                    // Update split_name if needed.
                    string readerPosition = ReaderPosition.Text;
                    if (!string.IsNullOrEmpty(readerPosition))
                    {
                        using (var updateCmd = new MySqlCommand(
                            "UPDATE results SET split_name = @split_name WHERE split_name IS NULL OR split_name = ''", localConn))
                        {
                            updateCmd.Parameters.AddWithValue("@split_name", readerPosition);
                            updateCmd.ExecuteNonQuery();
                        }
                    }

                    // Load all local records.
                    List<LocalResult> localRecords = new List<LocalResult>();
                    using (var cmd = new MySqlCommand("SELECT * FROM results", localConn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LocalResult lr = new LocalResult
                                {
                                    Rfid = reader["rfid"].ToString(),
                                    Timestamp = (DateTime)reader["timestamp"],
                                    Gap = reader["gap"] != DBNull.Value ? Convert.ToSingle(reader["gap"]) : 0f,
                                    FirstName = reader["first_name"].ToString(),
                                    LastName = reader["last_name"].ToString(),
                                    Gender = reader["gender"].ToString(),
                                    Birthday = (DateTime)reader["birthday"],
                                    Age = reader["age"] != DBNull.Value ? Convert.ToInt32(reader["age"]) : 0,
                                    RaceName = reader["race_name"].ToString(),
                                    DistanceName = reader["distance_name"].ToString(),
                                    RaceDate = (DateTime)reader["race_date"],
                                    DistanceLaps = reader["distance_laps"] != DBNull.Value ? Convert.ToInt32(reader["distance_laps"]) : 0,
                                    DistanceIntervals = reader["distance_intervals"] != DBNull.Value ? Convert.ToInt32(reader["distance_intervals"]) : 0,
                                    Category = reader["category"].ToString(),
                                    ElapsedTime = reader["elapsed_time"].ToString(),
                                    Position = reader["position"] != DBNull.Value ? Convert.ToInt32(reader["position"]) : (int?)null,
                                    CategoryPosition = reader["category_position"] != DBNull.Value ? Convert.ToInt32(reader["category_position"]) : (int?)null,
                                    GenderPosition = reader["gender_position"] != DBNull.Value ? Convert.ToInt32(reader["gender_position"]) : (int?)null,
                                    Bib = reader["bib"].ToString(),
                                    SplitName = reader["split_name"].ToString(),
                                    Team = reader["team"].ToString()
                                };
                                localRecords.Add(lr);
                            }
                        }
                    }

                    // Connect to remote and fetch existing keys in one query.
                    using (var remoteConn = new MySqlConnection(remoteConnectionString))
                    {
                        remoteConn.Open();
                        HashSet<string> existingKeys = new HashSet<string>();
                        using (var fetchCmd = new MySqlCommand(
                            "SELECT rfid, timestamp FROM results_zebra WHERE event_id = @event_id", remoteConn))
                        {
                            fetchCmd.Parameters.AddWithValue("@event_id", Convert.ToInt32(textBoxEventId.Text));
                            using (var reader = fetchCmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string rfid = reader["rfid"].ToString();
                                    DateTime ts = (DateTime)reader["timestamp"];
                                    string key = $"{rfid}||{ts:O}";
                                    existingKeys.Add(key);
                                }
                            }
                        }

                        // Insert new records that are missing.
                        using (var transaction = remoteConn.BeginTransaction())
                        {
                            foreach (var lr in localRecords)
                            {
                                string localKey = $"{lr.Rfid}||{lr.Timestamp:O}";
                                if (!existingKeys.Contains(localKey))
                                {
                                    using (var insertCmd = new MySqlCommand(
                                        @"INSERT INTO results_zebra 
                                          (rfid, timestamp, gap, first_name, last_name, gender, birthday, age, race_name, distance_name, race_date, distance_laps, distance_intervals, category, elapsed_time, position, category_position, gender_position, event_id, bib, split_name, team)
                                          VALUES 
                                          (@rfid, @timestamp, @gap, @first_name, @last_name, @gender, @birthday, @age, @race_name, @distance_name, @race_date, @distance_laps, @distance_intervals, @category, @elapsed_time, @position, @category_position, @gender_position, @event_id, @bib, @split_name, @team);",
                                        remoteConn, transaction))
                                    {
                                        insertCmd.Parameters.AddWithValue("@rfid", lr.Rfid);
                                        insertCmd.Parameters.AddWithValue("@timestamp", lr.Timestamp);
                                        insertCmd.Parameters.AddWithValue("@gap", lr.Gap);
                                        insertCmd.Parameters.AddWithValue("@first_name", lr.FirstName);
                                        insertCmd.Parameters.AddWithValue("@last_name", lr.LastName);
                                        insertCmd.Parameters.AddWithValue("@gender", lr.Gender);
                                        insertCmd.Parameters.AddWithValue("@birthday", lr.Birthday);
                                        insertCmd.Parameters.AddWithValue("@age", lr.Age);
                                        insertCmd.Parameters.AddWithValue("@race_name", lr.RaceName);
                                        insertCmd.Parameters.AddWithValue("@distance_name", lr.DistanceName);
                                        insertCmd.Parameters.AddWithValue("@race_date", lr.RaceDate);
                                        insertCmd.Parameters.AddWithValue("@distance_laps", lr.DistanceLaps);
                                        insertCmd.Parameters.AddWithValue("@distance_intervals", lr.DistanceIntervals);
                                        insertCmd.Parameters.AddWithValue("@category", lr.Category);
                                        insertCmd.Parameters.AddWithValue("@elapsed_time", lr.ElapsedTime);
                                        insertCmd.Parameters.AddWithValue("@position", lr.Position.HasValue ? (object)lr.Position.Value : DBNull.Value);
                                        insertCmd.Parameters.AddWithValue("@category_position", lr.CategoryPosition.HasValue ? (object)lr.CategoryPosition.Value : DBNull.Value);
                                        insertCmd.Parameters.AddWithValue("@gender_position", lr.GenderPosition.HasValue ? (object)lr.GenderPosition.Value : DBNull.Value);
                                        insertCmd.Parameters.AddWithValue("@event_id", Convert.ToInt32(textBoxEventId.Text));
                                        insertCmd.Parameters.AddWithValue("@bib", lr.Bib);
                                        insertCmd.Parameters.AddWithValue("@split_name", lr.SplitName);
                                        insertCmd.Parameters.AddWithValue("@team", lr.Team);
                                        insertCmd.ExecuteNonQuery();
                                    }
                                }
                            }
                            transaction.Commit();
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Optionally log error.
            }
        }

        // New event handler for auto-upload checkbox.
        private void chkAutoUpload_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoUpload.Checked)
            {
                if (int.TryParse(txtUploadInterval.Text, out int minutes) && minutes > 0)
                {
                    uploadTimer.Interval = minutes * 60 * 1000; // Convert minutes to milliseconds.
                    uploadTimer.Start();
                }
                else
                {
                    MessageBox.Show("Please enter a valid number of minutes for the upload interval.");
                    chkAutoUpload.Checked = false;
                }
            }
            else
            {
                uploadTimer.Stop();
            }
        }

        // Timer tick for auto-upload.
        private void UploadTimer_Tick(object sender, EventArgs e)
        {
            Task.Run(() => PushDataToRemote());
        }

        private void comboBoxEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEvents.SelectedValue != null)
            {
                selectedEventId = (int)comboBoxEvents.SelectedValue;
                textBoxEventId.Text = selectedEventId.ToString();
            }
            else
            {
                MessageBox.Show("No event selected.");
            }
        }

        private void NotifyUser(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region Helper Methods (LoadRaces, LoadDistances, LoadEvents, GetRecordCount)

        private void LoadRaces()
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT id, name FROM races", conn);
                    var reader = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    cmbRaces.DisplayMember = "name";
                    cmbRaces.ValueMember = "id";
                    cmbRaces.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading races: " + ex.Message);
            }
        }

        private void LoadDistances(int raceId)
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT id, name FROM distances WHERE race_id = @race_id", conn);
                    cmd.Parameters.AddWithValue("@race_id", raceId);
                    var reader = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    cmbDistances.DisplayMember = "name";
                    cmbDistances.ValueMember = "id";
                    cmbDistances.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading distances: " + ex.Message);
            }
        }

        private void LoadEvents()
        {
            string connectionStringRemote = GetConnectionString("../../../../dbconfig.txt", true);
            try
            {
                using (remoteConnection = new MySqlConnection(connectionStringRemote))
                {
                    remoteConnection.Open();
                    string query = "SELECT id, event_name FROM events";
                    MySqlCommand cmd = new MySqlCommand(query, remoteConnection);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    dt.Columns.Add("Display", typeof(string), "event_name + ' (' + CONVERT(id, 'System.String') + ')'");
                    comboBoxEvents.DisplayMember = "Display";
                    comboBoxEvents.ValueMember = "id";
                    comboBoxEvents.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading events: " + ex.Message);
            }
        }


        #endregion

        #region Helper Classes

        private class RunnerInfo
        {
            public int DistanceIntervals { get; set; }
            public int DistanceLaps { get; set; }
            public int DistanceId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Gender { get; set; }
            public DateTime Birthday { get; set; }
            public int Age { get; set; }
            public string RaceName { get; set; }
            public string DistanceName { get; set; }
            public DateTime RaceDate { get; set; }
            public string Category { get; set; }
            public string Bib { get; set; }
            public string Team { get; set; }
        }

        // This class represents a record from the local 'results' table.
        private class LocalResult
        {
            public string Rfid { get; set; }
            public DateTime Timestamp { get; set; }
            public float Gap { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Gender { get; set; }
            public DateTime Birthday { get; set; }
            public int Age { get; set; }
            public string RaceName { get; set; }
            public string DistanceName { get; set; }
            public DateTime RaceDate { get; set; }
            public int DistanceLaps { get; set; }
            public int DistanceIntervals { get; set; }
            public string Category { get; set; }
            public string ElapsedTime { get; set; }
            public int? Position { get; set; }
            public int? CategoryPosition { get; set; }
            public int? GenderPosition { get; set; }
            public string Bib { get; set; }
            public string SplitName { get; set; }
            public string Team { get; set; }
        }

        // This class is used in batch insertion.
        private class ResultRecord
        {
            public string RFID { get; set; }
            public DateTime Timestamp { get; set; }
            public float Gap { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Gender { get; set; }
            public DateTime Birthday { get; set; }
            public int Age { get; set; }
            public string RaceName { get; set; }
            public string DistanceName { get; set; }
            public DateTime RaceDate { get; set; }
            public int DistanceLaps { get; set; }
            public int DistanceIntervals { get; set; }
            public string Category { get; set; }
            public string ElapsedTime { get; set; }
            public int? Position { get; set; }
            public int? CategoryPosition { get; set; }
            public int? GenderPosition { get; set; }
            public string Bib { get; set; }
            public string Team { get; set; }
            public bool IsFinish { get; set; }
        }

        #endregion
    }
}
