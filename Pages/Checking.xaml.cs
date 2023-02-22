using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace VideoClub
{
    public partial class Checking : Page
    {
        public Checking()
        {
            InitializeComponent();
        }

        readonly string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FillDGV();
        }

        void FillDGV(string cmdstr = "SELECT * FROM Checking")
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

        private void update_button_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridView.SelectedItem == null) return;

            DataRowView drv = (DataRowView)dataGridView.SelectedItem;
            int id = Convert.ToInt32(drv["id"]);

            AddChecking window = new AddChecking() { Checking_id = id };
            if (window.ShowDialog() == true) FillDGV();
        }

        private void delete_button_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridView.SelectedItem == null) return;

            DataRowView drv = (DataRowView)dataGridView.SelectedItem;
            int id = Convert.ToInt32(drv["id"]);

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand($"DELETE FROM Checking WHERE id = {id}", con);
                cmd.ExecuteNonQuery();
            }

            FillDGV();
        }
    }
}