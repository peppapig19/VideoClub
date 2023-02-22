using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace VideoClub
{
    public partial class Members : Page
    {
        public Members()
        {
            InitializeComponent();
        }

        readonly string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FillDGV();
        }

        void FillDGV(string cmdstr = "SELECT Member.*, Account.password FROM Member INNER JOIN Account ON Member.login = Account.login")
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

        private void search_button_Click(object sender, RoutedEventArgs e)
        {
            string cmdstr = $"SELECT Member.*, Account.password FROM Member " +
                $"INNER JOIN Account ON Member.login = Account.login " +
                $"WHERE name LIKE N'%{query_textBox.Text}%'";
            FillDGV(cmdstr);
        }

        private void reset_button_Click(object sender, RoutedEventArgs e)
        {
            query_textBox.Text = string.Empty;
            FillDGV();
        }

        private void create_button_Click(object sender, RoutedEventArgs e)
        {
            AddMember window = new AddMember();
            if (window.ShowDialog() == true) FillDGV();
        }

        private void update_button_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridView.SelectedItem == null) return;

            DataRowView drv = (DataRowView)dataGridView.SelectedItem;
            int id = Convert.ToInt32(drv["id"]);

            AddMember window = new AddMember() { Member_id = id };
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
                SqlCommand cmd = new SqlCommand($"DELETE FROM Member WHERE id = {id}", con);
                cmd.ExecuteNonQuery();
            }

            FillDGV();
        }
    }
}