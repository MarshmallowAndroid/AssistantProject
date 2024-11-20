using System.IO;
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
                Directory.CreateDirectory("Piper");
                Directory.CreateDirectory("PiperVoice");
                Directory.CreateDirectory("WhisperCppModel");

                string[] piperVoices = Directory.GetFiles("PiperVoice", "*.onnx");
                string[] whisperCppModels = Directory.GetFiles("WhisperCppModel", "*.bin");

                if (Directory.GetFiles("Piper").Length < 1
                    || piperVoices.Length < 1
                    || Directory.GetFiles("WhisperCppModel").Length < 1)
                {
                    MessageBox.Show("Piper or whisper.cpp GGML models not found.\n" +
                        "Please copy them to their specific directories in the " +
                        "executable's path.", "Missing required files", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
                    return;
                }

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