using System.Windows;

namespace VideoClub
{
    public partial class Main_Librarian : Window
    {
        public Main_Librarian()
        {
            InitializeComponent();
        }

        private void orders_button_Click(object sender, RoutedEventArgs e)
        {
            Orders page = new Orders();
            frame.Content = page;
            Title = $"Библиотекарь | {page.Title}";
        }

        private void films_button_Click(object sender, RoutedEventArgs e)
        {
            Films page = new Films();
            frame.Content = page;
            Title = $"Библиотекарь | {page.Title}";
        }

        private void cassettes_button_Click(object sender, RoutedEventArgs e)
        {
            Cassettes page = new Cassettes();
            frame.Content = page;
            Title = $"Библиотекарь | {page.Title}";
        }

        private void checking_button_Click(object sender, RoutedEventArgs e)
        {
            Checking page = new Checking();
            frame.Content = page;
            Title = $"Библиотекарь | {page.Title}";
        }

        private void members_button_Click(object sender, RoutedEventArgs e)
        {
            Members page = new Members();
            frame.Content = page;
            Title = $"Библиотекарь | {page.Title}";
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}