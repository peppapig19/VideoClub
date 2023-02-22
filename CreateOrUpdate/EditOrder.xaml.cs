using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace VideoClub
{
    public partial class EditOrder : Window
    {
        public EditOrder()
        {
            InitializeComponent();
        }

        public int Order_id { get; set; }

        readonly string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand($"SELECT * FROM [Order] WHERE id = {Order_id}", con);
                SqlDataReader reader = cmd.ExecuteReader();

                reader.Read();
                cassette_id_label.Content = reader["cassette_id"].ToString();
                member_id_label.Content = reader["member_id"].ToString();
                ordered_id_label.Content = reader["ordered"].ToString();
                if (!(reader["issued"] is DBNull)) issued_datePicker.SelectedDate = Convert.ToDateTime(reader["issued"]);
                if (!(reader["returned"] is DBNull)) returned_datePicker.SelectedDate = Convert.ToDateTime(reader["returned"]);
                defects_textBox.Text = reader["defects"].ToString();
                fixed_checkBox.IsChecked = reader["defects_fixed"] is DBNull ? false : Convert.ToBoolean(reader["defects_fixed"]);
                reader.Close();
            }
        }

        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            string issued = issued_datePicker.SelectedDate == null ? "NULL" : $"'{issued_datePicker.SelectedDate:yyyy-MM-dd HH:mm:ss}'";
            string returned = returned_datePicker.SelectedDate == null ? "NULL" : $"'{returned_datePicker.SelectedDate:yyyy-MM-dd HH:mm:ss}'";
            string defects = defects_textBox.Text;
            bool is_fixed = fixed_checkBox.IsChecked ?? false;
            string cmdstr = $"UPDATE [Order] SET issued = {issued}, returned = {returned}, defects = N'{defects}', defects_fixed = '{is_fixed}' WHERE id = {Order_id}";

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