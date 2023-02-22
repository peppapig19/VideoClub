using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace VideoClub
{
    public partial class ToReplace : Page
    {
        public ToReplace()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT Cassette.id, Cassette.views_left, Unavailable.defects FROM Cassette " +
                    $"LEFT JOIN " +
                    $"(SELECT cassette_id, defects FROM [Order] " +
                    $"WHERE defects IS NOT NULL AND defects_fixed = 0 " +
                    $"UNION " +
                    $"SELECT cassette_id, defects FROM Checking " +
                    $"WHERE defects IS NOT NULL AND defects_fixed = 0) Unavailable " +
                    $"ON Cassette.id = Unavailable.cassette_id " +
                    $"WHERE Unavailable.cassette_id IS NOT NULL OR Cassette.views_left < 1", con);
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                ad.Fill(dt);
                dataGridView.ItemsSource = dt.DefaultView;
            }
        }
    }
}