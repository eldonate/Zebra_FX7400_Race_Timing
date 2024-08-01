using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace RaceManager
{
    public partial class NewParticipantsForm : Form
    {
        private string connectionString;
        private DateTime raceDate; // Declare raceDate here

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
            // You can implement any additional logic if needed when a distance is selected
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
    }
}
