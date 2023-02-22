using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace VideoClub
{
    public partial class AddLibrarian : Window
    {
        public AddLibrarian()
        {
            InitializeComponent();
        }

        public int Librarian_id { get; set; }

        readonly string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Librarian_id != 0)
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand($"SELECT * FROM Librarian WHERE id = {Librarian_id}", con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    reader.Read();
                    login_textBox.Text = reader["login"].ToString();
                    name_textBox.Text = reader["name"].ToString();
                    phone_textBox.Text = reader["phone"].ToString();
                    reader.Close();
                }
        }

        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            string login = login_textBox.Text;
            string name = name_textBox.Text;
            string phone = phone_textBox.Text;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand($"SELECT * FROM Account WHERE login = N'{login}'", con);

                if (cmd.ExecuteScalar() == null)
                {
                    string password = new Random().Next(100000, 1000000).ToString();
                    cmd = new SqlCommand($"INSERT INTO Account (login, password) VALUES (N'{login}', {password})", con);
                    cmd.ExecuteNonQuery();
                }

                string cmdstr = Librarian_id == 0 ? $"INSERT INTO Librarian (login, name, phone) VALUES (N'{login}', N'{name}', N'{phone}')" :
                    $"UPDATE Librarian SET login = N'{login}', name = N'{name}', phone = N'{phone}' WHERE id = {Librarian_id}";
                cmd = new SqlCommand(cmdstr, con);
                cmd.ExecuteNonQuery();
            }

            DialogResult = true;
            Close();
        }
    }
}