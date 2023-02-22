using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace VideoClub
{
    public partial class Main_Admin : Window
    {
        public Main_Admin()
        {
            InitializeComponent();
        }

        readonly string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        SqlDataAdapter ad;
        DataTable dt;

        void FillDGV(string cmdstr)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(cmdstr, con);
                ad = new SqlDataAdapter(cmd);
                dt = new DataTable();

                ad.Fill(dt);
                SqlCommandBuilder bld = new SqlCommandBuilder(ad);
                ad.InsertCommand = bld.GetInsertCommand();
                ad.UpdateCommand = bld.GetUpdateCommand();
                ad.DeleteCommand = bld.GetDeleteCommand();
                dataGridView.ItemsSource = dt.DefaultView;
            }
        }

        private void orders_button_Click(object sender, RoutedEventArgs e)
        {
            FillDGV("SELECT * FROM [Order]");
            Title = "Системный администратор | Заказы";
        }

        private void films_button_Click(object sender, RoutedEventArgs e)
        {
            FillDGV("SELECT * FROM Film");
            Title = "Системный администратор | Фильмы";
        }

        private void cassettes_button_Click(object sender, RoutedEventArgs e)
        {
            FillDGV("SELECT * FROM Cassette");
            Title = "Системный администратор | Кассеты";
        }

        private void checking_button_Click(object sender, RoutedEventArgs e)
        {
            FillDGV("SELECT * FROM Checking");
            Title = "Системный администратор | Контрольные просмотры";
        }

        private void members_button_Click(object sender, RoutedEventArgs e)
        {
            FillDGV("SELECT * FROM Member");
            Title = "Системный администратор | Члены клуба";
        }

        private void librarians_button_Click(object sender, RoutedEventArgs e)
        {
            FillDGV("SELECT * FROM Librarian");
            Title = "Системный администратор | Библиотекари";
        }

        private void accounts_button_Click(object sender, RoutedEventArgs e)
        {
            FillDGV("SELECT * FROM Account");
            Title = "Системный администратор | Учётные записи";
        }

        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridView.ItemsSource == null) return;

            using (SqlConnection con = new SqlConnection(constr))
            {
                ad.InsertCommand.Connection = ad.UpdateCommand.Connection = ad.DeleteCommand.Connection = con;
                ad.Update(dt);
            }
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}