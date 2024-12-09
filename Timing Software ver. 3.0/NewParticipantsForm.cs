using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace RaceManager
{
    public partial class NewParticipantsForm : Form
    {
        private string connectionString;
        private DateTime raceDate;

        public NewParticipantsForm()
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
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT id, name, date FROM races", connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                cmbRaces.DisplayMember = "name";
                cmbRaces.ValueMember = "id";
                cmbRaces.DataSource = dataTable;
            }
        }

        private void cmbRaces_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRaces.SelectedValue != null)
            {
                int raceId = (int)cmbRaces.SelectedValue;
                LoadDistances(raceId);
                GetRaceDate(raceId);
            }
        }

        private void LoadDistances(int raceId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT id, name FROM distances WHERE race_id = @RaceId", connection);
                adapter.SelectCommand.Parameters.AddWithValue("@RaceId", raceId);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                cmbDistances.DisplayMember = "name";
                cmbDistances.ValueMember = "id";
                cmbDistances.DataSource = dataTable;
            }
        }

        private void GetRaceDate(int raceId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("SELECT date FROM races WHERE id = @RaceId", connection);
                command.Parameters.AddWithValue("@RaceId", raceId);
                raceDate = (DateTime)command.ExecuteScalar();
            }
        }

        private void cmbDistances_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Handle any logic needed when a distance is selected
            // This could involve updating related fields or calculations
            MessageBox.Show("Selected Distance Changed", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAddParticipant_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                int age = CalculateAge(dtpBirthday.Value, raceDate);

                DataRowView selectedRace = (DataRowView)cmbRaces.SelectedItem;
                DataRowView selectedDistance = (DataRowView)cmbDistances.SelectedItem;

                string raceName = selectedRace["name"].ToString();
                string distanceName = selectedDistance["name"].ToString();

                // Get the category name using the correct parameters
                string categoryName = GetCategory(age, raceName, distanceName);

                // Pass the category name to AddParticipant
                AddParticipant(age, raceName, distanceName, categoryName);
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtRfid.Text) ||
                string.IsNullOrWhiteSpace(txtBib.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                cmbGender.SelectedItem == null ||
                cmbRaces.SelectedItem == null ||
                cmbDistances.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private int CalculateAge(DateTime birthday, DateTime raceDate)
        {
            int age = raceDate.Year - birthday.Year;
            if (raceDate < birthday.AddYears(age))
                age--;

            return age;
        }

        private void AddParticipant(int age, string raceName, string distanceName, string categoryName)
        {
            DataRowView selectedDistance = (DataRowView)cmbDistances.SelectedItem;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(
                    "INSERT INTO runners (rfid, bib, first_name, last_name, gender, birthday, age, race_name, distance_name, race_date, distance_laps, distance_intervals, distance_id, category, team) " +
                    "VALUES (@Rfid, @Bib, @FirstName, @LastName, @Gender, @Birthday, @Age, @RaceName, @DistanceName, @RaceDate, @DistanceLaps, @DistanceIntervals, @DistanceId, @Category, @Team)", connection);

                command.Parameters.AddWithValue("@Rfid", txtRfid.Text.Trim());

                // Ensure that the Bib is a valid integer
                if (!int.TryParse(txtBib.Text.Trim(), out int bibNumber))
                {
                    MessageBox.Show("Bib number must be a valid integer.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                command.Parameters.AddWithValue("@Bib", bibNumber);

                command.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                command.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                command.Parameters.AddWithValue("@Gender", cmbGender.SelectedItem.ToString());
                command.Parameters.AddWithValue("@Birthday", dtpBirthday.Value);
                command.Parameters.AddWithValue("@Age", age);
                command.Parameters.AddWithValue("@RaceName", raceName);
                command.Parameters.AddWithValue("@DistanceName", distanceName);
                command.Parameters.AddWithValue("@RaceDate", raceDate);
                command.Parameters.AddWithValue("@DistanceLaps", GetDistanceLaps((int)selectedDistance["id"]));
                command.Parameters.AddWithValue("@DistanceIntervals", GetDistanceIntervals((int)selectedDistance["id"]));
                command.Parameters.AddWithValue("@DistanceId", (int)selectedDistance["id"]);
                command.Parameters.AddWithValue("@Category", categoryName);

                // Handle team assignment
                string teamName = string.IsNullOrWhiteSpace(txtTeam.Text) ? "Individual" : txtTeam.Text.Trim();
                command.Parameters.AddWithValue("@Team", teamName);

                try
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show("Participant added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding participant: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private int GetDistanceLaps(int distanceId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("SELECT laps FROM distances WHERE id = @DistanceId", connection);
                command.Parameters.AddWithValue("@DistanceId", distanceId);
                return (int)command.ExecuteScalar();
            }
        }

        private int GetDistanceIntervals(int distanceId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("SELECT intervals FROM distances WHERE id = @DistanceId", connection);
                command.Parameters.AddWithValue("@DistanceId", distanceId);
                return (int)command.ExecuteScalar();
            }
        }

        // Import participants from a file
        private void btnImportParticipants_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|CSV files (*.csv)|*.csv|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                ImportParticipantsFromFile(filePath);
            }
        }


        private void ImportParticipantsFromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            int successCount = 0;
            int errorCount = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                foreach (string line in lines)
                {
                    // Skip empty lines
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parts = line.Split(',');

                    if (parts.Length < 7)
                    {
                        // Optionally log the error instead of showing a message box
                        // For simplicity, we'll increment the error count
                        errorCount++;
                        continue;
                    }

                    string rfid = parts[0].Trim();
                    if (!int.TryParse(parts[1].Trim(), out int bib))
                    {
                        errorCount++;
                        continue;
                    }
                    string firstName = parts[2].Trim();
                    string lastName = parts[3].Trim();
                    string gender = parts[4].Trim();
                    if (!DateTime.TryParse(parts[5].Trim(), out DateTime birthday))
                    {
                        errorCount++;
                        continue;
                    }
                    string team = parts[6].Trim();

                    // Calculate age based on the race date
                    int age = CalculateAge(birthday, raceDate);

                    // Get race and distance information
                    DataRowView selectedRace = (DataRowView)cmbRaces.SelectedItem;
                    DataRowView selectedDistance = (DataRowView)cmbDistances.SelectedItem;
                    string raceName = selectedRace["name"].ToString();
                    string distanceName = selectedDistance["name"].ToString();
                    string categoryName = GetCategory(age, raceName, distanceName);

                    // Default team to "Individual" if empty
                    team = string.IsNullOrWhiteSpace(team) ? "Individual" : team;

                    // Insert participant
                    MySqlCommand command = new MySqlCommand(
                        "INSERT INTO runners (rfid, bib, first_name, last_name, gender, birthday, age, race_name, distance_name, race_date, distance_laps, distance_intervals, distance_id, category, team) " +
                        "VALUES (@Rfid, @Bib, @FirstName, @LastName, @Gender, @Birthday, @Age, @RaceName, @DistanceName, @RaceDate, @DistanceLaps, @DistanceIntervals, @DistanceId, @Category, @Team)", connection);

                    command.Parameters.AddWithValue("@Rfid", rfid);
                    command.Parameters.AddWithValue("@Bib", bib);
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Gender", gender);
                    command.Parameters.AddWithValue("@Birthday", birthday);
                    command.Parameters.AddWithValue("@Age", age);
                    command.Parameters.AddWithValue("@RaceName", raceName);
                    command.Parameters.AddWithValue("@DistanceName", distanceName);
                    command.Parameters.AddWithValue("@RaceDate", raceDate);
                    command.Parameters.AddWithValue("@DistanceLaps", GetDistanceLaps((int)selectedDistance["id"]));
                    command.Parameters.AddWithValue("@DistanceIntervals", GetDistanceIntervals((int)selectedDistance["id"]));
                    command.Parameters.AddWithValue("@DistanceId", (int)selectedDistance["id"]);
                    command.Parameters.AddWithValue("@Category", categoryName);
                    command.Parameters.AddWithValue("@Team", team);

                    try
                    {
                        command.ExecuteNonQuery();
                        successCount++;
                    }
                    catch
                    {
                        errorCount++;
                        // Optionally log the error details
                    }
                }
            }

            string message = $"Import completed.\nSuccessfully imported: {successCount}\nFailed to import: {errorCount}";
            MessageBox.Show(message, "Import Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }



        private string GetCategory(int age, string raceName, string distanceName)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Get race ID
                int raceId;
                MySqlCommand getRaceIdCommand = new MySqlCommand("SELECT id FROM races WHERE name = @RaceName", connection);
                getRaceIdCommand.Parameters.AddWithValue("@RaceName", raceName);
                object raceIdResult = getRaceIdCommand.ExecuteScalar();
                if (raceIdResult == null)
                {
                    throw new Exception($"Race with name '{raceName}' not found.");
                }
                raceId = Convert.ToInt32(raceIdResult);

                // Get distance ID
                int distanceId;
                MySqlCommand getDistanceIdCommand = new MySqlCommand("SELECT id FROM distances WHERE name = @DistanceName AND race_id = @RaceId", connection);
                getDistanceIdCommand.Parameters.AddWithValue("@DistanceName", distanceName);
                getDistanceIdCommand.Parameters.AddWithValue("@RaceId", raceId);
                object distanceIdResult = getDistanceIdCommand.ExecuteScalar();
                if (distanceIdResult == null)
                {
                    throw new Exception($"Distance with name '{distanceName}' and race ID '{raceId}' not found.");
                }
                distanceId = Convert.ToInt32(distanceIdResult);

                // Get category
                MySqlCommand getCategoryCommand = new MySqlCommand("SELECT name FROM categories WHERE distance_id = @DistanceId AND start_age <= @Age AND end_age >= @Age", connection);
                getCategoryCommand.Parameters.AddWithValue("@DistanceId", distanceId);
                getCategoryCommand.Parameters.AddWithValue("@Age", age);
                object categoryResult = getCategoryCommand.ExecuteScalar();
                return categoryResult != null ? categoryResult.ToString() : "Unknown";
            }
        }
    }
}
