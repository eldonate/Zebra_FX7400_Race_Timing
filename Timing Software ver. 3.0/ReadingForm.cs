using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RaceManager
{
    public partial class ReadingForm : Form
    {
        private string connectionString;
        private string selectedFilePath;
        private long lastMaxOffset = 0;
        private SoundPlayer player;
        private DateTime? selectedDistanceStartTime = null;
        private System.Windows.Forms.Timer realTimeDurationTimer;

        public ReadingForm()
        {
            InitializeComponent();
            connectionString = GetConnectionString("dbconfig.txt");
            InitializeDataGridView();
            player = new SoundPlayer("beep.wav"); // Load the WAV file
            LoadRaces();
            InitializeRealTimeDurationTimer();
            lblRealTimeDuration.Text = string.Empty; // Initialize label to be empty
        }

        private void InitializeDataGridView()
        {
            dataGridView.ColumnCount = 14;
            dataGridView.Columns[0].Name = "Gap";
            dataGridView.Columns[1].Name = "Timestamp";
            dataGridView.Columns[2].Name = "RFID";
            dataGridView.Columns[3].Name = "Bib";
            dataGridView.Columns[4].Name = "First Name";
            dataGridView.Columns[5].Name = "Last Name";
            dataGridView.Columns[6].Name = "Gender";
            dataGridView.Columns[7].Name = "Birthday";
            dataGridView.Columns[8].Name = "Race ID";
            dataGridView.Columns[9].Name = "Distance ID";
            dataGridView.Columns[10].Name = "Age";
            dataGridView.Columns[11].Name = "Category ID";
            dataGridView.Columns[12].Name = "Elapsed Time";
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
                MySqlCommand cmd = new MySqlCommand("SELECT id, name, start_time FROM distances WHERE race_id = @race_id", conn);
                cmd.Parameters.AddWithValue("@race_id", raceId);
                MySqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                cmbDistances.DisplayMember = "name";
                cmbDistances.ValueMember = "id";
                cmbDistances.DataSource = dt;

                // Reset selectedDistanceStartTime and real-time duration label
                selectedDistanceStartTime = null;
                lblRealTimeDuration.Text = string.Empty;
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
            if (cmbDistances.SelectedValue != null)
            {
                int distanceId = (int)cmbDistances.SelectedValue;

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT start_time FROM distances WHERE id = @distance_id", conn);
                    cmd.Parameters.AddWithValue("@distance_id", distanceId);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read() && !reader.IsDBNull(0))
                    {
                        selectedDistanceStartTime = reader.GetDateTime(0);
                    }
                    else
                    {
                        selectedDistanceStartTime = null;
                    }
                }

                lblRealTimeDuration.Text = selectedDistanceStartTime.HasValue
                    ? $"Real-Time Duration: {(DateTime.Now - selectedDistanceStartTime.Value).Days}d {(DateTime.Now - selectedDistanceStartTime.Value):hh\\:mm\\:ss}"
                    : "Real-Time Duration: N/A";
            }
        }

        private void InitializeRealTimeDurationTimer()
        {
            realTimeDurationTimer = new System.Windows.Forms.Timer();
            realTimeDurationTimer.Interval = 1000; // Update every second
            realTimeDurationTimer.Tick += RealTimeDurationTimer_Tick;
            realTimeDurationTimer.Start();
        }

        private void RealTimeDurationTimer_Tick(object sender, EventArgs e)
        {
            if (selectedDistanceStartTime.HasValue)
            {
                TimeSpan duration = DateTime.Now - selectedDistanceStartTime.Value;
                lblRealTimeDuration.Text = $"Real-Time Duration: {duration.Days}d {duration:hh\\:mm\\:ss}";
            }
            else
            {
                lblRealTimeDuration.Text = "Real-Time Duration: N/A";
            }
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = openFileDialog.FileName;
                lastMaxOffset = 0;
                Task.Run(() => MonitorFile());
            }
        }

        private async Task MonitorFile()
        {
            while (true)
            {
                if (!string.IsNullOrEmpty(selectedFilePath))
                {
                    using (var stream = new FileStream(selectedFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        stream.Seek(lastMaxOffset, SeekOrigin.Begin);
                        using (var reader = new StreamReader(stream))
                        {
                            string line;
                            while ((line = await reader.ReadLineAsync()) != null)
                            {
                                ProcessLine(line);
                                lastMaxOffset = stream.Position;
                            }
                        }
                    }
                }
                await Task.Delay(1000); // Check for new lines every second
            }
        }

        private void ProcessLine(string line)
        {
            var parts = line.Split(',');
            if (parts.Length >= 3)
            {
                string gapStr = parts[0];
                string timestampStr = parts[1];
                string rfid = parts[2];

                if (float.TryParse(gapStr, out float gap) && DateTime.TryParse(timestampStr, out DateTime timestamp))
                {
                    if (!RecordExists(rfid))
                    {
                        AddToDataGridViewAndDatabase(gap, timestamp, rfid);
                    }
                }
            }
        }

        private bool RecordExists(string rfid)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM readings WHERE rfid = @rfid", conn);
                cmd.Parameters.AddWithValue("@rfid", rfid);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        private void AddToDataGridViewAndDatabase(float gap, DateTime timestamp, string rfid)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT p.*, d.start_time FROM participants p JOIN distances d ON p.distance_id = d.id WHERE p.rfid = @rfid", conn);
                cmd.Parameters.AddWithValue("@rfid", rfid);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var bib = reader.GetInt32("bib");
                    var firstName = reader.GetString("first_name");
                    var lastName = reader.GetString("last_name");
                    var gender = reader.GetString("gender");
                    var birthday = reader.GetDateTime("birthday");
                    var raceId = reader.GetInt32("race_id");
                    var distanceId = reader.GetInt32("distance_id");
                    var age = reader.GetInt32("age");
                    var categoryId = reader.GetInt32("category_id");
                    var startTime = reader.IsDBNull(reader.GetOrdinal("start_time")) ? (DateTime?)null : reader.GetDateTime("start_time");

                    // Calculate elapsed time
                    DateTime distanceStartTime = startTime ?? DateTime.Now;
                    TimeSpan elapsedTimeSpan = timestamp - distanceStartTime;

                    // Convert elapsed time to DD:HH:MM:SS
                    string elapsedTime = FormatTimeSpanToDDHHMMSS(elapsedTimeSpan);

                    // Add to DataGridView
                    var row = new string[]
                    {
                        gap.ToString(),
                        timestamp.ToString(),
                        rfid,
                        bib.ToString(),
                        firstName,
                        lastName,
                        gender,
                        birthday.ToShortDateString(),
                        raceId.ToString(),
                        distanceId.ToString(),
                        age.ToString(),
                        categoryId.ToString(),
                        elapsedTime // Formatted time
                    };

                    this.Invoke(new MethodInvoker(() =>
                    {
                        dataGridView.Rows.Add(row);
                        // Play beep sound after adding the record
                        player.Play();
                    }));

                    reader.Close();

                    // Insert into database
                    cmd = new MySqlCommand("INSERT INTO readings (rfid, timestamp, gap, bib, first_name, last_name, gender, birthday, race_id, distance_id, age, category_id, elapsed_time) VALUES (@rfid, @timestamp, @gap, @bib, @first_name, @last_name, @gender, @birthday, @race_id, @distance_id, @age, @category_id, @elapsed_time)", conn);
                    cmd.Parameters.AddWithValue("@rfid", rfid);
                    cmd.Parameters.AddWithValue("@timestamp", timestamp);
                    cmd.Parameters.AddWithValue("@gap", gap);
                    cmd.Parameters.AddWithValue("@bib", bib);
                    cmd.Parameters.AddWithValue("@first_name", firstName);
                    cmd.Parameters.AddWithValue("@last_name", lastName);
                    cmd.Parameters.AddWithValue("@gender", gender);
                    cmd.Parameters.AddWithValue("@birthday", birthday);
                    cmd.Parameters.AddWithValue("@race_id", raceId);
                    cmd.Parameters.AddWithValue("@distance_id", distanceId);
                    cmd.Parameters.AddWithValue("@age", age);
                    cmd.Parameters.AddWithValue("@category_id", categoryId);
                    cmd.Parameters.AddWithValue("@elapsed_time", elapsedTime); // Store formatted time
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private string FormatTimeSpanToDDHHMMSS(TimeSpan timeSpan)
        {
            int days = (int)timeSpan.TotalDays;
            int hours = timeSpan.Hours;
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;

            return $"{days:D2}:{hours:D2}:{minutes:D2}:{seconds:D2}";
        }

        private void btnSetCurrentTime_Click(object sender, EventArgs e)
        {
            if (cmbDistances.SelectedValue != null)
            {
                DateTime startTime = DateTime.Now;
                SetStartTime(startTime);
                MessageBox.Show("Start time set to current time.");
            }
            else
            {
                MessageBox.Show("Please select a distance.");
            }
        }

        private void btnSetSelectedRecordTime_Click(object sender, EventArgs e)
        {
            if (cmbDistances.SelectedValue != null && dataGridView.SelectedRows.Count > 0)
            {
                DateTime startTime = Convert.ToDateTime(dataGridView.SelectedRows[0].Cells["Timestamp"].Value);
                SetStartTime(startTime);
                MessageBox.Show("Start time set to selected record time.");
            }
            else
            {
                MessageBox.Show("Please select a distance and a record from the DataGridView.");
            }
        }

        private void positions_update_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Call UpdateGenderPositions
                    using (MySqlCommand cmd = new MySqlCommand("CALL UpdateGenderPosition();", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Call UpdateGeneralPosition
                    using (MySqlCommand cmd = new MySqlCommand("CALL UpdateGeneralPosition();", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Positions updated successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void SetStartTime(DateTime startTime)
        {
            if (cmbDistances.SelectedValue != null)
            {
                int distanceId = (int)cmbDistances.SelectedValue;
                selectedDistanceStartTime = startTime;

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("UPDATE distances SET start_time = @start_time WHERE id = @distance_id", conn);
                    cmd.Parameters.AddWithValue("@start_time", startTime);
                    cmd.Parameters.AddWithValue("@distance_id", distanceId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void btnClearDataGrid_Click(object sender, EventArgs e)
        {
            dataGridView.Rows.Clear();
        }
        private void btnSetRaceStartTime_Click(object sender, EventArgs e)
        {
            if (cmbDistances.SelectedValue != null)
            {
                DateTime selectedStartTime = dtpRaceStartTime.Value;
                SetDistanceStartTime(selectedStartTime);
                MessageBox.Show("Distance start time set to the selected time.");
            }
            else
            {
                MessageBox.Show("Please select a distance.");
            }
        }
        private void SetDistanceStartTime(DateTime startTime)
        {
            if (cmbDistances.SelectedValue != null)
            {
                int distanceId = (int)cmbDistances.SelectedValue;

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("UPDATE distances SET start_time = @start_time WHERE id = @distance_id", conn);
                    cmd.Parameters.AddWithValue("@start_time", startTime);
                    cmd.Parameters.AddWithValue("@distance_id", distanceId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void SetRaceStartTime(DateTime startTime)
        {
            if (cmbRaces.SelectedValue != null)
            {
                int raceId = (int)cmbRaces.SelectedValue;

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("UPDATE races SET start_time = @start_time WHERE id = @race_id", conn);
                    cmd.Parameters.AddWithValue("@start_time", startTime);
                    cmd.Parameters.AddWithValue("@race_id", raceId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
