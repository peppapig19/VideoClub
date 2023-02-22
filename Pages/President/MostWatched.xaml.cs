using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace VideoClub
{
    public partial class MostWatched : Page
    {
        public MostWatched()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT TOP 10 Film.id, Film.name, Film.director, Film.year, COUNT(Film.id) AS times FROM Film " +
                    "INNER JOIN Cassette ON Film.id = Cassette.film_id " +
                    "INNER JOIN [Order] ON Cassette.id = [Order].cassette_id " +
                    "GROUP BY Film.id, Film.name, Film.director, Film.year ORDER BY times DESC", con);
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                ad.Fill(dt);
                dataGridView.ItemsSource = dt.DefaultView;
            }
        }
    }
}