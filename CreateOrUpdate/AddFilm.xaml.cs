using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace VideoClub
{
    public partial class AddFilm : Window
    {
        public AddFilm()
        {
            InitializeComponent();
        }

        public int Film_id { get; set; }

        readonly string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Film_id != 0)
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand($"SELECT * FROM Film WHERE id = {Film_id}", con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    reader.Read();
                    name_textBox.Text = reader["name"].ToString();
                    director_textBox.Text = reader["director"].ToString();
                    actor1_textBox.Text = reader["actor1"].ToString();
                    actor2_textBox.Text = reader["actor2"].ToString();
                    year_textBox.Text = reader["year"].ToString();
                    studio_textBox.Text = reader["studio"].ToString();
                    description_textBox.Text = reader["description"].ToString();
                    reader.Close();
                }
        }

        public void save_button_Click(object sender, RoutedEventArgs e)
        {
            string name = name_textBox.Text;
            string director = director_textBox.Text;
            string actor1 = actor1_textBox.Text;
            string actor2 = actor2_textBox.Text;
            string year = year_textBox.Text;
            string studio = studio_textBox.Text;
            string description = description_textBox.Text;
            string cmdstr = Film_id == 0 ? $"INSERT INTO Film (name, director, actor1, actor2, year, studio, description) VALUES (N'{name}', N'{director}', N'{actor1}', N'{actor2}', {year}, N'{studio}', N'{description}')" :
                $"UPDATE Film SET name = N'{name}', director = N'{director}', actor1 = N'{actor1}', actor2 = N'{actor2}', year = {year}, studio = N'{studio}', description = N'{description}' WHERE id = {Film_id}";

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