using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace VideoClub
{
    public partial class Profile : Window
    {
        public Profile()
        {
            InitializeComponent();
        }

        public int Id { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand($"SELECT * FROM Member WHERE id = {Id}", con);
                SqlDataReader reader = cmd.ExecuteReader();

                reader.Read();
                fund_label.Content = Convert.ToInt32(reader["fund"]);
                reader.Close();

                cmd = new SqlCommand($"SELECT Cassette.id FROM Cassette " +
                    $"INNER JOIN " +
                    $"(SELECT * FROM [Order] WHERE member_id = {Id} AND ordered IS NOT NULL AND returned IS NULL) Not_Returned " +
                    $"ON Cassette.id = Not_Returned.cassette_id", con);
                object cassette_id = cmd.ExecuteScalar();

                if (cassette_id != null)
                {
                    cassette_label.Content = Convert.ToInt32(cassette_id);

                    cmd = new SqlCommand($"SELECT ordered FROM [Order] WHERE member_id = {Id} AND cassette_id = {cassette_id}", con);
                    object ordered = cmd.ExecuteScalar();

                    TimeSpan term = Convert.ToDateTime(ordered).AddDays(2) - DateTime.Now;
                    term_label.Content = (term.Days + 1).ToString();
                }
            }
        }
    }
}