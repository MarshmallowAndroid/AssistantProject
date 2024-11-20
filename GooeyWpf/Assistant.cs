using GooeyWpf.Commands;
using GooeyWpf.Services.Synthesizer;
using GooeyWpf.Services.Transcriber;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace GooeyWpf
{
    internal class Assistant : Singleton<Assistant>, IDisposable
    {
        public const string AssistantName = "Britney";
        public static readonly string[] WakeWordVariations =
        {
            "Hey Britney", "Hey Brittany", "Hey Brittney", "Hey Britany", "Hey Britny", "Hey Brit Knee"
        };

        private readonly WasapiOut outputDevice = new();
        private readonly ISynthesizer synthesizer;
        private readonly ITranscriber transcriber;
        private readonly CommandManager commandManager;

        private DirectCommand? directCommand;

        public Assistant()
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

            synthesizer = new PiperSynthesizer(
                @"Piper\piper.exe",
                piperVoices[0],
                3, 1.05f);
            RawSourceWaveStream rawSourceWaveStream = new(synthesizer.OutputStream, new WaveFormat(22050, 1));
            LipsyncSampleProvider sampleProvider = new(rawSourceWaveStream.ToSampleProvider());
            sampleProvider.Lipsync += SampleProvider_Lipsync;
            outputDevice.Init(sampleProvider);
            outputDevice.Play();
            MMDeviceEnumerator deviceEnumerator = new();
            WasapiCapture captureDevice = new(deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia), true)
            {
                WaveFormat = new(16000, 1)
            };
            transcriber = new WhisperDotNetTranscriber(whisperCppModels[0], captureDevice);
            transcriber.Initialize();
            transcriber.StartTranscribing();
            commandManager = new(transcriber, "hey " + AssistantName, WakeWordVariations);
            commandManager.Wake += CommandManager_Wake;

            ((WhisperDotNetTranscriber)transcriber).VoiceActivity += Assistant_VoiceActivity;
            ((WhisperDotNetTranscriber)transcriber).VoiceActivityDone += Assistant_VoiceActivityDone;
        }

        private void Assistant_VoiceActivityDone(object? sender, EventArgs e)
        {
            VoiceActivityDone?.Invoke(sender, e);
        }

        private void Assistant_VoiceActivity(object? sender, EventArgs e)
        {
            VoiceActivity?.Invoke(sender, e);
        }

        public delegate void OnLipSync(bool mouthOpen);

        public event OnLipSync? LipSync;

        public event EventHandler? VoiceActivity;

        public event EventHandler? VoiceActivityDone;

        private void SampleProvider_Lipsync(bool mouthOpen)
        {
            LipSync?.Invoke(mouthOpen);
        }

        private void CommandManager_Wake(object? sender, EventArgs e)
        {
            directCommand?.DirectResponse(
            [
                "What can I do for you?",
                "What's up?"
            ]);
        }

        public CommandManager CommandManager => commandManager;

        public void BindCommands(ListBox chatLog, AvatarController avatarController)
        {
            directCommand = new DirectCommand(transcriber, synthesizer, chatLog, avatarController);

            IEnumerable<TypeInfo> commandTypes = Assembly.GetExecutingAssembly().DefinedTypes
                .Where(t => t.CustomAttributes.Any(t => t.AttributeType == typeof(CommandAttribute)));

            IEnumerable<Command> commands = commandTypes.Select(ct =>
            {
                return (Command)Activator.CreateInstance(ct, [transcriber, synthesizer, chatLog, avatarController])!;
            });

            commandManager.RegisterCommand(new KillCommand(transcriber, synthesizer, chatLog, avatarController, commandManager));
            commandManager.RegisterCommands(commands);
        }

        public void Dispose()
        {
            transcriber.StopTranscribing();
            commandManager.Stop();
        }
    }
}