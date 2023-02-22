using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace VideoClub
{
    public partial class Films : Page
    {
        public Films()
        {
            InitializeComponent();
        }

        public int Member_id { get; set; }

        readonly string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Member_id != 0) action_panel.Visibility = Visibility.Collapsed;
            else order_button.Visibility = Visibility.Collapsed;

            FillDGV();
        }

        void FillDGV(string cmdstr = "SELECT * FROM Film")
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(cmdstr, con);
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                ad.Fill(dt);
                dataGridView.ItemsSource = dt.DefaultView;
            }
        }

        void Update()
        {
            if (available_checkBox.IsChecked == true) available_checkBox_Click(this, new RoutedEventArgs());
            else FillDGV();
        }

        private void available_checkBox_Click(object sender, RoutedEventArgs e)
        {
            if (available_checkBox.IsChecked == true)
            {
                string cmdstr = "SELECT DISTINCT Film.* FROM Film " +
                    "INNER JOIN " +
                    "(SELECT Cassette.id, Cassette.film_id, Cassette.views_left FROM Cassette " +
                    "LEFT JOIN " +
                    "(SELECT cassette_id FROM [Order] " +
                    "WHERE returned IS NULL OR (defects IS NOT NULL AND defects_fixed = 0) " +
                    "UNION " +
                    "SELECT cassette_id FROM Checking " +
                    "WHERE defects IS NOT NULL AND defects_fixed = 0) Unavailable " +
                    "ON Cassette.id = Unavailable.cassette_id " +
                    "WHERE Unavailable.cassette_id IS NULL AND Cassette.views_left > 0) Available " +
                    "ON Film.id = Available.film_id";
                FillDGV(cmdstr);
            }
            else FillDGV();
        }

        public void search_button_Click(object sender, RoutedEventArgs e)
        {
            string term = string.Empty;
            
            switch (term_comboBox.Text)
            {
                case "название":
                    term = "name";
                    break;
                case "режиссёр":
                    term = "director";
                    break;
                case "актёр":
                    term = "actor1";
                    break;
                case "год":
                    term = "year";
                    break;
                case "киностудия":
                    term = "studio";
                    break;
                case "описание":
                    term = "description";
                    break;
            }

            string cmdstr = $"SELECT * FROM Film WHERE {term} LIKE N'%{query_textBox.Text}%'";
            if (term == "actor1") cmdstr += $" OR actor2 LIKE N'%{query_textBox.Text}%'";

            FillDGV(cmdstr);
        }

        private void reset_button_Click(object sender, RoutedEventArgs e)
        {
            query_textBox.Text = string.Empty;
            Update();
        }

        private void cassette_button_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridView.SelectedItem == null) return;

            DataRowView drv = (DataRowView)dataGridView.SelectedItem;
            int id = Convert.ToInt32(drv["id"]);

            AddCassette window = new AddCassette() { Film_id = id };
            if (window.ShowDialog() == true) Update();
        }

        private void create_button_Click(object sender, RoutedEventArgs e)
        {
            AddFilm window = new AddFilm();
            if (window.ShowDialog() == true) Update();
        }

        private void update_button_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridView.SelectedItem == null) return;

            DataRowView drv = (DataRowView)dataGridView.SelectedItem;
            int id = Convert.ToInt32(drv["id"]);

            AddFilm window = new AddFilm() { Film_id = id };
            if (window.ShowDialog() == true) Update();
        }

        private void delete_button_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridView.SelectedItem == null) return;

            DataRowView drv = (DataRowView)dataGridView.SelectedItem;
            int id = Convert.ToInt32(drv["id"]);

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand($"DELETE FROM Film WHERE id = {id}", con);
                cmd.ExecuteNonQuery();
            }

            Update();
        }

        private void order_button_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand($"SELECT * FROM [Order] WHERE member_id = {Member_id} AND returned IS NULL", con);

                if (cmd.ExecuteScalar() != null)
                {
                    MessageBox.Show("На руки выдаётся не более 1 кассеты.");
                    return;
                }

                cmd = new SqlCommand($"SELECT fund FROM Member WHERE id = {Member_id}", con);
                int fund = Convert.ToInt32(cmd.ExecuteScalar());

                if (fund < 50)
                {
                    MessageBox.Show("В вашем страховом фонде недостаточно средств.");
                    return;
                }

                DataRowView drv = (DataRowView)dataGridView.SelectedItem;
                int film_id = Convert.ToInt32(drv["id"]);

                cmd = new SqlCommand($"SELECT Cassette.id, Cassette.film_id, Cassette.views_left FROM Cassette " +
                    $"LEFT JOIN " +
                    $"(SELECT cassette_id FROM [Order] " +
                    $"WHERE returned IS NULL OR (defects IS NOT NULL AND defects_fixed = 0) " +
                    $"UNION " +
                    $"SELECT cassette_id FROM Checking " +
                    $"WHERE defects IS NOT NULL AND defects_fixed = 0) Unavailable " +
                    $"ON Cassette.id = Unavailable.cassette_id " +
                    $"WHERE Unavailable.cassette_id IS NULL AND Cassette.views_left > 0 AND Cassette.film_id = {film_id}", con);
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    MessageBox.Show("Свободных кассет с этим фильмом пока нет.");
                    return;
                }

                reader.Read();
                int cassette_id = Convert.ToInt32(reader["id"]);
                reader.Close();

                cmd = new SqlCommand($"INSERT INTO [Order] (cassette_id, member_id, ordered) VALUES ({cassette_id}, {Member_id}, '{DateTime.Now:yyyy-MM-dd HH:mm:ss}')", con);
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand($"UPDATE Member SET fund = fund - 10 WHERE id = {Member_id}", con);
                cmd.ExecuteNonQuery();

                MessageBox.Show($"Вы заказали кассету n.{cassette_id}. Заберите её у библиотекаря.");
                Update();
            }
        }
    }
}