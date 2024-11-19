using System.Windows;
using Windows98;

namespace GooeyWpf
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class LoadingWindow : Windows98Window
    {
        private Thread? loadThread;

        public LoadingWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadThread = new(() =>
            {
                _ = Assistant.Instance;

                Dispatcher.Invoke(() =>
                {
                    new MainWindow().Show();
                    Close();
                });
            });
            loadThread.Start();
        }
    }
}