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
    // Class to track the state of each runner
    public class RunnerState
    {
        public int LastLapProcessed { get; set; }
        public bool IsExpectingFirstRead { get; set; }
        public DateTime FirstReadTimestamp { get; set; }
        public DateTime LastProcessedTimestamp { get; set; }
        public bool IsDisqualified { get; set; }

        public RunnerState()
        {
            LastLapProcessed = 0;
            IsExpectingFirstRead = true;
            FirstReadTimestamp = DateTime.MinValue;
            LastProcessedTimestamp = DateTime.MinValue;
            IsDisqualified = false;
        }
    }

    public partial class timetrial : Form
    {
        private string connectionString;
        private string selectedFilePath;
        private CancellationTokenSource cancellationTokenSource;
        private long lastPosition;
        private SoundPlayer soundPlayer;

        // Dictionary to track RunnerState per RFID
        private Dictionary<string, RunnerState> runnerStates;

        // Global lap number
        private int currentLapNumber;

        public timetrial()
        {
            InitializeComponent();

            // Read local DB config
            connectionString = GetConnectionString("dbconfig.txt", false);

            cancellationTokenSource = new CancellationTokenSource();
            lastPosition = 0;

            // Load the beep sound (place beep.wav in your executable folder or adjust path)
            soundPlayer = new SoundPlayer("beep.wav");

            // Initialize the dictionary that holds runner states
            runnerStates = new Dictionary<string, RunnerState>();

            // Start with lap 1
            currentLapNumber = 1;
            textBoxLapNumber.Text = currentLapNumber.ToString();

            // Connect to local database and update status label
            ConnectToDatabase();
        }

        /// <summary>
        /// Reads the connection string from the configuration file.
        /// If isRemote is true, read remote credentials, else read local credentials.
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

        /// <summary>
        /// Attempts to connect to the database and updates the labelConnectionStatus.
        /// </summary>
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

        /// <summary>
        /// Handles the file selection process using OpenFileDialog.
        /// </summary>
        private void SelectFileButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select Data File";
                openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFilePath = openFileDialog.FileName;
                    lastPosition = 0; // Reset position for new file
                    MessageBox.Show($"File selected: {selectedFilePath}", "File Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Starts monitoring the selected file for new entries.
        /// </summary>
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
                await Task.Run(() => MonitorFile(selectedFilePath, cancellationTokenSource.Token));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting monitoring: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Monitors the specified file for new lines and processes them.
        /// </summary>
        private async Task MonitorFile(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(fileStream))
                {
                    // Read from the beginning of the file
                    fileStream.Seek(0, SeekOrigin.Begin);
                    lastPosition = 0;

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        string line = await reader.ReadLineAsync();
                        if (line != null)
                        {
                            // Example line format: some_id, 2023-09-01 10:00:00, RFID1234
                            var columns = line.Split(',');
                            if (columns.Length < 3)
                                continue; // Not enough columns

                            if (!DateTime.TryParse(columns[1], out DateTime timestamp))
                                continue; // Invalid timestamp

                            string rfid = columns[2].Trim();
                            if (string.IsNullOrEmpty(rfid))
                                continue; // Invalid RFID

                            ProcessRfidEntry(rfid, timestamp);
                        }
                        else
                        {
                            // No new lines, wait a bit before checking again
                            await Task.Delay(500, cancellationToken);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Monitoring was canceled
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Processes an RFID entry according to the specified logic (laps, intervals, disqualification).
        /// </summary>
        private void ProcessRfidEntry(string rfid, DateTime timestamp)
        {
            lock (runnerStates) // Thread safety
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        // Get runner data by RFID
                        var runnerData = GetRunnerData(connection, rfid);
                        if (runnerData == null) return; // Runner not found, ignore

                        // distance_intervals from 'runners' table
                        int distanceIntervals = runnerData["distance_intervals"] != DBNull.Value
                            ? Convert.ToInt32(runnerData["distance_intervals"])
                            : 0;

                        // Ensure we have a RunnerState for this RFID
                        if (!runnerStates.ContainsKey(rfid))
                        {
                            runnerStates[rfid] = new RunnerState();
                        }

                        RunnerState state = runnerStates[rfid];

                        // If runner is disqualified, ignore further reads from this method
                        if (state.IsDisqualified)
                            return;

                        // Check if the runner missed a lap => disqualification
                        if (state.LastLapProcessed < currentLapNumber - 1)
                        {
                            state.IsDisqualified = true;
                            LogDisqualification(rfid, state.LastLapProcessed, currentLapNumber);
                            return;
                        }

                        // Process if the runner is on the "currentLapNumber - 1" lap
                        if (state.LastLapProcessed == currentLapNumber - 1)
                        {
                            // Distance interval check
                            if (state.LastProcessedTimestamp != DateTime.MinValue)
                            {
                                TimeSpan timeSinceLastProcessed = timestamp - state.LastProcessedTimestamp;
                                if (timeSinceLastProcessed.TotalSeconds <= distanceIntervals)
                                {
                                    // Skip this entry due to distance_intervals constraint
                                    return;
                                }
                            }

                            if (state.IsExpectingFirstRead)
                            {
                                // First read for the lap (lap start)
                                string elapsedTime = "00:00:00:00";
                                int lapCount = currentLapNumber;
                                string splitName = null;

                                // Insert record (lap start)
                                InsertIntoResults(connection, runnerData, timestamp, elapsedTime, lapCount, splitName);

                                // Update state
                                state.IsExpectingFirstRead = false;
                                state.FirstReadTimestamp = timestamp;
                                state.LastProcessedTimestamp = timestamp;

                                // Play beep
                                soundPlayer.Play();
                            }
                            else
                            {
                                // Second read for the lap (lap completion)
                                TimeSpan elapsed = timestamp - state.FirstReadTimestamp;

                                if (elapsed.TotalSeconds <= distanceIntervals)
                                    return; // Too soon according to distance_intervals

                                // Format: dd:HH:mm:ss
                                string elapsedTime = $"{(int)elapsed.TotalDays:D2}:{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
                                int lapCount = currentLapNumber;
                                string splitName = $"Lap {currentLapNumber}";

                                // Optionally override the lap label if the user typed something in textBoxLapNumber
                                if (!string.IsNullOrWhiteSpace(textBoxLapNumber.Text))
                                {
                                    splitName = $"Lap {textBoxLapNumber.Text.Trim()}";
                                }

                                // Insert record (lap completion)
                                InsertIntoResults(connection, runnerData, timestamp, elapsedTime, lapCount, splitName);

                                // Update state
                                state.IsExpectingFirstRead = true;
                                state.FirstReadTimestamp = DateTime.MinValue;
                                state.LastLapProcessed = currentLapNumber;
                                state.LastProcessedTimestamp = timestamp;

                                // Play beep
                                soundPlayer.Play();
                            }
                        }
                        else
                        {
                            // Runner has already completed this lap or is behind
                            // If behind => disqualified above. If ahead => do nothing.
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error processing RFID entry: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Logs the disqualification of a runner.
        /// </summary>
        private void LogDisqualification(string rfid, int lastLapProcessed, int currentLap)
        {
            string message = $"Runner with RFID {rfid} has been disqualified. " +
                             $"Last completed lap: {lastLapProcessed}, current lap: {currentLap}.";
            Console.WriteLine(message);
        }

        /// <summary>
        /// Retrieves runner data from the 'runners' table using RFID.
        /// </summary>
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

        /// <summary>
        /// Inserts a new record into the 'results' table with relevant data.
        /// </summary>
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

                // NEW: team column
                cmd.Parameters.AddWithValue("@team",
                    runnerData.Table.Columns.Contains("team") && runnerData["team"] != DBNull.Value
                        ? runnerData["team"]
                        : (object)DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Increments the global lap number when the button is clicked.
        /// </summary>
        private void IncrementLapButton_Click(object sender, EventArgs e)
        {
            currentLapNumber += 1;
            textBoxLapNumber.Text = currentLapNumber.ToString();
            labelCurrentLap.Text = $"Current Lap: {currentLapNumber}";
            MessageBox.Show($"Lap number incremented to {currentLapNumber}", "Lap Incremented", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Reset LastProcessedTimestamp for all runners
            lock (runnerStates)
            {
                foreach (var state in runnerStates.Values)
                {
                    state.LastProcessedTimestamp = DateTime.MinValue;
                }
            }
        }

        /// <summary>
        /// Updates the lap number when the textbox value changes.
        /// </summary>
        private void TextBoxLapNumber_TextChanged(object sender, EventArgs e)
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
        }

        /// <summary>
        /// Stops monitoring the file.
        /// </summary>
        private void StopMonitoringButton_Click(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
            MessageBox.Show("Stopped monitoring the file.", "Monitoring Stopped", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Transfers data from the local 'results' table to the remote 'results_zebra' table.
        /// Uses 'textBoxEventId' for event_id and 'ReaderPosition' for split_name if provided.
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            // Get the remote DB connection string
            string remoteConnectionString = GetConnectionString("../../../../dbconfig.txt", true); // 'true' for remote DB

            try
            {
                using (var localConn = new MySqlConnection(connectionString)) // local connection
                {
                    localConn.Open();

                    // Update 'split_name' in local 'results' based on 'ReaderPosition' textbox
                    string readerPosition = ReaderPosition.Text;
                    if (!string.IsNullOrEmpty(readerPosition))
                    {
                        using (var updateCmd = new MySqlCommand(
                            "UPDATE results SET split_name = @split_name WHERE split_name IS NULL OR split_name = ''",
                            localConn))
                        {
                            updateCmd.Parameters.AddWithValue("@split_name", readerPosition);
                            updateCmd.ExecuteNonQuery();
                        }
                    }

                    // Transfer data from local results -> remote results_zebra
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
                                            AND event_id = @event_id",
                                        remoteConn))
                                    {
                                        checkCmd.Parameters.AddWithValue("@rfid", reader["rfid"]);
                                        checkCmd.Parameters.AddWithValue("@timestamp", reader["timestamp"]);
                                        checkCmd.Parameters.AddWithValue("@event_id", Convert.ToInt32(textBoxEventId.Text));

                                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                                        if (count == 0) // record not found, insert
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
                                                 @event_id, @bib, @split_name, @team)",
                                                remoteConn))
                                            {
                                                // Prepare parameters
                                                insertCmd.Parameters.AddWithValue("@rfid", reader["rfid"]);
                                                insertCmd.Parameters.AddWithValue("@timestamp", reader["timestamp"]);
                                                insertCmd.Parameters.AddWithValue("@gap", reader["gap"] != DBNull.Value
                                                                                    ? reader["gap"]
                                                                                    : (object)DBNull.Value);
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
                                                insertCmd.Parameters.AddWithValue("@position",
                                                    reader["position"] != DBNull.Value
                                                        ? reader["position"]
                                                        : (object)DBNull.Value);
                                                insertCmd.Parameters.AddWithValue("@category_position",
                                                    reader["category_position"] != DBNull.Value
                                                        ? reader["category_position"]
                                                        : (object)DBNull.Value);
                                                insertCmd.Parameters.AddWithValue("@gender_position",
                                                    reader["gender_position"] != DBNull.Value
                                                        ? reader["gender_position"]
                                                        : (object)DBNull.Value);

                                                insertCmd.Parameters.AddWithValue("@event_id", Convert.ToInt32(textBoxEventId.Text));
                                                insertCmd.Parameters.AddWithValue("@bib", reader["bib"]);
                                                insertCmd.Parameters.AddWithValue("@split_name", reader["split_name"]);
                                                insertCmd.Parameters.AddWithValue("@team",
                                                    reader["team"] != DBNull.Value
                                                        ? reader["team"]
                                                        : (object)DBNull.Value);

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

        // =================================================================
        //  NEW FEATURE: MANUAL BIB-BASED LAP SUBMISSION (OVERRIDES DQ)
        // =================================================================

        /// <summary>
        /// When the user clicks the "Submit Lap" button, we find the runner by bib number
        /// and manually insert either a start or finish entry for the current lap,
        /// regardless of disqualification or missed laps.
        /// </summary>
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

                    // 1. Look up runner by bib number
                    var runnerData = GetRunnerDataByBib(connection, bibNumber);
                    if (runnerData == null)
                    {
                        MessageBox.Show($"No runner found with bib number: {bibNumber}",
                                        "Runner Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 2. Extract RFID from runner data
                    string rfid = runnerData["rfid"].ToString();

                    // 3. Manually submit a lap (start or finish) unconditionally
                    ManualLapSubmission(rfid, runnerData, connection);

                    MessageBox.Show($"Manual lap submission for bib {bibNumber} completed.",
                                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    textBoxBibNumber.Text = string.Empty; // Clear input
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Retrieves runner data from the database using the bib number.
        /// </summary>
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

        /// <summary>
        /// Manually simulates either a lap start or lap finish for the current lap,
        /// overriding disqualification and missed-lap checks.
        /// </summary>
        private void ManualLapSubmission(string rfid, DataRow runnerData, MySqlConnection connection)
        {
            lock (runnerStates)
            {
                // Ensure there's a RunnerState entry
                if (!runnerStates.ContainsKey(rfid))
                {
                    runnerStates[rfid] = new RunnerState();
                }

                RunnerState state = runnerStates[rfid];

                // 1. Override disqualification
                if (state.IsDisqualified)
                {
                    // Re-instate the runner
                    state.IsDisqualified = false;
                    Console.WriteLine($"Runner {rfid} was disqualified but has been reinstated via manual submission.");
                }

                // 2. We do NOT check if they've missed a lap or not.
                //    We simply proceed with either a start or finish.

                DateTime currentTimestamp = DateTime.Now;

                if (state.IsExpectingFirstRead)
                {
                    // This simulates the start of the lap
                    string elapsedTime = "00:00:00:00";
                    int lapCount = currentLapNumber;
                    string splitName = null; // No specific split name for start

                    InsertIntoResults(connection, runnerData, currentTimestamp, elapsedTime, lapCount, splitName);

                    // Update RunnerState for the next (finish) read
                    state.IsExpectingFirstRead = false;
                    state.FirstReadTimestamp = currentTimestamp;
                    state.LastProcessedTimestamp = currentTimestamp;

                    // Play beep
                    soundPlayer.Play();
                }
                else
                {
                    // This simulates the finish of the lap
                    TimeSpan elapsed = currentTimestamp - state.FirstReadTimestamp;

                    // Format elapsed time as dd:HH:mm:ss
                    string elapsedTime = $"{(int)elapsed.TotalDays:D2}:{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
                    int lapCount = currentLapNumber;
                    string splitName = $"Lap {currentLapNumber}";

                    // If user typed a custom lap name in textBoxLapNumber, override
                    if (!string.IsNullOrWhiteSpace(textBoxLapNumber.Text))
                    {
                        splitName = $"Lap {textBoxLapNumber.Text.Trim()}";
                    }

                    InsertIntoResults(connection, runnerData, currentTimestamp, elapsedTime, lapCount, splitName);

                    // Mark this lap as finished
                    state.LastLapProcessed = currentLapNumber;
                    state.IsExpectingFirstRead = true;
                    state.FirstReadTimestamp = DateTime.MinValue;
                    state.LastProcessedTimestamp = currentTimestamp;

                    // Play beep
                    soundPlayer.Play();
                }
            }
        }
    }
}
