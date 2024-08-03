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
                AddParticipant(age);
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

        private void AddParticipant(int age)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(
                    "INSERT INTO runners (rfid, bib, first_name, last_name, gender, birthday, age, race_name, distance_name, race_date, distance_laps, distance_intervals, distance_id) " +
                    "VALUES (@Rfid, @Bib, @FirstName, @LastName, @Gender, @Birthday, @Age, @RaceName, @DistanceName, @RaceDate, @DistanceLaps, @DistanceIntervals, @DistanceId)", connection);

                command.Parameters.AddWithValue("@Rfid", txtRfid.Text);
                command.Parameters.AddWithValue("@Bib", int.Parse(txtBib.Text));
                command.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                command.Parameters.AddWithValue("@LastName", txtLastName.Text);
                command.Parameters.AddWithValue("@Gender", cmbGender.SelectedItem.ToString());
                command.Parameters.AddWithValue("@Birthday", dtpBirthday.Value);
                command.Parameters.AddWithValue("@Age", age);

                DataRowView selectedRace = (DataRowView)cmbRaces.SelectedItem;
                command.Parameters.AddWithValue("@RaceName", selectedRace["name"].ToString());
                command.Parameters.AddWithValue("@RaceDate", raceDate);

                DataRowView selectedDistance = (DataRowView)cmbDistances.SelectedItem;
                command.Parameters.AddWithValue("@DistanceName", selectedDistance["name"].ToString());
                command.Parameters.AddWithValue("@DistanceLaps", GetDistanceLaps((int)selectedDistance["id"]));
                command.Parameters.AddWithValue("@DistanceIntervals", GetDistanceIntervals((int)selectedDistance["id"]));
                command.Parameters.AddWithValue("@DistanceId", (int)selectedDistance["id"]);

                command.ExecuteNonQuery();
                MessageBox.Show("Participant added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                ImportParticipantsFromFile(filePath);
            }
        }

        private void ImportParticipantsFromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                foreach (string line in lines)
                {
                    var parts = line.Split(',');

                    if (parts.Length < 6)
                    {
                        MessageBox.Show($"Invalid format in line: {line}", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }

                    string rfid = parts[0];
                    int bib = int.Parse(parts[1]);
                    string firstName = parts[2];
                    string lastName = parts[3];
                    string gender = parts[4];
                    DateTime birthday = DateTime.Parse(parts[5]);

                    // Calculate age based on the race date
                    int age = CalculateAge(birthday, raceDate);

                    DataRowView selectedRace = (DataRowView)cmbRaces.SelectedItem;
                    DataRowView selectedDistance = (DataRowView)cmbDistances.SelectedItem;

                    MySqlCommand command = new MySqlCommand(
                        "INSERT INTO runners (rfid, bib, first_name, last_name, gender, birthday, age, race_name, distance_name, race_date, distance_laps, distance_intervals, distance_id) " +
                        "VALUES (@Rfid, @Bib, @FirstName, @LastName, @Gender, @Birthday, @Age, @RaceName, @DistanceName, @RaceDate, @DistanceLaps, @DistanceIntervals, @DistanceId)", connection);

                    command.Parameters.AddWithValue("@Rfid", rfid);
                    command.Parameters.AddWithValue("@Bib", bib);
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Gender", gender);
                    command.Parameters.AddWithValue("@Birthday", birthday);
                    command.Parameters.AddWithValue("@Age", age);
                    command.Parameters.AddWithValue("@RaceName", selectedRace["name"].ToString());
                    command.Parameters.AddWithValue("@RaceDate", raceDate);
                    command.Parameters.AddWithValue("@DistanceName", selectedDistance["name"].ToString());
                    command.Parameters.AddWithValue("@DistanceLaps", GetDistanceLaps((int)selectedDistance["id"]));
                    command.Parameters.AddWithValue("@DistanceIntervals", GetDistanceIntervals((int)selectedDistance["id"]));
                    command.Parameters.AddWithValue("@DistanceId", (int)selectedDistance["id"]);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding participant from line: {line}\nError: {ex.Message}", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                MessageBox.Show("Import completed.", "Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
