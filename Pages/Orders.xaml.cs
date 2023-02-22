using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace VideoClub
{
    public partial class Orders : Page
    {
        public Orders()
        {
            InitializeComponent();
        }

        readonly string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FillDGV();
        }

        void FillDGV(string cmdstr = "SELECT * FROM [Order]")
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
            if (all_radioButton.IsChecked == true) all_radioButton_Checked(this, new RoutedEventArgs());
            if (ordered_radioButton.IsChecked == true) ordered_radioButton_Checked(this, new RoutedEventArgs());
            if (issued_radioButton.IsChecked == true) issued_radioButton_Checked(this, new RoutedEventArgs());
        }

        private void all_radioButton_Checked(object sender, RoutedEventArgs e)
        {
            FillDGV();
        }

        private void ordered_radioButton_Checked(object sender, RoutedEventArgs e)
        {
            FillDGV("SELECT * FROM [Order] WHERE issued IS NULL");
        }

        private void issued_radioButton_Checked(object sender, RoutedEventArgs e)
        {
            FillDGV("SELECT * FROM [Order] WHERE issued IS NOT NULL AND returned IS NULL");
        }

        private void issue_button_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridView.SelectedItem == null) return;

            DataRowView drv = (DataRowView)dataGridView.SelectedItem;
            int id = Convert.ToInt32(drv["id"]);
            int cassette_id = Convert.ToInt32(drv["cassette_id"]);

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand($"UPDATE [Order] SET issued = GETDATE() WHERE id = {id}", con);
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand($"UPDATE Cassette SET views_left = views_left - 1 WHERE id = {cassette_id}", con);
                cmd.ExecuteNonQuery();
            }

            Update();
        }

        private void return_button_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridView.SelectedItem == null) return;

            DataRowView drv = (DataRowView)dataGridView.SelectedItem;
            int id = Convert.ToInt32(drv["id"]);

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand($"UPDATE [Order] SET returned = GETDATE() WHERE id = {id}", con);
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand($"SELECT ordered, member_id FROM [Order] WHERE id = {id}", con);
                SqlDataReader reader = cmd.ExecuteReader();

                reader.Read();
                DateTime ordered = Convert.ToDateTime(reader["ordered"]);
                int member_id = Convert.ToInt32(reader["member_id"]);
                double expired = (ordered.AddDays(2) - DateTime.Now).TotalDays;

                if (expired < 0)
                {
                    cmd = new SqlCommand($"UPDATE Member SET fund = fund - 10 * {Math.Abs(expired)} WHERE member_id = {member_id}", con);
                    cmd.ExecuteNonQuery();
                }

                reader.Close();
            }

            Update();
        }

        private void update_button_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridView.SelectedItem == null) return;

            DataRowView drv = (DataRowView)dataGridView.SelectedItem;
            int id = Convert.ToInt32(drv["id"]);

            EditOrder window = new EditOrder() { Order_id = id };
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
                SqlCommand cmd = new SqlCommand($"DELETE FROM [Order] WHERE id = {id}", con);
                cmd.ExecuteNonQuery();
            }

            Update();
        }
    }
}