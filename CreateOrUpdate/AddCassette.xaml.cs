using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace VideoClub
{
    public partial class AddCassette : Window
    {
        public AddCassette()
        {
            InitializeComponent();
        }

        public int Cassette_id { get; set; } //при обновлении
        public int Film_id { get; set; } //при добавлении

        readonly string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            film_id_label.Content = Film_id.ToString();

            if (Cassette_id != 0)
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand($"SELECT * FROM Cassette WHERE id = {Cassette_id}", con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    reader.Read();
                    views_textBox.Text = reader["views_left"].ToString();
                    reader.Close();
                }
        }

        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            int views_left = Convert.ToInt32(views_textBox.Text);
            string cmdstr = Cassette_id == 0 ? $"INSERT INTO Cassette (film_id, views_left) VALUES ({Film_id}, {views_left})" :
                $"UPDATE Cassette SET views_left = {views_left} WHERE id = {Cassette_id}";

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