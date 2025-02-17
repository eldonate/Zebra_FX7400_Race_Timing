using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Timing_Software_ver._3._0
{
    // Class to track each runner's state.
    public class RunnerState
    {
        public int LastLapProcessed { get; set; }
        public bool IsExpectingFirstRead { get; set; }
        public DateTime FirstReadTimestamp { get; set; }
        public DateTime LastProcessedTimestamp { get; set; }
        public bool IsDisqualified { get; set; }
        public bool IsManualStart { get; set; }
        public bool IsManualFinish { get; set; }

        public RunnerState()
        {
            LastLapProcessed = 0;
            IsExpectingFirstRead = true;
            FirstReadTimestamp = DateTime.MinValue;
            LastProcessedTimestamp = DateTime.MinValue;
            IsDisqualified = false;
            IsManualStart = false;
            IsManualFinish = false;
        }
    }

    public partial class timetrial : Form
    {
        // Fields
        private bool monitoringStarted = false;
        private string connectionString;
        private string selectedFilePath;
        private CancellationTokenSource cancellationTokenSource;
        private long lastPosition;

        // Sound players.
        private SoundPlayer beepSoundPlayer;
        private SoundPlayer lapStartSoundPlayer;
        private SoundPlayer countdownSoundPlayer;

        // Runner states dictionary.
        private Dictionary<string, RunnerState> runnerStates;

        // Lap number and counters.
        private int currentLapNumber;
        private int runnersStartedCurrentLap;
        private int runnersFinishedCurrentLap;

        // Countdown timer variables.
        private System.Windows.Forms.Timer countdownTimer;
        private int currentLapRemainingSeconds;  // in seconds
        private int autoLapDurationSeconds;        // full lap duration (in seconds)

        public timetrial()
        {
            InitializeComponent();

            connectionString = GetConnectionString("dbconfig.txt", false);
            cancellationTokenSource = new CancellationTokenSource();
            lastPosition = 0;
            // Load sound files. (Replace "beep.wav" with your actual file names if needed.)
            beepSoundPlayer = new SoundPlayer("beep.wav");
            lapStartSoundPlayer = new SoundPlayer("beep.wav"); // Change to "lapstart.wav" as desired.
            countdownSoundPlayer = new SoundPlayer("beep.wav");  // Change to "countdown.wav" as desired.

            runnerStates = new Dictionary<string, RunnerState>();

            // Start with lap 1.
            currentLapNumber = 1;
            textBoxLapNumber.Text = currentLapNumber.ToString();

            runnersStartedCurrentLap = 0;
            runnersFinishedCurrentLap = 0;
            textBoxStartedCount.Text = runnersStartedCurrentLap.ToString();
            textBoxFinishedCount.Text = runnersFinishedCurrentLap.ToString();

            ConnectToDatabase();

            // Initialize the countdown timer (tick every 1 second) but do NOT start it.
            countdownTimer = new System.Windows.Forms.Timer();
            countdownTimer.Interval = 1000;
            countdownTimer.Tick += CountdownTimer_Tick;
        }

        /// <summary>
        /// Reads connection string from the configuration file.
        /// </summary>
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
                            case "remote_server": connectionStringBuilder.Server = value; break;
                            case "remote_user": connectionStringBuilder.UserID = value; break;
                            case "remote_database": connectionStringBuilder.Database = value; break;
                            case "remote_port": connectionStringBuilder.Port = uint.Parse(value); break;
                            case "remote_password": connectionStringBuilder.Password = value; break;
                        }
                    }
                    else
                    {
                        switch (key)
                        {
                            case "server": connectionStringBuilder.Server = value; break;
                            case "user": connectionStringBuilder.UserID = value; break;
                            case "database": connectionStringBuilder.Database = value; break;
                            case "port": connectionStringBuilder.Port = uint.Parse(value); break;
                            case "password": connectionStringBuilder.Password = value; break;
                        }
                    }
                }
            }
            return connectionStringBuilder.ToString();
        }

        private void ConnectToDatabase()
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    labelConnectionStatus.Text = "Connected";
                    labelConnectionStatus.ForeColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
                labelConnectionStatus.Text = "Not Connected";
                labelConnectionStatus.ForeColor = Color.Red;
                MessageBox.Show("Connection failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectFileButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Data File";
                ofd.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedFilePath = ofd.FileName;
                    lastPosition = 0;
                    MessageBox.Show($"File selected: {selectedFilePath}", "File Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private async void StartMonitoringButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                MessageBox.Show("Please select a file to monitor first.", "No File Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            cancellationTokenSource = new CancellationTokenSource();
            try
            {
                // Start monitoring the file on a background task.
                await Task.Run(() => MonitorFile(selectedFilePath, cancellationTokenSource.Token));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting monitoring: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            monitoringStarted = true;  // Mark that monitoring has started.
            // Immediately start the countdown for the first lap.
            StartLapCountdown();
        }

        private async Task MonitorFile(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(fs))
                {
                    fs.Seek(0, SeekOrigin.Begin);
                    lastPosition = 0;
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        string line = await reader.ReadLineAsync();
                        if (line != null)
                        {
                            var columns = line.Split(',');
                            if (columns.Length < 3) continue;
                            if (!DateTime.TryParse(columns[1], out DateTime timestamp)) continue;
                            string rfid = columns[2].Trim();
                            if (string.IsNullOrEmpty(rfid)) continue;
                            ProcessRfidEntry(rfid, timestamp);
                        }
                        else
                        {
                            await Task.Delay(500, cancellationToken);
                        }
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcessRfidEntry(string rfid, DateTime timestamp)
        {
            lock (runnerStates)
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        var runnerData = GetRunnerData(connection, rfid);
                        if (runnerData == null) return;
                        int distanceIntervals = runnerData["distance_intervals"] != DBNull.Value
                            ? Convert.ToInt32(runnerData["distance_intervals"])
                            : 0;
                        if (!runnerStates.ContainsKey(rfid))
                            runnerStates[rfid] = new RunnerState();
                        RunnerState state = runnerStates[rfid];
                        if (state.IsDisqualified) return;
                        if (state.LastLapProcessed < currentLapNumber - 1)
                        {
                            state.IsDisqualified = true;
                            LogDisqualification(rfid, state.LastLapProcessed, currentLapNumber);
                            return;
                        }
                        if (state.LastLapProcessed == currentLapNumber - 1)
                        {
                            if (state.LastProcessedTimestamp != DateTime.MinValue)
                            {
                                TimeSpan timeSinceLast = timestamp - state.LastProcessedTimestamp;
                                if (timeSinceLast.TotalSeconds <= distanceIntervals) return;
                            }
                            if (state.IsExpectingFirstRead)
                            {
                                // Register a start event.
                                string elapsedTime = "00:00:00:00";
                                int lapCount = currentLapNumber;
                                string splitName = null;
                                InsertIntoResults(connection, runnerData, timestamp, elapsedTime, lapCount, splitName);
                                state.IsExpectingFirstRead = false;
                                state.FirstReadTimestamp = timestamp;
                                state.LastProcessedTimestamp = timestamp;
                                runnersStartedCurrentLap++;
                                textBoxStartedCount.Invoke((Action)(() =>
                                {
                                    textBoxStartedCount.Text = runnersStartedCurrentLap.ToString();
                                }));
                                // Play beep for runner read.
                                beepSoundPlayer.Play();
                            }
                            else
                            {
                                // Register a finish event.
                                TimeSpan elapsed = timestamp - state.FirstReadTimestamp;
                                if (elapsed.TotalSeconds <= distanceIntervals) return;
                                string elapsedTime = $"{(int)elapsed.TotalDays:D2}:{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
                                int lapCount = currentLapNumber;
                                string splitName = $"Lap {currentLapNumber}";
                                if (!string.IsNullOrWhiteSpace(textBoxLapNumber.Text))
                                    splitName = $"Lap {textBoxLapNumber.Text.Trim()}";
                                InsertIntoResults(connection, runnerData, timestamp, elapsedTime, lapCount, splitName);
                                state.IsExpectingFirstRead = true;
                                state.FirstReadTimestamp = DateTime.MinValue;
                                state.LastLapProcessed = currentLapNumber;
                                state.LastProcessedTimestamp = timestamp;
                                runnersFinishedCurrentLap++;
                                textBoxFinishedCount.Invoke((Action)(() =>
                                {
                                    textBoxFinishedCount.Text = runnersFinishedCurrentLap.ToString();
                                }));
                                // Play beep for runner read.
                                beepSoundPlayer.Play();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error processing RFID entry: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LogDisqualification(string rfid, int lastLapProcessed, int currentLap)
        {
            string message = $"Runner with RFID {rfid} has been disqualified. Last completed lap: {lastLapProcessed}, current lap: {currentLap}.";
            Console.WriteLine(message);
        }

        private DataRow GetRunnerData(MySqlConnection connection, string rfid)
        {
            var query = "SELECT * FROM runners WHERE rfid = @rfid";
            var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@rfid", rfid);
            var adapter = new MySqlDataAdapter(cmd);
            var runnerTable = new DataTable();
            adapter.Fill(runnerTable);
            return runnerTable.Rows.Count > 0 ? runnerTable.Rows[0] : null;
        }

        private void InsertIntoResults(MySqlConnection connection, DataRow runnerData, DateTime timestamp, string elapsedTime, int lapCount, string splitName)
        {
            var query = @"INSERT INTO results 
                  (rfid, timestamp, elapsed_time, distance_laps, split_name, 
                   first_name, last_name, gender, birthday, age, 
                   race_name, distance_name, race_date, distance_intervals, 
                   category, bib, team) 
                  VALUES 
                  (@rfid, @timestamp, @elapsed_time, @distance_laps, @split_name, 
                   @first_name, @last_name, @gender, @birthday, @age, 
                   @race_name, @distance_name, @race_date, @distance_intervals, 
                   @category, @bib, @team)";
            using (var cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@rfid", runnerData["rfid"]);
                cmd.Parameters.AddWithValue("@timestamp", timestamp);
                cmd.Parameters.AddWithValue("@elapsed_time", elapsedTime);
                cmd.Parameters.AddWithValue("@distance_laps", lapCount);
                cmd.Parameters.AddWithValue("@split_name", splitName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@first_name", runnerData["first_name"]);
                cmd.Parameters.AddWithValue("@last_name", runnerData["last_name"]);
                cmd.Parameters.AddWithValue("@gender", runnerData["gender"]);
                cmd.Parameters.AddWithValue("@birthday", runnerData["birthday"]);
                cmd.Parameters.AddWithValue("@age", runnerData["age"]);
                cmd.Parameters.AddWithValue("@race_name", runnerData["race_name"]);
                cmd.Parameters.AddWithValue("@distance_name", runnerData["distance_name"]);
                cmd.Parameters.AddWithValue("@race_date", runnerData["race_date"]);
                cmd.Parameters.AddWithValue("@distance_intervals", runnerData["distance_intervals"]);
                cmd.Parameters.AddWithValue("@category", runnerData["category"]);
                cmd.Parameters.AddWithValue("@bib", runnerData["bib"]);
                cmd.Parameters.AddWithValue("@team",
                    runnerData.Table.Columns.Contains("team") && runnerData["team"] != DBNull.Value
                        ? runnerData["team"]
                        : (object)DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        // Manually increments the lap.
        private void IncrementLapButton_Click(object sender, EventArgs e)
        {
            currentLapNumber += 1;
            textBoxLapNumber.Text = currentLapNumber.ToString();
            labelCurrentLap.Text = $"Current Lap: {currentLapNumber}";
            runnersStartedCurrentLap = 0;
            runnersFinishedCurrentLap = 0;
            textBoxStartedCount.Text = "0";
            textBoxFinishedCount.Text = "0";
            // Removed runnerStates.Clear() to preserve states across laps
            StartLapCountdown();
        }

        // Corrected single definition of TextBoxLapNumber_TextChanged.
        private void TextBoxLapNumber_TextChanged(object sender, EventArgs e)
        {
            if (!monitoringStarted)
            {
                if (int.TryParse(textBoxLapNumber.Text.Trim(), out int lapNumber) && lapNumber > 0)
                {
                    currentLapNumber = lapNumber;
                    labelCurrentLap.Text = $"Current Lap: {currentLapNumber}";
                }
                else
                {
                    MessageBox.Show("Please enter a valid lap number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBoxLapNumber.Text = currentLapNumber.ToString();
                }
                return;
            }

            if (int.TryParse(textBoxLapNumber.Text.Trim(), out int newLap) && newLap > 0)
            {
                currentLapNumber = newLap;
                labelCurrentLap.Text = $"Current Lap: {currentLapNumber}";
                runnersStartedCurrentLap = 0;
                runnersFinishedCurrentLap = 0;
                textBoxStartedCount.Text = "0";
                textBoxFinishedCount.Text = "0";
                // Removed runnerStates.Clear() to preserve states across laps
                StartLapCountdown();
            }
            else
            {
                MessageBox.Show("Please enter a valid lap number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxLapNumber.Text = currentLapNumber.ToString();
            }
        }

        private void StopMonitoringButton_Click(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
            MessageBox.Show("Stopped monitoring the file.", "Monitoring Stopped", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string remoteConnectionString = GetConnectionString("../../../../dbconfig.txt", true);
            try
            {
                using (var localConn = new MySqlConnection(connectionString))
                {
                    localConn.Open();
                    string readerPosition = ReaderPosition.Text;
                    if (!string.IsNullOrEmpty(readerPosition))
                    {
                        using (var updateCmd = new MySqlCommand("UPDATE results SET split_name = @split_name WHERE split_name IS NULL OR split_name = ''", localConn))
                        {
                            updateCmd.Parameters.AddWithValue("@split_name", readerPosition);
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                    using (var cmd = new MySqlCommand("SELECT * FROM results", localConn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            using (var remoteConn = new MySqlConnection(remoteConnectionString))
                            {
                                remoteConn.Open();
                                while (reader.Read())
                                {
                                    using (var checkCmd = new MySqlCommand(
                                        @"SELECT COUNT(*) FROM results_zebra
                                          WHERE rfid = @rfid 
                                            AND timestamp = @timestamp 
                                            AND event_id = @event_id", remoteConn))
                                    {
                                        checkCmd.Parameters.AddWithValue("@rfid", reader["rfid"]);
                                        checkCmd.Parameters.AddWithValue("@timestamp", reader["timestamp"]);
                                        checkCmd.Parameters.AddWithValue("@event_id", Convert.ToInt32(textBoxEventId.Text));
                                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                                        if (count == 0)
                                        {
                                            using (var insertCmd = new MySqlCommand(
                                                @"INSERT INTO results_zebra 
                                                (rfid, timestamp, gap, 
                                                 first_name, last_name, gender, birthday, age,
                                                 race_name, distance_name, race_date, distance_laps, distance_intervals,
                                                 category, elapsed_time, position, category_position, gender_position,
                                                 event_id, bib, split_name, team) 
                                                VALUES 
                                                (@rfid, @timestamp, @gap, 
                                                 @first_name, @last_name, @gender, @birthday, @age,
                                                 @race_name, @distance_name, @race_date, @distance_laps, @distance_intervals,
                                                 @category, @elapsed_time, @position, @category_position, @gender_position,
                                                 @event_id, @bib, @split_name, @team)", remoteConn))
                                            {
                                                insertCmd.Parameters.AddWithValue("@rfid", reader["rfid"]);
                                                insertCmd.Parameters.AddWithValue("@timestamp", reader["timestamp"]);
                                                insertCmd.Parameters.AddWithValue("@gap", reader["gap"] != DBNull.Value ? reader["gap"] : (object)DBNull.Value);
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
                                                insertCmd.Parameters.AddWithValue("@category_position", reader["category_position"] != DBNull.Value ? reader["category_position"] : (object)DBNull.Value);
                                                insertCmd.Parameters.AddWithValue("@gender_position", reader["gender_position"] != DBNull.Value ? reader["gender_position"] : (object)DBNull.Value);
                                                insertCmd.Parameters.AddWithValue("@event_id", Convert.ToInt32(textBoxEventId.Text));
                                                insertCmd.Parameters.AddWithValue("@bib", reader["bib"]);
                                                insertCmd.Parameters.AddWithValue("@split_name", reader["split_name"]);
                                                insertCmd.Parameters.AddWithValue("@team", reader["team"] != DBNull.Value ? reader["team"] : (object)DBNull.Value);
                                                insertCmd.ExecuteNonQuery();
                                            }
                                        }
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

        // Manual lap submission.
        private void buttonSubmitLap_Click(object sender, EventArgs e)
        {
            string bibNumber = textBoxBibNumber.Text.Trim();
            if (string.IsNullOrEmpty(bibNumber))
            {
                MessageBox.Show("Please enter a bib number.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var runnerData = GetRunnerDataByBib(connection, bibNumber);
                    if (runnerData == null)
                    {
                        MessageBox.Show($"No runner found with bib number: {bibNumber}", "Runner Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    ManualLapSubmission(runnerData, connection);
                    MessageBox.Show($"Manual lap submission for bib {bibNumber} completed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBoxBibNumber.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataRow GetRunnerDataByBib(MySqlConnection connection, string bibNumber)
        {
            var query = "SELECT * FROM runners WHERE bib = @bibNumber";
            var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@bibNumber", bibNumber);
            var adapter = new MySqlDataAdapter(cmd);
            var runnerTable = new DataTable();
            adapter.Fill(runnerTable);
            return runnerTable.Rows.Count > 0 ? runnerTable.Rows[0] : null;
        }

        private void ManualLapSubmission(DataRow runnerData, MySqlConnection connection)
        {
            string rfid = runnerData["rfid"].ToString();
            lock (runnerStates)
            {
                if (!runnerStates.ContainsKey(rfid))
                    runnerStates[rfid] = new RunnerState();
                RunnerState state = runnerStates[rfid];
                if (state.IsDisqualified)
                {
                    state.IsDisqualified = false;
                    Console.WriteLine($"Runner {rfid} was disqualified but has been reinstated via manual submission.");
                }
                DateTime manualTime;
                if (!string.IsNullOrWhiteSpace(textBoxManualTime.Text) &&
                   DateTime.TryParse(textBoxManualTime.Text.Trim(), out manualTime))
                {
                    // Use provided manual time.
                }
                else
                {
                    manualTime = DateTime.Now;
                }
                if (state.FirstReadTimestamp == DateTime.MinValue || state.IsExpectingFirstRead)
                {
                    string elapsedTime = "00:00:00:00";
                    int lapCount = currentLapNumber;
                    string splitName = null;
                    InsertIntoResults(connection, runnerData, manualTime, elapsedTime, lapCount, splitName);
                    state.IsExpectingFirstRead = false;
                    state.FirstReadTimestamp = manualTime;
                    state.LastProcessedTimestamp = manualTime;
                    state.IsManualStart = true;
                    runnersStartedCurrentLap++;
                    textBoxStartedCount.Text = runnersStartedCurrentLap.ToString();
                    beepSoundPlayer.Play();
                }
                else
                {
                    TimeSpan elapsed = manualTime - state.FirstReadTimestamp;
                    string elapsedTime = $"{(int)elapsed.TotalDays:D2}:{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
                    int lapCount = currentLapNumber;
                    string splitName = $"Lap {currentLapNumber}";
                    if (!string.IsNullOrWhiteSpace(textBoxLapNumber.Text))
                        splitName = $"Lap {textBoxLapNumber.Text.Trim()}";
                    InsertIntoResults(connection, runnerData, manualTime, elapsedTime, lapCount, splitName);
                    state.LastLapProcessed = currentLapNumber;
                    state.IsExpectingFirstRead = true;
                    state.FirstReadTimestamp = DateTime.MinValue;
                    state.LastProcessedTimestamp = manualTime;
                    state.IsManualFinish = true;
                    runnersFinishedCurrentLap++;
                    textBoxFinishedCount.Text = runnersFinishedCurrentLap.ToString();
                    beepSoundPlayer.Play();
                }
            }
        }

        // ------------------ COUNTDOWN TIMER LOGIC ------------------

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            currentLapRemainingSeconds--;
            TimeSpan ts = TimeSpan.FromSeconds(currentLapRemainingSeconds);
            labelCountdown.Text = $"Countdown: {ts:mm\\:ss}";

            if (currentLapRemainingSeconds <= 0)
            {
                countdownTimer.Stop();
                if (checkBoxAutoLap.Checked)
                {
                    // If auto mode is enabled, play final countdown sequence then auto-increment lap.
                    PlayCountdownSequenceAsync().ContinueWith(t => IncrementLapAutomatically());
                }
            }
        }

        private async Task PlayCountdownBeepsAsync(int count)
        {
            for (int i = 0; i < count; i++)
            {
                countdownSoundPlayer.Play();
                await Task.Delay(500);
            }
        }

        private async Task PlayCountdownSequenceAsync()
        {
            // Play a 3-2-1 countdown sequence.
            for (int i = 0; i < 3; i++)
            {
                countdownSoundPlayer.Play();
                await Task.Delay(500);
            }
            lapStartSoundPlayer.Play();
        }

        // Corrected single definition of IncrementLapAutomatically.
        private void IncrementLapAutomatically()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(IncrementLapAutomatically));
                return;
            }

            currentLapNumber += 1;
            textBoxLapNumber.Text = currentLapNumber.ToString();
            labelCurrentLap.Text = $"Current Lap: {currentLapNumber}";
            runnersStartedCurrentLap = 0;
            runnersFinishedCurrentLap = 0;
            textBoxStartedCount.Text = "0";
            textBoxFinishedCount.Text = "0";
            // Removed runnerStates.Clear() to preserve states across laps
            StartLapCountdown();
        }

        private async void StartLapCountdown()
        {
            // Read lap duration from textbox; default to 60 minutes if parsing fails.
            if (!int.TryParse(textBoxAutoLapDuration.Text.Trim(), out int durationMinutes))
                durationMinutes = 60;
            autoLapDurationSeconds = durationMinutes * 60;
            currentLapRemainingSeconds = autoLapDurationSeconds;
            // Update countdown label immediately.
            TimeSpan ts = TimeSpan.FromSeconds(currentLapRemainingSeconds);
            labelCountdown.Text = $"Countdown: {ts:mm\\:ss}";
            // Wait 1 second then play the lap start sound.
            await Task.Delay(1000);
            lapStartSoundPlayer.Play();
            // Start the countdown timer.
            countdownTimer.Start();
        }

        private void checkBoxAutoLap_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAutoLap.Checked)
            {
                if (!monitoringStarted)
                {
                    // Do nothing if monitoring hasn't started yet.
                    return;
                }
                // If not on lap 1, immediately increment lap.
                if (currentLapNumber != 1)
                {
                    currentLapNumber++;
                    textBoxLapNumber.Text = currentLapNumber.ToString();
                    labelCurrentLap.Text = $"Current Lap: {currentLapNumber}";
                }
                StartLapCountdown();
            }
            else
            {
                countdownTimer.Stop();
            }
        }
    }
}
