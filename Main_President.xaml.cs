using System.Windows;

namespace VideoClub
{
    public partial class Main_President : Window
    {
        public Main_President()
        {
            InitializeComponent();
        }

        private void top_button_Click(object sender, RoutedEventArgs e)
        {
            MostWatched page = new MostWatched();
            frame.Content = page;
            Title = $"Президент | {page.Title}";
        }

        private void anti_button_Click(object sender, RoutedEventArgs e)
        {
            LeastWatched page = new LeastWatched();
            frame.Content = page;
            Title = $"Президент | {page.Title}";
        }

        private void replace_button_Click(object sender, RoutedEventArgs e)
        {
            ToReplace page = new ToReplace();
            frame.Content = page;
            Title = $"Президент | {page.Title}";
        }

        private void librarians_button_Click(object sender, RoutedEventArgs e)
        {
            Librarians page = new Librarians();
            frame.Content = page;
            Title = $"Президент | {page.Title}";
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}