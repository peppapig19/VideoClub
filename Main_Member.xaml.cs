using System.Windows;

namespace VideoClub
{
    public partial class Main_Member : Window
    {
        public Main_Member()
        {
            InitializeComponent();
        }

        public int Member_id { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Films page = new Films() { Member_id = Member_id };
            frame.Content = page;
        }

        private void profile_button_Click(object sender, RoutedEventArgs e)
        {
            Profile window = new Profile() { Id = Member_id };
            window.ShowDialog();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}