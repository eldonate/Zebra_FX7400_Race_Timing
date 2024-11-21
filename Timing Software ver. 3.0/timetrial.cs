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
        public DateTime LastProcessedTimestamp { get; set; } // For distance_intervals check
        public bool IsDisqualified { get; set; } // Indicates if the runner is disqualified

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
            connectionString = GetConnectionString("dbconfig.txt", false);
            cancellationTokenSource = new CancellationTokenSource();
            lastPosition = 0;
            soundPlayer = new SoundPlayer("beep.wav");
            runnerStates = new Dictionary<string, RunnerState>();
            currentLapNumber = 1; // Initialize lap number to 1
            textBoxLapNumber.Text = currentLapNumber.ToString();

            // Connect to database and update status label
            ConnectToDatabase();
        }

        /// <summary>
        /// Reads the connection string from the configuration file.
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
        /// Attempts to connect to the database and updates the connection status label.
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
                    lastPosition = 0; // Reset position when a new file is selected
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
                            var columns = line.Split(',');
                            if (columns.Length < 3)
                                continue; // Ensure there are enough columns

                            if (!DateTime.TryParse(columns[1], out DateTime timestamp))
                                continue; // Invalid timestamp format

                            string rfid = columns[2].Trim();
                            if (string.IsNullOrEmpty(rfid))
                                continue; // Invalid RFID

                            ProcessRfidEntry(rfid, timestamp);
                        }
                        else
                        {
                            // No more lines to read, wait for new data
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
        /// Processes an RFID entry according to the specified logic.
        /// </summary>
        private void ProcessRfidEntry(string rfid, DateTime timestamp)
        {
            lock (runnerStates) // Ensure thread safety
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        // Get runner data by RFID
                        var runnerData = GetRunnerData(connection, rfid);
                        if (runnerData == null) return;

                        // Get distance_intervals value for the runner
                        int distanceIntervals = runnerData["distance_intervals"] != DBNull.Value
                                                ? Convert.ToInt32(runnerData["distance_intervals"])
                                                : 0;

                        // Initialize or get RunnerState
                        if (!runnerStates.ContainsKey(rfid))
                        {
                            runnerStates[rfid] = new RunnerState();
                        }

                        RunnerState state = runnerStates[rfid];

                        // Check if the runner is disqualified
                        if (state.IsDisqualified)
                        {
                            // Runner has missed a lap previously, ignore
                            return;
                        }

                        // Check if the runner has missed a lap
                        if (state.LastLapProcessed < currentLapNumber - 1)
                        {
                            // Runner has missed a lap, disqualify them
                            state.IsDisqualified = true;
                            // Optionally, log or notify about the disqualification
                            LogDisqualification(rfid, state.LastLapProcessed, currentLapNumber);
                            return;
                        }

                        // Check if we can process this RFID for the current lap
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
                                // First read for the lap
                                string elapsedTime = "00:00:00:00";
                                int lapCount = currentLapNumber;
                                string splitName = null;

                                // Insert record
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
                                // Second read for the lap
                                TimeSpan elapsed = timestamp - state.FirstReadTimestamp;

                                if (elapsed.TotalSeconds <= distanceIntervals)
                                {
                                    // Skip this entry due to distance_intervals constraint
                                    return;
                                }

                                string elapsedTime = $"{(int)elapsed.TotalDays:D2}:{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
                                int lapCount = currentLapNumber;
                                string splitName = $"Lap {currentLapNumber}";

                                // Use custom split name if provided
                                if (!string.IsNullOrWhiteSpace(textBoxLapNumber.Text))
                                {
                                    splitName = $"Lap {textBoxLapNumber.Text.Trim()}";
                                }

                                // Insert record
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
                            // Runner has already processed this lap or missed a lap (handled above)
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
            string message = $"Runner with RFID {rfid} has been disqualified. Last completed lap: {lastLapProcessed}, current lap: {currentLap}.";
            // Optionally write to a log file or display a message
            // For demonstration, we'll write to the console
            Console.WriteLine(message);
            // You could also display in a UI element like a list box or status label
        }

        /// <summary>
        /// Retrieves runner data from the database based on RFID.
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
        /// Inserts a new record into the results table with the provided data.
        /// </summary>
        private void InsertIntoResults(MySqlConnection connection, DataRow runnerData, DateTime timestamp, string elapsedTime, int lapCount, string splitName)
        {
            var query = @"INSERT INTO results 
                          (rfid, timestamp, elapsed_time, distance_laps, split_name, first_name, last_name, gender, birthday, age, race_name, distance_name, race_date, distance_intervals, category, bib) 
                          VALUES 
                          (@rfid, @timestamp, @elapsed_time, @distance_laps, @split_name, @first_name, @last_name, @gender, @birthday, @age, @race_name, @distance_name, @race_date, @distance_intervals, @category, @bib)";
            var cmd = new MySqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@rfid", runnerData["rfid"]);
            cmd.Parameters.AddWithValue("@timestamp", timestamp);
            cmd.Parameters.AddWithValue("@elapsed_time", elapsedTime);
            cmd.Parameters.AddWithValue("@distance_laps", lapCount);
            cmd.Parameters.AddWithValue("@split_name", splitName ?? (object)DBNull.Value); // Set to NULL if splitName is null
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

            cmd.ExecuteNonQuery();
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
    }
}
