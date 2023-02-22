using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace VideoClub
{
    public partial class AddChecking : Window
    {
        public AddChecking()
        {
            InitializeComponent();
        }

        public int Checking_id { get; set; } //при обновлении
        public int Cassette_id { get; set; } //при добавлении

        readonly string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cassette_id_label.Content = Cassette_id.ToString();

            if (Checking_id != 0)
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand($"SELECT * FROM Checking WHERE id = {Checking_id}", con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    reader.Read();
                    checked_datePicker.SelectedDate = Convert.ToDateTime(reader["checked"]);
                    defects_textBox.Text = reader["defects"].ToString();
                    fixed_checkBox.IsChecked = reader["fixed"] is DBNull ? false : Convert.ToBoolean(reader["fixed"]);
                    reader.Close();
                }
            else checked_datePicker.SelectedDate = DateTime.Now;
        }

        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            DateTime checked_dt = checked_datePicker.SelectedDate ?? DateTime.Now;
            string defects = defects_textBox.Text;
            bool is_fixed = fixed_checkBox.IsChecked ?? false;
            string cmdstr = Checking_id == 0 ? $"INSERT INTO Checking (cassette_id, checked, defects, defects_fixed) VALUES ({Cassette_id}, '{checked_dt:yyyy-MM-dd HH:mm:ss}', N'{defects}', '{is_fixed}')" :
                $"UPDATE Checking SET checked = '{checked_dt:yyyy-MM-dd HH:mm:ss}', defects = N'{defects}', defects_fixed = '{is_fixed}' WHERE id = {Checking_id}";

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(cmdstr, con);
                cmd.ExecuteNonQuery();
            }

            DialogResult = true;
            Close();
        }
    }
}