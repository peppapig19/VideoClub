using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace VideoClub
{
    public partial class Cassettes : Page
    {
        public Cassettes()
        {
            InitializeComponent();
        }

        readonly string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FillDGV();
        }

        void FillDGV(string cmdstr = "SELECT Cassette.*, Film.name FROM Cassette INNER JOIN Film ON Cassette.film_id = Film.id")
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
            if (checking_checkBox.IsChecked == true) checking_checkBox_Click(this, new RoutedEventArgs());
            else FillDGV();
        }

        private void search_button_Click(object sender, RoutedEventArgs e)
        {
            string cmdstr = $"SELECT Cassette.*, Film.name FROM Cassette " +
                $"INNER JOIN Film ON Cassette.film_id = Film.id WHERE Film.name LIKE N'%{query_textBox.Text}%'";
            FillDGV(cmdstr);
        }

        private void reset_button_Click(object sender, RoutedEventArgs e)
        {
            query_textBox.Text = string.Empty;
            Update();
        }

        private void checking_checkBox_Click(object sender, RoutedEventArgs e)
        {
            if (checking_checkBox.IsChecked == true)
            {
                string cmdstr = "SELECT Cassette.*, Film.name FROM Cassette " +
                    "INNER JOIN Film ON Cassette.film_id = Film.id " +
                    "WHERE Cassette.views_left < 1";
                FillDGV(cmdstr);
            }
            else FillDGV();
        }

        private void checking_button_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridView.SelectedItem == null) return;

            DataRowView drv = (DataRowView)dataGridView.SelectedItem;
            int id = Convert.ToInt32(drv["id"]);

            AddChecking window = new AddChecking() { Cassette_id = id };
            window.ShowDialog();
        }

        private void update_button_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridView.SelectedItem == null) return;

            DataRowView drv = (DataRowView)dataGridView.SelectedItem;
            int id = Convert.ToInt32(drv["id"]);

            AddCassette window = new AddCassette() { Cassette_id = id };
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
                SqlCommand cmd = new SqlCommand($"DELETE FROM Cassette WHERE id = {id}", con);
                cmd.ExecuteNonQuery();
            }

            Update();
        }
    }
}