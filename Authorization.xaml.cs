using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace VideoClub
{
    public partial class Authorization : Window
    {
        public Authorization()
        {
            InitializeComponent();
        }

        public void login_button_Click(object sender, RoutedEventArgs e)
        {
            string login = login_textBox.Text;
            string password = passwordBox.Password;
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand($"SELECT * FROM Account WHERE login = N'{login}' AND password = N'{password}'", con);
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    MessageBox.Show("Неправильный логин или пароль.");
                    return;
                }

                reader.Read();

                if (!(reader["is_president"] is DBNull) && Convert.ToBoolean(reader["is_president"]))
                {
                    Main_President president = new Main_President();
                    president.Show();
                    Hide();
                    return;
                }

                if (!(reader["is_sysadmin"] is DBNull) && Convert.ToBoolean(reader["is_sysadmin"]))
                {
                    Main_Admin admin = new Main_Admin();
                    admin.Show();
                    Hide();
                    return;
                }

                reader.Close();

                cmd = new SqlCommand($"SELECT id FROM Librarian WHERE login = N'{login}'", con);

                if (cmd.ExecuteScalar() != null)
                {
                    Main_Librarian librarian = new Main_Librarian();
                    librarian.Show();
                    Hide();
                    return;
                }

                cmd = new SqlCommand($"SELECT id FROM Member WHERE login = N'{login}'", con);
                int id = Convert.ToInt32(cmd.ExecuteScalar());

                if (id != 0)
                {
                    Main_Member member = new Main_Member() { Member_id = id };
                    member.Show();
                    Hide();
                    return;
                }
            }
        }
    }
}