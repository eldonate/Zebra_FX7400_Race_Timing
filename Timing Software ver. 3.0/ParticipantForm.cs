using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Windows.Forms;

namespace RaceManager
{
    public partial class ParticipantForm : Form
    {
        private string connectionString;
        private int raceId;
        private int distanceId;
        private DateTime raceDate;

        public ParticipantForm(int raceId, int distanceId, DateTime raceDate, string raceName, string distanceName)
        {
            InitializeComponent();
            this.raceId = raceId;
            this.distanceId = distanceId;
            this.raceDate = raceDate;
            lblRace.Text = $"Race: {raceName}";
            lblDistance.Text = $"Distance: {distanceName}";
            connectionString = GetConnectionString("../../../../dbconfig.txt");
        }

        private string GetConnectionString(string configFile)
        {
            var lines = File.ReadAllLines(configFile);
            var connectionString = new MySqlConnectionStringBuilder();

            foreach (var line in lines)
            {
                var parts = line.Split('=');
                if (parts.Length == 2)
                {
                    switch (parts[0].Trim().ToLower())
                    {
                        case "server":
                            connectionString.Server = parts[1].Trim();
                            break;
                        case "user":
                            connectionString.UserID = parts[1].Trim();
                            break;
                        case "database":
                            connectionString.Database = parts[1].Trim();
                            break;
                        case "port":
                            connectionString.Port = uint.Parse(parts[1].Trim());
                            break;
                        case "password":
                            connectionString.Password = parts[1].Trim();
                            break;
                    }
                }
            }

            return connectionString.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string rfid = txtRFID.Text;
                int bib = int.Parse(txtBib.Text);
                string firstName = txtFirstName.Text;
                string lastName = txtLastName.Text;
                string gender = cmbGender.SelectedItem.ToString();
                DateTime birthday = dtpBirthday.Value;
                int age = raceDate.Year - birthday.Year;
                if (birthday.Date > raceDate.AddYears(-age)) age--;

                int categoryId = GetCategoryId(age);
                if (categoryId == -1)
                {
                    MessageBox.Show("No matching category found for the participant's age.");
                    return;
                }

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO participants (rfid, bib, first_name, last_name, gender, birthday, race_id, distance_id, age, category_id) VALUES (@rfid, @bib, @first_name, @last_name, @gender, @birthday, @race_id, @distance_id, @age, @category_id)", conn);
                    cmd.Parameters.AddWithValue("@rfid", rfid);
                    cmd.Parameters.AddWithValue("@bib", bib);
                    cmd.Parameters.AddWithValue("@first_name", firstName);
                    cmd.Parameters.AddWithValue("@last_name", lastName);
                    cmd.Parameters.AddWithValue("@gender", gender);
                    cmd.Parameters.AddWithValue("@birthday", birthday);
                    cmd.Parameters.AddWithValue("@race_id", raceId);
                    cmd.Parameters.AddWithValue("@distance_id", distanceId);
                    cmd.Parameters.AddWithValue("@age", age);
                    cmd.Parameters.AddWithValue("@category_id", categoryId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Participant added successfully!");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving participant: {ex.Message}");
            }
        }

        private int GetCategoryId(int age)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT id FROM categories WHERE distance_id = @distance_id AND @age BETWEEN start_age AND end_age", conn);
                cmd.Parameters.AddWithValue("@distance_id", distanceId);
                cmd.Parameters.AddWithValue("@age", age);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return reader.GetInt32("id");
                }
                return -1;
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (string line in lines)
                    {
                        string[] data = line.Split(',');
                        if (data.Length == 6)
                        {
                            string rfid = data[0];
                            int bib = int.Parse(data[1]);
                            string firstName = data[2];
                            string lastName = data[3];
                            string gender = data[4];
                            DateTime birthday = DateTime.Parse(data[5]);
                            int age = raceDate.Year - birthday.Year;
                            if (birthday.Date > raceDate.AddYears(-age)) age--;

                            int categoryId = GetCategoryId(age);
                            if (categoryId == -1)
                            {
                                MessageBox.Show($"No matching category found for the participant's age: {firstName} {lastName}");
                                continue;
                            }

                            using (MySqlConnection conn = new MySqlConnection(connectionString))
                            {
                                conn.Open();
                                MySqlCommand cmd = new MySqlCommand("INSERT INTO participants (rfid, bib, first_name, last_name, gender, birthday, race_id, distance_id, age, category_id) VALUES (@rfid, @bib, @first_name, @last_name, @gender, @birthday, @race_id, @distance_id, @age, @category_id)", conn);
                                cmd.Parameters.AddWithValue("@rfid", rfid);
                                cmd.Parameters.AddWithValue("@bib", bib);
                                cmd.Parameters.AddWithValue("@first_name", firstName);
                                cmd.Parameters.AddWithValue("@last_name", lastName);
                                cmd.Parameters.AddWithValue("@gender", gender);
                                cmd.Parameters.AddWithValue("@birthday", birthday);
                                cmd.Parameters.AddWithValue("@race_id", raceId);
                                cmd.Parameters.AddWithValue("@distance_id", distanceId);
                                cmd.Parameters.AddWithValue("@age", age);
                                cmd.Parameters.AddWithValue("@category_id", categoryId);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Invalid data format in line: {line}");
                        }
                    }
                    MessageBox.Show("Participants imported successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error importing participants: {ex.Message}");
                }
            }
        }
    }
}
