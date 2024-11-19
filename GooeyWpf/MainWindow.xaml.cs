using GooeyWpf.Services;
using GooeyWpf.Services.Transcriber;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows98;

namespace GooeyWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Windows98Window
    {
        private readonly AvatarController avatarController;

        public MainWindow()
        {
            InitializeComponent();

            Title = Assistant.AssistantName;

            avatarController = new(avatar, Assistant.Instance);

            Assistant.Instance.CommandManager.Transcribe += CommandManager_Transcribe;
            Assistant.Instance.CommandManager.Wake += CommandManager_Wake;
            Assistant.Instance.CommandManager.Sleep += CommandManager_Sleep;

            Assistant.Instance.VoiceActivity += Instance_VoiceActivity;
            Assistant.Instance.VoiceActivityDone += Instance_VoiceActivityDone;

            Assistant.Instance.BindCommands(listBoxChatLog, avatarController);

            //listBoxChatLog.Items.Add(new ChatLog(ChatLog.ChatSpeaker.Program, "kdsjhfshfhishiidfiuhiushdifhihdsihfkdjhfkjshfhsfjhsgffsgghfg" +
            //    "cvbjflkjsbfjbjsgblfjgbjdbfgkfngnjfnxgkjbkxjfgnbjdkfjgnknlkjgnfkjghzjkgkjzdhfgkjzdnhgndjg" +
            //    "lkgnkjfdnhgnjdnjhnjkgdhnjkdnkjhgnkjdkgjhnk"));
        }

        private void Instance_VoiceActivityDone(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                checkBoxVoiceActivity.IsChecked = false;
            });
        }

        private void Instance_VoiceActivity(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                checkBoxVoiceActivity.IsChecked = true;
            });
        }

        private void CommandManager_Sleep(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                checkBoxListening.IsChecked = false;
            });
        }

        private void CommandManager_Wake(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                checkBoxListening.IsChecked = true;
            });
        }

        private void Instance_LipSync(bool mouthOpen)
        {
            avatar.Dispatcher.Invoke(() =>
            {
                if (mouthOpen)
                {
                    avatar.FaceImage = new BitmapImage(new Uri("/Images/NormalOpen.png", UriKind.Relative));
                }
                else
                {
                    avatar.FaceImage = new BitmapImage(new Uri("/Images/NormalClosed.png", UriKind.Relative));
                }
            });
        }

        public static void ScrollToEnd(ListBox listBox)
        {
            if (VisualTreeHelper.GetChildrenCount(listBox) < 1) return;
            if (VisualTreeHelper.GetChild(listBox, 0) is Border border)
            {
                if (border.Child is Grid grid)
                {
                    foreach (var child in grid.Children)
                    {
                        if (child is ScrollViewer scrollViewer) scrollViewer.ScrollToEnd();
                    }
                }
            }
        }

        private void CommandManager_Transcribe(object? sender, ITranscriber.TranscribeEventArgs eventArgs)
        {
            Dispatcher.Invoke(() =>
            {
                if (eventArgs.Text == "[BLANK_AUDIO]" || eventArgs.Text == "[END]") return;

                while (listBoxChatLog.Items.Count > 100)
                {
                    listBoxChatLog.Items.RemoveAt(0);
                }

                listBoxChatLog.Items.Add(new ChatLog(ChatLog.ChatSpeaker.User, eventArgs.Text, eventArgs.Language));
                ScrollToEnd(listBoxChatLog);
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            avatarController.Dispose();
            Assistant.Instance.Dispose();
        }

        private void ButtonGo_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxChat.Text)) return;

            Assistant.Instance.CommandManager.ManualInput(textBoxChat.Text);
            textBoxChat.Text = "";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MusicService.Instance.Play(Application.GetResourceStream(new Uri("/Sounds/Boot.wav", UriKind.Relative)).Stream);
        }
    }
}